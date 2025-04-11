using UnityEngine;

public class GameSetup : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject[] player1CharacterPrefabs; // Prefabs de los personajes de Player 1
    public GameObject[] player2CharacterPrefabs; // Prefabs de los personajes de Player 2

    void Start()
    {
        // Obtener los nombres de los personajes seleccionados
        string p1CharacterName = PlayerPrefs.GetString("Player1Character");
        string p2CharacterName = PlayerPrefs.GetString("Player2Character");

        // Instanciar el prefab correspondiente para Player 1
        GameObject player1Prefab = GetCharacterPrefab(p1CharacterName, player1CharacterPrefabs);
        if (player1Prefab != null)
        {
            Instantiate(player1Prefab, new Vector2(-2, 0), Quaternion.identity);
        }

        // Instanciar el prefab correspondiente para Player 2
        GameObject player2Prefab = GetCharacterPrefab(p2CharacterName, player2CharacterPrefabs);
        if (player2Prefab != null)
        {
            Instantiate(player2Prefab, new Vector2(2, 0), Quaternion.identity);
        }
    }

    // Método para obtener el prefab según el nombre
    GameObject GetCharacterPrefab(string characterName, GameObject[] characterPrefabs)
    {
        foreach (GameObject prefab in characterPrefabs)
        {
            if (prefab.name == characterName)
            {
                return prefab;
            }
        }
        return null;
    }
}