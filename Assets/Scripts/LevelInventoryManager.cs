using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelInventoryManager : MonoBehaviour {

    public static LevelInventoryManager Instance { get; private set; }

    public event EventHandler OnStockChanged;
    public event EventHandler OnShopPurchaseSuccess;
    public event EventHandler OnShopPurchaseFailed;

    [SerializeField] private LevelListSO levelListSO;
    [SerializeField] private IngredientStockSO[] ingredientStockSOArray;

    private readonly Dictionary<KitchenObjectSO, int> stockLookup = new();
    private readonly Dictionary<KitchenObjectSO, IngredientStockSO> ingredientConfigLookup = new();

    private LevelSO currentLevel;

    private void Awake() {
        Instance = this;
        BuildIngredientConfigLookup();
        LoadSelectedLevelStock();
    }

    private void BuildIngredientConfigLookup() {
        ingredientConfigLookup.Clear();

        if (ingredientStockSOArray == null) return;

        foreach (IngredientStockSO ingredientStockSO in ingredientStockSOArray) {
            if (ingredientStockSO == null) continue;
            if (ingredientStockSO.kitchenObjectSO == null) continue;

            ingredientConfigLookup[ingredientStockSO.kitchenObjectSO] = ingredientStockSO;
        }
    }

    private void LoadSelectedLevelStock() {
        stockLookup.Clear();

        if (levelListSO == null) {
            Debug.LogError("LevelInventoryManager: LevelListSO not assigned.");
            return;
        }

        int selectedSerial = SelectedLevel.GetSelectedSerial();
        currentLevel = levelListSO.GetLevelBySerial(selectedSerial);

        if (currentLevel == null) {
            Debug.LogError($"LevelInventoryManager: No level found for serial {selectedSerial}");
            return;
        }

        if (currentLevel.startingStockArray != null) {
            foreach (LevelSO.StartingStockEntry entry in currentLevel.startingStockArray) {
                if (entry.kitchenObjectSO == null) continue;

                int safeAmount = Mathf.Max(0, entry.amount);

                if (stockLookup.ContainsKey(entry.kitchenObjectSO)) {
                    stockLookup[entry.kitchenObjectSO] += safeAmount;
                } else {
                    stockLookup.Add(entry.kitchenObjectSO, safeAmount);
                }
            }
        }

        OnStockChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetStock(KitchenObjectSO kitchenObjectSO) {
        if (kitchenObjectSO == null) return 0;

        if (stockLookup.TryGetValue(kitchenObjectSO, out int amount)) {
            return amount;
        }

        return 0;
    }

    public bool HasStock(KitchenObjectSO kitchenObjectSO) {
        return GetStock(kitchenObjectSO) > 0;
    }

    public bool TryConsumeStock(KitchenObjectSO kitchenObjectSO) {
        if (kitchenObjectSO == null) return false;

        int currentStock = GetStock(kitchenObjectSO);
        if (currentStock <= 0) {
            return false;
        }

        stockLookup[kitchenObjectSO] = currentStock - 1;
        OnStockChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public void AddStock(KitchenObjectSO kitchenObjectSO, int amount) {
        if (kitchenObjectSO == null || amount <= 0) return;

        if (stockLookup.ContainsKey(kitchenObjectSO)) {
            stockLookup[kitchenObjectSO] += amount;
        } else {
            stockLookup.Add(kitchenObjectSO, amount);
        }

        OnStockChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetBuyPrice(KitchenObjectSO kitchenObjectSO) {
        if (kitchenObjectSO == null) return -1;

        if (ingredientConfigLookup.TryGetValue(kitchenObjectSO, out IngredientStockSO config)) {
            return Mathf.Max(0, config.buyPrice);
        }

        return -1;
    }

    public bool CanBuy(KitchenObjectSO kitchenObjectSO, int amount) {
        if (kitchenObjectSO == null || amount <= 0) return false;

        int buyPrice = GetBuyPrice(kitchenObjectSO);
        if (buyPrice < 0) return false;

        int totalCost = buyPrice * amount;
        return EconomyManager.GetLevelAvailableMoney() >= totalCost;
    }

    public bool TryBuyStock(KitchenObjectSO kitchenObjectSO, int amount) {
        if (kitchenObjectSO == null || amount <= 0) {
            OnShopPurchaseFailed?.Invoke(this, EventArgs.Empty);
            return false;
        }

        int buyPrice = GetBuyPrice(kitchenObjectSO);
        if (buyPrice < 0) {
            Debug.LogWarning($"LevelInventoryManager: No buy config for {kitchenObjectSO.name}");
            OnShopPurchaseFailed?.Invoke(this, EventArgs.Empty);
            return false;
        }

        int totalCost = buyPrice * amount;

        if (!EconomyManager.TrySpendInLevel(totalCost)) {
            OnShopPurchaseFailed?.Invoke(this, EventArgs.Empty);
            return false;
        }

        AddStock(kitchenObjectSO, amount);
        OnShopPurchaseSuccess?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public int GetDiscountPercentByRemainingTime() {
        if (KitchenGameManager.Instance == null) return 0;

        float normalized = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();

        // More remaining time = better discount
        // 100% time left -> 50% discount
        // 0% time left   -> 0% discount
        int discountPercent = Mathf.RoundToInt(normalized * 50f);

        return Mathf.Clamp(discountPercent, 0, 50);
    }

    public int GetDiscountedUnitPrice(KitchenObjectSO kitchenObjectSO) {
        int basePrice = GetBuyPrice(kitchenObjectSO);
        if (basePrice < 0) return -1;

        int discountPercent = GetDiscountPercentByRemainingTime();

        float multiplier = 1f - (discountPercent / 100f);

        // no less than half of original price
        multiplier = Mathf.Max(0.5f, multiplier);

        int discountedPrice = Mathf.CeilToInt(basePrice * multiplier);
        int minimumAllowed = Mathf.CeilToInt(basePrice * 0.5f);

        return Mathf.Max(discountedPrice, minimumAllowed);
    }

    public bool CanBuyAtDiscountedPrice(KitchenObjectSO kitchenObjectSO, int amount) {
        if (kitchenObjectSO == null || amount <= 0) return false;

        int discountedUnitPrice = GetDiscountedUnitPrice(kitchenObjectSO);
        if (discountedUnitPrice < 0) return false;

        int totalCost = discountedUnitPrice * amount;
        return EconomyManager.GetLevelAvailableMoney() >= totalCost;
    }

    public bool TryBuyStockAtDiscountedPrice(KitchenObjectSO kitchenObjectSO, int amount) {
        if (kitchenObjectSO == null || amount <= 0) {
            OnShopPurchaseFailed?.Invoke(this, EventArgs.Empty);
            return false;
        }

        int discountedUnitPrice = GetDiscountedUnitPrice(kitchenObjectSO);
        if (discountedUnitPrice < 0) {
            OnShopPurchaseFailed?.Invoke(this, EventArgs.Empty);
            return false;
        }

        int totalCost = discountedUnitPrice * amount;

        if (!EconomyManager.TrySpendInLevel(totalCost)) {
            OnShopPurchaseFailed?.Invoke(this, EventArgs.Empty);
            return false;
        }

        AddStock(kitchenObjectSO, amount);
        OnShopPurchaseSuccess?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public LevelSO GetCurrentLevel() {
        return currentLevel;
    }

    public Dictionary<KitchenObjectSO, int> GetStockSnapshot() {
        return new Dictionary<KitchenObjectSO, int>(stockLookup);
    }

    public IngredientStockSO[] GetIngredientStockSOArray() {
        return ingredientStockSOArray;
    }
}