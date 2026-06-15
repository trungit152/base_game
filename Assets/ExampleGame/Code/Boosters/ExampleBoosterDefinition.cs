using trungnhd.puzzlecore.Boosters;

namespace ExampleGame.Boosters
{
    /// <summary>
    /// Definition booster dạng class THUẦN (không ScriptableObject) để ví dụ chạy được hoàn toàn bằng
    /// code, không cần tạo file <c>.asset</c> trong editor. Khi muốn designer chỉnh trong Inspector,
    /// đổi sang kế thừa <c>BoosterDefinitionAsset</c> (ScriptableObject) của package puzzlecore.unity.
    /// </summary>
    public sealed class ExampleBoosterDefinition : IBoosterDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public IBoosterEffect Effect { get; }

        public ExampleBoosterDefinition(string id, string displayName, IBoosterEffect effect)
        {
            Id = id;
            DisplayName = displayName;
            Effect = effect;
        }
    }
}
