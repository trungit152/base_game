using System;
using trungnhd.puzzlecore.Signals;

namespace trungnhd.puzzlecore.Flow.Signals
{
    /// <summary>Phát ra sau khi <c>Enter</c> của một state đã chạy xong và nó trở thành state hiện tại.</summary>
    public sealed class StateEnteredSignal : ISignal
    {
        public Type StateType { get; }
        public StateEnteredSignal(Type stateType) { StateType = stateType; }
    }
}
