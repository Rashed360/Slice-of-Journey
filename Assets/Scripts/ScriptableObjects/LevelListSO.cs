using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu()]
public class LevelListSO : ScriptableObject {

    public List<LevelSO> levelSOList = new();

    public LevelSO GetLevelBySerial(int serial) {
        if (levelSOList == null) return null;
        for (int i = 0; i < levelSOList.Count; i++) {
            LevelSO lvl = levelSOList[i];
            if (lvl != null && lvl.levelSerial == serial) return lvl;
        }
        return null;
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if (levelSOList == null) return;
        levelSOList.Sort((a, b) => {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            return a.levelSerial.CompareTo(b.levelSerial);
        });
    }
#endif

}