using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour {

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnDeliverySuccess;
    public event EventHandler OnDeliveryFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private readonly float spawnRecipeTimerMax = 4f;
    private readonly float waitingRecipeMax = 4;
    private int successfulRecipesAmount;

    public void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    public void Update() {
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax) {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public bool DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
                bool plateContentMatchesRecipe = true;

                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    bool ingredientFound = false;

                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound) {
                        plateContentMatchesRecipe = false;
                        break;
                    }
                }

                if (plateContentMatchesRecipe) {
                    // baked/raw must match
                    if (waitingRecipeSO.requiresBaking != plateKitchenObject.IsBaked()) {
                        continue;
                    }

                    successfulRecipesAmount++;
                    waitingRecipeSOList.RemoveAt(i);

                    EconomyManager.AddLevelEarning(waitingRecipeSO.recipePrice);

                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnDeliverySuccess?.Invoke(this, EventArgs.Empty);

                    return true; // SUCCESS
                }
            }
        }

        // FAILED
        OnDeliveryFailed?.Invoke(this, EventArgs.Empty);
        return false;
    }

    public void DiscardRecipe(RecipeSO recipeSO) {
        if (recipeSO == null) return;

        if (waitingRecipeSOList.Remove(recipeSO)) {
            OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount() {
        return successfulRecipesAmount;
    }
}