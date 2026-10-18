using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI recipePriceText;
    [SerializeField] private TextMeshProUGUI recipeTimeText;
    [SerializeField] private Button discardButton;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private RecipeSO recipeSO;

    public void Awake() {
        iconTemplate.gameObject.SetActive(false);

        if (discardButton != null) {
            discardButton.onClick.AddListener(OnDiscardButtonClicked);
        }
    }

    public void SetRecipeSO(RecipeSO recipeSO) {
        this.recipeSO = recipeSO;

        recipeNameText.text = recipeSO.recipeName;
        recipePriceText.text = recipeSO.recipePrice + " eur";
        recipeTimeText.text = recipeSO.recipeTime + "-" + (recipeSO.recipeTime + 2) + " sec";

        foreach (Transform child in iconContainer) {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList) {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
    }

    private void OnDiscardButtonClicked() {
        if (recipeSO == null) return;
        if (DeliveryManager.Instance == null) return;
        // to do: add discard penalty (money/time?)
        DeliveryManager.Instance.DiscardRecipe(recipeSO);
    }
}