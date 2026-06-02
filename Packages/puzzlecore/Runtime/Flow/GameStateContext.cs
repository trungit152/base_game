using System.Collections.Generic;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Puzzle;

namespace trungnhd.puzzlecore.Flow
{
    /// <summary>
    /// Các service dùng chung và một blackboard được trao cho mọi state. Giữ state nhẹ và tách rời:
    /// state đọc cộng tác viên từ đây thay vì phụ thuộc một DI container.
    /// </summary>
    public sealed class GameStateContext
    {
        public IEventBus Events { get; }
        public IClock Clock { get; }
        public IGameStateMachine Machine { get; }

        /// <summary>Puzzle đang được chơi; được gán bởi state loading, đọc bởi Playing/Win/Lose.</summary>
        public IPuzzleSession ActivePuzzle { get; set; }

        /// <summary>Lối thoát cho dữ liệu riêng của game, truyền giữa các state mà không cần kế thừa context.</summary>
        public IDictionary<string, object> Blackboard { get; }

        public GameStateContext(IEventBus events, IClock clock, IGameStateMachine machine)
        {
            Events = events;
            Clock = clock;
            Machine = machine;
            Blackboard = new Dictionary<string, object>();
        }
    }
}
