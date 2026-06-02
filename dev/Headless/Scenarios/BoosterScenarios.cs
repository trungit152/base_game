using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.Boosters;
using trungnhd.puzzlecore.Boosters.Events;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Puzzle;
using trungnhd.puzzlecore.Headless.Demo;
using trungnhd.puzzlecore.Headless.Doubles;

namespace trungnhd.puzzlecore.Headless.Scenarios
{
    /// <summary>Kiểm chứng kích hoạt booster: trừ kho + tác động + event, hết kho, hoàn lại, guard terminal.</summary>
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
            var (service, inventory, events) = Build(amount: 3);
            inventory.Add(BoosterId, 1);
            Asserts.Expect(inventory.Has(BoosterId) && inventory.GetCount(BoosterId) == 1, "inventory seeded +1");

            int activated = 0;
            events.Subscribe<BoosterActivatedEvent>(_ => activated++);

            var puzzle = new CountdownSession(new CountdownRules(3, 10), events);
            var result = service.Activate(BoosterId, puzzle);

            Asserts.Expect(result.Activated, "activation succeeded");
            Asserts.ExpectEqual(0, result.RemainingCount, "remaining count is 0 after consume");
            Asserts.Expect(puzzle.Outcome == PuzzleOutcome.Won, "decrement(3) drove the puzzle to Won");
            Asserts.ExpectEqual(1, activated, "BoosterActivatedEvent raised once (consume + mutate + event)");
        }

        private static void InsufficientStock()
        {
            var (service, inventory, events) = Build(amount: 3);
            int failed = 0;
            events.Subscribe<BoosterActivationFailedEvent>(_ => failed++);

            var puzzle = new CountdownSession(new CountdownRules(3, 10), events);
            var result = service.Activate(BoosterId, puzzle);

            Asserts.Expect(!result.Activated && result.Error.Contains("insufficient"), "empty inventory -> insufficient");
            Asserts.ExpectEqual(0, inventory.GetCount(BoosterId), "count stays 0 when nothing was consumed");
            Asserts.ExpectEqual(1, failed, "BoosterActivationFailedEvent raised on insufficient stock");
        }

        private static void RefundOnEffectFailure()
        {
            var (service, inventory, events) = Build(amount: 3);
            inventory.Add(BoosterId, 2);
            int failed = 0;
            events.Subscribe<BoosterActivationFailedEvent>(_ => failed++);

            // Session thuần KHÔNG có capability IDecrementable -> effect thất bại.
            var puzzle = new PuzzleSession<CountdownState, CountdownAction>(new CountdownRules(3, 10), events);
            var result = service.Activate(BoosterId, puzzle);

            Asserts.Expect(!result.Activated, "effect fails on a puzzle lacking the capability");
            Asserts.ExpectEqual(2, inventory.GetCount(BoosterId), "inventory refunded to 2 after effect failure");
            Asserts.ExpectEqual(1, failed, "BoosterActivationFailedEvent raised on effect failure");
        }

        private static void TerminalPuzzle()
        {
            var (service, inventory, events) = Build(amount: 3);
            inventory.Add(BoosterId, 1);

            var puzzle = new CountdownSession(new CountdownRules(1, 10), events);
            puzzle.ApplyAction(CountdownAction.Tick); // Remaining 0 -> Won (terminal)

            var result = service.Activate(BoosterId, puzzle);
            Asserts.Expect(!result.Activated && result.Error == "no active/usable puzzle", "activating on a terminal puzzle fails");
            Asserts.ExpectEqual(1, inventory.GetCount(BoosterId), "inventory is untouched when the puzzle is terminal");
        }

        private static (IBoosterService service, IBoosterInventory inventory, IEventBus events) Build(int amount)
        {
            var events = new EventBus();
            var definition = new CountdownBoosterDefinition(BoosterId, "Decrement", new DecrementBoosterEffect(amount));
            var definitions = new IBoosterDefinition[] { definition };
            var inventory = new BoosterInventory(definitions);
            var lookup = new Dictionary<string, IBoosterDefinition> { { definition.Id, definition } };
            var service = new BoosterService(inventory, lookup, events, new ManualClock());
            return (service, inventory, events);
        }
    }
}
