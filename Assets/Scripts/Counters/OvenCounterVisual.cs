using UnityEngine;

public class OvenCounterVisual : MonoBehaviour {

    [SerializeField] private OvenCounter ovenCounter;
    [SerializeField] private GameObject ovenOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start() {
        ovenCounter.OnStateChanged += OvenCounter_OnStateChanged;
    }

    private void OvenCounter_OnStateChanged(object sender, OvenCounter.OnStateChangedEventArgs e) {
        bool showVisual = e.state == OvenCounter.State.Baking || e.state == OvenCounter.State.Burned;
        ovenOnGameObject.SetActive(showVisual);
        particlesGameObject.SetActive(showVisual);
    }
}
