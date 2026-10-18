using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI ordersCompletedText;
    [SerializeField] private TextMeshProUGUI moneyEarnedText;
    [SerializeField] private TextMeshProUGUI moneySpentText;
    [SerializeField] private TextMeshProUGUI actionButtonText;

    [Header("Buttons")]
    [SerializeField] private Button actionButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button levelMapButton;

    private void Awake() {
        actionButton.onClick.AddListener(OnActionButtonClicked);

        mainMenuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        levelMapButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.LevelScene);
        });
    }

    private void Start() {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e) {
        if (KitchenGameManager.Instance.IsGameOver()) {
            UpdateVisual();
            Show();
        } else {
            Hide();
        }
    }

    private void UpdateVisual() {
        int delivered = DeliveryManager.Instance.GetSuccessfulRecipesAmount();
        int target = KitchenGameManager.Instance.GetTargetDeliverCount();
        bool completed = KitchenGameManager.Instance.IsLevelCompleted();

        if (resultText != null) {
            resultText.text = completed ? "Level Complete" : "Game Over";
        }

        if (ordersCompletedText != null) {
            ordersCompletedText.text = $"{delivered}/{target}";
        }

        if (moneyEarnedText != null) {
            moneyEarnedText.text = EconomyManager.GetLevelEarnedMoney().ToString();
        }

        if (moneySpentText != null) {
            moneySpentText.text = EconomyManager.GetLevelSpentMoney().ToString();
        }

        if (actionButtonText != null) {
            if (completed && KitchenGameManager.Instance.HasNextLevel()) {
                actionButtonText.text = "Next Level";
            } else {
                actionButtonText.text = "Play Again";
            }
        }
    }

    private void OnActionButtonClicked() {
        bool completed = KitchenGameManager.Instance.IsLevelCompleted();

        if (completed && KitchenGameManager.Instance.HasNextLevel()) {
            int nextLevelSerial = KitchenGameManager.Instance.GetNextLevelSerial();
            SelectedLevel.SetSelectedSerial(nextLevelSerial);
        } else {
            int currentLevelSerial = KitchenGameManager.Instance.GetCurrentLevelSerial();
            SelectedLevel.SetSelectedSerial(currentLevelSerial);
        }

        Loader.Load(Loader.Scene.GameScene);
    }

    private void Show() {
        gameObject.SetActive(true);
        actionButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}