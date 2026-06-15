using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CatchHim.Gameplay.Grid
{
    [CreateAssetMenu(menuName = "CatchHim/Cell Visual Config")]                                                                                                                                                                       
    public class CellVisualConfig : ScriptableObject
    {
        [SerializeField] Entry[] _entries;
        Dictionary<CellVisual, Sprite> _map;

        void OnEnable()
        {
            _map = _entries.ToDictionary(e => e.key, e => e.sprite);
        }

        public Sprite Resolve(CellVisual key) => _map.TryGetValue(key, out var s) ? s : null;              
    }

    [Serializable]
    public struct Entry
    {
        public CellVisual key;
        public Sprite sprite;
    }                                                                                                                                    

}