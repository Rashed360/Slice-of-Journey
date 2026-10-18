using UnityEngine;

public class OvenBurnWarningUI : MonoBehaviour {

    [SerializeField] private OvenCounter ovenCounter;

    void Start() {
        ovenCounter.OnProgressChanged += OvenCounter_OnProgressChanged;

        Hide();
    }

    private void OvenCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        float burnShowProgressAmount = 0.5f;
        bool show = ovenCounter.IsBaked() && e.progressNormalized >= burnShowProgressAmount;
        if (show) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
