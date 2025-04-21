using System.Collections;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float countdownTime = 300f;
    private bool isTimerRunning = true;
    private float currentTime;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI countdownText;

    [SerializeField] private LifeManager player1LifeManager;
    [SerializeField] private LifeManager player2LifeManager;

    [SerializeField] private InputManager player1InputManager;
    [SerializeField] private InputManager player2InputManager;

    [SerializeField] private BulletManager player1BulletManager;
    [SerializeField] private BulletManager player2BulletManager;

    private void Awake()
    {
        // Asegurarse de no tener referencias obsoletas a HitDetector
    }

    private void Start()
    {
        currentTime = countdownTime;
        StartCoroutine(PreMatchCountdown());

        AssignInitialSpawn(player1LifeManager);
        AssignInitialSpawn(player2LifeManager);
    }

    // Congela o reanuda las animaciones de los jugadores
    private void SetPlayersAnimatorSpeed(float speed)
    {
        if (player1LifeManager != null)
        {
            Animator anim = player1LifeManager.GetComponent<Animator>();
            if (anim) anim.speed = speed;
        }
        if (player2LifeManager != null)
        {
            Animator anim = player2LifeManager.GetComponent<Animator>();
            if (anim) anim.speed = speed;
        }
    }

    private IEnumerator PreMatchCountdown()
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

        // Congelar animaciones
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

        // Reanudar animaciones
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

        // Aquí tu lógica de final de partida...
        Time.timeScale = 0f;
    }

    private void AssignInitialSpawn(LifeManager lifeManager)
    {
        if (lifeManager == null) return;

        Transform[] respawnPoints = lifeManager.GetRespawnPoints();
        if (respawnPoints == null || respawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, respawnPoints.Length);
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0f, 0f);
        lifeManager.transform.position = respawnPoints[randomIndex].position + randomOffset;
    }
}