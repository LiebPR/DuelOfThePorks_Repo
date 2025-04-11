using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CharacterSelectorUI : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string name;
        public GameObject previewPrefab; // Prefab del personaje con animación idle
        public GameObject renderCameraPrefab; // Prefab que contiene una cámara con RenderTexture
    }

    [Header("Character Options")]
    public Character[] player1Characters;
    public Character[] player2Characters;

    [Header("Player 1 UI")]
    public RawImage player1RenderImage;
    public TextMeshProUGUI player1Name;

    [Header("Player 2 UI")]
    public RawImage player2RenderImage;
    public TextMeshProUGUI player2Name;

    [Header("Scene Settings")]
    public string nextScene = "JUEGO";

    private int p1Index = 0;
    private int p2Index = 0;
    private bool p1Ready = false;
    private bool p2Ready = false;

    private GameObject p1PreviewInstance;
    private GameObject p2PreviewInstance;

    private GameObject p1CameraInstance;
    private GameObject p2CameraInstance;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (!p1Ready)
        {
            if (Input.GetKeyDown(KeyCode.A)) { ChangeCharacter(-1, true); }
            if (Input.GetKeyDown(KeyCode.D)) { ChangeCharacter(1, true); }
            if (Input.GetKeyDown(KeyCode.W)) { p1Ready = true; UpdateUI(); }
        }

        if (!p2Ready)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { ChangeCharacter(-1, false); }
            if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeCharacter(1, false); }
            if (Input.GetKeyDown(KeyCode.UpArrow)) { p2Ready = true; UpdateUI(); }
        }

        if (p1Ready && p2Ready)
        {
            PlayerPrefs.SetString("Player1Character", player1Characters[p1Index].name);
            PlayerPrefs.SetString("Player2Character", player2Characters[p2Index].name);
            SceneManager.LoadScene(nextScene);
        }
    }

    void ChangeCharacter(int direction, bool isPlayer1)
    {
        if (isPlayer1)
        {
            p1Index = (p1Index + direction + player1Characters.Length) % player1Characters.Length;
        }
        else
        {
            p2Index = (p2Index + direction + player2Characters.Length) % player2Characters.Length;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        // Actualizar vista previa P1
        if (p1PreviewInstance) Destroy(p1PreviewInstance);
        if (p1CameraInstance) Destroy(p1CameraInstance);
        var char1 = player1Characters[p1Index];
        p1PreviewInstance = Instantiate(char1.previewPrefab, new Vector3(-1000, 0, 0), Quaternion.identity);
        p1CameraInstance = Instantiate(char1.renderCameraPrefab);
        player1RenderImage.texture = p1CameraInstance.GetComponent<Camera>().targetTexture;
        player1Name.text = char1.name + (p1Ready ? " [READY]" : "");

        // Actualizar vista previa P2
        if (p2PreviewInstance) Destroy(p2PreviewInstance);
        if (p2CameraInstance) Destroy(p2CameraInstance);
        var char2 = player2Characters[p2Index];
        p2PreviewInstance = Instantiate(char2.previewPrefab, new Vector3(1000, 0, 0), Quaternion.identity);
        p2CameraInstance = Instantiate(char2.renderCameraPrefab);
        player2RenderImage.texture = p2CameraInstance.GetComponent<Camera>().targetTexture;
        player2Name.text = char2.name + (p2Ready ? " [READY]" : "");
    }
}
