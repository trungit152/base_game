using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.Puzzle;

namespace trungnhd.puzzlecore.Headless.Demo
{
    /// <summary>
    /// Transition system demo: mỗi Tick giảm Remaining và tăng Moves.
    /// Thắng khi Remaining về 0; Thua khi Moves chạm budget trước. Thuần khiết và tất định.
    /// </summary>
    public sealed class CountdownRules : IPuzzleRules<CountdownState, CountdownAction>
    {
        private static readonly IReadOnlyList<CountdownAction> TickOnly = new[] { CountdownAction.Tick };
        private static readonly IReadOnlyList<CountdownAction> None = Array.Empty<CountdownAction>();

        private readonly int _start;
        private readonly int _budget;

        public CountdownRules(int start, int budget)
        {
            _start = start;
            _budget = budget;
        }

        public CountdownState InitialState => new CountdownState(_start, 0, _budget);

        public IReadOnlyList<CountdownAction> GetLegalActions(CountdownState state)
            => GetOutcome(state) == PuzzleOutcome.Undecided ? TickOnly : None;

        public CountdownState Apply(CountdownState state, CountdownAction action)
            => new CountdownState(state.Remaining - 1, state.Moves + 1, state.MoveBudget);

        public PuzzleOutcome GetOutcome(CountdownState state)
        {
            if (state.Remaining <= 0) return PuzzleOutcome.Won;
            if (state.Moves >= state.MoveBudget) return PuzzleOutcome.Lost;
            return PuzzleOutcome.Undecided;
        }
    }
}
