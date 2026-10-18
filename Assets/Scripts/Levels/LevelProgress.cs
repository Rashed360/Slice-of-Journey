using System;
using System.Collections.Generic;
using UnityEngine;

public static class LevelProgress {

    private const string PLAYER_PREFS_KEY = "KitchenGame_LevelProgress_v1";

    [Serializable]
    private class SaveData {
        public List<int> unlockedSerials = new List<int>();
        public List<int> completedSerials = new List<int>();
    }

    private static SaveData cached;


    private static SaveData LoadOrCreate() {
        if (cached != null) return cached;

        if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY)) {
            string json = PlayerPrefs.GetString(PLAYER_PREFS_KEY);
            try {
                cached = JsonUtility.FromJson<SaveData>(json);
            } catch {
                cached = null;
            }
        }

        if (cached == null) {
            cached = new SaveData();
            // Default: only level 1 unlocked
            cached.unlockedSerials.Add(1);
            Save();
        }

        if (cached.unlockedSerials == null) cached.unlockedSerials = new List<int>();
        if (cached.completedSerials == null) cached.completedSerials = new List<int>();

        return cached;
    }

    private static void Save() {
        if (cached == null) return;
        string json = JsonUtility.ToJson(cached);
        PlayerPrefs.SetString(PLAYER_PREFS_KEY, json);
        PlayerPrefs.Save();
    }

    private static bool Contains(List<int> list, int serial) {
        if (list == null) return false;
        for (int i = 0; i < list.Count; i++) {
            if (list[i] == serial) return true;
        }
        return false;
    }

    private static void AddUnique(List<int> list, int serial) {
        if (list == null) return;
        if (!Contains(list, serial)) list.Add(serial);
    }

    private static void RemoveIfExists(List<int> list, int serial) {
        if (list == null) return;
        for (int i = list.Count - 1; i >= 0; i--) {
            if (list[i] == serial) list.RemoveAt(i);
        }
    }


    public static bool IsUnlocked(int serial) {
        SaveData data = LoadOrCreate();
        return Contains(data.unlockedSerials, serial);
    }

    public static bool IsCompleted(int serial) {
        SaveData data = LoadOrCreate();
        return Contains(data.completedSerials, serial);
    }

    public static void Unlock(int serial) {
        SaveData data = LoadOrCreate();
        AddUnique(data.unlockedSerials, serial);
        Save();
    }

    public static void SetCompleted(int serial, bool completed) {
        SaveData data = LoadOrCreate();

        if (completed) {
            AddUnique(data.completedSerials, serial);
        } else {
            RemoveIfExists(data.completedSerials, serial);
        }

        Save();
    }

    public static void CompleteLevelAndUnlockNext(int completedSerial, int nextSerial) {
        SetCompleted(completedSerial, true);
        Unlock(nextSerial);
    }

    public static bool HasAnyProgress() {
        // level 1 is completed OR any level >1 is unlocked
        if (IsCompleted(1)) return true;

        // any higher level is unlocked
        for (int i = 2; i <= 50; i++) {
            if (IsUnlocked(i)) return true;
        }

        return false;
    }

    public static int GetHighestUnlockedLevel() {
        int highest = 1;

        for (int i = 2; i <= 50; i++) {
            if (IsUnlocked(i)) {
                highest = i;
            } else {
                break;
            }
        }

        return highest;
    }

    public static void ResetAll() {
        cached = new SaveData();
        cached.unlockedSerials.Add(1);
        Save();
    }
}