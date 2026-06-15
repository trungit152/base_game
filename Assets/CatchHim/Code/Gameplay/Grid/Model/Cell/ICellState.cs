namespace CatchHim.Gameplay.Grid
{
    public enum CellVisual
    {
        None,
        Wall,
    }
    public interface ICellState
    {
        public bool CanGoThrough();
        public CellVisual Visual { get; }
    }
}