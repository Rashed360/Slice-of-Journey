using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI playButtonText;
    [SerializeField] private Button customizeButton;
    [SerializeField] private Button quitButton;

    void Awake() {

        playButton.onClick.AddListener(OnPlayButtonClicked);

        customizeButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.CustomizeScene);
        });

        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });  

        Time.timeScale = 1f;
    }

    private void Start() {
        EconomyManager.ClearLevelSession();

        UpdatePlayButtonText();   
    }

    private void UpdatePlayButtonText() {
        bool hasProgress = LevelProgress.HasAnyProgress();

        if (playButtonText != null) {
            playButtonText.text = hasProgress ? "Continue"  : "New Game";
        }
    }

    private void OnPlayButtonClicked() {
        Loader.Load(Loader.Scene.LevelScene);
    }

}
