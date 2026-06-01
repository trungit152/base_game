using trungnhd.puzzlecore.Boosters;
using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Headless.Demo
{
    /// <summary>
    /// Effect booster mẫu: giảm puzzle đang chạy đi một lượng cố định, nhưng chỉ khi puzzle có khai
    /// báo capability <see cref="IDecrementable"/>. Effect không hề biết gì về CountdownState — chỉ
    /// biết đúng interface hẹp mà chính nó yêu cầu.
    /// </summary>
    public sealed class DecrementBoosterEffect : IBoosterEffect
    {
        private readonly int _amount;

        public DecrementBoosterEffect(int amount)
        {
            _amount = amount;
        }

        public Result Apply(BoosterActivationContext context)
        {
            if (context.Puzzle is IDecrementable decrementable)
            {
                return decrementable.Decrement(_amount);
            }
            return Result.Fail("puzzle does not support decrement");
        }
    }
}
