namespace trungnhd.puzzlecore.Boosters
{
    /// <summary>Định nghĩa của một booster đi kèm số lượng đang sở hữu.</summary>
    public interface IBooster
    {
        IBoosterDefinition Definition { get; }
        int Count { get; }
    }
}
