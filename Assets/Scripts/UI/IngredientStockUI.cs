using System;
using UnityEngine;

public class IngredientStockUI : MonoBehaviour {

    [SerializeField] private Transform container;
    [SerializeField] private Transform stockItemTemplate;

    private void Awake() {
        stockItemTemplate.gameObject.SetActive(false);
    }

    private void Start() {
        if (LevelInventoryManager.Instance != null) {
            LevelInventoryManager.Instance.OnStockChanged += LevelInventoryManager_OnStockChanged;
        }

        UpdateVisual();
    }

    private void OnDestroy() {
        if (LevelInventoryManager.Instance != null) {
            LevelInventoryManager.Instance.OnStockChanged -= LevelInventoryManager_OnStockChanged;
        }
    }

    private void LevelInventoryManager_OnStockChanged(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        foreach (Transform child in container) {
            if (child == stockItemTemplate) continue;
            Destroy(child.gameObject);
        }

        if (LevelInventoryManager.Instance == null) return;

        IngredientStockSO[] ingredientArray = LevelInventoryManager.Instance.GetIngredientStockSOArray();
        if (ingredientArray == null) return;

        foreach (IngredientStockSO ingredientStockSO in ingredientArray) {
            if (ingredientStockSO == null) continue;
            if (ingredientStockSO.kitchenObjectSO == null) continue;

            Transform itemTransform = Instantiate(stockItemTemplate, container);
            itemTransform.gameObject.SetActive(true);

            int stockAmount = LevelInventoryManager.Instance.GetStock(ingredientStockSO.kitchenObjectSO);
            itemTransform.GetComponent<IngredientStockSingleUI>().SetData(
                ingredientStockSO.kitchenObjectSO,
                stockAmount
            );
        }
    }
}