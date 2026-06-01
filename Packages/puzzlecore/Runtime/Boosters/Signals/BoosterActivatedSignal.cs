using trungnhd.puzzlecore.Signals;

namespace trungnhd.puzzlecore.Boosters.Signals
{
    /// <summary>Phát ra khi một booster được kích hoạt thành công.</summary>
    public sealed class BoosterActivatedSignal : ISignal
    {
        public string BoosterId { get; }
        public int RemainingCount { get; }

        public BoosterActivatedSignal(string boosterId, int remainingCount)
        {
            BoosterId = boosterId;
            RemainingCount = remainingCount;
        }
    }
}
