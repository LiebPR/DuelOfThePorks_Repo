using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<CharacterData> characters;
    public Transform player1Spawn;
    public Transform player2Spawn;

    private void Start()
    {
        int player1Index = PlayerPrefs.GetInt("Player1Character", 0);
        int player2Index = PlayerPrefs.GetInt("Player2Character", 0);

        Instantiate(characters[player1Index].characterPrefab, player1Spawn.position, Quaternion.identity);
        Instantiate(characters[player2Index].characterPrefab, player2Spawn.position, Quaternion.identity);
    }
}