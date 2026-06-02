using System;
using System.Collections.Generic;

namespace trungnhd.puzzlecore.Pooling
{
    /// <summary>
    /// Object pool generic thuần C#. Tạo mới qua <c>factory</c> khi pool rỗng, gọi callback
    /// <c>onGet</c>/<c>onRelease</c> tại mỗi lần lấy/trả, và giới hạn theo <c>maxSize</c> (vượt quá thì
    /// gọi <c>onDestroy</c> và bỏ phần tử thừa). Không phụ thuộc engine — test được headless.
    /// <para>Nếu phần tử hiện thực <see cref="IPoolable"/>, pool tự gọi <c>OnGet</c>/<c>OnRelease</c>
    /// (chạy trước callback tương ứng) — không cần truyền callback.</para>
    /// <para>Lưu ý: pool không tự phát hiện việc Release cùng một phần tử hai lần — đó là trách nhiệm
    /// của bên gọi.</para>
    /// </summary>
    public sealed class ObjectPool<T> : IObjectPool<T> where T : class
    {
        private readonly Stack<T> _inactive;
        private readonly Func<T> _factory;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly Action<T> _onDestroy;
        private readonly int _maxSize;

        public int CountInactive => _inactive.Count;
        public int CountAll { get; private set; }
        public int CountActive => CountAll - CountInactive;

        /// <param name="factory">Hàm tạo phần tử mới (bắt buộc).</param>
        /// <param name="onGet">Gọi mỗi khi lấy phần tử ra (vd reset/bật active).</param>
        /// <param name="onRelease">Gọi mỗi khi trả phần tử về (vd tắt active/dọn dẹp).</param>
        /// <param name="onDestroy">Gọi khi phần tử bị huỷ (pool đầy hoặc Clear).</param>
        /// <param name="maxSize">Số phần tử tối đa giữ lại trong pool; &lt;= 0 nghĩa là không giới hạn.</param>
        public ObjectPool(
            Func<T> factory,
            Action<T> onGet = null,
            Action<T> onRelease = null,
            Action<T> onDestroy = null,
            int maxSize = 0)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _onGet = onGet;
            _onRelease = onRelease;
            _onDestroy = onDestroy;
            _maxSize = maxSize;
            _inactive = new Stack<T>();
        }

        /// <summary>Tạo trước <paramref name="count"/> phần tử và đưa vào pool.</summary>
        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _inactive.Push(Create());
            }
        }

        public T Get()
        {
            T item = _inactive.Count > 0 ? _inactive.Pop() : Create();
            if (item is IPoolable poolable) poolable.OnGet();
            _onGet?.Invoke(item);
            return item;
        }

        public void Release(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item is IPoolable poolable) poolable.OnRelease();
            _onRelease?.Invoke(item);

            if (_maxSize <= 0 || _inactive.Count < _maxSize)
            {
                _inactive.Push(item);
            }
            else
            {
                // Pool đã đầy -> huỷ phần tử thừa.
                _onDestroy?.Invoke(item);
                CountAll--;
            }
        }

        public void Clear()
        {
            if (_onDestroy != null)
            {
                foreach (var item in _inactive)
                {
                    _onDestroy(item);
                }
            }
            CountAll -= _inactive.Count;
            _inactive.Clear();
        }

        private T Create()
        {
            var item = _factory();
            CountAll++;
            return item;
        }
    }
}
