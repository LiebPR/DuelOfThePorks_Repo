using UnityEngine;
using TMPro; // Para TextMeshPro

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

    [Header("Colores de texto")]
    public Color defaultColor = Color.white;
    public Color confirmedColor = Color.green;

    [Header("Audio Clips")]
    [Tooltip("Sonido al navegar entre personajes")]
    public AudioClip navigateClip;
    [Tooltip("Sonido al confirmar selección")]
    public AudioClip confirmClip;
    [Tooltip("Sonido al deseleccionar personaje")]
    public AudioClip deselectClip;

    [Header("Teclas - solo teclado")]
    public KeyCode nextKey = KeyCode.D;
    public KeyCode prevKey = KeyCode.A;
    public KeyCode confirmKey = KeyCode.W;
    public KeyCode deselectKey = KeyCode.S;

    [Header("Botones - solo gamepad")]
    public string nextButton = "joystick button 5"; // RB
    public string prevButton = "joystick button 4"; // LB
    public string confirmButton = "joystick button 0"; // A/X
    public string deselectButton = "joystick button 1"; // B/Círculo

    [Header("Escala del preview")]
    public float previewScale = 2f;

    private int currentIndex;
    private bool confirmed;
    private GameObject previewInstance;
    private AudioSource audioSource;

    public bool IsConfirmed => confirmed;
    public string SelectedCharacterName => characterPreviewPrefabs.Length > 0 ? characterPreviewPrefabs[currentIndex].name : string.Empty;

    void Awake()
    {
        // Obtener o crear AudioSource local para reproducir efectos
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
    }

    void Start()
    {
        currentIndex = 0;
        if (nameDisplay != null)
            nameDisplay.color = defaultColor;
        ShowCharacter(currentIndex);
    }

    void Update()
    {
        if (!confirmed)
        {
            // Navegación y confirmación
            if (controlType == ControlType.Keyboard)
            {
                if (Input.GetKeyDown(prevKey)) Navigate(-1);
                if (Input.GetKeyDown(nextKey)) Navigate(1);
                if (Input.GetKeyDown(confirmKey)) Confirm();
            }
            else
            {
                if (Input.GetKeyDown(prevButton)) Navigate(-1);
                if (Input.GetKeyDown(nextButton)) Navigate(1);
                if (Input.GetKeyDown(confirmButton)) Confirm();
            }
        }
        else
        {
            // Deseleccionar
            if ((controlType == ControlType.Keyboard && Input.GetKeyDown(deselectKey)) ||
                (controlType == ControlType.Gamepad && Input.GetKeyDown(deselectButton)))
            {
                Deselect();
            }
        }
    }

    void Navigate(int dir)
    {
        if (characterPreviewPrefabs == null || characterPreviewPrefabs.Length == 0) return;
        currentIndex = (currentIndex + dir + characterPreviewPrefabs.Length) % characterPreviewPrefabs.Length;
        ShowCharacter(currentIndex);
        PlaySound(navigateClip);
    }

    void Confirm()
    {
        confirmed = true;
        if (nameDisplay != null)
            nameDisplay.color = confirmedColor;
        PlaySound(confirmClip);
        Debug.Log($"{playerType} seleccionó: {SelectedCharacterName}");
    }

    void Deselect()
    {
        confirmed = false;
        PlaySound(deselectClip);
        if (nameDisplay != null)
            nameDisplay.color = defaultColor;
        UpdateNameDisplay();
        Debug.Log($"{playerType} deseleccionó su elección.");
    }

    void ShowCharacter(int index)
    {
        if (previewInstance != null)
            Destroy(previewInstance);
        if (characterPreviewPrefabs == null || characterPreviewPrefabs.Length == 0) return;

        previewInstance = Instantiate(characterPreviewPrefabs[index], previewArea);
        previewInstance.transform.localPosition = Vector3.zero;
        previewInstance.transform.localScale = Vector3.one * previewScale;

        UpdateNameDisplay();
        if (nameDisplay != null)
            nameDisplay.color = defaultColor;
    }

    void UpdateNameDisplay()
    {
        if (nameDisplay != null)
            nameDisplay.text = SelectedCharacterName;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}