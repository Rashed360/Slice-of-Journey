using UnityEngine;

public static class SelectedLevel {

    private const string PLAYER_PREFS_KEY = "KitchenGame_SelectedLevelSerial";

    public static int GetSelectedSerial() {
        return PlayerPrefs.GetInt(PLAYER_PREFS_KEY, 1);
    }

    public static void SetSelectedSerial(int serial) {
        PlayerPrefs.SetInt(PLAYER_PREFS_KEY, serial);
        PlayerPrefs.Save();
    }
}