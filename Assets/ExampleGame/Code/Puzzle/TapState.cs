namespace ExampleGame.Puzzle
{
    /// <summary>
    /// State bất biến của puzzle "Tap Target": chạm để tăng điểm. Là <c>readonly struct</c> nên
    /// transition (<see cref="TapRules.Apply"/>) luôn trả về một bản MỚI thay vì mutate — giữ đúng
    /// hợp đồng PURE của <c>IPuzzleRules</c>.
    /// </summary>
    public readonly struct TapState
    {
        public int Score { get; }
        public int TapsUsed { get; }
        public int Target { get; }
        public int TapBudget { get; }

        public TapState(int score, int tapsUsed, int target, int tapBudget)
        {
            Score = score;
            TapsUsed = tapsUsed;
            Target = target;
            TapBudget = tapBudget;
        }

        public override string ToString()
            => "Tap(score=" + Score + "/" + Target + ", taps=" + TapsUsed + "/" + TapBudget + ")";
    }
}
