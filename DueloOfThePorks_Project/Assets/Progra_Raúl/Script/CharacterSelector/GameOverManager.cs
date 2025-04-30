using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Textos de resultado")]
    public TextMeshPro winnerText;
    public TextMeshPro loserText;

    [Header("Posiciones finales")]
    public Transform player1Position;
    public Transform player2Position;

    [Header("Prefabs finales por personaje")]
    public CharacterFinalAnimation[] finalCharacters;

    void Start()
    {
        DisplayResults();
    }

    void DisplayResults()
    {
        string winner = PlayerPrefs.GetString("Winner");
        string loser = PlayerPrefs.GetString("Loser");

        if (winner == "Draw")
        {
            if (winnerText != null) winnerText.text = "Draw Player 1";
            if (loserText != null) loserText.text = "Draw Player 2";
        }
        else
        {
            if (winnerText != null) winnerText.text = $"{winner} WINNER";
            if (loserText != null) loserText.text = $"{loser} LOSER";
        }

        bool p1Won = winner == "Player 1";
        bool p2Won = winner == "Player 2";

        SpawnCharacter(PlayerPrefs.GetString("Player1Character"), player1Position.position, p1Won);
        SpawnCharacter(PlayerPrefs.GetString("Player2Character"), player2Position.position, p2Won);
    }

    void SpawnCharacter(string characterName, Vector3 position, bool isWinner)
    {
        foreach (var entry in finalCharacters)
        {
            if (entry.characterName == characterName)
            {
                GameObject prefabToSpawn = isWinner ? entry.victoryPrefab : entry.defeatPrefab;
                if (prefabToSpawn != null)
                {
                    Instantiate(prefabToSpawn, position, Quaternion.identity);
                }
                else
                {
                    Debug.LogError($"Prefab {(isWinner ? "victoryPrefab" : "defeatPrefab")} no asignado para {characterName}");
                }
                return;
            }
        }
        Debug.LogError($"No se encontró CharacterFinalAnimation para '{characterName}'");
    }
}

[System.Serializable]
public class CharacterFinalAnimation
{
    public string characterName;
    public GameObject victoryPrefab;
    public GameObject defeatPrefab;
}
