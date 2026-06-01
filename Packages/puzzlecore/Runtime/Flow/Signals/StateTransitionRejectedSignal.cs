using System;
using trungnhd.puzzlecore.Signals;

namespace trungnhd.puzzlecore.Flow.Signals
{
    /// <summary>Phát ra khi một transition bị guard của state hiện tại chặn lại.</summary>
    public sealed class StateTransitionRejectedSignal : ISignal
    {
        public Type FromState { get; }
        public Type ToState { get; }
        public string Reason { get; }

        public StateTransitionRejectedSignal(Type fromState, Type toState, string reason)
        {
            FromState = fromState;
            ToState = toState;
            Reason = reason;
        }
    }
}
