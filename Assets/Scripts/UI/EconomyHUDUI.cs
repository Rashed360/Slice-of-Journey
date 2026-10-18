using System;
using TMPro;
using UnityEngine;

public class EconomyHUDUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI globalMoneyText;

    [Header("Session Only")]
    [SerializeField] private GameObject sessionRoot;
    [SerializeField] private TextMeshProUGUI sessionEarnedText;
    [SerializeField] private TextMeshProUGUI sessionSpentText;
    [SerializeField] private TextMeshProUGUI sessionNetText;
    [SerializeField] private TextMeshProUGUI availableMoneyText;

    private void Start() {
        EconomyManager.EnsureGlobalMoneyExists();

        EconomyManager.OnEconomyChanged += EconomyManager_OnEconomyChanged;

        if (KitchenGameManager.Instance != null) {
            KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        }

        UpdateVisual();
    }

    private void OnDestroy() {
        EconomyManager.OnEconomyChanged -= EconomyManager_OnEconomyChanged;

        if (KitchenGameManager.Instance != null) {
            KitchenGameManager.Instance.OnStateChanged -= KitchenGameManager_OnStateChanged;
        }
    }

    private void EconomyManager_OnEconomyChanged() {
        UpdateVisual();
    }

    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        if (globalMoneyText != null) {
            globalMoneyText.text = EconomyManager.GetGlobalMoney().ToString("0");
        }

        bool showSession = ShouldShowSessionMoney();

        if (sessionRoot != null) {
            sessionRoot.SetActive(showSession);
        }

        if (!showSession) return;

        if (sessionEarnedText != null) {
            sessionEarnedText.text = EconomyManager.GetLevelEarnedMoney().ToString("0");
        }

        if (sessionSpentText != null) {
            sessionSpentText.text = EconomyManager.GetLevelSpentMoney().ToString("0");
        }

        if (sessionNetText != null) {
            int net = EconomyManager.GetLevelNetMoney();
            sessionNetText.text = net.ToString("0");
        }

        if (availableMoneyText != null) {
            availableMoneyText.text = EconomyManager.GetLevelAvailableMoney().ToString("0");
        }
    }

    private bool ShouldShowSessionMoney() {
        if (!EconomyManager.HasLevelSessionStarted()) {
            return false;
        }

        if (KitchenGameManager.Instance == null) {
            return false;
        }

        return true;
    }
}