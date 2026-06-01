using trungnhd.puzzlecore.Signals;

namespace trungnhd.puzzlecore.Puzzle.Signals
{
    /// <summary>Phát ra sau khi một action được áp dụng thành công lên session.</summary>
    public sealed class PuzzleActionAppliedSignal : ISignal
    {
        public object Action { get; }
        public PuzzleOutcome Outcome { get; }

        public PuzzleActionAppliedSignal(object action, PuzzleOutcome outcome)
        {
            Action = action;
            Outcome = outcome;
        }
    }
}
