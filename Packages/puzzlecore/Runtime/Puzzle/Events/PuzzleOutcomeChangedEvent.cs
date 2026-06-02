using trungnhd.puzzlecore.Events;

namespace trungnhd.puzzlecore.Puzzle.Events
{
    /// <summary>Phát ra khi outcome của session chuyển trạng thái (vd Undecided -&gt; Won).</summary>
    public sealed class PuzzleOutcomeChangedEvent : IEvent
    {
        public PuzzleOutcome Previous { get; }
        public PuzzleOutcome Current { get; }

        public PuzzleOutcomeChangedEvent(PuzzleOutcome previous, PuzzleOutcome current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
