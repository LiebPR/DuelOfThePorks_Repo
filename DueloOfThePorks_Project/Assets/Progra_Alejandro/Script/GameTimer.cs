using System.Collections;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] float countdownTime = 300f;
    bool isTimerRunning = true;
    float currentTime;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI countdownText;

    public LifeManager player1LifeManager;
    public LifeManager player2LifeManager;
    public HitDetector player1HitDetector;
    public HitDetector player2HitDetector;

    public InputManager player1InputManager;
    public InputManager player2InputManager;

    public BulletManager player1BulletManager;
    public BulletManager player2BulletManager;

    private void Start()
    {
        currentTime = countdownTime;
        StartCoroutine(PreMatchCountdown());

        AssignInitialSpawn(player1LifeManager);
        AssignInitialSpawn(player2LifeManager);
    }

    // Congela o reanuda las animaciones de los jugadores
    void SetPlayersAnimatorSpeed(float speed)
    {
        if (player1LifeManager != null)
        {
            var anim = player1LifeManager.GetComponent<Animator>();
            if (anim) anim.speed = speed;
        }
        if (player2LifeManager != null)
        {
            var anim = player2LifeManager.GetComponent<Animator>();
            if (anim) anim.speed = speed;
        }
    }

    IEnumerator PreMatchCountdown()
    {
        // Bloquear inputs
        if (player1InputManager != null)
        {
            player1InputManager.inputLocked = true;
            player1InputManager.ResetAllInputs();
        }
        if (player2InputManager != null)
        {
            player2InputManager.inputLocked = true;
            player2InputManager.ResetAllInputs();
        }

        // Bloquear disparos
        if (player1BulletManager != null) player1BulletManager.isLocked = true;
        if (player2BulletManager != null) player2BulletManager.isLocked = true;

        // Congelar animaciones de los jugadores
        SetPlayersAnimatorSpeed(0f);

        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return StartCoroutine(WaitForRealSeconds(1f));
            count--;
        }

        countdownText.text = "GO!";
        yield return StartCoroutine(WaitForRealSeconds(1f));

        countdownText.text = string.Empty;

        // Reanudar animaciones de los jugadores
        SetPlayersAnimatorSpeed(1f);

        // Desbloquear inputs
        if (player1InputManager != null)
        {
            player1InputManager.ResetAllInputs();
            player1InputManager.inputLocked = false;
        }
        if (player2InputManager != null)
        {
            player2InputManager.ResetAllInputs();
            player2InputManager.inputLocked = false;
        }

        // Desbloquear disparos
        if (player1BulletManager != null) player1BulletManager.isLocked = false;
        if (player2BulletManager != null) player2BulletManager.isLocked = false;

        isTimerRunning = true;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator WaitForRealSeconds(float seconds)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + seconds)
            yield return null;
    }

    private IEnumerator UpdateTimer()
    {
        while (currentTime > 0 && isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();
            yield return null;
        }

        isTimerRunning = false;
        DetermineWinner();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void DetermineWinner()
    {
        int player1Lives = player1LifeManager.GetLives();
        int player2Lives = player2LifeManager.GetLives();

        // Lógica de ganadores...

        Time.timeScale = 0f;
    }

    void AssignInitialSpawn(LifeManager lifeManager)
    {
        if (lifeManager == null) return;

        Transform[] respawnPoints = lifeManager.GetRespawnPoints();
        if (respawnPoints == null || respawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, respawnPoints.Length);
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
        lifeManager.transform.position = respawnPoints[randomIndex].position + randomOffset;
    }
}
