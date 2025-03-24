using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance;

    [Header("Índices de Escenas")]
    public int loadingSceneIndex = 0;   // Índice de la escena de carga
    public int mainMenuSceneIndex = 1;  // Índice del Menú Principal
    public int gameSceneIndex = 2;      // Índice del Juego

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(int sceneIndex)
    {
        PlayerPrefs.SetInt("NextScene", sceneIndex);  // Guardamos el índice de la próxima escena
        SceneManager.LoadScene(loadingSceneIndex);   // Carga la escena de carga
    }

    // Método para cargar el menú principal
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    // Método para cargar la escena del juego
    public void LoadGame()
    {
        LoadScene(gameSceneIndex);  // Carga la escena de carga antes de cargar el juego
    }
}

