using UnityEngine;

namespace CatchHim.Gameplay.Grid
{
    public struct GridData
    {
        public int X;
        public int Y;
    }

    public class GridManager
    {
        public ICellState[,] Cells;

        public Vector3 GetPositionAfterSwipe(GridData currentGrid)
        {
            return Vector3.zero;
        }

        public Vector3 GetGridPosition(GridData gridData)
        {
            return Vector3.zero;
        }

        public void CreateGridMap()
        {
            
        }
    }
}
