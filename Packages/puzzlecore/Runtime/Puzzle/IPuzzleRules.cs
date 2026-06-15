using System.Collections.Generic;

namespace trungnhd.puzzlecore.Puzzle
{
    public interface IPuzzleRules<TState, TAction>
    {
        /// <summary>State khởi đầu của một ván puzzle mới</summary>
        TState InitialState { get; }

        /// <summary>Các action hợp lệ từ <paramref name="state"/> (rỗng khi đã kết thúc)</summary>
        IReadOnlyList<TAction> GetLegalActions(TState state);

        /// <summary>Transition thuần: trả về state thu được sau khi áp dụng <paramref name="action"/></summary>
        TState Apply(TState state, TAction action);

        /// <summary>Phân loại một state thành Undecided / Won / Lost</summary>
        PuzzleOutcome GetOutcome(TState state);
    }
}
