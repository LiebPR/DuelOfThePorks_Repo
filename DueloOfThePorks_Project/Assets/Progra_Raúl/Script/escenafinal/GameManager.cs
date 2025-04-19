using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Aplicar patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Termina el juego y guarda los datos del ganador y personajes.
    /// </summary>
    /// <param name="winner">Nombre del personaje ganador.</param>
    /// <param name="player1">Nombre del personaje del Jugador 1.</param>
    /// <param name="player2">Nombre del personaje del Jugador 2.</param>
    public void EndGame(string winner, string player1, string player2)
    {
        PlayerPrefs.SetString("Winner", winner);
        PlayerPrefs.SetString("Player1Character", player1);
        PlayerPrefs.SetString("Player2Character", player2);
        PlayerPrefs.Save();

        SceneManager.LoadScene("FinalScene");
    }
}
