using System.Collections.Generic;
using trungnhd.puzzlecore.Puzzle;

namespace trungnhd.puzzlecore.Boosters
{
    public interface IBoosterService
    {
        /// <summary>True nếu booster tồn tại, còn trong kho, và puzzle đang chạy & chưa kết thúc.</summary>
        bool CanActivate(string boosterId, IPuzzleSession puzzle);

        /// <summary>Trừ một booster và áp dụng effect của nó lên <paramref name="puzzle"/>.</summary>
        BoosterActivationResult Activate(string boosterId, IPuzzleSession puzzle, IReadOnlyDictionary<string, object> args = null);
    }
}
