using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Importar TextMeshPro

public class CharacterSelector : MonoBehaviour
{
    public List<CharacterData> characters; // Lista de personajes disponibles
    public Image characterImage; // Imagen UI del personaje
    public TMP_Text characterNameText; // Nombre del personaje con TextMeshPro
    public int playerNumber = 1; // Número del jugador (1 o 2)

    private int currentIndex = 0;
    private string playerKey;

    private void Start()
    {
        playerKey = "Player" + playerNumber + "Character";
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        characterImage.sprite = characters[currentIndex].characterSprite;
        characterNameText.text = characters[currentIndex].characterName;
    }

    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characters.Count;
        UpdateCharacterDisplay();
    }

    public void PreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + characters.Count) % characters.Count;
        UpdateCharacterDisplay();
    }

    public void SelectCharacter()
    {
        PlayerPrefs.SetInt(playerKey, currentIndex);
        Debug.Log("Jugador " + playerNumber + " seleccionó: " + characters[currentIndex].characterName);
    }
}
