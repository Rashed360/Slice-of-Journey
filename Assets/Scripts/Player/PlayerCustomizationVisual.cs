using UnityEngine;

public class PlayerCustomizationVisual : MonoBehaviour {

    [SerializeField] private Renderer[] targetRenderers;
    [SerializeField] private Material[] materialOptions;

    private void Start() {
        ApplySavedMaterial();
    }

    public void ApplySavedMaterial() {
        ApplyMaterial(PlayerProfile.GetMaterialIndex());
    }

    public void ApplyMaterial(int materialIndex) {
        if (materialOptions == null || materialOptions.Length == 0) {
            Debug.LogWarning("PlayerCustomizationVisual: No materials assigned.");
            return;
        }

        materialIndex = Mathf.Clamp(materialIndex, 0, materialOptions.Length - 1);
        Material selectedMaterial = materialOptions[materialIndex];

        foreach (Renderer targetRenderer in targetRenderers) {
            if (targetRenderer != null) {
                targetRenderer.material = selectedMaterial;
            }
        }
    }

    public int GetMaterialCount() {
        return materialOptions != null ? materialOptions.Length : 0;
    }
}