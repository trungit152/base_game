using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CatchHim.Gameplay.Actors
{
    [CreateAssetMenu(menuName = "CatchHim/Actor Visual Config")]
    public class ActorVisualConfig : ScriptableObject
    {
        [SerializeField] ActorVisualEntry[] _entries;
        Dictionary<ActorType, Sprite> _map;

        void OnEnable()
        {
            _map = _entries.ToDictionary(e => e.key, e => e.sprite);
        }

        public Sprite Resolve(ActorType key) => _map.TryGetValue(key, out var s) ? s : null;
    }

    [Serializable]
    public struct ActorVisualEntry
    {
        public ActorType key;
        public Sprite sprite;
    }
}
