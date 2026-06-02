using System;
using System.Collections.Generic;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Puzzle.Events;

namespace trungnhd.puzzlecore.Puzzle
{
    /// <summary>
    /// Session puzzle cụ thể duy nhất: giữ state hiện tại, kiểm tra và áp dụng action qua
    /// <see cref="IPuzzleRules{TState,TAction}"/>, tính lại outcome cùng danh sách action hợp lệ, và
    /// phát event. Engine-agnostic và tất định.
    /// </summary>
    public class PuzzleSession<TState, TAction> : IPuzzleSession<TState, TAction>
    {
        private readonly IEventBus _events;

        public IPuzzleRules<TState, TAction> Rules { get; }
        public TState State { get; private set; }
        public PuzzleOutcome Outcome { get; private set; }
        public IReadOnlyList<TAction> LegalActions { get; private set; }

        public bool IsTerminal => Outcome != PuzzleOutcome.Undecided;

        public PuzzleSession(IPuzzleRules<TState, TAction> rules, IEventBus events)
        {
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _events = events ?? throw new ArgumentNullException(nameof(events));
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

            _events.Publish(new PuzzleActionAppliedEvent(action, Outcome));
            if (Outcome != previousOutcome)
            {
                _events.Publish(new PuzzleOutcomeChangedEvent(previousOutcome, Outcome));
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
                _events.Publish(new PuzzleOutcomeChangedEvent(previousOutcome, Outcome));
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
