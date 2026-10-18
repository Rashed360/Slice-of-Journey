using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientStockSingleUI : MonoBehaviour {

    private const string FLASH = "Flash";

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI stockAmountText;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void SetData(KitchenObjectSO kitchenObjectSO, int stockAmount) {
        if (kitchenObjectSO == null) return;

        if (iconImage != null) {
            iconImage.sprite = kitchenObjectSO.sprite;
        }

        if (stockAmountText != null) {
            stockAmountText.text = "x" + stockAmount;
        }
    }

    public void FlashRed() {
        animator?.SetTrigger(FLASH);
    }
}