using System;

namespace trungnhd.puzzlecore.input
{
    /// <summary>Phát ra vị trí khi con trỏ được giữ tại chỗ quá ngưỡng thời gian (long-press).</summary>
    public interface IHoldInput
    {
        event Action<Point2> Held;
    }
}
