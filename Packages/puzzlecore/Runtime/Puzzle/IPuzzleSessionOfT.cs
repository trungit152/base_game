using System.Collections.Generic;
using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Puzzle
{
    public interface IPuzzleSession<TState, TAction> : IPuzzleSession
    {
        TState State { get; }
        IReadOnlyList<TAction> LegalActions { get; }
        IPuzzleRules<TState, TAction> Rules { get; }
        Result ApplyAction(TAction action);
    }
}
