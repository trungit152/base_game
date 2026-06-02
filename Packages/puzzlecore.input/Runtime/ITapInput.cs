using System;

namespace trungnhd.puzzlecore.input
{
    /// <summary>Phát ra vị trí được chạm khi nhận diện một cú tap/click (chạm rồi nhả nhanh tại chỗ).</summary>
    public interface ITapInput
    {
        event Action<Point2> Tapped;
    }
}
