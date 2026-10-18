using UnityEngine;

public class DeliveryCounter : BaseCounter {

    public static DeliveryCounter Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {

                bool success = DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                if (success) {
                    // destroy only if correct
                    player.GetKitchenObject().DestroySelf();
                }
            }
        }
    }
}
