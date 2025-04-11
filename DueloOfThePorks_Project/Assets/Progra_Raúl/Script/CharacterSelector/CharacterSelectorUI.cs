using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectorUI : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string name;
        public Sprite image;
        public GameObject prefab; // Prefab de cada personaje
    }

    [Header("Character Options")]
    public Character[] player1Characters; // Los personajes para Player 1
    public Character[] player2Characters; // Los personajes para Player 2

    [Header("Player 1 UI")]
    public Image player1Image;
    public TextMeshProUGUI player1Name;

    [Header("Player 2 UI")]
    public Image player2Image;
    public TextMeshProUGUI player2Name;

    [Header("Scene Settings")]
    public string nextScene = "JUEGO";

    private int p1Index = 0;
    private int p2Index = 0;
    private bool p1Ready = false;
    private bool p2Ready = false;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        // --- Player 1: A/D para mover, W para confirmar ---
        if (!p1Ready)
        {
            if (Input.GetKeyDown(KeyCode.A)) { p1Index = (p1Index - 1 + player1Characters.Length) % player1Characters.Length; UpdateUI(); }
            if (Input.GetKeyDown(KeyCode.D)) { p1Index = (p1Index + 1) % player1Characters.Length; UpdateUI(); }
            if (Input.GetKeyDown(KeyCode.W)) { p1Ready = true; UpdateUI(); }
        }

        // --- Player 2: ←/→ para mover, ↑ para confirmar ---
        if (!p2Ready)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { p2Index = (p2Index - 1 + player2Characters.Length) % player2Characters.Length; UpdateUI(); }
            if (Input.GetKeyDown(KeyCode.RightArrow)) { p2Index = (p2Index + 1) % player2Characters.Length; UpdateUI(); }
            if (Input.GetKeyDown(KeyCode.UpArrow)) { p2Ready = true; UpdateUI(); }
        }

        // --- Si ambos están listos, cargar escena ---
        if (p1Ready && p2Ready)
        {
            PlayerPrefs.SetString("Player1Character", player1Characters[p1Index].name);
            PlayerPrefs.SetString("Player2Character", player2Characters[p2Index].name);
            SceneManager.LoadScene(nextScene);
        }
    }

    void UpdateUI()
    {
        // Actualiza la UI de Player 1
        player1Image.sprite = player1Characters[p1Index].image;
        player1Name.text = player1Characters[p1Index].name + (p1Ready ? " [READY]" : "");

        // Actualiza la UI de Player 2
        player2Image.sprite = player2Characters[p2Index].image;
        player2Name.text = player2Characters[p2Index].name + (p2Ready ? " [READY]" : "");
    }
}
