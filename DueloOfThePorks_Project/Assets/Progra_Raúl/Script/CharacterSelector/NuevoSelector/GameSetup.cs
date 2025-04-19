// GameSetup.cs
// Lee la selección y crea los prefabs en la partida
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject[] player1CharacterPrefabs;
    public GameObject[] player2CharacterPrefabs;

    void Start()
    {
        string p1Name = PlayerPrefs.GetString("Player1Character");
        string p2Name = PlayerPrefs.GetString("Player2Character");

        GameObject p1Prefab = GetCharacterPrefab(p1Name, player1CharacterPrefabs);
        if (p1Prefab != null)
            Instantiate(p1Prefab, new Vector2(-2, 0), Quaternion.identity);

        GameObject p2Prefab = GetCharacterPrefab(p2Name, player2CharacterPrefabs);
        if (p2Prefab != null)
            Instantiate(p2Prefab, new Vector2(2, 0), Quaternion.identity);
    }

    GameObject GetCharacterPrefab(string characterName, GameObject[] characterPrefabs)
    {
        foreach (GameObject prefab in characterPrefabs)
            if (prefab.name == characterName)
                return prefab;
        return null;
    }
}
