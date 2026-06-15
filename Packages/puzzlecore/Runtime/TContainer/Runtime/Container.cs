using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace trungnhd.puzzlecore.TContainer
{
    /// <summary>
    /// Container DI tối giản, engine-agnostic (không phụ thuộc Unity) — bản dựng từ
    /// <see cref="ContainerBuilder"/>. Hỗ trợ: lifetime Singleton/Transient, đăng ký theo instance /
    /// factory / implementation type, <b>constructor injection</b> tự động, phân giải tập hợp
    /// (<c>IEnumerable&lt;T&gt;</c>, <c>IReadOnlyList&lt;T&gt;</c>, <c>T[]</c>, …), tự phân giải chính nó qua
    /// <see cref="IObjectResolver"/>, phát hiện phụ thuộc vòng và dispose các singleton do container tạo.
    /// </summary>
    public sealed class Container : IObjectResolver, IDisposable
    {
        readonly Dictionary<Type, List<Registration>> _registrations;
        readonly Dictionary<Registration, object> _singletons = new Dictionary<Registration, object>();
        readonly HashSet<Registration> _resolving = new HashSet<Registration>();
        readonly List<IDisposable> _disposables = new List<IDisposable>();
        readonly List<object> _entryPoints = new List<object>();
        bool _disposed;

        /// <summary>
        /// Các entry point (đăng ký qua <c>RegisterEntryPoint</c>) đã được khởi tạo sẵn (eager) khi build.
        /// Adapter (vd. Unity) có thể duyệt danh sách này để điều khiển vòng đời (Start/Tick/Dispose).
        /// </summary>
        public IReadOnlyList<object> EntryPoints => _entryPoints;

        public Container(List<Registration> registrations)
        {
            if (registrations == null) throw new ArgumentNullException(nameof(registrations));

            _registrations = new Dictionary<Type, List<Registration>>();
            foreach (var r in registrations)
            {
                if (r == null) throw new ArgumentException("Registration is null", nameof(registrations));
                if (r.ServiceType == null) throw new ArgumentException("Registration.ServiceType is null", nameof(registrations));

                if (!_registrations.TryGetValue(r.ServiceType, out var list))
                {
                    list = new List<Registration>();
                    _registrations[r.ServiceType] = list;
                }
                list.Add(r);
            }

            // Khởi tạo entry point ngay khi build: chạy constructor + tác dụng phụ, và fail-fast nếu
            // thiếu phụ thuộc. Làm sau khi đã nạp toàn bộ registration để không phụ thuộc thứ tự đăng ký.
            foreach (var r in registrations)
            {
                if (r.IsEntryPoint)
                    _entryPoints.Add(ResolveRegistration(r));
            }
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public object Resolve(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (_disposed) throw new ObjectDisposedException(nameof(Container));

            // 1) Đăng ký tường minh thắng tuyệt đối (kể cả khi type là một kiểu collection được
            //    RegisterInstance trực tiếp, vd. IReadOnlyList<IBoosterDefinition>). "Last wins".
            if (_registrations.TryGetValue(type, out var list) && list.Count > 0)
                return ResolveRegistration(list[list.Count - 1]);

            // 2) Container tự phân giải chính nó.
            if (type == typeof(IObjectResolver))
                return this;

            // 3) Phân giải tập hợp: gom mọi registration của kiểu phần tử.
            if (TryGetElementType(type, out var elementType))
                return ResolveCollection(type, elementType);

            throw new InvalidOperationException("No registration for type '" + type + "'.");
        }

        object ResolveRegistration(Registration r)
        {
            if (r.Lifetime == Lifetime.Singleton && _singletons.TryGetValue(r, out var cached))
                return cached;

            if (!_resolving.Add(r))
                throw new InvalidOperationException("Circular dependency detected while resolving '" + Describe(r) + "'.");

            try
            {
                object instance;
                bool ownedByContainer;
                if (r.Instance != null)
                {
                    instance = r.Instance;          // instance do người dùng cấp -> container không sở hữu vòng đời.
                    ownedByContainer = false;
                }
                else if (r.Factory != null)
                {
                    instance = r.Factory(this);
                    ownedByContainer = true;
                }
                else if (r.ImplementationType != null)
                {
                    instance = CreateInstance(r.ImplementationType);
                    ownedByContainer = true;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Registration for '" + r.ServiceType + "' has no instance, factory, or implementation type.");
                }

                if (instance == null)
                    throw new InvalidOperationException("Resolved a null instance for '" + Describe(r) + "'.");

                if (r.Lifetime == Lifetime.Singleton)
                {
                    _singletons[r] = instance;
                    if (ownedByContainer)
                        TrackDisposable(instance);
                }

                return instance;
            }
            finally
            {
                _resolving.Remove(r);
            }
        }

        object CreateInstance(Type implementationType)
        {
            var ctor = SelectConstructor(implementationType);
            if (ctor == null)
                throw new InvalidOperationException("Type '" + implementationType + "' has no usable constructor.");

            var parameters = ctor.GetParameters();
            var args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                args[i] = Resolve(parameters[i].ParameterType);

            return ctor.Invoke(args);
        }

        // Chọn constructor "tham lam" nhiều tham số nhất (giống mặc định của VContainer). Ưu tiên public,
        // nếu không có thì xét cả non-public để hỗ trợ kiểu có ctor nội bộ.
        static ConstructorInfo SelectConstructor(Type implementationType)
        {
            var ctors = implementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (ctors.Length == 0)
                ctors = implementationType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

            ConstructorInfo best = null;
            int bestCount = -1;
            foreach (var c in ctors)
            {
                int count = c.GetParameters().Length;
                if (count > bestCount)
                {
                    best = c;
                    bestCount = count;
                }
            }
            return best;
        }

        object ResolveCollection(Type collectionType, Type elementType)
        {
            _registrations.TryGetValue(elementType, out var items);
            int count = items != null ? items.Count : 0;

            if (collectionType.IsArray)
            {
                var array = Array.CreateInstance(elementType, count);
                for (int i = 0; i < count; i++)
                    array.SetValue(ResolveRegistration(items[i]), i);
                return array;
            }

            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType, count);
            for (int i = 0; i < count; i++)
                list.Add(ResolveRegistration(items[i]));
            return list;
        }

        // Nhận diện các kiểu tập hợp mà container biết tự dựng. Dùng cho đường phân giải tự động
        // (chỉ chạy khi kiểu tập hợp đó KHÔNG được đăng ký tường minh).
        static bool TryGetElementType(Type type, out Type elementType)
        {
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }

            if (type.IsGenericType)
            {
                var def = type.GetGenericTypeDefinition();
                if (def == typeof(IEnumerable<>) ||
                    def == typeof(IReadOnlyList<>) ||
                    def == typeof(IReadOnlyCollection<>) ||
                    def == typeof(IList<>) ||
                    def == typeof(ICollection<>) ||
                    def == typeof(List<>))
                {
                    elementType = type.GetGenericArguments()[0];
                    return true;
                }
            }

            elementType = null;
            return false;
        }

        void TrackDisposable(object instance)
        {
            if (instance is IDisposable disposable && !ReferenceEquals(disposable, this))
                _disposables.Add(disposable);
        }

        static string Describe(Registration r)
        {
            if (r.ImplementationType != null) return r.ImplementationType.ToString();
            if (r.ServiceType != null) return r.ServiceType.ToString();
            return "<unknown>";
        }

        /// <summary>Dispose các singleton do container tạo ra (theo thứ tự ngược lại khi tạo).</summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                try { _disposables[i].Dispose(); }
                catch { /* không để một disposable lỗi chặn các disposable còn lại */ }
            }

            _disposables.Clear();
            _singletons.Clear();
            _entryPoints.Clear();
        }
    }
}
