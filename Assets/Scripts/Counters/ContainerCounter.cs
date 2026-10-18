using System;
using UnityEngine;

public class ContainerCounter : BaseCounter {

    public event EventHandler OnPlayerGrabbedObject;
    public event EventHandler OnOutOfStock;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) return;

        if (LevelInventoryManager.Instance == null) {
            Debug.LogError("ContainerCounter: LevelInventoryManager not found.");
            return;
        }

        if (!LevelInventoryManager.Instance.TryConsumeStock(kitchenObjectSO)) {
            OnOutOfStock?.Invoke(this, EventArgs.Empty);
            return;
        }

        KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}