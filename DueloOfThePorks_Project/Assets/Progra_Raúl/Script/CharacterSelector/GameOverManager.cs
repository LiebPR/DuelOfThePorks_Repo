using UnityEngine;
using TMPro;

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
        // Recuperar datos de PlayerPrefs
        string winner = PlayerPrefs.GetString("Winner");
        string loser = PlayerPrefs.GetString("Loser");

        // Mostrar textos
        if (winnerText != null) winnerText.text = $"🏆 {winner} WINNER";
        if (loserText != null) loserText.text = $"❌ {loser} LOSER";

        // ¿Quién ganó?
        bool p1Won = winner == "Player 1";
        bool p2Won = winner == "Player 2";

        // Instanciar cada personaje usando el prefab adecuado
        SpawnCharacter(PlayerPrefs.GetString("Player1Character"), player1Position.position, p1Won);
        SpawnCharacter(PlayerPrefs.GetString("Player2Character"), player2Position.position, p2Won);
    }

    void SpawnCharacter(string characterName, Vector3 position, bool isWinner)
    {
        foreach (var entry in finalCharacters)
        {
            if (entry.characterName == characterName)
            {
                // Seleccionar el prefab de victoria o derrota
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
    public string characterName;     // Nombre exacto del prefab (debe coincidir con PlayerPrefs)
    public GameObject victoryPrefab; // Prefab con animación de victoria (Play On Awake activo)
    public GameObject defeatPrefab;  // Prefab con animación de derrota
}
