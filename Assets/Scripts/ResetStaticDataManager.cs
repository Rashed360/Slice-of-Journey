using UnityEngine;

public class ResetStaticData : MonoBehaviour {

    private void Awake() {
        CuttingCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
    }
}
