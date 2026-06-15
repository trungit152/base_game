using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Puzzle
{
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
