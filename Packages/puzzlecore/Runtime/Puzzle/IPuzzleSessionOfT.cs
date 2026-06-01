using System.Collections.Generic;
using trungnhd.puzzlecore.Common;

namespace trungnhd.puzzlecore.Puzzle
{
    /// <summary>Góc nhìn có kiểu của một puzzle đang chạy, cho code sở hữu rules cụ thể.</summary>
    public interface IPuzzleSession<TState, TAction> : IPuzzleSession
    {
        TState State { get; }
        IReadOnlyList<TAction> LegalActions { get; }
        IPuzzleRules<TState, TAction> Rules { get; }
        Result ApplyAction(TAction action);
    }
}
