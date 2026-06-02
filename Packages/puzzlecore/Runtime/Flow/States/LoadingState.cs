using trungnhd.puzzlecore.Puzzle;

namespace trungnhd.puzzlecore.Flow.States
{
    /// <summary>
    /// Chuẩn bị puzzle cho màn chơi, rồi chuyển sang <see cref="PlayingState"/> khi
    /// <see cref="GameStateContext.ActivePuzzle"/> đã sẵn sàng.
    /// <para>
    /// Điểm mở rộng: override <see cref="CreatePuzzle"/> để dựng session cụ thể cho game của bạn (nó
    /// nhận event bus từ context). Hoặc gán <see cref="GameStateContext.ActivePuzzle"/> trước khi vào
    /// state này.
    /// </para>
    /// </summary>
    public class LoadingState : GameStateBase
    {
        public override void Enter(GameStateContext context)
        {
            if (context.ActivePuzzle == null)
            {
                context.ActivePuzzle = CreatePuzzle(context);
            }
        }

        public override void Tick(GameStateContext context, double deltaTime)
        {
            if (context.ActivePuzzle != null)
            {
                RequestState<PlayingState>(context);
            }
        }

        /// <summary>Dựng puzzle session cho màn chơi này. Mặc định trả về null
        /// (thay vào đó hãy gán <see cref="GameStateContext.ActivePuzzle"/> từ bên ngoài).</summary>
        protected virtual IPuzzleSession CreatePuzzle(GameStateContext context) => null;
    }
}
