using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.Boosters.Signals;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Puzzle;
using trungnhd.puzzlecore.Signals;

namespace trungnhd.puzzlecore.Boosters
{
    /// <summary>
    /// Điều phối việc kích hoạt booster: kiểm tra, trừ kho, áp dụng effect, và phát signal. Tổng quát
    /// trên mọi puzzle — chỉ biết <see cref="IBoosterEffect"/> và <see cref="IPuzzleSession"/>, không
    /// bao giờ biết state cụ thể của puzzle. Trừ trước và hoàn lại nếu effect thất bại, nên với bên gọi
    /// thì kết quả mang tính nguyên tử (atomic).
    /// </summary>
    public sealed class BoosterService : IBoosterService
    {
        private readonly IBoosterInventory _inventory;
        private readonly IReadOnlyDictionary<string, IBoosterDefinition> _definitions;
        private readonly ISignalBus _signals;
        private readonly IClock _clock;

        public BoosterService(
            IBoosterInventory inventory,
            IReadOnlyDictionary<string, IBoosterDefinition> definitions,
            ISignalBus signals,
            IClock clock)
        {
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _signals = signals ?? throw new ArgumentNullException(nameof(signals));
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

            var context = new BoosterActivationContext(puzzle, _signals, _clock, args);
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
            _signals.Publish(new BoosterActivatedSignal(boosterId, remaining));
            return BoosterActivationResult.Success(boosterId, remaining);
        }

        private BoosterActivationResult Fail(string boosterId, string reason)
        {
            _signals.Publish(new BoosterActivationFailedSignal(boosterId, reason));
            return BoosterActivationResult.Failure(boosterId, reason);
        }
    }
}
