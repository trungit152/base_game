namespace CatchHim.Gameplay.Grid
{
    public class EmptyCellState : ICellState
    {
        public bool CanGoThrough()
        {
            return true;
        }

        public CellVisual Visual => CellVisual.None;
    }
}