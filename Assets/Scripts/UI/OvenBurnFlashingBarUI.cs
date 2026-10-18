using UnityEngine;

public class OvenBurnFlashingBarUI : MonoBehaviour {

    private const string IS_FLASHING = "IsFlashing";

    [SerializeField] private OvenCounter ovenCounter;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void Start() {
        ovenCounter.OnProgressChanged += OvenCounter_OnProgressChanged;

        animator.SetBool(IS_FLASHING, false);
    }

    private void OvenCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        float burnShowProgressAmount = 0.5f;
        bool show = ovenCounter.IsBaked() && e.progressNormalized >= burnShowProgressAmount;

        animator.SetBool(IS_FLASHING, show);
    }

}
