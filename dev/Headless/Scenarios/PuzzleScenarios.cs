using System;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Puzzle;
using trungnhd.puzzlecore.Puzzle.Events;
using trungnhd.puzzlecore.Headless.Demo;

namespace trungnhd.puzzlecore.Headless.Scenarios
{
    /// <summary>Kiểm chứng seam transition của puzzle: action hợp lệ, phát hiện thắng/thua, guard terminal, event.</summary>
    public static class PuzzleScenarios
    {
        public static void Run()
        {
            Console.WriteLine("[Puzzle]");

            var events = new EventBus();
            int applied = 0;
            int outcomeChanged = 0;
            events.Subscribe<PuzzleActionAppliedEvent>(_ => applied++);
            events.Subscribe<PuzzleOutcomeChangedEvent>(_ => outcomeChanged++);

            var session = new PuzzleSession<CountdownState, CountdownAction>(new CountdownRules(3, 10), events);

            Asserts.Expect(session.Outcome == PuzzleOutcome.Undecided, "fresh session is Undecided");
            Asserts.Expect(session.LegalActions.Count == 1 && session.LegalActions[0] == CountdownAction.Tick,
                "fresh session legal actions = [Tick]");

            Asserts.Expect(session.ApplyAction(CountdownAction.Tick).IsSuccess, "apply tick #1 ok");
            Asserts.Expect(session.ApplyAction(CountdownAction.Tick).IsSuccess, "apply tick #2 ok");
            Asserts.Expect(session.ApplyAction(CountdownAction.Tick).IsSuccess, "apply tick #3 ok");

            Asserts.Expect(session.Outcome == PuzzleOutcome.Won, "after 3 ticks -> Won");
            Asserts.Expect(session.IsTerminal, "won session is terminal");
            Asserts.Expect(session.LegalActions.Count == 0, "terminal session has no legal actions");

            var terminal = session.ApplyAction(CountdownAction.Tick);
            Asserts.Expect(terminal.IsFailure && terminal.Error == "session is terminal", "applying on a terminal session fails");

            Asserts.ExpectEqual(3, applied, "PuzzleActionAppliedEvent fired once per successful apply");
            Asserts.ExpectEqual(1, outcomeChanged, "PuzzleOutcomeChangedEvent fired exactly once (Undecided -> Won)");

            // Đường thua
            var lose = new PuzzleSession<CountdownState, CountdownAction>(new CountdownRules(5, 3), events);
            lose.ApplyAction(CountdownAction.Tick);
            lose.ApplyAction(CountdownAction.Tick);
            lose.ApplyAction(CountdownAction.Tick);
            Asserts.Expect(lose.Outcome == PuzzleOutcome.Lost, "start 5, budget 3, 3 ticks -> Lost");

            // Sai kiểu action qua facade không generic
            IPuzzleSession facade = new PuzzleSession<CountdownState, CountdownAction>(new CountdownRules(3, 10), events);
            Asserts.Expect(facade.ApplyAction("not-an-action").IsFailure, "wrong action type via facade fails");
        }
    }
}
