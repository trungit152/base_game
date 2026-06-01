using System;

namespace trungnhd.puzzlecore.Signals
{
    /// <summary>
    /// Hub publish/subscribe đồng bộ, trong tiến trình. Tách rời bên phát khỏi bên nhận để các flow
    /// state, puzzle session và booster có thể phát sự kiện mà không cần tham chiếu tầng trình bày.
    /// </summary>
    public interface ISignalBus
    {
        void Subscribe<TSignal>(Action<TSignal> handler) where TSignal : ISignal;
        void Unsubscribe<TSignal>(Action<TSignal> handler) where TSignal : ISignal;
        void Publish<TSignal>(TSignal signal) where TSignal : ISignal;
    }
}
