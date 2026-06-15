using System.Collections.Generic;
using trungnhd.puzzlecore.Common;
using trungnhd.puzzlecore.Events;
using trungnhd.puzzlecore.Puzzle;

namespace trungnhd.puzzlecore.Boosters
{
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
