namespace trungnhd.puzzlecore.Boosters
{
    /// <summary>Kết quả của một lần thử kích hoạt: có kích hoạt được không, id booster, số lượng còn lại,
    /// và thông điệp lỗi (nếu có).</summary>
    public readonly struct BoosterActivationResult
    {
        public bool Activated { get; }
        public string BoosterId { get; }
        public int RemainingCount { get; }
        public string Error { get; }

        private BoosterActivationResult(bool activated, string boosterId, int remainingCount, string error)
        {
            Activated = activated;
            BoosterId = boosterId;
            RemainingCount = remainingCount;
            Error = error;
        }

        public static BoosterActivationResult Success(string boosterId, int remainingCount)
            => new BoosterActivationResult(true, boosterId, remainingCount, null);

        public static BoosterActivationResult Failure(string boosterId, string error)
            => new BoosterActivationResult(false, boosterId, 0, error);
    }
}
