using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Unity.Time
{
    /// <summary>
    /// Cầu nối <see cref="IClock"/> tới <c>UnityEngine.Time</c>. Đây là nơi DUY NHẤT mà khái niệm thời
    /// gian của core chạm vào Unity, nhờ vậy phần còn lại của framework vẫn engine-agnostic.
    /// </summary>
    public sealed class UnityClock : IClock
    {
        public double Now => UnityEngine.Time.timeAsDouble;
        public double DeltaTime => UnityEngine.Time.deltaTime;
    }
}
