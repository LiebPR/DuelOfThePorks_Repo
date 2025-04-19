using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public TextMeshPro winnerText;
    public TextMeshPro loserText;

    public Transform player1Position;
    public Transform player2Position;

    public CharacterFinalAnimation[] finalCharacters; // nombre + prefab base + animaciones

    void Start()
    {
        string winner = PlayerPrefs.GetString("Winner");
        string loser = PlayerPrefs.GetString("Loser");

        string p1Char = PlayerPrefs.GetString("Player1Character");
        string p2Char = PlayerPrefs.GetString("Player2Character");

        if (winnerText != null) winnerText.text = $"🏆 {winner} ha ganado";
        if (loserText != null) loserText.text = $"❌ {loser} ha perdido";

        bool p1Won = winner == "Player 1";
        bool p2Won = winner == "Player 2";

        SpawnCharacter(p1Char, player1Position.position, p1Won);
        SpawnCharacter(p2Char, player2Position.position, p2Won);
    }

    void SpawnCharacter(string characterName, Vector3 position, bool isWinner)
    {
        foreach (var entry in finalCharacters)
        {
            if (entry.characterName == characterName)
            {
                GameObject instance = Instantiate(entry.basePrefab, position, Quaternion.identity);
                var display = instance.AddComponent<VictoryDefeatPlayerDisplay>();
                display.victoryAnimation = entry.victoryAnim;
                display.defeatAnimation = entry.defeatAnim;
                display.isWinner = isWinner;
                return;
            }
        }
    }
}

[System.Serializable]
public class CharacterFinalAnimation
{
    public string characterName;
    public GameObject basePrefab;
    public AnimationClip victoryAnim;
    public AnimationClip defeatAnim;
}
