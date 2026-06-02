using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Puzzle;

namespace trungnhd.puzzlecore.Headless.Demo
{
    /// <summary>
    /// Một Countdown session đồng thời hiện thực capability <see cref="IDecrementable"/>. Decrement áp
    /// dụng N action Tick hợp lệ qua rules, nên booster không bao giờ vượt mặt transition system hay
    /// chạm trực tiếp vào state.
    /// </summary>
    public sealed class CountdownSession : PuzzleSession<CountdownState, CountdownAction>, IDecrementable
    {
        public CountdownSession(CountdownRules rules, IEventBus events) : base(rules, events)
        {
        }

        public Result Decrement(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (IsTerminal) break;
                var result = ApplyAction(CountdownAction.Tick);
                if (result.IsFailure) return result;
            }
            return Result.Ok();
        }
    }
}
