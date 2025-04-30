// FinalResultTextController.cs
using UnityEngine;
using TMPro;

public class FinalResultTextController : MonoBehaviour
{
    [Header("TextMeshPro 3D – Final results")]
    [SerializeField] private TextMeshPro player1ResultText;
    [SerializeField] private TextMeshPro player2ResultText;

    /// <summary>
    /// Shows in English which player wins, loses, or if it's a draw.
    /// </summary>
    /// <param name="winner">"Player 1", "Player 2" or "Draw"</param>
    public void DisplayResults(string winner)
    {
        switch (winner)
        {
            case "Draw":
                player1ResultText.text = "Player 1 Draw";
                player2ResultText.text = "Player 2 Draw";
                break;
            case "Player 1":
                player1ResultText.text = "Player 1 Winner";
                player2ResultText.text = "Player 2 Loser";
                break;
            case "Player 2":
                player1ResultText.text = "Player 1 Loser";
                player2ResultText.text = "Player 2 Winner";
                break;
            default:
                Debug.LogWarning($"Unknown winner: {winner}");
                player1ResultText.text = "";
                player2ResultText.text = "";
                break;
        }
    }
}
