using System.Collections.Generic;

namespace trungnhd.puzzlecore.Puzzle
{
    /// <summary>
    /// Transition system tất định — trái tim của một puzzle:
    /// state -&gt; các action hợp lệ -&gt; state kế tiếp -&gt; kết quả.
    /// <para>
    /// Hợp đồng PURE: phần cài đặt không được mutate đầu vào hay giữ state biến đổi; <see cref="Apply"/>
    /// trả về một state MỚI. Tính thuần khiết là thứ giúp về sau viết undo/redo, replay, solver và
    /// validator một lần duy nhất và tổng quát.
    /// </para>
    /// <para>
    /// Board/Grid/Cell chỉ là MỘT cách biểu diễn <typeparamref name="TState"/> — KHÔNG được giả định ở đây.
    /// </para>
    /// </summary>
    public interface IPuzzleRules<TState, TAction>
    {
        /// <summary>State khởi đầu của một ván puzzle mới.</summary>
        TState InitialState { get; }

        /// <summary>Các action hợp lệ từ <paramref name="state"/> (rỗng khi đã kết thúc).</summary>
        IReadOnlyList<TAction> GetLegalActions(TState state);

        /// <summary>Transition thuần: trả về state thu được sau khi áp dụng <paramref name="action"/>.</summary>
        TState Apply(TState state, TAction action);

        /// <summary>Phân loại một state thành Undecided / Won / Lost.</summary>
        PuzzleOutcome GetOutcome(TState state);
    }
}
