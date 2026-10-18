using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour {

    [Serializable]
    public struct KitchenObjectVisualPair {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject rawGameObject;
        public GameObject bakedGameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectVisualPair> kitchenObjectVisualPairList;

    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        plateKitchenObject.OnBakedStateChanged += PlateKitchenObject_OnBakedStateChanged;

        UpdateAllVisuals();
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        UpdateAllVisuals();
    }

    private void PlateKitchenObject_OnBakedStateChanged(object sender, PlateKitchenObject.OnBakedStateChangedEventArgs e) {
        UpdateAllVisuals();
    }

    private void UpdateAllVisuals() {
        bool isBaked = plateKitchenObject.IsBaked();

        foreach (KitchenObjectVisualPair visualPair in kitchenObjectVisualPairList) {
            bool hasIngredient = plateKitchenObject.HasIngredient(visualPair.kitchenObjectSO);

            if (visualPair.rawGameObject != null) {
                visualPair.rawGameObject.SetActive(hasIngredient && !isBaked);
            }

            if (visualPair.bakedGameObject != null) {
                visualPair.bakedGameObject.SetActive(hasIngredient && isBaked);
            }
        }
    }
}