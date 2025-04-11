using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI loserText;
    public Image victoryImage;
    public Image defeatImage;

    [Header("Character Sprites")]
    public Sprite player1VictorySprite;
    public Sprite player2VictorySprite;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip victoryClip;

    private void Start()
    {
        // Cargar datos
        string winner = PlayerPrefs.GetString("Winner");
        string player1 = PlayerPrefs.GetString("Player1Character");
        string player2 = PlayerPrefs.GetString("Player2Character");

        bool isPlayer1Winner = winner == player1;

        winnerText.text = $"{winner} Wins!";
        loserText.text = $"{(isPlayer1Winner ? player2 : player1)} Loses!";

        // Imágenes
        if (victoryImage != null)
        {
            victoryImage.sprite = isPlayer1Winner ? player1VictorySprite : player2VictorySprite;
        }

        if (defeatImage != null)
        {
            defeatImage.sprite = isPlayer1Winner ? player2VictorySprite : player1VictorySprite;
        }

        // Sonido de victoria
        if (audioSource != null && victoryClip != null)
        {
            audioSource.PlayOneShot(victoryClip);
        }
    }

    public void RetryGame()
    {
        SceneManager.LoadScene("CharacterSelector");
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
