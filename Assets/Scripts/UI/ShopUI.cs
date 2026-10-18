using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour {

    public static ShopUI Instance { get; private set; }

    [SerializeField] private Button openShopButton;
    [SerializeField] private Button closeButton;

    [Header("Money Texts")]
    [SerializeField] private TextMeshProUGUI globalMoneyText;
    [SerializeField] private TextMeshProUGUI levelNetMoneyText;
    [SerializeField] private TextMeshProUGUI availableMoneyText;

    [Header("Shop List")]
    [SerializeField] private Transform container;
    [SerializeField] private Transform shopItemTemplate;

    private readonly List<ShopSingleUI> shopSingleUIList = new();

    private bool isOpen;

    private void Awake() {
        Instance = this;

        openShopButton?.onClick.AddListener(Toggle);

        closeButton?.onClick.AddListener(Hide);

        shopItemTemplate?.gameObject.SetActive(false);
    }

    private void Start() {
        if (LevelInventoryManager.Instance != null) {
            LevelInventoryManager.Instance.OnStockChanged += LevelInventoryManager_OnStockChanged;
            LevelInventoryManager.Instance.OnShopPurchaseSuccess += LevelInventoryManager_OnShopPurchaseSuccess;
            LevelInventoryManager.Instance.OnShopPurchaseFailed += LevelInventoryManager_OnShopPurchaseFailed;
        }

        HideImmediate();
        UpdateVisual();
    }

    private void OnDestroy() {
        if (LevelInventoryManager.Instance != null) {
            LevelInventoryManager.Instance.OnStockChanged -= LevelInventoryManager_OnStockChanged;
            LevelInventoryManager.Instance.OnShopPurchaseSuccess -= LevelInventoryManager_OnShopPurchaseSuccess;
            LevelInventoryManager.Instance.OnShopPurchaseFailed -= LevelInventoryManager_OnShopPurchaseFailed;
        }
    }

    private void Update() {
        if (!isOpen) return;

        UpdateMoneyTexts();
        RefreshAllItems();
    }

    private void LevelInventoryManager_OnStockChanged(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void LevelInventoryManager_OnShopPurchaseSuccess(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void LevelInventoryManager_OnShopPurchaseFailed(object sender, EventArgs e) {
        UpdateVisual();
    }

    public void Toggle() {
        if (isOpen) {
            Hide();
        } else {
            Show();
        }
    }

    public void Show() {
        if (isOpen) return;

        isOpen = true;
        gameObject.SetActive(true);

        if (KitchenGameManager.Instance != null && !IsGameAlreadyPaused()) {
            KitchenGameManager.Instance.TogglePauseGame();
        }

        UpdateVisual();
    }

    public void Hide() {
        if (!isOpen) return;

        isOpen = false;
        gameObject.SetActive(false);

        if (KitchenGameManager.Instance != null && IsGameAlreadyPaused()) {
            KitchenGameManager.Instance.TogglePauseGame();
        }
    }

    private void HideImmediate() {
        isOpen = false;
        gameObject.SetActive(false);
    }

    private bool IsGameAlreadyPaused() {
        return KitchenGameManager.Instance != null && KitchenGameManager.Instance.IsGamePaused();
    }

    private void UpdateVisual() {
        UpdateMoneyTexts();
        RebuildShopList();
    }

    private void UpdateMoneyTexts() {
        if (globalMoneyText != null) {
            globalMoneyText.text = "Wallet: " + EconomyManager.GetGlobalMoney() + "€";
        }

        if (levelNetMoneyText != null) {
            levelNetMoneyText.text = "Run Net: " + EconomyManager.GetLevelNetMoney() + "€";
        }

        if (availableMoneyText != null) {
            availableMoneyText.text = "Available: " + EconomyManager.GetLevelAvailableMoney() + "€";
        }
    }

    private void RebuildShopList() {
        shopSingleUIList.Clear();

        foreach (Transform child in container) {
            if (child == shopItemTemplate) continue;
            Destroy(child.gameObject);
        }

        if (LevelInventoryManager.Instance == null) return;

        IngredientStockSO[] ingredientArray = LevelInventoryManager.Instance.GetIngredientStockSOArray();
        if (ingredientArray == null) return;

        foreach (IngredientStockSO ingredientStockSO in ingredientArray) {
            if (ingredientStockSO == null || ingredientStockSO.kitchenObjectSO == null) continue;

            Transform itemTransform = Instantiate(shopItemTemplate, container);
            itemTransform.gameObject.SetActive(true);

            ShopSingleUI shopSingleUI = itemTransform.GetComponent<ShopSingleUI>();
            shopSingleUI.SetData(ingredientStockSO.kitchenObjectSO);
            shopSingleUIList.Add(shopSingleUI);
        }
    }

    public void RefreshAllItems() {
        foreach (ShopSingleUI shopSingleUI in shopSingleUIList) {
            shopSingleUI?.RefreshVisual();
        }
    }
}