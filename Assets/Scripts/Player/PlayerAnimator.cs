using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private const string IS_WALKING = "isWalking";
    private const string IS_HOLDING = "isHolding";
    private const string PICKED_ITEM = "pickedItem";
    private const string DROPPED_ITEM = "droppedItem";

    [SerializeField] private Player player;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        player.OnPickedSomething += Player_OnPickedSomething;
        player.OnDroppedSomething += Player_OnDroppedSomething;
    }

    private void OnDestroy() {
        if (player != null) {
            player.OnPickedSomething -= Player_OnPickedSomething;
            player.OnDroppedSomething -= Player_OnDroppedSomething;
        }
    }

    private void Update() {
        animator.SetBool(IS_WALKING, player.IsWalking());
        animator.SetBool(IS_HOLDING, player.IsHoldingObject());
    }

    private void Player_OnPickedSomething(object sender, EventArgs e) {
        animator.SetTrigger(PICKED_ITEM);
    }

    private void Player_OnDroppedSomething(object sender, EventArgs e) {
        animator.SetTrigger(DROPPED_ITEM);
    }
}