using System;
using UnityEngine;

public class OvenCounter : BaseCounter, IHasProgress {

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }

    public enum State {
        Idle,
        Baking,
        Baked,
        Burned
    }

    [Header("Baking Time")]
    [SerializeField] private float bakingTimerMax = 5f;
    [SerializeField] private float burningTimerMax = 4f;
    [SerializeField] private float burnedFlashTimerMax = 0.75f;

    [Header("Pizza Validation")]
    [SerializeField] private KitchenObjectSO doughKitchenObjectSO;
    [SerializeField] private KitchenObjectSO tomatoSlicesKitchenObjectSO;
    [SerializeField] private KitchenObjectSO cheeseSlicesKitchenObjectSO;

    private State state;
    private float bakingTimer;
    private float burningTimer;
    private float burnedFlashTimer;
    private PlateKitchenObject currentPlateKitchenObject;

    private void Start() {
        state = State.Idle;
    }

    private void Update() {
        switch (state) {
            case State.Idle:
                break;

            case State.Baking:
                if (!HasKitchenObject()) {
                    ResetOvenState();
                    return;
                }

                EnsureCurrentPlateReference();
                if (currentPlateKitchenObject == null) {
                    ResetOvenState();
                    return;
                }

                bakingTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = bakingTimer / bakingTimerMax
                });

                if (bakingTimer >= bakingTimerMax) {
                    currentPlateKitchenObject.SetBaked(true);

                    state = State.Baked;
                    burningTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = 0f
                    });
                }
                break;

            case State.Baked:
                if (!HasKitchenObject()) {
                    ResetOvenState();
                    return;
                }

                EnsureCurrentPlateReference();
                if (currentPlateKitchenObject == null) {
                    ResetOvenState();
                    return;
                }

                burningTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = burningTimer / burningTimerMax
                });

                if (burningTimer >= burningTimerMax) {
                    if (HasKitchenObject()) {
                        GetKitchenObject().DestroySelf();
                    }

                    state = State.Burned;
                    burnedFlashTimer = 0f;
                    currentPlateKitchenObject = null;
                    bakingTimer = 0f;
                    burningTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = 0f
                    });
                }
                break;

            case State.Burned:
                burnedFlashTimer += Time.deltaTime;

                if (burnedFlashTimer >= burnedFlashTimerMax) {
                    ResetOvenState();
                }
                break;
        }
    }

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (!player.HasKitchenObject()) {
                return;
            }

            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                if (CanBakePlate(plateKitchenObject)) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    currentPlateKitchenObject = plateKitchenObject;
                    bakingTimer = 0f;
                    burningTimer = 0f;
                    burnedFlashTimer = 0f;
                    state = State.Baking;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = 0f
                    });
                }
            }
        } else {
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
                ResetOvenState();
            }
        }
    }

    private void EnsureCurrentPlateReference() {
        if (currentPlateKitchenObject == null && HasKitchenObject()) {
            GetKitchenObject().TryGetPlate(out currentPlateKitchenObject);
        }
    }

    private bool CanBakePlate(PlateKitchenObject plateKitchenObject) {
        if (plateKitchenObject == null) {
            return false;
        }

        if (plateKitchenObject.IsBaked()) {
            return false;
        }

        return IsValidPizzaForBaking(plateKitchenObject);
    }

    private bool IsValidPizzaForBaking(PlateKitchenObject plateKitchenObject) {
        bool hasDough = plateKitchenObject.HasIngredient(doughKitchenObjectSO);
        bool hasTomato = plateKitchenObject.HasIngredient(tomatoSlicesKitchenObjectSO);
        bool hasCheese = plateKitchenObject.HasIngredient(cheeseSlicesKitchenObjectSO);

        return hasDough && (hasTomato || hasCheese);
    }

    private void ResetOvenState() {
        state = State.Idle;
        currentPlateKitchenObject = null;
        bakingTimer = 0f;
        burningTimer = 0f;
        burnedFlashTimer = 0f;

        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
            state = state
        });

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = 0f
        });
    }

    public bool IsBaked() {
        return state == State.Baked;
    }

    public bool IsBurned() {
        return state == State.Burned;
    }
}