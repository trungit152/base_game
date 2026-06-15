using trungnhd.puzzlecore.Common;

namespace ExampleGame.Puzzle
{
    /// <summary>
    /// Capability HẸP do chính game khai báo, để booster có thể "cộng điểm" mà hệ thống booster không
    /// cần biết <c>TapState</c>. Effect sẽ ép kiểu <c>IPuzzleSession</c> xuống interface này (ISP) —
    /// đúng pattern khử-coupling của lõi (xem demo <c>IDecrementable</c> trong dev/Headless).
    /// </summary>
    public interface IScorable
    {
        Result AddPoints(int amount);
    }
}
