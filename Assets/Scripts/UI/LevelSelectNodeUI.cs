using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectNodeUI : MonoBehaviour {

    [SerializeField] private Button button;
    [SerializeField] private GameObject unlockedVisual; 
    [SerializeField] private GameObject selectedGlow;
    [SerializeField] private GameObject completedMark; 

    private LevelSO level;
    private Action<LevelSO> onClick;

    public void SetLevel(LevelSO level, Action<LevelSO> onClick) {
        this.level = level;
        this.onClick = onClick;

        button.onClick.AddListener(() => onClick?.Invoke(level));
    }

    public void Refresh(LevelSO selectedLevel) {
        if (level == null) return;

        bool unlocked = LevelProgress.IsUnlocked(level.levelSerial);
        bool completed = LevelProgress.IsCompleted(level.levelSerial);
        bool selected = selectedLevel != null && selectedLevel.levelSerial == level.levelSerial;

        button.interactable = unlocked;

        unlockedVisual?.SetActive(unlocked);
        completedMark?.SetActive(completed);
        selectedGlow?.SetActive(selected);
    }
}