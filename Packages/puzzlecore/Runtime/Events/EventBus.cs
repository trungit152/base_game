using System;
using System.Collections.Generic;

namespace trungnhd.puzzlecore.Events
{
    /// <summary>
    /// <see cref="IEventBus"/> mặc định: phát đồng bộ theo thứ tự đăng ký. Khi publish sẽ lặp trên
    /// một bản snapshot của danh sách handler, nên việc subscribe/unsubscribe ngay trong lúc phát là
    /// an toàn (re-entrancy). Cố tình đơn luồng — core mang tính tất định và chạy được headless.
    /// </summary>
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            var type = typeof(TEvent);
            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _handlers[type] = list;
            }
            list.Add(handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) return;
            if (_handlers.TryGetValue(typeof(TEvent), out var list))
            {
                list.Remove(handler);
            }
        }

        public void Publish<TEvent>(TEvent evt) where TEvent : IEvent
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var list) || list.Count == 0)
            {
                return;
            }

            // Snapshot để handler có thể (un)subscribe trong lúc phát mà không làm hỏng vòng lặp.
            var snapshot = list.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
            {
                ((Action<TEvent>)snapshot[i]).Invoke(evt);
            }
        }
    }
}
