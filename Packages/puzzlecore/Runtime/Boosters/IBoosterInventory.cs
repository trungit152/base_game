using System.Collections.Generic;
using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Boosters
{
    public interface IBoosterInventory
    {
        int GetCount(string boosterId);
        bool Has(string boosterId);
        void Add(string boosterId, int amount);
        Result TryConsume(string boosterId, int amount = 1);

        IReadOnlyCollection<IBooster> All { get; }
    }
}
