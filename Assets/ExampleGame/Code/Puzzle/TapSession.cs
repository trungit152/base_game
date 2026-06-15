using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Puzzle;

namespace ExampleGame.Puzzle
{
    /// <summary>
    /// Session cụ thể của game, đồng thời hiện thực <see cref="IScorable"/>. <see cref="AddPoints"/> áp
    /// dụng N action <c>Tap</c> HỢP LỆ qua rules — nên booster KHÔNG bao giờ vượt mặt transition system
    /// hay sờ trực tiếp vào state thô.
    /// </summary>
    public sealed class TapSession : PuzzleSession<TapState, TapAction>, IScorable
    {
        public TapSession(TapRules rules, IEventBus events) : base(rules, events)
        {
        }

        public Result AddPoints(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (IsTerminal) break;
                var result = ApplyAction(TapAction.Tap);
                if (result.IsFailure) return result;
            }
            return Result.Ok();
        }
    }
}
