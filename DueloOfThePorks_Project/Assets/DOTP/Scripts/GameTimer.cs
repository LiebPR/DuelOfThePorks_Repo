using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GameTimer : MonoBehaviour
{
    [SerializeField] float countdownTime = 300f;
    bool isTimerRunning = true;
    float currentTime;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI countdownText;

    public LifeManager player1LifeManager;
    public LifeManager player2LifeManager;

    public InputManager player1InputManager;
    public InputManager player2InputManager;

    public BulletManager player1BulletManager;
    public BulletManager player2BulletManager;

    private void Start()
    {
        // Asignar los scripts automáticamente si no están asignados
        AssignPlayerScripts();

        currentTime = countdownTime;
        StartCoroutine(PreMatchCountdown());

        // Asignar respawn y corazones automáticamente al iniciar
        AssignInitialSpawn(player1LifeManager, "Player1Respawn");
        AssignInitialSpawn(player2LifeManager, "Player2Respawn");
    }

    // Asigna automáticamente los componentes de los jugadores si no están asignados en el Inspector
    void AssignPlayerScripts()
    {
        // Asignar LifeManager
        if (player1LifeManager == null)
        {
            player1LifeManager = GameObject.FindWithTag("Player1").GetComponent<LifeManager>();
        }
        if (player2LifeManager == null)
        {
            player2LifeManager = GameObject.FindWithTag("Player2").GetComponent<LifeManager>();
        }

        // Asignar InputManager
        if (player1InputManager == null)
        {
            player1InputManager = GameObject.FindWithTag("Player1").GetComponent<InputManager>();
        }
        if (player2InputManager == null)
        {
            player2InputManager = GameObject.FindWithTag("Player2").GetComponent<InputManager>();
        }

        // Asignar BulletManager
        if (player1BulletManager == null)
        {
            player1BulletManager = GameObject.FindWithTag("Player1").GetComponent<BulletManager>();
        }
        if (player2BulletManager == null)
        {
            player2BulletManager = GameObject.FindWithTag("Player2").GetComponent<BulletManager>();
        }
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
        if (player1Lives > player2Lives)
        {
            Debug.Log("Player 1 wins!");
        }
        else if (player2Lives > player1Lives)
        {
            Debug.Log("Player 2 wins!");
        }
        else
        {
            Debug.Log("It's a draw!");
        }

        Time.timeScale = 0f;
    }

    void AssignInitialSpawn(LifeManager lifeManager, string respawnTag)
    {
        if (lifeManager == null) return;

        // Asignar puntos de respawn usando el tag
        GameObject[] respawnObjects = GameObject.FindGameObjectsWithTag(respawnTag);
        if (respawnObjects.Length == 0)
        {
            Debug.LogError("No se encontraron puntos de respawn con tag: " + respawnTag);
            return;
        }

        Transform[] respawnPoints = new Transform[respawnObjects.Length];
        for (int i = 0; i < respawnObjects.Length; i++)
        {
            respawnPoints[i] = respawnObjects[i].transform;
        }

        // Asignar el punto de respawn de manera aleatoria
        int randomIndex = Random.Range(0, respawnPoints.Length);
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
        lifeManager.transform.position = respawnPoints[randomIndex].position + randomOffset;

        // Asignar corazones automáticamente (similar a PlayerInitializer)
        string panelName = lifeManager.GetComponent<HitDetector>().isPlayerOne ? "UI_Player1" : "UI_Player2";
        GameObject panel = GameObject.Find(panelName);

        if (panel != null)
        {
            Image[] heartImages = panel.GetComponentsInChildren<Image>()
                                        .OrderBy(h => h.transform.GetSiblingIndex())
                                        .ToArray();
            lifeManager.SetHeartImages(heartImages);
        }
        else
        {
            Debug.LogError("No se encontró el panel de corazones: " + panelName);
        }
    }
}