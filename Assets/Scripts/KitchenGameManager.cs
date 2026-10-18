using System;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour {

    [SerializeField] private LevelListSO levelListSO;

    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        CountdownToGameOver,
        GameOver,
    }

    private State state;
    private float countdownToStartTimer = 3f;
    private float countdownToGameOverTimer = 3f;
    private float countdownToGameOverTimerMax = 3f;

    private float gamePlayingTimer;
    private float gamePlayingTimerMax;

    private LevelSO currentLevel;
    private bool isGamePaused = false;

    private bool levelCompleted;
    private bool gameResultProcessed;
    private bool sceneTransitionHandled;

    private void Awake() {
        Instance = this;
        state = State.WaitingToStart;

        EconomyManager.StartLevelSession();
        LoadCurrentLevelData();
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void LoadCurrentLevelData() {
        if (levelListSO == null) {
            Debug.LogError("LevelListSO not found!");
            gamePlayingTimerMax = 120f;
            return;
        }

        int selectedSerial = SelectedLevel.GetSelectedSerial();
        currentLevel = levelListSO.GetLevelBySerial(selectedSerial);

        if (currentLevel != null) {
            gamePlayingTimerMax = currentLevel.gameplayTime;
        } else {
            Debug.LogWarning("Selected level not found, fallback to 120s.");
            gamePlayingTimerMax = 120f;
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (state == State.WaitingToStart) {
            state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    private void Update() {
        switch (state) {
            case State.WaitingToStart:
                break;

            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f) {
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.GamePlaying:
                HandleGamePlaying();
                break;

            case State.CountdownToGameOver:
                countdownToGameOverTimer -= Time.deltaTime;
                if (countdownToGameOverTimer <= 0f) {
                    countdownToGameOverTimer = 0f;
                    EndGame();
                }
                break;

            case State.GameOver:
                break;
        }
    }

    private void HandleGamePlaying() {
        gamePlayingTimer -= Time.deltaTime;

        int delivered = DeliveryManager.Instance.GetSuccessfulRecipesAmount();
        int target = GetTargetDeliverCount();

        // Win -> use end countdown
        if (delivered >= target) {
            levelCompleted = true;
            StartGameOverCountdown();
            return;
        }

        // Lose by timer -> end immediately
        if (gamePlayingTimer <= 0f) {
            gamePlayingTimer = 0f;
            levelCompleted = false;
            EndGame();
        }
    }

    private void StartGameOverCountdown() {
        if (state == State.CountdownToGameOver || state == State.GameOver) return;

        countdownToGameOverTimer = countdownToGameOverTimerMax;
        state = State.CountdownToGameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void EndGame() {
        if (state == State.GameOver) return;

        state = State.GameOver;
        ProcessGameResult();
        HandleSceneTransition();
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ProcessGameResult() {
        if (gameResultProcessed) return;
        gameResultProcessed = true;

        if (levelCompleted) {
            EconomyManager.CommitLevelResultToGlobal();
        } else {
            EconomyManager.EndLevelSessionWithoutCommit();
        }

        if (!levelCompleted || currentLevel == null) return;

        int currentSerial = currentLevel.levelSerial;
        int nextSerial = currentSerial + 1;

        LevelProgress.SetCompleted(currentSerial, true);

        if (levelListSO != null && levelListSO.GetLevelBySerial(nextSerial) != null) {
            LevelProgress.Unlock(nextSerial);
        }
    }

    private void HandleSceneTransition() {
        if (sceneTransitionHandled) return;
        sceneTransitionHandled = true;

        if (!levelCompleted) {
            return;
        }

        if (!HasNextLevel()) {
            Loader.Load(Loader.Scene.EndScene);
            return;
        }
    }

    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    public bool IsGamePaused() {
        return isGamePaused;
    }

    public bool IsCountdownToStartActive() {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }

    public bool IsCountdownToGameOverActive() {
        return state == State.CountdownToGameOver;
    }

    public float GetCountdownToGameOverTimer() {
        return countdownToGameOverTimer;
    }

    public bool IsGameOver() {
        return state == State.GameOver;
    }

    public bool IsLevelCompleted() {
        return levelCompleted;
    }

    public float GetGamePlayingTimerNormalized() {
        if (gamePlayingTimerMax <= 0f) return 0f;
        return gamePlayingTimer / gamePlayingTimerMax;
    }

    public float GetGamePlayingTimerRemaining() {
        return gamePlayingTimer;
    }

    public int GetTargetDeliverCount() {
        return currentLevel != null ? currentLevel.deliverCount : 1;
    }

    public int GetCurrentLevelSerial() {
        return currentLevel != null ? currentLevel.levelSerial : 1;
    }

    public bool HasNextLevel() {
        if (currentLevel == null || levelListSO == null) return false;
        return levelListSO.GetLevelBySerial(currentLevel.levelSerial + 1) != null;
    }

    public int GetNextLevelSerial() {
        if (!HasNextLevel()) return GetCurrentLevelSerial();
        return currentLevel.levelSerial + 1;
    }

    public void TogglePauseGame() {
        isGamePaused = !isGamePaused;

        if (isGamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }
}