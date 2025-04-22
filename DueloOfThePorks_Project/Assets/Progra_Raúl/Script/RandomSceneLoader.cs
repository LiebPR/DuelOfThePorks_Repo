using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomSceneLoader : MonoBehaviour
{
    public CharacterSelector player1Selector;
    public CharacterSelector player2Selector;

    [Header("Escenas disponibles")]
    public string[] scenesToLoad;

    private bool sceneLoaded = false;

    void Update()
    {
        if (!sceneLoaded && player1Selector.IsConfirmed && player2Selector.IsConfirmed)
        {
            LoadRandomScene();
        }
    }

    void LoadRandomScene()
    {
        if (scenesToLoad.Length == 0)
        {
            Debug.LogError("No se han especificado escenas en el array 'scenesToLoad'.");
            return;
        }

        // Guardar la selección de personajes en PlayerPrefs
        PlayerPrefs.SetString("Player1Character", player1Selector.SelectedCharacterName);
        PlayerPrefs.SetString("Player2Character", player2Selector.SelectedCharacterName);
        PlayerPrefs.Save();

        // Elegir escena al azar
        int randomIndex = Random.Range(0, scenesToLoad.Length);
        string selectedScene = scenesToLoad[randomIndex];

        Debug.Log($"Cargando escena aleatoria: {selectedScene}");

        sceneLoaded = true;

        SceneManager.LoadScene(selectedScene);
    }
}
