using System;
using UnityEngine;

public static class EconomyManager {

    private const string PLAYER_PREFS_GLOBAL_MONEY = "KitchenGame_GlobalMoney_v1";
    private const int DEFAULT_STARTING_MONEY = 1000;

    public static event Action OnEconomyChanged;

    private static bool levelSessionStarted = false;
    private static int levelEarnedMoney = 0;
    private static int levelSpentMoney = 0;
    private static bool levelResultCommitted = false;

    private static void TriggerChanged() {
        OnEconomyChanged?.Invoke();
    }

    public static void EnsureGlobalMoneyExists() {
        if (!PlayerPrefs.HasKey(PLAYER_PREFS_GLOBAL_MONEY)) {
            PlayerPrefs.SetInt(PLAYER_PREFS_GLOBAL_MONEY, DEFAULT_STARTING_MONEY);
            PlayerPrefs.Save();
            TriggerChanged();
        }
    }

    public static int GetGlobalMoney() {
        EnsureGlobalMoneyExists();
        return PlayerPrefs.GetInt(PLAYER_PREFS_GLOBAL_MONEY, DEFAULT_STARTING_MONEY);
    }

    public static void SetGlobalMoney(int amount) {
        amount = Mathf.Max(0, amount);
        PlayerPrefs.SetInt(PLAYER_PREFS_GLOBAL_MONEY, amount);
        PlayerPrefs.Save();
        TriggerChanged();
    }

    public static void AddToGlobalMoney(int amount) {
        if (amount <= 0) return;
        SetGlobalMoney(GetGlobalMoney() + amount);
    }

    public static void StartLevelSession() {
        levelSessionStarted = true;
        levelEarnedMoney = 0;
        levelSpentMoney = 0;
        levelResultCommitted = false;
        EnsureGlobalMoneyExists();
        TriggerChanged();
    }

    public static bool HasLevelSessionStarted() {
        return levelSessionStarted;
    }

    public static void AddLevelEarning(int amount) {
        if (!levelSessionStarted) StartLevelSession();
        if (amount <= 0) return;

        levelEarnedMoney += amount;
        TriggerChanged();
    }

    public static bool TrySpendInLevel(int amount) {
        if (!levelSessionStarted) StartLevelSession();
        if (amount <= 0) return true;

        int available = GetLevelAvailableMoney();
        if (amount > available) return false;

        levelSpentMoney += amount;
        TriggerChanged();
        return true;
    }

    public static int GetLevelEarnedMoney() {
        return levelEarnedMoney;
    }

    public static int GetLevelSpentMoney() {
        return levelSpentMoney;
    }

    public static int GetLevelNetMoney() {
        return levelEarnedMoney - levelSpentMoney;
    }

    public static int GetLevelAvailableMoney() {
        return GetGlobalMoney() + GetLevelNetMoney();
    }

    public static void CommitLevelResultToGlobal() {
        if (!levelSessionStarted || levelResultCommitted) return;

        int net = GetLevelNetMoney();
        if (net > 0) {
            AddToGlobalMoney(net);
        }

        levelResultCommitted = true;
        TriggerChanged();
    }

    public static void EndLevelSessionWithoutCommit() {
        levelResultCommitted = true;
        TriggerChanged();
    }

    public static void ClearLevelSession() {
        levelSessionStarted = false;
        levelEarnedMoney = 0;
        levelSpentMoney = 0;
        levelResultCommitted = false;
        TriggerChanged();
    }

    public static void ResetAllEconomy() {
        SetGlobalMoney(DEFAULT_STARTING_MONEY);
        levelSessionStarted = false;
        levelEarnedMoney = 0;
        levelSpentMoney = 0;
        levelResultCommitted = false;
        TriggerChanged();
    }
}