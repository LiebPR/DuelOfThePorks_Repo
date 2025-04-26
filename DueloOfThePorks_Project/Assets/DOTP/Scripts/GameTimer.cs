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
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return null; // Espera 1 frame para asegurarte que los objetos existen

        AssignPlayerScripts();

        currentTime = countdownTime;
        StartCoroutine(PreMatchCountdown());

        AssignInitialSpawn(player1LifeManager, "Player1Respawn");
        AssignInitialSpawn(player2LifeManager, "Player2Respawn");
    }

    void AssignPlayerScripts()
    {
        int player1Layer = LayerMask.NameToLayer("Player1");
        int player2Layer = LayerMask.NameToLayer("Player2");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == player1Layer)
            {
                if (player1LifeManager == null) player1LifeManager = obj.GetComponent<LifeManager>();
                if (player1InputManager == null) player1InputManager = obj.GetComponent<InputManager>();
                if (player1BulletManager == null) player1BulletManager = obj.GetComponent<BulletManager>();
            }
            else if (obj.layer == player2Layer)
            {
                if (player2LifeManager == null) player2LifeManager = obj.GetComponent<LifeManager>();
                if (player2InputManager == null) player2InputManager = obj.GetComponent<InputManager>();
                if (player2BulletManager == null) player2BulletManager = obj.GetComponent<BulletManager>();
            }
        }
    }

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

        if (player1BulletManager != null) player1BulletManager.isLocked = true;
        if (player2BulletManager != null) player2BulletManager.isLocked = true;

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

        SetPlayersAnimatorSpeed(1f);

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

        int randomIndex = Random.Range(0, respawnPoints.Length);
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
        lifeManager.transform.position = respawnPoints[randomIndex].position + randomOffset;

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