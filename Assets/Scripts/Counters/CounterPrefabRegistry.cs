using System.Collections.Generic;
using UnityEngine;

public class CounterPrefabRegistry : MonoBehaviour {
    [System.Serializable]
    public struct Entry {
        public CounterType type;
        public BaseCounter prefab;
    }

    [SerializeField] private List<Entry> entries;

    private Dictionary<CounterType, BaseCounter> lookup;

    private void Awake() {
        BuildLookup();
    }

    private void BuildLookup() {
        if (lookup != null) return;

        lookup = new Dictionary<CounterType, BaseCounter>();

        if (entries == null) return;

        foreach (Entry entry in entries) {
            if (entry.prefab == null) continue;

            if (!lookup.ContainsKey(entry.type)) {
                lookup.Add(entry.type, entry.prefab);
            }
            else {
                lookup[entry.type] = entry.prefab;
            }
        }
    }

    public BaseCounter GetPrefab(CounterType type) {
        BuildLookup();

        if (lookup.TryGetValue(type, out BaseCounter prefab)) {
            return prefab;
        }

        Debug.LogError($"CounterPrefabRegistry: No prefab registered for counter type {type}");
        return null;
    }
}