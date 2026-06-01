namespace trungnhd.puzzlecore.Headless.Demo
{
    /// <summary>State bất biến cho puzzle demo Countdown.</summary>
    public readonly struct CountdownState
    {
        public int Remaining { get; }
        public int Moves { get; }
        public int MoveBudget { get; }

        public CountdownState(int remaining, int moves, int moveBudget)
        {
            Remaining = remaining;
            Moves = moves;
            MoveBudget = moveBudget;
        }

        public override string ToString()
            => "Countdown(remaining=" + Remaining + ", moves=" + Moves + "/" + MoveBudget + ")";
    }
}
