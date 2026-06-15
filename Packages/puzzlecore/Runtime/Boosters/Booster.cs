namespace trungnhd.puzzlecore.Boosters
{ 
    public sealed class Booster : IBooster
    {
        public IBoosterDefinition Definition { get; }
        public int Count { get; internal set; }

        public Booster(IBoosterDefinition definition, int count = 0)
        {
            Definition = definition;
            Count = count;
        }
    }
}
