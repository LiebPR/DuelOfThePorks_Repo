using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndByTimer : MonoBehaviour
{
    public float countdownTime = 300f;
    private float currentTime;
    private bool gameEnded = false;

    private LifeManager player1LifeManager;
    private LifeManager player2LifeManager;

    void Start()
    {
        StartCoroutine(WaitForPlayersThenStartTimer());
    }

    IEnumerator WaitForPlayersThenStartTimer()
    {
        // Espera hasta que ambos LifeManager existan en escena
        while (player1LifeManager == null || player2LifeManager == null)
        {
            LifeManager[] all = FindObjectsOfType<LifeManager>();
            foreach (var lm in all)
            {
                var hd = lm.GetComponent<HitDetector>();
                if (hd != null)
                {
                    if (hd.isPlayerOne && player1LifeManager == null)
                        player1LifeManager = lm;
                    else if (!hd.isPlayerOne && player2LifeManager == null)
                        player2LifeManager = lm;
                }
            }

            yield return null; // Espera al siguiente frame
        }

        Debug.Log("Jugadores detectados, iniciando temporizador...");
        currentTime = countdownTime;

        while (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
            yield return null;
        }

        if (!gameEnded)
        {
            gameEnded = true;
            DetermineWinnerAndLoadFinalScene();
        }
    }

    void DetermineWinnerAndLoadFinalScene()
    {
        int lives1 = player1LifeManager.GetLives();
        int lives2 = player2LifeManager.GetLives();

        string winner, loser;

        if (lives1 > lives2)
        {
            winner = "Player 1";
            loser = "Player 2";
        }
        else if (lives2 > lives1)
        {
            winner = "Player 2";
            loser = "Player 1";
        }
        else
        {
            winner = "Draw";
            loser = "Draw";
        }

        PlayerPrefs.SetString("Winner", winner);
        PlayerPrefs.SetString("Loser", loser);
        PlayerPrefs.Save();

        Debug.Log($"[FIN TIEMPO] Ganador: {winner}, Perdedor: {loser}");
        SceneManager.LoadScene("FinalScene");
    }
}
