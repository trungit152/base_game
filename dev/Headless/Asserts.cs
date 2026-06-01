using System;
using System.Collections.Generic;

namespace trungnhd.puzzlecore.Headless
{
    /// <summary>Trợ giúp assertion tối giản — không dùng test framework, để harness chạy như một console Exe.</summary>
    public static class Asserts
    {
        public static int Passed;
        public static int Failed;
        public static readonly List<string> Failures = new List<string>();

        public static void Expect(bool condition, string message)
        {
            if (condition)
            {
                Passed++;
                Console.WriteLine("  PASS: " + message);
            }
            else
            {
                Failed++;
                Failures.Add(message);
                Console.WriteLine("  FAIL: " + message);
            }
        }

        public static void ExpectEqual<T>(T expected, T actual, string message)
        {
            bool equal = EqualityComparer<T>.Default.Equals(expected, actual);
            Expect(equal, message + " (expected=" + expected + ", actual=" + actual + ")");
        }
    }
}
