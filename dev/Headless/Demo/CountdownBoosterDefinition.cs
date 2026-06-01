using trungnhd.puzzlecore.Boosters;

namespace trungnhd.puzzlecore.Headless.Demo
{
    /// <summary>Definition booster thuần (không Unity) dùng cho harness headless.</summary>
    public sealed class CountdownBoosterDefinition : IBoosterDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public IBoosterEffect Effect { get; }

        public CountdownBoosterDefinition(string id, string displayName, IBoosterEffect effect)
        {
            Id = id;
            DisplayName = displayName;
            Effect = effect;
        }
    }
}
