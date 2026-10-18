using UnityEngine;

public class LevelBuilder : MonoBehaviour {
    [SerializeField] private LevelListSO levelListSO;
    [SerializeField] private CounterPrefabRegistry prefabRegistry;

    [Header("Game Scene Slots")]
    [SerializeField] private BaseCounter slot1;
    [SerializeField] private BaseCounter slot2;
    [SerializeField] private BaseCounter slot3;
    [SerializeField] private BaseCounter slot4;
    [SerializeField] private BaseCounter slot5;
    [SerializeField] private BaseCounter slot6;
    [SerializeField] private BaseCounter slot7;
    [SerializeField] private BaseCounter slot8;
    [SerializeField] private BaseCounter slot9;
    [SerializeField] private BaseCounter slot10;
    [SerializeField] private BaseCounter slot11;
    [SerializeField] private BaseCounter slot12;
    [SerializeField] private BaseCounter slot13;
    [SerializeField] private BaseCounter slot14;
    [SerializeField] private BaseCounter slot15;

private BaseCounter[] slotArray;

    private void Awake() {
        CacheSlots();
        BuildSelectedLevel();
    }

    private void CacheSlots() {
        slotArray = new BaseCounter[] {
            slot1, slot2, slot3, slot4, slot5,
            slot6, slot7, slot8, slot9, slot10,
            slot11, slot12, slot13, slot14, slot15
        };
    }

    private void BuildSelectedLevel() {
        if (levelListSO == null) {
            Debug.LogError("LevelBuilder: LevelListSO is not assigned.");
            return;
        }

        if (prefabRegistry == null) {
            Debug.LogError("LevelBuilder: CounterPrefabRegistry is not assigned.");
            return;
        }

        int selectedSerial = SelectedLevel.GetSelectedSerial();
        LevelSO levelSO = levelListSO.GetLevelBySerial(selectedSerial);

        if (levelSO == null) {
            Debug.LogError($"LevelBuilder: No LevelSO found for selected serial {selectedSerial}.");
            return;
        }

        CounterType[] layout = levelSO.GetSlots();

        if (layout == null || layout.Length != slotArray.Length) {
            Debug.LogError($"LevelBuilder: Level '{levelSO.levelName}' layout is invalid. Expected {slotArray.Length} slots.");
            return;
        }

        if (Player.Instance != null) {
            Player.Instance.ClearSelectedCounter();
        }

        for (int i = 0; i < slotArray.Length; i++) {
            BuildSlot(i, layout[i]);
        }
    }

private void BuildSlot(int index, CounterType counterType) {
    BaseCounter placeholder = slotArray[index];

    if (placeholder == null) {
        Debug.LogWarning($"LevelBuilder: Slot {index + 1} placeholder is missing.");
        return;
    }

    if (counterType == CounterType.ClearCounter) {
        return;
    }

    BaseCounter prefab = prefabRegistry.GetPrefab(counterType);
    if (prefab == null) {
        Debug.LogError($"LevelBuilder: No prefab found for {counterType} in slot {index + 1}.");
        return;
    }

    Vector3 spawnPosition = placeholder.transform.position;
    Quaternion spawnRotation = placeholder.transform.rotation;
    Transform parent = placeholder.transform.parent;

    Destroy(placeholder.gameObject);

    BaseCounter spawnedCounter = Instantiate(prefab, spawnPosition, spawnRotation, parent);
    slotArray[index] = spawnedCounter;
}
}