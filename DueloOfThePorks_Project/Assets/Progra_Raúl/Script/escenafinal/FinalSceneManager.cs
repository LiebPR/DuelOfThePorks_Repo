using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FinalSceneManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI loserText;

    [Header("Áreas de Instanciación")]
    [SerializeField] private Transform victorySpawnPoint;
    [SerializeField] private Transform defeatSpawnPoint;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Ruta de Prefabs")]
    [SerializeField] private string prefabFolderPath = "Characters";

    private void Start()
    {
        string winnerName = PlayerPrefs.GetString("Winner", "Player1");
        string player1 = PlayerPrefs.GetString("Player1Character", "Player1");
        string player2 = PlayerPrefs.GetString("Player2Character", "Player2");

        string loserName = (winnerName == player1) ? player2 : player1;

        // Cargar los prefabs de los personajes
        GameObject winnerPrefab = Resources.Load<GameObject>($"{prefabFolderPath}/{winnerName}");
        GameObject loserPrefab = Resources.Load<GameObject>($"{prefabFolderPath}/{loserName}");

        if (winnerPrefab == null || loserPrefab == null)
        {
            Debug.LogError("No se encontraron los prefabs de los personajes.");
            return;
        }

        // Obtener datos visuales desde los prefabs
        CharacterVisuals winnerVisuals = winnerPrefab.GetComponent<CharacterVisuals>();
        CharacterVisuals loserVisuals = loserPrefab.GetComponent<CharacterVisuals>();

        if (winnerText != null) winnerText.text = $"{winnerName} Wins!";
        if (loserText != null) loserText.text = $"{loserName} Loses!";

        // Instanciar animación de victoria y derrota
        if (winnerVisuals?.victoryPrefab != null && victorySpawnPoint != null)
            Instantiate(winnerVisuals.victoryPrefab, victorySpawnPoint.position, Quaternion.identity, victorySpawnPoint);

        if (loserVisuals?.defeatPrefab != null && defeatSpawnPoint != null)
            Instantiate(loserVisuals.defeatPrefab, defeatSpawnPoint.position, Quaternion.identity, defeatSpawnPoint);

        // Reproducir sonido
        if (audioSource != null && winnerVisuals?.victorySound != null)
            audioSource.PlayOneShot(winnerVisuals.victorySound);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene("CharacterSelector");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
