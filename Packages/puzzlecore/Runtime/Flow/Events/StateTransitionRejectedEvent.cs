using System;
using trungnhd.puzzlecore.Events;

namespace trungnhd.puzzlecore.Flow.Events
{
    /// <summary>Phát ra khi một transition bị guard của state hiện tại chặn lại.</summary>
    public sealed class StateTransitionRejectedEvent : IEvent
    {
        public Type FromState { get; }
        public Type ToState { get; }
        public string Reason { get; }

        public StateTransitionRejectedEvent(Type fromState, Type toState, string reason)
        {
            FromState = fromState;
            ToState = toState;
            Reason = reason;
        }
    }
}
