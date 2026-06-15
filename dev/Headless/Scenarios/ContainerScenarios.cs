using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.TContainer;

namespace trungnhd.puzzlecore.Headless.Scenarios
{
    /// <summary>
    /// Kiểm chứng TContainer (DI core): lifetime Singleton/Transient, đăng ký interface/factory/instance,
    /// constructor injection, phân giải tập hợp, ưu tiên đăng ký tường minh, entry point eager, tự phân
    /// giải <see cref="IObjectResolver"/>, phát hiện phụ thuộc vòng, lỗi khi thiếu đăng ký và Dispose.
    /// </summary>
    public static class ContainerScenarios
    {
        private interface IService { }
        private sealed class ServiceA : IService { }
        private sealed class ServiceB : IService { }

        private sealed class Dependency { }

        private sealed class Consumer
        {
            public readonly Dependency Dep;
            public readonly IService Service;
            public Consumer(Dependency dep, IService service) { Dep = dep; Service = service; }
        }

        private interface IPlugin { }
        private sealed class PluginX : IPlugin { }
        private sealed class PluginY : IPlugin { }
        private sealed class PluginZ : IPlugin { }

        private sealed class TracksDispose : IDisposable
        {
            public int Disposed;
            public void Dispose() => Disposed++;
        }

        private sealed class ExternalDisposable : IDisposable
        {
            public int Disposed;
            public void Dispose() => Disposed++;
        }

        private sealed class EntryPoint
        {
            public static int Constructed;
            public EntryPoint(Dependency dep) { Constructed++; }
        }

        // Phụ thuộc vòng: Ping <-> Pong
        private sealed class Ping { public Ping(Pong p) { } }
        private sealed class Pong { public Pong(Ping p) { } }

        public static void Run()
        {
            Console.WriteLine("[TContainer]");

            Singleton_returns_same_instance();
            Transient_returns_new_instance();
            Interface_maps_to_implementation();
            Factory_receives_resolver();
            Instance_registration_returned_asis();
            Constructor_injection();
            Collection_resolution();
            Explicit_collection_registration_wins();
            EntryPoint_eager_instantiation();
            SelfResolve_object_resolver();
            Missing_registration_throws();
            Circular_dependency_throws();
            Dispose_disposes_owned_singletons();
        }

        private static void Singleton_returns_same_instance()
        {
            var b = new ContainerBuilder();
            b.Register<ServiceA>(Lifetime.Singleton);
            var c = b.Build();
            Asserts.Expect(ReferenceEquals(c.Resolve<ServiceA>(), c.Resolve<ServiceA>()),
                "Singleton: hai lần Resolve trả về cùng instance");
        }

        private static void Transient_returns_new_instance()
        {
            var b = new ContainerBuilder();
            b.Register<ServiceA>(Lifetime.Transient);
            var c = b.Build();
            Asserts.Expect(!ReferenceEquals(c.Resolve<ServiceA>(), c.Resolve<ServiceA>()),
                "Transient: mỗi lần Resolve là instance mới");
        }

        private static void Interface_maps_to_implementation()
        {
            var b = new ContainerBuilder();
            b.Register<IService, ServiceA>(Lifetime.Singleton);
            var c = b.Build();
            Asserts.Expect(c.Resolve<IService>() is ServiceA,
                "Register<IService, ServiceA>: Resolve<IService> trả về ServiceA");
        }

        private static void Factory_receives_resolver()
        {
            var b = new ContainerBuilder();
            b.Register<Dependency>(Lifetime.Singleton);
            b.Register<IService>(r => { var dep = r.Resolve<Dependency>(); return new ServiceA(); }, Lifetime.Singleton);
            var c = b.Build();
            Asserts.Expect(c.Resolve<IService>() is ServiceA, "Factory: dựng được và resolver phân giải được phụ thuộc");
        }

        private static void Instance_registration_returned_asis()
        {
            var b = new ContainerBuilder();
            var instance = new ServiceA();
            b.RegisterInstance(instance);
            var c = b.Build();
            Asserts.Expect(ReferenceEquals(c.Resolve<ServiceA>(), instance),
                "RegisterInstance: trả về đúng instance đã đăng ký");
        }

        private static void Constructor_injection()
        {
            var b = new ContainerBuilder();
            b.Register<Dependency>(Lifetime.Singleton);
            b.Register<IService, ServiceB>(Lifetime.Singleton);
            b.Register<Consumer>(Lifetime.Singleton);
            var c = b.Build();
            var consumer = c.Resolve<Consumer>();
            Asserts.Expect(consumer.Dep != null && consumer.Service is ServiceB,
                "Constructor injection: cả hai tham số được phân giải đúng");
        }

