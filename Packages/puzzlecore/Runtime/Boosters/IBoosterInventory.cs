using System.Collections.Generic;
using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Boosters
{
    /// <summary>Sở hữu số lượng booster; nơi duy nhất stock được thay đổi.</summary>
    public interface IBoosterInventory
    {
        int GetCount(string boosterId);
        bool Has(string boosterId);
        void Add(string boosterId, int amount);

        /// <summary>Thử trừ <paramref name="amount"/>; thất bại nếu không đủ stock hoặc id không tồn tại.</summary>
        Result TryConsume(string boosterId, int amount = 1);

        IReadOnlyCollection<IBooster> All { get; }
    }
}
