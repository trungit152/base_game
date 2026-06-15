using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.Puzzle;

namespace ExampleGame.Puzzle
{
    /// <summary>
    /// Trái tim của game: transition system tất định. Mỗi <c>Tap</c> làm Score+1 và TapsUsed+1.
    /// Thắng khi đạt Target; Thua nếu hết lượt (TapsUsed chạm TapBudget) trước khi đạt Target.
    /// Thuần khiết — không giữ state biến đổi, <see cref="Apply"/> trả về state mới.
    /// </summary>
    public sealed class TapRules : IPuzzleRules<TapState, TapAction>
    {
        private static readonly IReadOnlyList<TapAction> TapOnly = new[] { TapAction.Tap };
        private static readonly IReadOnlyList<TapAction> None = Array.Empty<TapAction>();

        private readonly int _target;
        private readonly int _tapBudget;

        public TapRules(int target, int tapBudget)
        {
            _target = target;
            _tapBudget = tapBudget;
        }

        public TapState InitialState => new TapState(0, 0, _target, _tapBudget);

        public IReadOnlyList<TapAction> GetLegalActions(TapState state)
            => GetOutcome(state) == PuzzleOutcome.Undecided ? TapOnly : None;

        public TapState Apply(TapState state, TapAction action)
            => new TapState(state.Score + 1, state.TapsUsed + 1, state.Target, state.TapBudget);

        public PuzzleOutcome GetOutcome(TapState state)
        {
            if (state.Score >= state.Target) return PuzzleOutcome.Won;
            if (state.TapsUsed >= state.TapBudget) return PuzzleOutcome.Lost;
            return PuzzleOutcome.Undecided;
        }
    }
}
