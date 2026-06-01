using System.Collections.Generic;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Puzzle;
using trungnhd.puzzlecore.Signals;

namespace trungnhd.puzzlecore.Flow
{
    /// <summary>
    /// Các service dùng chung và một blackboard được trao cho mọi state. Giữ state nhẹ và tách rời:
    /// state đọc cộng tác viên từ đây thay vì phụ thuộc một DI container.
    /// </summary>
    public sealed class GameStateContext
    {
        public ISignalBus Signals { get; }
        public IClock Clock { get; }
        public IGameStateMachine Machine { get; }

        /// <summary>Puzzle đang được chơi; được gán bởi state loading, đọc bởi Playing/Win/Lose.</summary>
        public IPuzzleSession ActivePuzzle { get; set; }

        /// <summary>Lối thoát cho dữ liệu riêng của game, truyền giữa các state mà không cần kế thừa context.</summary>
        public IDictionary<string, object> Blackboard { get; }

        public GameStateContext(ISignalBus signals, IClock clock, IGameStateMachine machine)
        {
            Signals = signals;
            Clock = clock;
            Machine = machine;
            Blackboard = new Dictionary<string, object>();
        }
    }
}
