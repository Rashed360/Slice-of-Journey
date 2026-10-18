using UnityEngine;
using UnityEngine.UI;

public class EndCreditsUI : MonoBehaviour {

    [SerializeField] private RectTransform creditsTransform;
    [SerializeField] private float scrollSpeed = 60f;
    [SerializeField] private float endY = 1200f;
    [SerializeField] private Button backButton;

    private void Awake() {
        if (backButton != null) {
            backButton.onClick.AddListener(() => {
                Loader.Load(Loader.Scene.MainMenuScene);
            });
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)) {
            Loader.Load(Loader.Scene.MainMenuScene);
            return;
        }

        if (creditsTransform != null) {
            creditsTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            if (creditsTransform.anchoredPosition.y >= endY) {
                Loader.Load(Loader.Scene.MainMenuScene);
            }
        }
    }
}