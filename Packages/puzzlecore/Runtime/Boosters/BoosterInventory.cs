using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Boosters
{
    public sealed class BoosterInventory : IBoosterInventory
    {
        private readonly Dictionary<string, Booster> _boosters = new Dictionary<string, Booster>();

        public BoosterInventory(IEnumerable<IBoosterDefinition> definitions)
        {
            if (definitions == null) throw new ArgumentNullException(nameof(definitions));
            foreach (var def in definitions)
            {
                if (def == null) continue;
                _boosters[def.Id] = new Booster(def, 0);
            }
        }

        public int GetCount(string boosterId)
            => boosterId != null && _boosters.TryGetValue(boosterId, out var b) ? b.Count : 0;

        public bool Has(string boosterId) => GetCount(boosterId) > 0;

        public void Add(string boosterId, int amount)
        {
            if (amount <= 0) return;
            if (boosterId == null || !_boosters.TryGetValue(boosterId, out var b))
            {
                throw new InvalidOperationException("unknown booster: " + boosterId);
            }
            b.Count += amount;
        }

        public Result TryConsume(string boosterId, int amount = 1)
        {
            if (amount <= 0) return Result.Fail("amount must be positive");
            if (boosterId == null || !_boosters.TryGetValue(boosterId, out var b))
            {
                return Result.Fail("unknown booster: " + boosterId);
            }
            if (b.Count < amount)
            {
                return Result.Fail("insufficient: " + boosterId);
            }
            b.Count -= amount;
            return Result.Ok();
        }

        public IReadOnlyCollection<IBooster> All
        {
            get
            {
                var list = new List<IBooster>(_boosters.Count);
                foreach (var b in _boosters.Values) list.Add(b);
                return list;
            }
        }
    }
}
