using UnityEngine;
using TMPro; // Importante: esto es para usar TextMeshPro normal (no UI)

public class CharacterSelector : MonoBehaviour
{
    public enum PlayerType { Player1, Player2 }
    public enum ControlType { Keyboard, Gamepad }

    [Header("Jugador")]
    public PlayerType playerType;

    [Header("Control")]
    public ControlType controlType = ControlType.Keyboard;

    [Header("Prefabs de vista previa (solo idle)")]
    public GameObject[] characterPreviewPrefabs;
    public Transform previewArea;

    [Header("Texto de nombre (TextMeshPro en el mundo)")]
    public TextMeshPro nameDisplay;

    [Header("Teclas - solo teclado")]
    public KeyCode nextKey = KeyCode.D;
    public KeyCode prevKey = KeyCode.A;
    public KeyCode confirmKey = KeyCode.W;

    [Header("Botones - solo gamepad")]
    public string nextButton = "joystick button 5";    // RB
    public string prevButton = "joystick button 4";    // LB
    public string confirmButton = "joystick button 0"; // A (Xbox), X (PS)

    [Header("Escala del preview")]
    public float previewScale = 2f;

    private int currentIndex = 0;
    private bool confirmed = false;
    private GameObject previewInstance;

    public bool IsConfirmed => confirmed;
    public string SelectedCharacterName => characterPreviewPrefabs[currentIndex].name;

    void Start()
    {
        ShowCharacter(currentIndex);
    }

    void Update()
    {
        if (confirmed) return;

        if (controlType == ControlType.Keyboard)
        {
            if (Input.GetKeyDown(prevKey)) Navigate(-1);
            if (Input.GetKeyDown(nextKey)) Navigate(1);
            if (Input.GetKeyDown(confirmKey)) Confirm();
        }
        else if (controlType == ControlType.Gamepad)
        {
            if (Input.GetKeyDown(prevButton)) Navigate(-1);
            if (Input.GetKeyDown(nextButton)) Navigate(1);
            if (Input.GetKeyDown(confirmButton)) Confirm();
        }
    }

    void Navigate(int dir)
    {
        currentIndex = (currentIndex + dir + characterPreviewPrefabs.Length) % characterPreviewPrefabs.Length;
        ShowCharacter(currentIndex);
    }

    void ShowCharacter(int index)
    {
        if (previewInstance != null) Destroy(previewInstance);

        previewInstance = Instantiate(characterPreviewPrefabs[index], previewArea);
        previewInstance.transform.localPosition = Vector3.zero;
        previewInstance.transform.localScale = Vector3.one * previewScale;

        UpdateNameDisplay();
    }

    void Confirm()
    {
        confirmed = true;
        UpdateNameDisplay();
        Debug.Log($"{playerType} seleccionó: {SelectedCharacterName}");
    }

    void UpdateNameDisplay()
    {
        if (nameDisplay != null)
        {
            nameDisplay.text = confirmed
                ? SelectedCharacterName + " [OK]"
                : SelectedCharacterName;
        }
    }
}
