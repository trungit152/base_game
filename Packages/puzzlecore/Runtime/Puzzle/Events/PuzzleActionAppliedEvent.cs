using trungnhd.puzzlecore.Events;

namespace trungnhd.puzzlecore.Puzzle.Events
{
    /// <summary>Phát ra sau khi một action được áp dụng thành công lên session.</summary>
    public sealed class PuzzleActionAppliedEvent : IEvent
    {
        public object Action { get; }
        public PuzzleOutcome Outcome { get; }

        public PuzzleActionAppliedEvent(object action, PuzzleOutcome outcome)
        {
            Action = action;
            Outcome = outcome;
        }
    }
}
