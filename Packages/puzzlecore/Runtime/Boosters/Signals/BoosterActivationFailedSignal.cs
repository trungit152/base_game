using trungnhd.puzzlecore.Signals;

namespace trungnhd.puzzlecore.Boosters.Signals
{
    /// <summary>Phát ra khi một lần thử kích hoạt booster thất bại (không tồn tại, hết kho, không có puzzle, hoặc effect lỗi).</summary>
    public sealed class BoosterActivationFailedSignal : ISignal
    {
        public string BoosterId { get; }
        public string Reason { get; }

        public BoosterActivationFailedSignal(string boosterId, string reason)
        {
            BoosterId = boosterId;
            Reason = reason;
        }
    }
}
