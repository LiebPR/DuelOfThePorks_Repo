using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance;

    [Header("�ndices de Escenas")]
    public int loadingSceneIndex = 0;   // �ndice de la escena de carga
    public int mainMenuSceneIndex = 1;  // �ndice del Men� Principal
    public int gameSceneIndex = 2;      // �ndice del Juego

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
        PlayerPrefs.SetInt("NextScene", sceneIndex);  // Guardamos el �ndice de la pr�xima escena
        SceneManager.LoadScene(loadingSceneIndex);   // Carga la escena de carga
    }

    // M�todo para cargar el men� principal
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    // M�todo para cargar la escena del juego
    public void LoadGame()
    {
        LoadScene(gameSceneIndex);  // Carga la escena de carga antes de cargar el juego
    }
}

