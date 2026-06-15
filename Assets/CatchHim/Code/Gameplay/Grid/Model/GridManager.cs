using trungnhd.puzzlecore.input;
using UnityEngine;

namespace CatchHim.Gameplay.Grid
{
    public class GridManager
    {
        public GridData[,] Cells { get; private set; }
        public int X { get; private set;}
        public int Y { get; private set;}
        public float CellSize { get; private set;}
        
        public Vector2 GetPositionAfterSwipe(GridData currentGrid, SwipeDirection swipeDirection)
        {
            return currentGrid.Position;
        }

        public Vector2 GetCellLocalPosition(Vector2Int coordinate)
        {
            Vector2 gridCenter = new Vector2(X * CellSize / 2, Y * CellSize / 2);
            return Cells[coordinate.x, coordinate.y].Position - gridCenter;
        }

        public void CreateGridMap()
        {
            X = 4;
            Y = 4;
            CellSize = 60;
            Cells = new GridData[X,Y];
            for (int width = 0; width < X; width++)
            {
                for (int height = 0; height < Y; height++)
                {
                    Vector2 position = new Vector2(width*CellSize , height*CellSize) + Vector2.one * CellSize/2;
                    if (width == 1 && height == 1)
                    {
                        Cells[width, height] = new GridData(width, height, position, new WallCellState());
                    }
                    else
                    {
                        Cells[width, height] = new GridData(width, height, position, new EmptyCellState());
                    }
                }
            }
        }
    }
}