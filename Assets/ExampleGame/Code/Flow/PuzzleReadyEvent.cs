using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Puzzle;

namespace ExampleGame.Flow
{
    /// <summary>
    /// Phát ra khi màn loading đã dựng xong puzzle. Nhờ event này, tầng điều khiển (controller/UI) lấy
    /// được tham chiếu <see cref="IPuzzleSession"/> để tương tác mà KHÔNG cần chọc vào nội bộ state
    /// machine — đúng tinh thần tách rời của <c>EventBus</c>.
    /// </summary>
    public sealed class PuzzleReadyEvent : IEvent
    {
        public IPuzzleSession Puzzle { get; }

        public PuzzleReadyEvent(IPuzzleSession puzzle)
        {
            Puzzle = puzzle;
        }
    }
}
