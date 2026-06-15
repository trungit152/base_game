namespace trungnhd.puzzlecore.Boosters
{
    public interface IBoosterDefinition
    {
        string Id { get; }
        string DisplayName { get; }

        IBoosterEffect Effect { get; }
    }
}
