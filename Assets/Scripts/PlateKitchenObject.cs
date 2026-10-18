using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public event EventHandler<OnBakedStateChangedEventArgs> OnBakedStateChanged;

    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO kitchenObjectSO;
    }

    public class OnBakedStateChangedEventArgs : EventArgs {
        public bool isBaked;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    private List<KitchenObjectSO> kitchenObjectSOList;
    private bool isBaked = false;

    private void Awake() {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (isBaked) {
            return false;
        }

        if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) {
            return false;
        }

        if (kitchenObjectSOList.Contains(kitchenObjectSO)) {
            return false;
        }

        kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
            kitchenObjectSO = kitchenObjectSO
        });

        return true;
    }

    public bool IsBaked() {
        return isBaked;
    }

    public void SetBaked(bool baked) {
        if (isBaked == baked) {
            return;
        }

        isBaked = baked;

        OnBakedStateChanged?.Invoke(this, new OnBakedStateChangedEventArgs {
            isBaked = isBaked
        });
    }

    public bool HasIngredient(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectSOList.Contains(kitchenObjectSO);
    }

    public bool HasAnyIngredient() {
        return kitchenObjectSOList.Count > 0;
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjectSOList;
    }
}