using System;
using TMPro;
using UnityEngine;

public class GameCountdownUI : MonoBehaviour {

    private const string NUMBER_POPUP = "NumberPopup";

    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI labelText;

    private Animator animator;
    private int previousCountdownNumber;

    private void Awake() {
        animator = countdownText.GetComponent<Animator>();
    }

    private void Start() {
        if (KitchenGameManager.Instance != null) {
            KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        }

        UpdateVisual();
    }

    private void OnDestroy() {
        if (KitchenGameManager.Instance != null) {
            KitchenGameManager.Instance.OnStateChanged -= KitchenGameManager_OnStateChanged;
        }
    }

    private void Update() {
        UpdateVisual();
    }

    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        if (KitchenGameManager.Instance == null) {
            Hide();
            return;
        }

        if (KitchenGameManager.Instance.IsCountdownToStartActive()) {
            Show();

            if (labelText != null) {
                labelText.text = "Starting In";
            }

            if (countdownText != null) {
                int countdownNumber = Mathf.CeilToInt(KitchenGameManager.Instance.GetCountdownToStartTimer());
                countdownText.text = countdownNumber.ToString();

                if (previousCountdownNumber != countdownNumber) {
                    previousCountdownNumber = countdownNumber;
                    animator.SetTrigger(NUMBER_POPUP);
                    SoundManager.Instance.PlayCountdownSound();
                }
            }

            return;
        }

        if (KitchenGameManager.Instance.IsCountdownToGameOverActive()) {
            Show();

            if (labelText != null) {
                labelText.text = KitchenGameManager.Instance.IsLevelCompleted()
                    ? "Level Complete"
                    : "Game Over";
            }

            if (countdownText != null) {
                int countdownNumber = Mathf.CeilToInt(KitchenGameManager.Instance.GetCountdownToGameOverTimer());
                countdownText.text = countdownNumber.ToString();

                if (previousCountdownNumber != countdownNumber) {
                    previousCountdownNumber = countdownNumber;
                    animator.SetTrigger(NUMBER_POPUP);
                    SoundManager.Instance.PlayCountdownSound();
                }
            }

            return;
        }

        Hide();
    }

    private void Show() {
        if (container != null) {
            container.SetActive(true);
        }
    }

    private void Hide() {
        if (container != null) {
            container.SetActive(false);
        }
    }
}
