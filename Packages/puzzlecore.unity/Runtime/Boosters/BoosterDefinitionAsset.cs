using trungnhd.puzzlecore.Boosters;
using UnityEngine;

namespace trungnhd.puzzlecore.Unity.Boosters
{
    /// <summary>
    /// Lớp cơ sở ScriptableObject hiện thực <see cref="IBoosterDefinition"/> để metadata booster có thể
    /// được tạo dưới dạng asset. Effect không serialize trực tiếp được (vì là interface), nên hãy tạo
    /// một lớp con cụ thể cho mỗi loại booster để dựng <see cref="Effect"/> — điều này giữ cho việc tạo
    /// booster trong Unity cũng theo Open/Closed (booster mới = loại asset mới, không sửa file này).
    /// </summary>
    public abstract class BoosterDefinitionAsset : ScriptableObject, IBoosterDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;

        public string Id => _id;
        public string DisplayName => _displayName;

        public abstract IBoosterEffect Effect { get; }
    }
}