        private static void Collection_resolution()
        {
            var b = new ContainerBuilder();
            b.Register<IPlugin, PluginX>(Lifetime.Singleton);
            b.Register<IPlugin, PluginY>(Lifetime.Singleton);
            b.Register<IPlugin, PluginZ>(Lifetime.Singleton);
            var c = b.Build();

            var asReadOnly = c.Resolve<IReadOnlyList<IPlugin>>();
            Asserts.ExpectEqual(3, asReadOnly.Count, "Collection: Resolve<IReadOnlyList<IPlugin>> trả về cả 3");

            var asEnumerable = c.Resolve<IEnumerable<IPlugin>>();
            int n = 0; foreach (var _ in asEnumerable) n++;
            Asserts.ExpectEqual(3, n, "Collection: IEnumerable<IPlugin> duyệt được 3 phần tử");

            var asArray = c.Resolve<IPlugin[]>();
            Asserts.ExpectEqual(3, asArray.Length, "Collection: IPlugin[] trả về mảng 3 phần tử");

            // Single resolve khi có nhiều đăng ký -> "last wins"
            Asserts.Expect(c.Resolve<IPlugin>() is PluginZ, "Nhiều đăng ký: Resolve đơn lấy cái đăng ký cuối (last wins)");

            // Không có đăng ký phần tử -> tập hợp rỗng (không ném lỗi)
            var empty = new ContainerBuilder().Build().Resolve<IReadOnlyList<IPlugin>>();
            Asserts.ExpectEqual(0, empty.Count, "Collection: không có đăng ký -> tập rỗng");
        }

        private static void Explicit_collection_registration_wins()
        {
            var b = new ContainerBuilder();
            var explicitList = new List<IPlugin> { new PluginX() };
            b.RegisterInstance<IReadOnlyList<IPlugin>>(explicitList);
            // PluginY KHÔNG được đăng ký dưới dạng phần tử -> nếu auto-collection nhảy vào sẽ trả về rỗng.
            var c = b.Build();
            Asserts.Expect(ReferenceEquals(c.Resolve<IReadOnlyList<IPlugin>>(), explicitList),
                "Đăng ký tường minh IReadOnlyList<T> thắng phân giải tập hợp tự động");
        }

        private static void EntryPoint_eager_instantiation()
        {
            EntryPoint.Constructed = 0;
            var b = new ContainerBuilder();
            b.Register<Dependency>(Lifetime.Singleton);
            b.RegisterEntryPoint<EntryPoint>();
            var c = (Container)b.Build();
            Asserts.ExpectEqual(1, EntryPoint.Constructed, "EntryPoint được khởi tạo eager ngay khi Build");
            Asserts.ExpectEqual(1, c.EntryPoints.Count, "EntryPoints liệt kê đúng 1 entry point");
            Asserts.Expect(ReferenceEquals(c.Resolve<EntryPoint>(), c.EntryPoints[0]),
                "Entry point là singleton: Resolve trùng instance đã eager");
            Asserts.ExpectEqual(1, EntryPoint.Constructed, "Resolve lại entry point KHÔNG dựng thêm lần nữa");
        }

        private static void SelfResolve_object_resolver()
        {
            var c = new ContainerBuilder().Build();
            Asserts.Expect(ReferenceEquals(c.Resolve<IObjectResolver>(), c),
                "Container tự phân giải chính nó qua IObjectResolver");
        }

        private static void Missing_registration_throws()
        {
            var c = new ContainerBuilder().Build();
            bool threw = false;
            try { c.Resolve<ServiceA>(); } catch (InvalidOperationException) { threw = true; }
            Asserts.Expect(threw, "Thiếu đăng ký: Resolve ném InvalidOperationException");
        }

        private static void Circular_dependency_throws()
        {
            var b = new ContainerBuilder();
            b.Register<Ping>(Lifetime.Singleton);
            b.Register<Pong>(Lifetime.Singleton);
            var c = b.Build();
            bool threw = false;
            try { c.Resolve<Ping>(); } catch (InvalidOperationException) { threw = true; }
            Asserts.Expect(threw, "Phụ thuộc vòng: ném InvalidOperationException thay vì tràn stack");
        }

        private static void Dispose_disposes_owned_singletons()
        {
            var owned = new TracksDispose();
            var external = new ExternalDisposable();
            var b = new ContainerBuilder();
            b.Register<TracksDispose>(_ => owned, Lifetime.Singleton); // do container tạo qua factory -> sở hữu
            b.RegisterInstance(external);                              // do người dùng cấp -> không sở hữu
            var c = (Container)b.Build();
            c.Resolve<TracksDispose>();                               // ép tạo singleton từ factory
            c.Dispose();
            Asserts.ExpectEqual(1, owned.Disposed, "Dispose: singleton do container tạo bị dispose");
            Asserts.ExpectEqual(0, external.Disposed, "Dispose: instance do người dùng cấp KHÔNG bị dispose");
        }
    }
}
