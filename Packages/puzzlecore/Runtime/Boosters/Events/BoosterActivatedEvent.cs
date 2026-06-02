using trungnhd.puzzlecore.Events;

namespace trungnhd.puzzlecore.Boosters.Events
{
    /// <summary>Phát ra khi một booster được kích hoạt thành công.</summary>
    public sealed class BoosterActivatedEvent : IEvent
    {
        public string BoosterId { get; }
        public int RemainingCount { get; }

        public BoosterActivatedEvent(string boosterId, int remainingCount)
        {
            BoosterId = boosterId;
            RemainingCount = remainingCount;
        }
    }
}
