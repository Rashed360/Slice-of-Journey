using UnityEngine;

public static class PlayerProfile {

    private const string PLAYER_PREFS_NAME = "PlayerProfile_Name";
    private const string PLAYER_PREFS_MATERIAL_INDEX = "PlayerProfile_MaterialIndex";

    private static readonly string[] firstParts = {
        "User", "Journey", "Road", "Slice", "Swift", "Lucky", "Pizza", "Burger"
    };

    private static readonly string[] secondParts = {
        "Man", "Star", "Hero", "Chef", "Driver", "Rider", "Master", "Buddy"
    };

    public static string GetPlayerName() {
        if (!PlayerPrefs.HasKey(PLAYER_PREFS_NAME)) {
            string generatedName = GenerateRandomName();
            SetPlayerName(generatedName);
            return generatedName;
        }

        string savedName = PlayerPrefs.GetString(PLAYER_PREFS_NAME, "").Trim();

        if (string.IsNullOrWhiteSpace(savedName)) {
            string generatedName = GenerateRandomName();
            SetPlayerName(generatedName);
            return generatedName;
        }

        return savedName;
    }

    public static void SetPlayerName(string playerName) {
        string finalName = playerName.Trim();

        if (string.IsNullOrWhiteSpace(finalName)) {
            finalName = GenerateRandomName();
        }

        PlayerPrefs.SetString(PLAYER_PREFS_NAME, finalName);
        PlayerPrefs.Save();
    }

    public static int GetMaterialIndex() {
        return PlayerPrefs.GetInt(PLAYER_PREFS_MATERIAL_INDEX, 0);
    }

    public static void SetMaterialIndex(int materialIndex) {
        PlayerPrefs.SetInt(PLAYER_PREFS_MATERIAL_INDEX, materialIndex);
        PlayerPrefs.Save();
    }

    public static void ResetProfile() {
        PlayerPrefs.DeleteKey(PLAYER_PREFS_NAME);
        PlayerPrefs.DeleteKey(PLAYER_PREFS_MATERIAL_INDEX);
        PlayerPrefs.Save();
    }

    private static string GenerateRandomName() {
        string first = firstParts[Random.Range(0, firstParts.Length)];
        string second = secondParts[Random.Range(0, secondParts.Length)];
        int number = Random.Range(10, 99);

        return first + second + number;
    }
}