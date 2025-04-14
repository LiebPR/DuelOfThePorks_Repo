using UnityEngine;

public class PlayerDeathWatcher : MonoBehaviour
{
    private GameObject player1;
    private GameObject player2;

    private string player1Character;
    private string player2Character;

    private bool gameEnded = false;

    void Start()
    {
        // Obtener nombres de personajes seleccionados
        player1Character = PlayerPrefs.GetString("Player1Character", "Player1");
        player2Character = PlayerPrefs.GetString("Player2Character", "Player2");

        // Buscar los GameObjects instanciados por nombre
        player1 = GameObject.Find(player1Character);
        player2 = GameObject.Find(player2Character);

        if (player1 == null || player2 == null)
        {
            Debug.LogError("No se encontraron los jugadores instanciados en la escena.");
        }
    }

    void Update()
    {
        if (gameEnded || player1 == null || player2 == null) return;

        if (!player1.activeSelf)
        {
            EndGame(player2Character, player1Character);
        }
        else if (!player2.activeSelf)
        {
            EndGame(player1Character, player2Character);
        }
    }

    void EndGame(string winnerChar, string loserChar)
    {
        gameEnded = true;

        Debug.Log($"El ganador es {winnerChar}. Cargando escena final...");
        GameManager.Instance.EndGame(winnerChar, player1Character, player2Character);
    }
}
