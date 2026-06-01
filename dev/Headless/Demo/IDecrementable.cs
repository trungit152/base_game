using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Headless.Demo
{
    /// <summary>
    /// Capability hẹp mà một puzzle chủ động hiện thực để booster có thể "giảm" nó — MÀ hệ thống
    /// booster không cần biết kiểu state cụ thể. Đây là cách effect giữ được sự tách rời (ISP).
    /// </summary>
    public interface IDecrementable
    {
        Result Decrement(int amount);
    }
}
