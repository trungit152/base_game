using System;
using trungnhd.puzzlecore.Flow;
using trungnhd.puzzlecore.Flow.Signals;
using trungnhd.puzzlecore.Flow.States;
using trungnhd.puzzlecore.Signals;
using trungnhd.puzzlecore.Headless.Demo;
using trungnhd.puzzlecore.Headless.Doubles;

namespace trungnhd.puzzlecore.Headless.Scenarios
{
    /// <summary>Kiểm chứng state machine luồng game: transition, guard từ chối, state chưa đăng ký.</summary>
    public static class FlowScenarios
    {
        private sealed class UnregisteredState : GameStateBase { }

        public static void Run()
        {
            Console.WriteLine("[Flow]");
            HappyPath();
            GuardsAndErrors();
        }

        private static void HappyPath()
        {
            var signals = new SignalBus();
            var clock = new ManualClock();

            int entered = 0;
            signals.Subscribe<StateEnteredSignal>(_ => entered++);

            var puzzle = new CountdownSession(new CountdownRules(3, 10), signals);
            var machine = new GameStateMachine(signals, clock, puzzle);
            RegisterStandardStates(machine);

            Asserts.Expect(machine.ChangeState<BootState>().IsSuccess, "enter Boot ok");
            machine.Tick(0);
            Asserts.Expect(machine.Current is MainMenuState, "Boot auto-advances to MainMenu on tick");
            Asserts.ExpectEqual(2, entered, "two StateEntered signals fired (Boot, MainMenu)");

            Asserts.Expect(machine.ChangeState<LoadingState>().IsSuccess, "MainMenu -> Loading ok");
            machine.Tick(0);
            Asserts.Expect(machine.Current is PlayingState, "Loading advances to Playing (ActivePuzzle present)");

            puzzle.ApplyAction(CountdownAction.Tick);
            puzzle.ApplyAction(CountdownAction.Tick);
            puzzle.ApplyAction(CountdownAction.Tick);
            machine.Tick(0);
            Asserts.Expect(machine.Current is WinState, "Playing advances to Win when the puzzle is won");
        }

        private static void GuardsAndErrors()
        {
            var signals = new SignalBus();
            var clock = new ManualClock();

            StateTransitionRejectedSignal rejected = null;
            signals.Subscribe<StateTransitionRejectedSignal>(s => rejected = s);

            var machine = new GameStateMachine(signals, clock);
            RegisterStandardStates(machine);

            machine.ChangeState<PlayingState>();
            Asserts.Expect(machine.ChangeState<PausedState>().IsSuccess, "Playing -> Paused ok");

            var rej = machine.ChangeState<WinState>();
            Asserts.Expect(rej.IsFailure, "Paused -> Win rejected by guard");
            Asserts.Expect(machine.Current is PausedState, "Current is unchanged after a rejected transition");
            Asserts.Expect(rejected != null
                           && rejected.FromState == typeof(PausedState)
                           && rejected.ToState == typeof(WinState),
                "StateTransitionRejected signal carries the correct From/To");

            var unreg = machine.ChangeState(typeof(UnregisteredState));
            Asserts.Expect(unreg.IsFailure, "changing to an unregistered state fails");
            Asserts.Expect(machine.Current is PausedState, "Current is unchanged after an unregistered transition");
        }

        private static void RegisterStandardStates(GameStateMachine machine)
        {
            machine.RegisterState(new BootState());
            machine.RegisterState(new MainMenuState());
            machine.RegisterState(new LoadingState());
            machine.RegisterState(new PlayingState());
            machine.RegisterState(new PausedState());
            machine.RegisterState(new WinState());
            machine.RegisterState(new LoseState());
        }
    }
}
