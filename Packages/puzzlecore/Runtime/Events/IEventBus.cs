using System;

namespace trungnhd.puzzlecore.Events
{
    /// <summary>
    /// Hub publish/subscribe đồng bộ, trong tiến trình. Tách rời bên phát khỏi bên nhận để các flow
    /// state, puzzle session và booster có thể phát sự kiện mà không cần tham chiếu tầng trình bày.
    /// </summary>
    public interface IEventBus
    {
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
        void Publish<TEvent>(TEvent evt) where TEvent : IEvent;
    }
}
