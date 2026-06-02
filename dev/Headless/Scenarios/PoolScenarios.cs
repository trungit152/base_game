using System;
using trungnhd.puzzlecore.Pooling;

namespace trungnhd.puzzlecore.Headless.Scenarios
{
    /// <summary>Kiểm chứng ObjectPool: tái dùng, callback onGet/onRelease, maxSize + huỷ phần tử thừa, Prewarm, Clear.</summary>
    public static class PoolScenarios
    {
        private sealed class Box { public bool Active; }

        private sealed class PoolableBox : IPoolable
        {
            public int Gets;
            public int Releases;
            public void OnGet() => Gets++;
            public void OnRelease() => Releases++;
        }

        public static void Run()
        {
            Console.WriteLine("[Pool]");

            int created = 0, destroyed = 0;
            var pool = new ObjectPool<Box>(
                factory: () => { created++; return new Box(); },
                onGet: b => b.Active = true,
                onRelease: b => b.Active = false,
                onDestroy: _ => destroyed++,
                maxSize: 2);

            var a = pool.Get();
            Asserts.ExpectEqual(1, created, "factory called on empty pool");
            Asserts.Expect(a.Active, "onGet ran (item active)");
            Asserts.ExpectEqual(1, pool.CountAll, "CountAll = 1 after first Get");
            Asserts.ExpectEqual(1, pool.CountActive, "CountActive = 1");
            Asserts.ExpectEqual(0, pool.CountInactive, "CountInactive = 0");

            pool.Release(a);
            Asserts.Expect(!a.Active, "onRelease ran (item inactive)");
            Asserts.ExpectEqual(1, pool.CountInactive, "CountInactive = 1 after release");
            Asserts.ExpectEqual(0, pool.CountActive, "CountActive = 0 after release");

            var a2 = pool.Get();
            Asserts.Expect(ReferenceEquals(a, a2), "Get reuses the released instance");
            Asserts.ExpectEqual(1, created, "factory NOT called again on reuse");

            // maxSize = 2: lấy thêm 2 cái mới (tổng 3 đang active), thả cả 3 -> 1 cái bị huỷ
            var b = pool.Get();
            var c = pool.Get();
            Asserts.ExpectEqual(3, created, "two more created (b, c) -> total 3");
            pool.Release(a2);
            pool.Release(b);
            pool.Release(c); // pool đã đủ 2 -> c bị huỷ
            Asserts.ExpectEqual(2, pool.CountInactive, "pool capped at maxSize = 2");
            Asserts.ExpectEqual(1, destroyed, "overflow item destroyed on release");

            // Prewarm + Clear trên một pool khác
            var pool2 = new ObjectPool<Box>(() => new Box());
            pool2.Prewarm(3);
            Asserts.ExpectEqual(3, pool2.CountInactive, "Prewarm(3) -> 3 inactive");
            Asserts.ExpectEqual(3, pool2.CountAll, "Prewarm(3) -> CountAll 3");
            pool2.Clear();
            Asserts.ExpectEqual(0, pool2.CountInactive, "Clear empties the pool");

            // IPoolable: pool tự gọi OnGet/OnRelease mà không cần callback
            var poolable = new ObjectPool<PoolableBox>(() => new PoolableBox());
            var pb = poolable.Get();
            Asserts.ExpectEqual(1, pb.Gets, "IPoolable.OnGet auto-called on Get");
            poolable.Release(pb);
            Asserts.ExpectEqual(1, pb.Releases, "IPoolable.OnRelease auto-called on Release");
            var pb2 = poolable.Get();
            Asserts.Expect(ReferenceEquals(pb, pb2) && pb.Gets == 2, "reused poolable gets OnGet again");

            // IPoolable + callback cùng chạy (interface trước, callback sau)
            int cbGet = 0;
            var combo = new ObjectPool<PoolableBox>(() => new PoolableBox(), onGet: _ => cbGet++);
            var c1 = combo.Get();
            Asserts.Expect(c1.Gets == 1 && cbGet == 1, "interface hook and pool callback both run on Get");
        }
    }
}
