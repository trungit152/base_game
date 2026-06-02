using System.Collections.Generic;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Puzzle;

namespace trungnhd.puzzlecore.Boosters
{
    /// <summary>
    /// Mọi thứ một effect có thể cần, do service truyền vào. Effect tác động lên puzzle qua
    /// <see cref="IPuzzleSession"/> (không generic) — hoặc một capability interface hẹp mà nó ép kiểu
    /// xuống — chứ không qua kiểu state cụ thể. <see cref="Args"/> mang các tham số kích hoạt tuỳ chọn
    /// (vd ô / mục tiêu được chọn).
    /// </summary>
    public sealed class BoosterActivationContext
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyArgs = new Dictionary<string, object>();

        public IPuzzleSession Puzzle { get; }
        public IEventBus Events { get; }
        public IClock Clock { get; }
        public IReadOnlyDictionary<string, object> Args { get; }

        public BoosterActivationContext(IPuzzleSession puzzle, IEventBus events, IClock clock, IReadOnlyDictionary<string, object> args)
        {
            Puzzle = puzzle;
            Events = events;
            Clock = clock;
            Args = args ?? EmptyArgs;
        }
    }
}
