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
    [Tooltip("Sonido al navegar entre personajes")] public AudioClip navigateClip;
    [Tooltip("Sonido al confirmar selección")] public AudioClip confirmClip;
    [Tooltip("Sonido al deseleccionar personaje")] public AudioClip deselectClip;

    [Header("Teclas - solo teclado")]
    public KeyCode nextKey = KeyCode.D;
    public KeyCode prevKey = KeyCode.A;
    public KeyCode confirmKey = KeyCode.W;
    public KeyCode deselectKey = KeyCode.S;

    [Header("Botones - solo gamepad")]
    public string nextButton = "joystick button 5";
    public string prevButton = "joystick button 4";
    public string confirmButton = "joystick button 0";
    public string deselectButton = "joystick button 1";

    [Header("Escala del preview")]
    public float previewScale = 2f;

    [Header("Opción Aleatoria")]
    [Tooltip("Activa una opción adicional que selecciona un personaje al azar")] public bool includeRandomOption = false;
    [Tooltip("Prefab para la vista previa que muestra el símbolo de interrogación")] public GameObject randomPreviewPrefab;

    [Header("Offset para ?")]
    [Tooltip("Desplazamiento local solo para la vista previa aleatoria")] public Vector3 randomPreviewOffset = Vector3.zero;

    private int currentIndex;
    private bool confirmed;
    private GameObject previewInstance;
    private AudioSource audioSource;

    // Índice real seleccionado al confirmar la opción aleatoria
    private int selectedActualIndex = 0;

    public bool IsConfirmed => confirmed;

    // Nombre real para guardar (resuelve random)
    public string SelectedCharacterName
    {
        get
        {
            if (includeRandomOption && currentIndex == characterPreviewPrefabs.Length)
                return characterPreviewPrefabs[selectedActualIndex].name;
            return characterPreviewPrefabs[Mathf.Clamp(currentIndex, 0, characterPreviewPrefabs.Length - 1)].name;
        }
    }

    // Nombre mostrado al usuario: mantiene '?' para random
    public string DisplayName
    {
        get
        {
            if (includeRandomOption && currentIndex == characterPreviewPrefabs.Length)
                return "?";
            return SelectedCharacterName;
        }
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
    }

    void Start()
    {
        currentIndex = 0;
        selectedActualIndex = 0;
        if (nameDisplay != null)
        {
            nameDisplay.color = defaultColor;
            nameDisplay.text = DisplayName;
        }
        ShowCharacter(currentIndex);
    }

    void Update()
    {
        if (!confirmed)
        {
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
            if ((controlType == ControlType.Keyboard && Input.GetKeyDown(deselectKey)) ||
                (controlType == ControlType.Gamepad && Input.GetKeyDown(deselectButton)))
            {
                Deselect();
            }
        }
    }

    void Navigate(int dir)
    {
        int options = characterPreviewPrefabs.Length + (includeRandomOption ? 1 : 0);
        if (options == 0) return;
        currentIndex = (currentIndex + dir + options) % options;
        ShowCharacter(currentIndex);
        PlaySound(navigateClip);
    }

    void Confirm()
    {
        confirmed = true;
        if (includeRandomOption && currentIndex == characterPreviewPrefabs.Length)
            selectedActualIndex = Random.Range(0, characterPreviewPrefabs.Length);
        else
            selectedActualIndex = currentIndex;

        if (nameDisplay != null)
        {
            nameDisplay.color = confirmedColor;
            nameDisplay.text = DisplayName;
        }
        PlaySound(confirmClip);
        Debug.Log($"{playerType} seleccionó: {SelectedCharacterName}");
    }

    void Deselect()
    {
        confirmed = false;
        selectedActualIndex = currentIndex;
        if (nameDisplay != null)
        {
            nameDisplay.color = defaultColor;
            nameDisplay.text = DisplayName;
        }
        PlaySound(deselectClip);
        Debug.Log($"{playerType} deseleccionó su elección.");
    }

    void ShowCharacter(int index)
    {
        if (previewInstance != null)
            Destroy(previewInstance);

        if (includeRandomOption && index == characterPreviewPrefabs.Length && randomPreviewPrefab != null)
            previewInstance = Instantiate(randomPreviewPrefab, previewArea);
        else if (characterPreviewPrefabs != null && characterPreviewPrefabs.Length > 0)
            previewInstance = Instantiate(
                characterPreviewPrefabs[Mathf.Clamp(index, 0, characterPreviewPrefabs.Length - 1)],
                previewArea
            );

        if (previewInstance != null)
        {
            previewInstance.transform.localScale = Vector3.one * previewScale;
            previewInstance.transform.localPosition = (includeRandomOption && index == characterPreviewPrefabs.Length)
                ? randomPreviewOffset
                : Vector3.zero;
        }

        if (nameDisplay != null)
        {
            nameDisplay.text = DisplayName;
            if (!confirmed)
                nameDisplay.color = defaultColor;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
