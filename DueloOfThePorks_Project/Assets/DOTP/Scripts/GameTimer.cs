// GameTimer.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Match Settings")]
    [SerializeField] private float countdownTime = 300f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Player Components (auto-assign if null)")]
    public LifeManager player1LifeManager;
    public LifeManager player2LifeManager;
    public InputManager player1InputManager;
    public InputManager player2InputManager;
    public BulletManager player1BulletManager;
    public BulletManager player2BulletManager;

    [Header("Audio")]
    public SceneAudioManager sceneAudioManTimer;
    public int countdownClipIndex = 0; // tick sound
    public int fightClipIndex = 1; // “FIGHT!” sound

    private AudioSource _sfxSource;
    private float currentTime;
    private bool isTimerRunning;

    private void Start()
    {
        // Cache the AudioSource and ensure looping is off
        _sfxSource = sceneAudioManTimer.GetComponent<AudioSource>();
        if (_sfxSource != null) _sfxSource.loop = false;

        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null; // wait one frame

        AssignPlayerScripts();

        // Pre-match 3-2-1 countdown
        yield return StartCoroutine(PreMatchCountdown());

        // Start main timer
        currentTime = countdownTime;
        isTimerRunning = true;
        StartCoroutine(UpdateTimer());
    }

    private void AssignPlayerScripts()
    {
        int p1Layer = LayerMask.NameToLayer("Player1");
        int p2Layer = LayerMask.NameToLayer("Player2");

        foreach (var go in FindObjectsOfType<GameObject>())
        {
            if (go.layer == p1Layer)
            {
                player1LifeManager ??= go.GetComponent<LifeManager>();
                player1InputManager ??= go.GetComponent<InputManager>();
                player1BulletManager ??= go.GetComponent<BulletManager>();
            }
            else if (go.layer == p2Layer)
            {
                player2LifeManager ??= go.GetComponent<LifeManager>();
                player2InputManager ??= go.GetComponent<InputManager>();
                player2BulletManager ??= go.GetComponent<BulletManager>();
            }
        }
    }

    private IEnumerator PreMatchCountdown()
    {
        // Lock inputs & bullets, freeze animations
        if (player1InputManager != null) { player1InputManager.inputLocked = true; player1InputManager.ResetAllInputs(); }
        if (player2InputManager != null) { player2InputManager.inputLocked = true; player2InputManager.ResetAllInputs(); }
        if (player1BulletManager != null) player1BulletManager.isLocked = true;
        if (player2BulletManager != null) player2BulletManager.isLocked = true;
        SetPlayersAnimatorSpeed(0f);

        // 3-2-1 ticks (stop previous clip before each tick)
        for (int tick = 3; tick > 0; tick--)
        {
            countdownText.text = tick.ToString();
            if (_sfxSource != null)
            {
                _sfxSource.Stop();
                sceneAudioManTimer.PlaySFX(countdownClipIndex);
            }
            yield return new WaitForSecondsRealtime(1f);
        }

        // FIGHT! (also stop any lingering tick)
        countdownText.text = "FIGHT!";
        if (_sfxSource != null)
        {
            _sfxSource.Stop();
            sceneAudioManTimer.PlaySFX(fightClipIndex);
        }
        yield return new WaitForSecondsRealtime(1f);
        countdownText.text = "";

        // Unlock inputs & bullets, resume animations
        SetPlayersAnimatorSpeed(1f);
        if (player1InputManager != null) { player1InputManager.ResetAllInputs(); player1InputManager.inputLocked = false; }
        if (player2InputManager != null) { player2InputManager.ResetAllInputs(); player2InputManager.inputLocked = false; }
        if (player1BulletManager != null) player1BulletManager.isLocked = false;
        if (player2BulletManager != null) player2BulletManager.isLocked = false;
    }

    private void SetPlayersAnimatorSpeed(float speed)
    {
        if (player1LifeManager != null)
            player1LifeManager.GetComponent<Animator>().speed = speed;
        if (player2LifeManager != null)
            player2LifeManager.GetComponent<Animator>().speed = speed;
    }

    private IEnumerator UpdateTimer()
    {
        while (isTimerRunning && currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
            int m = Mathf.FloorToInt(currentTime / 60f);
            int s = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = $"{m:00}:{s:00}";
            yield return null;
        }

        isTimerRunning = false;
        DetermineWinner();
    }

    private void DetermineWinner()
    {
        int p1 = player1LifeManager != null ? player1LifeManager.GetLives() : 0;
        int p2 = player2LifeManager != null ? player2LifeManager.GetLives() : 0;
        string winner = p1 > p2 ? "Player 1" : (p2 > p1 ? "Player 2" : "Draw");

        PlayerPrefs.SetString("Winner", winner);
        PlayerPrefs.Save();

        Time.timeScale = 0f;
        SceneManager.LoadScene("FinalScene");
    }
}
