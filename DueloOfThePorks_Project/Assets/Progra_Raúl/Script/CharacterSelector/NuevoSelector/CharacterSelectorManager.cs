// CharacterSelectorManager.cs
// (Sin cambios respecto al original) :contentReference[oaicite:0]{index=0}&#8203;:contentReference[oaicite:1]{index=1}

using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectorManager : MonoBehaviour
{
    [Header("Selectores de Jugador")]
    [Tooltip("Referencia al selector del Player 1")]
    public PlayerCharacterSelector player1Selector;
    [Tooltip("Referencia al selector del Player 2")]
    public PlayerCharacterSelector player2Selector;

    [Header("Configuración de Escenas")]
    [Tooltip("Nombre de la escena destino del juego")]
    public string nextScene = "Scene_Pract";
    [Tooltip("Nombre de la escena de carga")]
    public string loadingScene = "EscenaCarga";

    void Update()
    {
        if (player1Selector.State == PlayerCharacterSelector.SelectionState.Confirmed &&
            player2Selector.State == PlayerCharacterSelector.SelectionState.Confirmed)
        {
            PlayerPrefs.SetString("Player1Character", player1Selector.GetSelectedCharacterName());
            PlayerPrefs.SetString("Player2Character", player2Selector.GetSelectedCharacterName());
            PlayerPrefs.SetString("EscenaDestino", nextScene);
            SceneManager.LoadScene(loadingScene);
        }
    }
}
