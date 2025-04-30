// GameOverManager.cs (en FinalScene)
using UnityEngine;

[System.Serializable]
public class CharacterFinalAnimation
{
    public string characterName;
    public GameObject victoryPrefab;
    public GameObject defeatPrefab;
}

public class GameOverManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform player1Position;
    [SerializeField] private Transform player2Position;

    [Header("Animaciones")]
    [SerializeField] private CharacterFinalAnimation[] finalCharacters;

    [Header("Textos")]
    [SerializeField] private FinalResultTextController finalResultTextController;

    void Start()
    {
        string winner = PlayerPrefs.GetString("Winner", "Draw");
        string c1 = PlayerPrefs.GetString("Player1Character", "");
        string c2 = PlayerPrefs.GetString("Player2Character", "");

        SpawnCharacter(c1, player1Position.position, winner == "Player 1");
        SpawnCharacter(c2, player2Position.position, winner == "Player 2");
        finalResultTextController.DisplayResults(winner);
    }

    private void SpawnCharacter(string characterName, Vector3 pos, bool isWinner)
    {
        foreach (var e in finalCharacters)
        {
            if (e.characterName == characterName)
            {
                var prefab = isWinner ? e.victoryPrefab : e.defeatPrefab;
                if (prefab != null)
                    Instantiate(prefab, pos, Quaternion.identity);
                else
                    Debug.LogError($"Falta prefab {(isWinner ? "victoria" : "derrota")} para '{characterName}'");
                return;
            }
        }
        Debug.LogError($"No hay CharacterFinalAnimation para '{characterName}'");
    }
}
