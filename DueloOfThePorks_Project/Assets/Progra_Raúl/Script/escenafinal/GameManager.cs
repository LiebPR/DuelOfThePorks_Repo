using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton para acceso global (opcional pero útil)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Opcional si necesitas que persista
    }

    /// <summary>
    /// Llama este método cuando un jugador gana.
    /// </summary>
    /// <param name="winnerName">Nombre del personaje que ganó.</param>
    /// <param name="player1Name">Nombre del personaje del Player 1.</param>
    /// <param name="player2Name">Nombre del personaje del Player 2.</param>
    public void EndGame(string winnerName, string player1Name, string player2Name)
    {
        PlayerPrefs.SetString("Winner", winnerName);
        PlayerPrefs.SetString("Player1Character", player1Name);
        PlayerPrefs.SetString("Player2Character", player2Name);
        PlayerPrefs.Save();

        SceneManager.LoadScene("FinalScene");
    }
}
