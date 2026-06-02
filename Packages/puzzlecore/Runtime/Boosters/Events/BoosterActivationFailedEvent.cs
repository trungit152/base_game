using trungnhd.puzzlecore.Events;

namespace trungnhd.puzzlecore.Boosters.Events
{
    /// <summary>Phát ra khi một lần thử kích hoạt booster thất bại (không tồn tại, hết kho, không có puzzle, hoặc effect lỗi).</summary>
    public sealed class BoosterActivationFailedEvent : IEvent
    {
        public string BoosterId { get; }
        public string Reason { get; }

        public BoosterActivationFailedEvent(string boosterId, string reason)
        {
            BoosterId = boosterId;
            Reason = reason;
        }
    }
}
