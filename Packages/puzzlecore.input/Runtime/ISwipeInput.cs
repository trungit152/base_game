using System;

namespace trungnhd.puzzlecore.input
{
    /// <summary>Phát ra một <see cref="SwipeDirection"/> mỗi khi nhận diện được một cú vuốt hợp lệ.</summary>
    public interface ISwipeInput
    {
        event Action<SwipeDirection> Swiped;
    }
}
