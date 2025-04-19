using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    public CharacterSelector player1Selector;
    public CharacterSelector player2Selector;
    public string sceneToLoad = "Scene_Game";

    void Update()
    {
        if (player1Selector.IsConfirmed && player2Selector.IsConfirmed)
        {
            // Guardar solo el nombre del personaje seleccionado
            PlayerPrefs.SetString("Player1Character", player1Selector.SelectedCharacterName);
            PlayerPrefs.SetString("Player2Character", player2Selector.SelectedCharacterName);

            // Cargar la escena de combate
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
