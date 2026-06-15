using CatchHim.Gameplay.Grid;
using UnityEngine;

namespace CatchHim
{
    public class WallCellState : ICellState
    {
        public bool CanGoThrough()
        {
            return false;
        }

        public CellVisual Visual => CellVisual.Wall;
    }
}
