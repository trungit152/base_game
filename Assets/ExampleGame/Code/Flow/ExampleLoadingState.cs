using trungnhd.puzzlecore.Flow;
using trungnhd.puzzlecore.Flow.States;
using trungnhd.puzzlecore.Puzzle;
using ExampleGame.Puzzle;

namespace ExampleGame.Flow
{
    /// <summary>
    /// Điểm mở rộng chính của Flow cho game này: override <see cref="CreatePuzzle"/> để dựng
    /// <see cref="TapSession"/> cụ thể (Open/Closed — KHÔNG sửa framework). Sau khi puzzle sẵn sàng,
    /// phát <see cref="PuzzleReadyEvent"/> rồi để <c>LoadingState.Tick</c> (kế thừa) tự chuyển sang
    /// <c>PlayingState</c>.
    /// </summary>
    public sealed class ExampleLoadingState : LoadingState
    {
        private readonly TapRules _rules;

        public ExampleLoadingState(TapRules rules)
        {
            _rules = rules;
        }

        public override void Enter(GameStateContext context)
        {
            // Game này LUÔN dựng puzzle mới mỗi lần load để chơi lại được — chủ động bỏ qua cơ chế
            // "tái sử dụng ActivePuzzle" của LoadingState (né gap replay-terminal của lõi v1).
            context.ActivePuzzle = CreatePuzzle(context);
            context.Events.Publish(new PuzzleReadyEvent(context.ActivePuzzle));
        }

        protected override IPuzzleSession CreatePuzzle(GameStateContext context)
            => new TapSession(_rules, context.Events);
    }
}
