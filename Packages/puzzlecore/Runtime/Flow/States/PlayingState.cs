using trungnhd.puzzlecore.Puzzle;

namespace trungnhd.puzzlecore.Flow.States
{
    /// <summary>
    /// Đang chơi. Mỗi tick sẽ kiểm tra outcome của puzzle hiện tại và chuyển sang
    /// <see cref="WinState"/> hoặc <see cref="LoseState"/> tương ứng. Cho phép chuyển sang
    /// <see cref="PausedState"/> (và mặc định là bất kỳ state nào khác).
    /// </summary>
    public class PlayingState : GameStateBase
    {
        public override void Tick(GameStateContext context, double deltaTime)
        {
            var puzzle = context.ActivePuzzle;
            if (puzzle == null)
            {
                return;
            }

            if (puzzle.Outcome == PuzzleOutcome.Won)
            {
                RequestState<WinState>(context);
            }
            else if (puzzle.Outcome == PuzzleOutcome.Lost)
            {
                RequestState<LoseState>(context);
            }
        }
    }
}
