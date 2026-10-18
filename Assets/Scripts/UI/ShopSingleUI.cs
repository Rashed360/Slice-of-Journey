using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSingleUI : MonoBehaviour {

    private const int MIN_AMOUNT = 1;
    private const int MAX_AMOUNT = 99;

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI stockText;

    [Header("Pricing")]
    [SerializeField] private TextMeshProUGUI discountText;
    [SerializeField] private TextMeshProUGUI unitPriceText;
    [SerializeField] private TextMeshProUGUI totalPriceText;

    [Header("Amount")]
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;

    [Header("Buy")]
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI buyButtonText;

    private KitchenObjectSO kitchenObjectSO;
    private int selectedAmount = 1;

    private void Awake() {
        minusButton?.onClick.AddListener(OnMinusClicked);

        plusButton?.onClick.AddListener(OnPlusClicked);

        buyButton?.onClick.AddListener(OnBuyClicked);
    }

    public void SetData(KitchenObjectSO kitchenObjectSO) {
        this.kitchenObjectSO = kitchenObjectSO;
        RefreshVisual();
    }

    public void RefreshVisual() {
        if (kitchenObjectSO == null || LevelInventoryManager.Instance == null) return;

        int stockAmount = LevelInventoryManager.Instance.GetStock(kitchenObjectSO);
        int basePrice = LevelInventoryManager.Instance.GetBuyPrice(kitchenObjectSO);
        int discountedUnitPrice = LevelInventoryManager.Instance.GetDiscountedUnitPrice(kitchenObjectSO);
        int discountPercent = LevelInventoryManager.Instance.GetDiscountPercentByRemainingTime();
        int totalPrice = discountedUnitPrice * selectedAmount;

        if (iconImage != null) {
            iconImage.sprite = kitchenObjectSO.sprite;
        }

        if (itemNameText != null) {
            itemNameText.text = kitchenObjectSO.objectName;
        }

        if (stockText != null) {
            stockText.text = "Stock: " + stockAmount;
        }

        if (discountText != null) {
            discountText.text = "-" + discountPercent + "%";
        }

        if (unitPriceText != null) {
            unitPriceText.text = discountedUnitPrice + " eur";
        }

        if (totalPriceText != null) {
            if (selectedAmount > 1) {
                totalPriceText.text = totalPrice.ToString();
            } else {
                totalPriceText.text = (basePrice != discountedUnitPrice
                    ? discountedUnitPrice + " (" + basePrice + ")"
                    : discountedUnitPrice.ToString()) + " eur";
            }
        }

        if (amountText != null) {
            amountText.text = selectedAmount.ToString();
        }

        if (minusButton != null) {
            minusButton.interactable = selectedAmount > MIN_AMOUNT;
        }

        if (plusButton != null) {
            plusButton.interactable = selectedAmount < MAX_AMOUNT;
        }

        bool canBuy = LevelInventoryManager.Instance.CanBuyAtDiscountedPrice(kitchenObjectSO, selectedAmount);

        if (buyButton != null) {
            buyButton.interactable = canBuy;
        }

        if (buyButtonText != null) {
            buyButtonText.text = canBuy ? "Order" : "No Money";
        }
    }

    private void OnMinusClicked() {
        selectedAmount = Mathf.Max(MIN_AMOUNT, selectedAmount - 1);
        RefreshVisual();
    }

    private void OnPlusClicked() {
        selectedAmount = Mathf.Min(MAX_AMOUNT, selectedAmount + 1);
        RefreshVisual();
    }

    private void OnBuyClicked() {
        if (LevelInventoryManager.Instance == null || kitchenObjectSO == null) return;

        bool success = LevelInventoryManager.Instance.TryBuyStockAtDiscountedPrice(kitchenObjectSO, selectedAmount);

        if (success) {
            selectedAmount = 1;
        }

        RefreshVisual();

        ShopUI.Instance?.RefreshAllItems();
    }
}