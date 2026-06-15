using trungnhd.puzzlecore.Boosters;
using UnityEngine;

namespace trungnhd.puzzlecore.Unity.Boosters
{
    public abstract class BoosterDefinitionAsset : ScriptableObject, IBoosterDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;

        public string Id => _id;
        public string DisplayName => _displayName;

        public abstract IBoosterEffect Effect { get; }
    }
}
