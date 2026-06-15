namespace trungnhd.puzzlecore.Boosters
{
    public interface IBooster
    {
        IBoosterDefinition Definition { get; }
        int Count { get; }
    }
}
