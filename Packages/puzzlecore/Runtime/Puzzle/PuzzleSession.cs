using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Puzzle.Signals;
using trungnhd.puzzlecore.Signals;

namespace trungnhd.puzzlecore.Puzzle
{
    /// <summary>
    /// Session puzzle cụ thể duy nhất: giữ state hiện tại, kiểm tra và áp dụng action qua
    /// <see cref="IPuzzleRules{TState,TAction}"/>, tính lại outcome cùng danh sách action hợp lệ, và
    /// phát signal. Engine-agnostic và tất định.
    /// </summary>
    public class PuzzleSession<TState, TAction> : IPuzzleSession<TState, TAction>
    {
        private readonly ISignalBus _signals;

        public IPuzzleRules<TState, TAction> Rules { get; }
        public TState State { get; private set; }
        public PuzzleOutcome Outcome { get; private set; }
        public IReadOnlyList<TAction> LegalActions { get; private set; }

        public bool IsTerminal => Outcome != PuzzleOutcome.Undecided;

        public PuzzleSession(IPuzzleRules<TState, TAction> rules, ISignalBus signals)
        {
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _signals = signals ?? throw new ArgumentNullException(nameof(signals));
            State = rules.InitialState;
            Recompute();
        }

        public Result ApplyAction(TAction action)
        {
            if (IsTerminal)
            {
                return Result.Fail("session is terminal");
            }

            if (!IsLegal(action))
            {
                return Result.Fail("illegal action");
            }

            var previousOutcome = Outcome;
            State = Rules.Apply(State, action);
            Recompute();

            _signals.Publish(new PuzzleActionAppliedSignal(action, Outcome));
            if (Outcome != previousOutcome)
            {
                _signals.Publish(new PuzzleOutcomeChangedSignal(previousOutcome, Outcome));
            }

            return Result.Ok();
        }

        public Result ApplyAction(object action)
        {
            if (action is TAction typed)
            {
                return ApplyAction(typed);
            }
            return Result.Fail("action is not of type " + typeof(TAction).Name);
        }

        public void Reset()
        {
            var previousOutcome = Outcome;
            State = Rules.InitialState;
            Recompute();
            if (Outcome != previousOutcome)
            {
                _signals.Publish(new PuzzleOutcomeChangedSignal(previousOutcome, Outcome));
            }
        }

        private void Recompute()
        {
            Outcome = Rules.GetOutcome(State);
            LegalActions = Rules.GetLegalActions(State);
        }

        private bool IsLegal(TAction action)
        {
            var legal = LegalActions;
            if (legal == null)
            {
                return false;
            }

            var comparer = EqualityComparer<TAction>.Default;
            for (int i = 0; i < legal.Count; i++)
            {
                if (comparer.Equals(legal[i], action))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
