using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeUI : MonoBehaviour {

    [Header("Name")]
    [SerializeField] private TMP_InputField nameInputField;

    [Header("Buttons")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button optionsButton;
    
    [SerializeField] private Button saveAndBackButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button[] colorButtons;


    [Header("Preview")]
    [SerializeField] private PlayerCustomizationVisual previewVisual;

    private int selectedMaterialIndex;

    private void Awake() {
        resetButton.onClick.AddListener(ResetProgress);  
        saveAndBackButton.onClick.AddListener(SaveAndBack);
        leaveButton.onClick.AddListener(LeaveWithoutSaving);

        optionsButton.onClick.AddListener(() => {
            OptionsUI.Instance.Show();
        });

        for (int i = 0; i < colorButtons.Length; i++) {
            int index = i;
            colorButtons[i].onClick.AddListener(() => SelectColor(index));
        }
    }

    private void Start() {
        nameInputField.text = PlayerProfile.GetPlayerName();
        selectedMaterialIndex = PlayerProfile.GetMaterialIndex();

        if (previewVisual != null) {
            previewVisual.ApplyMaterial(selectedMaterialIndex);
        }
    }

    private void SelectColor(int index) {
        selectedMaterialIndex = index;

        if (previewVisual != null) {
            previewVisual.ApplyMaterial(selectedMaterialIndex);
        }
    }

    private void SaveAndBack() {
        PlayerProfile.SetPlayerName(nameInputField.text);
        PlayerProfile.SetMaterialIndex(selectedMaterialIndex);

        Loader.Load(Loader.Scene.MainMenuScene);
    }

    private void LeaveWithoutSaving() {
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    public void ResetProgress() {
        LevelProgress.ResetAll();
        SelectedLevel.SetSelectedSerial(1);
        EconomyManager.ResetAllEconomy();
    }
}