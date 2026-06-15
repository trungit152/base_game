using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.Boosters.Events;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Puzzle;

namespace trungnhd.puzzlecore.Boosters
{
    public sealed class BoosterService : IBoosterService
    {
        private readonly IBoosterInventory _inventory;
        private readonly IReadOnlyDictionary<string, IBoosterDefinition> _definitions;
        private readonly IEventBus _events;
        private readonly IClock _clock;

        public BoosterService(
            IBoosterInventory inventory,
            IReadOnlyDictionary<string, IBoosterDefinition> definitions,
            IEventBus events,
            IClock clock)
        {
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _clock = clock;
        }

        public bool CanActivate(string boosterId, IPuzzleSession puzzle)
        {
            if (string.IsNullOrEmpty(boosterId)) return false;
            if (!_definitions.ContainsKey(boosterId)) return false;
            if (puzzle == null || puzzle.IsTerminal) return false;
            return _inventory.Has(boosterId);
        }

        public BoosterActivationResult Activate(string boosterId, IPuzzleSession puzzle, IReadOnlyDictionary<string, object> args = null)
        {
            if (string.IsNullOrEmpty(boosterId) || !_definitions.TryGetValue(boosterId, out var definition))
            {
                return Fail(boosterId, "unknown booster");
            }

            if (puzzle == null || puzzle.IsTerminal)
            {
                return Fail(boosterId, "no active/usable puzzle");
            }

            var consume = _inventory.TryConsume(boosterId, 1);
            if (consume.IsFailure)
            {
                return Fail(boosterId, consume.Error);
            }

            var context = new BoosterActivationContext(puzzle, _events, _clock, args);
            Result effect;
            try
            {
                effect = definition.Effect.Apply(context);
            }
            catch (Exception ex)
            {
                _inventory.Add(boosterId, 1); // hoàn lại khi gặp lỗi bất ngờ
                return Fail(boosterId, "effect threw: " + ex.Message);
            }

            if (effect.IsFailure)
            {
                _inventory.Add(boosterId, 1); // hoàn lại: booster coi như chưa được dùng
                return Fail(boosterId, effect.Error);
            }

            var remaining = _inventory.GetCount(boosterId);
            _events.Publish(new BoosterActivatedEvent(boosterId, remaining));
            return BoosterActivationResult.Success(boosterId, remaining);
        }

        private BoosterActivationResult Fail(string boosterId, string reason)
        {
            _events.Publish(new BoosterActivationFailedEvent(boosterId, reason));
            return BoosterActivationResult.Failure(boosterId, reason);
        }
    }
}
