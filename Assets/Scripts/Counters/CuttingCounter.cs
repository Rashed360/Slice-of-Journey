using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {
    
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData() {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRepiceSO[] cuttingRepiceSOArray;

    private int cuttingProgress;

    public override void Interact(Player player) {
        if (!HasKitchenObject()) { // No Kitchen Object Here
            if (player.HasKitchenObject()) { // Player is Carrying Object
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingRepiceSO cuttingRepiceSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = (float) cuttingProgress / cuttingRepiceSO.cuttingProgressMax
                    });
                }
            }
        } else {
            if (player.HasKitchenObject()) {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {                    
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                }
            } else {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player) {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) { // Kitchen Object Present
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            CuttingRepiceSO cuttingRepiceSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
               progressNormalized = (float) cuttingProgress / cuttingRepiceSO.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingRepiceSO.cuttingProgressMax) {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);  
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingRepiceSO cuttingRepiceSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRepiceSO != null;
    }

    public KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingRepiceSO cuttingRepiceSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRepiceSO != null) {
            return cuttingRepiceSO.output;
        } else return null;
    }

    public CuttingRepiceSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingRepiceSO cuttingRepiceSO in cuttingRepiceSOArray) {
            if (cuttingRepiceSO.input == inputKitchenObjectSO) {
                return cuttingRepiceSO;
            }
        }
        return null;
    }
}
