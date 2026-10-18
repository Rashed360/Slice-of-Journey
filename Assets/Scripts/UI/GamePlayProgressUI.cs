using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayProgressUI : MonoBehaviour {

    [SerializeField] private Image orderProgressFill;
    [SerializeField] private Image timeProgressFill;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI orderProgressText;
    [SerializeField] private TextMeshProUGUI playerNameText;

    [SerializeField] private Button pauseButton;

    private void Awake() {
        pauseButton?.onClick.AddListener(() => {
            if (KitchenGameManager.Instance == null) return;
            KitchenGameManager.Instance.TogglePauseGame();
        });
    }

    private void Start() {
        if (playerNameText != null) {
            playerNameText.text = PlayerProfile.GetPlayerName();
        }
    }

    void Update() {
        UpdateTimeUI();
        UpdateOrderUI();
    }

    private void UpdateTimeUI() {
        float timeRemaining = KitchenGameManager.Instance.GetGamePlayingTimerRemaining();
        timeProgressFill.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();

        if (timeText != null) {
            int totalSeconds = Mathf.CeilToInt(timeRemaining);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void UpdateOrderUI() {
        int delivered = DeliveryManager.Instance.GetSuccessfulRecipesAmount();
        int target = KitchenGameManager.Instance.GetTargetDeliverCount();

        float progress = target > 0 ? (float)delivered / target : 0f;
        progress = Mathf.Clamp01(progress);

        orderProgressFill.fillAmount = progress;
        orderProgressText.text = $"Order Delivered: {delivered}/{target}";
    }
}
