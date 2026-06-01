using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Puzzle
{
    /// <summary>
    /// Góc nhìn KHÔNG generic của một puzzle đang chạy. Cho phép các hệ thống không biết
    /// <c>TState</c>/<c>TAction</c> cụ thể — booster, flow state Playing, UI — quan sát và điều khiển
    /// session. Đây là seam giúp hệ thống booster tách rời khỏi bất kỳ puzzle cụ thể nào.
    /// </summary>
    public interface IPuzzleSession
    {
        PuzzleOutcome Outcome { get; }

        /// <summary>True khi <see cref="Outcome"/> khác <see cref="PuzzleOutcome.Undecided"/>.</summary>
        bool IsTerminal { get; }

        /// <summary>Áp dụng một action dạng boxed; thất bại nếu sai kiểu, action không hợp lệ, hoặc đã kết thúc.</summary>
        Result ApplyAction(object action);

        /// <summary>Đưa session về lại state khởi đầu.</summary>
        void Reset();
    }
}
