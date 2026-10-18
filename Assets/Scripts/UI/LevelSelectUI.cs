using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour {

    [SerializeField] private LevelListSO levelListSO;

    [SerializeField] private TextMeshProUGUI levelNameText;

    [SerializeField] private Transform levelContainer;
    [SerializeField] private LevelSelectNodeUI levelTemplate;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backButton;

    private List<LevelSelectNodeUI> nodes = new();
    private LevelSO selectedLevel;

    public void Awake() {
        levelTemplate.gameObject.SetActive(false);

        continueButton.onClick.AddListener(() => {
            if (selectedLevel == null) return;
            Loader.Load(Loader.Scene.GameScene);
        });

        backButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    void Start() {
        BuildNodes();
        SelectInitialLevel();
        UpdateVisual();
    }

    private void BuildNodes() {
        foreach (LevelSO level in levelListSO.levelSOList) {
            LevelSelectNodeUI node = Instantiate(levelTemplate, levelContainer);
            node.gameObject.SetActive(true);
            node.SetLevel(level, OnNodeClicked);

            RectTransform rect = node.GetComponent<RectTransform>();
            rect.anchoredPosition = level.mapCoords;

            nodes.Add(node);
        }
    }

    private void OnNodeClicked(LevelSO level) {

        if (!LevelProgress.IsUnlocked(level.levelSerial))
            return;

        selectedLevel = level;
        levelNameText.text = level.levelName;

        SelectedLevel.SetSelectedSerial(level.levelSerial);

        UpdateVisual();
    }

    private void UpdateVisual() {
        foreach (var node in nodes)
            node.Refresh(selectedLevel);

        continueButton.interactable =
            selectedLevel != null &&
            LevelProgress.IsUnlocked(selectedLevel.levelSerial);
    }

    private void SelectInitialLevel() {

        int saved = SelectedLevel.GetSelectedSerial();
        LevelSO level = levelListSO.GetLevelBySerial(saved);

        if (level != null && LevelProgress.IsUnlocked(level.levelSerial)) {
            selectedLevel = level;
            levelNameText.text = level.levelName;
            SelectedLevel.SetSelectedSerial(level.levelSerial);
            return;
        }

        int highestUnlocked = LevelProgress.GetHighestUnlockedLevel();
        LevelSO latestLevel = levelListSO.GetLevelBySerial(highestUnlocked);

        if (latestLevel != null) {
            selectedLevel = latestLevel;
            levelNameText.text = latestLevel.levelName;
            SelectedLevel.SetSelectedSerial(latestLevel.levelSerial);
            return;
        }

        // fallback safety
        selectedLevel = null;
        levelNameText.text = "";
    }

}