using System;
using trungnhd.puzzlecore.Events;

namespace trungnhd.puzzlecore.Flow.Events
{
    /// <summary>Phát ra sau khi <c>Enter</c> của một state đã chạy xong và nó trở thành state hiện tại.</summary>
    public sealed class StateEnteredEvent : IEvent
    {
        public Type StateType { get; }
        public StateEnteredEvent(Type stateType) { StateType = stateType; }
    }
}
