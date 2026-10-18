using UnityEngine;
using UnityEngine.UI;

public class LoaderCallback : MonoBehaviour {

    [SerializeField] private Slider slider;
    private int progress = 0;
    private int progressMax = 21;


    private void Awake() {
        progress = 0;
        slider.value = progress;
    }

    private void Update() {
        if (progress < progressMax) {
            AdvanceProgress();
        } else {
            Loader.LoaderCallback();
        }
    }

    private void AdvanceProgress() {
        progress++;
        slider.value = progress;
    }

}