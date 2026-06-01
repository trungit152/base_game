using trungnhd.puzzlecore.Signals;

namespace trungnhd.puzzlecore.Puzzle.Signals
{
    /// <summary>Phát ra khi outcome của session chuyển trạng thái (vd Undecided -&gt; Won).</summary>
    public sealed class PuzzleOutcomeChangedSignal : ISignal
    {
        public PuzzleOutcome Previous { get; }
        public PuzzleOutcome Current { get; }

        public PuzzleOutcomeChangedSignal(PuzzleOutcome previous, PuzzleOutcome current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
