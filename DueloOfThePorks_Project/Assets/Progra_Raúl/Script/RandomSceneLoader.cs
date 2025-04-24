using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomSceneLoader : MonoBehaviour
{
    [Header("Selectores de personaje")]
    public CharacterSelector player1Selector;
    public CharacterSelector player2Selector;

    [Header("Escenas disponibles")]
    public string[] scenesToLoad;

    // Para evitar cargar repetidamente
    private bool hasLoaded = false;

    void Update()
    {
        // Esperar a que ambos jugadores confirmen su selección
        if (!hasLoaded
            && player1Selector != null && player2Selector != null
            && player1Selector.IsConfirmed && player2Selector.IsConfirmed)
        {
            // Guardar selección en PlayerPrefs
            PlayerPrefs.SetString("Player1Character", player1Selector.SelectedCharacterName);
            PlayerPrefs.SetString("Player2Character", player2Selector.SelectedCharacterName);
            PlayerPrefs.Save();

            // Elegir escena aleatoria del array
            if (scenesToLoad != null && scenesToLoad.Length > 0)
            {
                int index = Random.Range(0, scenesToLoad.Length);
                string sceneName = scenesToLoad[index];
                hasLoaded = true;
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("RandomSceneLoader: no hay escenas configuradas en 'Scenes To Load'.");
            }
        }
    }
}
