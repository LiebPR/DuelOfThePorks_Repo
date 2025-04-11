using UnityEngine;

public class GameSetup : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject[] player1CharacterPrefabs;
    public GameObject[] player2CharacterPrefabs;

    [Header("Spawn Settings")]
    public Vector2 player1SpawnPosition = new Vector2(-2, 0);
    public Vector2 player2SpawnPosition = new Vector2(2, 0);
    public string player1LayerName = "Player1";
    public string player2LayerName = "Player2";

    private void Start()
    {
        string p1Name = PlayerPrefs.GetString("Player1Character");
        string p2Name = PlayerPrefs.GetString("Player2Character");

        GameObject p1Prefab = GetCharacterPrefab(p1Name, player1CharacterPrefabs);
        GameObject p2Prefab = GetCharacterPrefab(p2Name, player2CharacterPrefabs);

        if (p1Prefab != null)
        {
            GameObject p1Instance = Instantiate(p1Prefab, player1SpawnPosition, Quaternion.identity);
            p1Instance.layer = LayerMask.NameToLayer(player1LayerName);
        }

        if (p2Prefab != null)
        {
            GameObject p2Instance = Instantiate(p2Prefab, player2SpawnPosition, Quaternion.identity);
            p2Instance.layer = LayerMask.NameToLayer(player2LayerName);
        }
    }

    private GameObject GetCharacterPrefab(string characterName, GameObject[] characterPrefabs)
    {
        foreach (GameObject prefab in characterPrefabs)
        {
            if (prefab.name == characterName)
                return prefab;
        }

        Debug.LogWarning($"No se encontró el prefab del personaje: {characterName}");
        return null;
    }
}
