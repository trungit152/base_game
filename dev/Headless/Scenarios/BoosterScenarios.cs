using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.Boosters;
using trungnhd.puzzlecore.Boosters.Signals;
using trungnhd.puzzlecore.Puzzle;
using trungnhd.puzzlecore.Signals;
using trungnhd.puzzlecore.Headless.Demo;
using trungnhd.puzzlecore.Headless.Doubles;

namespace trungnhd.puzzlecore.Headless.Scenarios
{
    /// <summary>Kiểm chứng kích hoạt booster: trừ kho + tác động + signal, hết kho, hoàn lại, guard terminal.</summary>
    public static class BoosterScenarios
    {
        private const string BoosterId = "decrement";

        public static void Run()
        {
            Console.WriteLine("[Booster]");
            HappyPath();
            InsufficientStock();
            RefundOnEffectFailure();
            TerminalPuzzle();
        }

        private static void HappyPath()
        {
            var (service, inventory, signals) = Build(amount: 3);
            inventory.Add(BoosterId, 1);
            Asserts.Expect(inventory.Has(BoosterId) && inventory.GetCount(BoosterId) == 1, "inventory seeded +1");

            int activated = 0;
            signals.Subscribe<BoosterActivatedSignal>(_ => activated++);

            var puzzle = new CountdownSession(new CountdownRules(3, 10), signals);
            var result = service.Activate(BoosterId, puzzle);

            Asserts.Expect(result.Activated, "activation succeeded");
            Asserts.ExpectEqual(0, result.RemainingCount, "remaining count is 0 after consume");
            Asserts.Expect(puzzle.Outcome == PuzzleOutcome.Won, "decrement(3) drove the puzzle to Won");
            Asserts.ExpectEqual(1, activated, "BoosterActivatedSignal raised once (consume + mutate + signal)");
        }

        private static void InsufficientStock()
        {
            var (service, inventory, signals) = Build(amount: 3);
            int failed = 0;
            signals.Subscribe<BoosterActivationFailedSignal>(_ => failed++);

            var puzzle = new CountdownSession(new CountdownRules(3, 10), signals);
            var result = service.Activate(BoosterId, puzzle);

            Asserts.Expect(!result.Activated && result.Error.Contains("insufficient"), "empty inventory -> insufficient");
            Asserts.ExpectEqual(0, inventory.GetCount(BoosterId), "count stays 0 when nothing was consumed");
            Asserts.ExpectEqual(1, failed, "BoosterActivationFailedSignal raised on insufficient stock");
        }

        private static void RefundOnEffectFailure()
        {
            var (service, inventory, signals) = Build(amount: 3);
            inventory.Add(BoosterId, 2);
            int failed = 0;
            signals.Subscribe<BoosterActivationFailedSignal>(_ => failed++);

            // Session thuần KHÔNG có capability IDecrementable -> effect thất bại.
            var puzzle = new PuzzleSession<CountdownState, CountdownAction>(new CountdownRules(3, 10), signals);
            var result = service.Activate(BoosterId, puzzle);

            Asserts.Expect(!result.Activated, "effect fails on a puzzle lacking the capability");
            Asserts.ExpectEqual(2, inventory.GetCount(BoosterId), "inventory refunded to 2 after effect failure");
            Asserts.ExpectEqual(1, failed, "BoosterActivationFailedSignal raised on effect failure");
        }

        private static void TerminalPuzzle()
        {
            var (service, inventory, signals) = Build(amount: 3);
            inventory.Add(BoosterId, 1);

            var puzzle = new CountdownSession(new CountdownRules(1, 10), signals);
            puzzle.ApplyAction(CountdownAction.Tick); // Remaining 0 -> Won (terminal)

            var result = service.Activate(BoosterId, puzzle);
            Asserts.Expect(!result.Activated && result.Error == "no active/usable puzzle", "activating on a terminal puzzle fails");
            Asserts.ExpectEqual(1, inventory.GetCount(BoosterId), "inventory is untouched when the puzzle is terminal");
        }

        private static (IBoosterService service, IBoosterInventory inventory, ISignalBus signals) Build(int amount)
        {
            var signals = new SignalBus();
            var definition = new CountdownBoosterDefinition(BoosterId, "Decrement", new DecrementBoosterEffect(amount));
            var definitions = new IBoosterDefinition[] { definition };
            var inventory = new BoosterInventory(definitions);
            var lookup = new Dictionary<string, IBoosterDefinition> { { definition.Id, definition } };
            var service = new BoosterService(inventory, lookup, signals, new ManualClock());
            return (service, inventory, signals);
        }
    }
}
