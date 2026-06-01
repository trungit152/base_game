namespace trungnhd.puzzlecore.Boosters
{
    /// <summary>Vật chứa số lượng có thể thay đổi. Kho (<see cref="IBoosterInventory"/>) là nơi duy nhất
    /// được phép đổi <see cref="Count"/> (setter ở mức internal).</summary>
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
