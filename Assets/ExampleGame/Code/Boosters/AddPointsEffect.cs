using ExampleGame.Puzzle;
using trungnhd.puzzlecore.Boosters;
using trungnhd.puzzlecore.Common;

namespace ExampleGame.Boosters
{
    /// <summary>
    /// Effect booster: cộng một lượng điểm cố định, NHƯNG chỉ khi puzzle có khai báo
    /// <see cref="IScorable"/>. Effect không biết gì về <c>TapState</c> — chỉ biết đúng interface hẹp
    /// mà chính nó yêu cầu, nên hệ thống booster vẫn tổng quát trên mọi puzzle.
    /// </summary>
    public sealed class AddPointsEffect : IBoosterEffect
    {
        private readonly int _amount;

        public AddPointsEffect(int amount)
        {
            _amount = amount;
        }

        public Result Apply(BoosterActivationContext context)
        {
            if (context.Puzzle is IScorable scorable)
            {
                return scorable.AddPoints(_amount);
            }
            return Result.Fail("puzzle không hỗ trợ cộng điểm (IScorable)");
        }
    }
}
