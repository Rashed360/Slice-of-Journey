using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {

    public static Player Instance { get; private set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler OnDroppedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    [Header("Player Settings")]
    [SerializeField] private float playerRadius = 0.48f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 15f;
    [SerializeField] private float interactionDistance = 1.5f;

    [Header("Game Objects")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir = Vector3.forward;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Multiple Player Instance Found!");
        }
        Instance = this;
    }

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void OnDestroy() {
        if (gameInput != null) {
            gameInput.OnInteractAction -= GameInput_OnInteractAction;
            gameInput.OnInteractAlternateAction -= GameInput_OnInteractAlternateAction;
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        selectedCounter?.InteractAlternate(this);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        selectedCounter?.Interact(this);
    }

    private void Update() {
        HandleMovements();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    public bool IsHoldingObject() {
        return kitchenObject != null;
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactionDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovements() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = moveDir != Vector3.zero &&
                       !Physics.CapsuleCast(
                           transform.position,
                           transform.position + Vector3.up * playerHeight,
                           playerRadius,
                           moveDir,
                           moveDistance
                       );

        if (!canMove && moveDir != Vector3.zero) {
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
            canMove = (moveDir.x < -0.5f || moveDir.x > 0.5f) &&
                      !Physics.CapsuleCast(
                          transform.position,
                          transform.position + Vector3.up * playerHeight,
                          playerRadius,
                          moveDirX,
                          moveDistance
                      );

            if (canMove) {
                moveDir = moveDirX;
            } else {
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
                canMove = (moveDir.z < -0.5f || moveDir.z > 0.5f) &&
                          !Physics.CapsuleCast(
                              transform.position,
                              transform.position + Vector3.up * playerHeight,
                              playerRadius,
                              moveDirZ,
                              moveDistance
                          );

                if (canMove) {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        if (isWalking) {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public void ClearSelectedCounter() {
        SetSelectedCounter(null);
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        bool wasHoldingObject = kitchenObject != null;

        kitchenObject = null;

        if (wasHoldingObject) {
            OnDroppedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}