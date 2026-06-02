using System;
using trungnhd.puzzlecore.Headless.Scenarios;

namespace trungnhd.puzzlecore.Headless
{
    /// <summary>
    /// Điểm vào kiểm chứng headless. Dựng object graph của framework bằng `new` thuần (không DI
    /// container) và chạy các kịch bản assertion, chứng minh core chạy được mà không cần Unity.
    /// </summary>
    public static class Program
    {
        public static int Main()
        {
            Console.WriteLine("trungnhd.puzzlecore — kiem chung headless");
            Console.WriteLine("=========================================");

            PuzzleScenarios.Run();
            InputScenarios.Run();
            FlowScenarios.Run();
            BoosterScenarios.Run();
            PoolScenarios.Run();

            Console.WriteLine();
            Console.WriteLine("RESULT: " + Asserts.Passed + " passed, " + Asserts.Failed + " failed");
            if (Asserts.Failed > 0)
            {
                Console.WriteLine("Failures:");
                foreach (var failure in Asserts.Failures)
                {
                    Console.WriteLine("  - " + failure);
                }
            }

            return Asserts.Failed == 0 ? 0 : 1;
        }
    }
}
