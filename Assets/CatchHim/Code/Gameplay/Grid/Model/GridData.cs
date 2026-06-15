using UnityEngine;

namespace CatchHim.Gameplay.Grid
{
    public struct GridData
    {
        public int X;
        public int Y;
        public Vector2 Position;
        public ICellState State;

        public GridData(int x, int y, Vector2 position, ICellState state)
        {
            this.X = x;
            this.Y = y;
            Position = position;
            State = state;
        }
    }

}