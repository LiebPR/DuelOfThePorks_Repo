// PlayerCharacterSelector.cs
using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayerCharacterSelector : MonoBehaviour
{
    public enum SelectionState { Idle, Transitioning, Confirmed }
    public enum InputMode { Both, OnlyKeyboard, OnlyGamepad }

    [Header("Identificador del Jugador")]
    public string playerID = "P1"; // P1 o P2

    [Header("Personajes (Prefabs)")]
    public GameObject[] characters;

    [Header("Opción Aleatoria")]
    public bool enableRandomOption = true;
    public string randomOptionLabel = "?";
    public GameObject randomPreviewPrefab;

    [Header("Referencias UI")]
    public TextMeshProUGUI characterNameText;
    public Transform previewArea;

    [Header("Transición")]
    public float slideDuration = 0.5f;
    public float slideDistance = 500f;

    [Header("Teclado")]
    public KeyCode nextKey = KeyCode.D;
    public KeyCode previousKey = KeyCode.A;
    public KeyCode confirmKey = KeyCode.W;

    [Header("Mando")]
    public string horizontalAxisName = "Horizontal";
    public string confirmButtonName = "Submit";
    public float axisInputDelay = 0.3f;
    public float gamepadThreshold = 0.5f;

    [Header("Modo de Entrada")]
    public InputMode allowedInput = InputMode.Both;

    [Header("Sonido")]
    public AudioClip navigateClip;
    public AudioClip confirmClip;
    public bool enableSound = true;

    private AudioSource audioSource;
    private int currentIndex = 0;
    private int totalOptions;
    private GameObject currentPreviewInstance;
    private SelectionState state = SelectionState.Idle;
    private float axisInputTimer = 0f;

    public SelectionState State => state;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        totalOptions = characters.Length + (enableRandomOption ? 1 : 0);

        if (totalOptions == 0)
        {
            Debug.LogError("No hay personajes asignados en " + gameObject.name);
            enabled = false;
            return;
        }

        currentIndex = 0;
        CreatePreviewImmediate(currentIndex);
        UpdateCharacterName();
    }

    void Update()
    {
        if (state != SelectionState.Idle) return;
        if (axisInputTimer > 0f) axisInputTimer -= Time.deltaTime;

        // Teclado
        if (allowedInput == InputMode.Both || allowedInput == InputMode.OnlyKeyboard)
        {
            if (Input.GetKeyDown(previousKey)) OnNavigate((currentIndex - 1 + totalOptions) % totalOptions, -1);
            else if (Input.GetKeyDown(nextKey)) OnNavigate((currentIndex + 1) % totalOptions, 1);
            else if (Input.GetKeyDown(confirmKey)) OnConfirm();
        }

        // Mando
        if (allowedInput == InputMode.Both || allowedInput == InputMode.OnlyGamepad)
        {
            float h = Input.GetAxis(horizontalAxisName);
            if (axisInputTimer <= 0f)
            {
                if (h > gamepadThreshold)
                {
                    OnNavigate((currentIndex + 1) % totalOptions, 1);
                    axisInputTimer = axisInputDelay;
                }
                else if (h < -gamepadThreshold)
                {
                    OnNavigate((currentIndex - 1 + totalOptions) % totalOptions, -1);
                    axisInputTimer = axisInputDelay;
                }
            }

            if (Input.GetButtonDown(confirmButtonName)) OnConfirm();
        }
    }

    void OnNavigate(int newIndex, int direction)
    {
        PlaySound(navigateClip);
        StartCoroutine(TransitionToIndex(newIndex, direction));
        currentIndex = newIndex;
    }

    void OnConfirm()
    {
        PlaySound(confirmClip);

        if (enableRandomOption && currentIndex == characters.Length)
        {
            currentIndex = Random.Range(0, characters.Length);
            ClearPreview();
            CreatePreviewImmediate(currentIndex);
        }

        state = SelectionState.Confirmed;
        UpdateCharacterName();
    }

    void PlaySound(AudioClip clip)
    {
        if (enableSound && clip && audioSource)
            audioSource.PlayOneShot(clip);
    }

    void UpdateCharacterName()
    {
        string name = (enableRandomOption && currentIndex == characters.Length)
            ? randomOptionLabel
            : characters[currentIndex].name;

        characterNameText.text = name + (state == SelectionState.Confirmed ? " [READY]" : "");
    }

    void CreatePreviewImmediate(int index)
    {
        ClearPreview();

        if (enableRandomOption && index == characters.Length)
        {
            if (randomPreviewPrefab)
            {
                currentPreviewInstance = Instantiate(randomPreviewPrefab, previewArea);
                currentPreviewInstance.transform.localPosition = Vector3.zero;
                currentPreviewInstance.transform.localScale = Vector3.one;
            }
        }
        else
        {
            currentPreviewInstance = Instantiate(characters[index], previewArea);
            currentPreviewInstance.transform.localPosition = Vector3.zero;
            currentPreviewInstance.transform.localScale = Vector3.one;
        }
    }

    IEnumerator TransitionToIndex(int newIndex, int direction)
    {
        state = SelectionState.Transitioning;

        if (currentPreviewInstance)
        {
            Vector3 start = currentPreviewInstance.transform.localPosition;
            Vector3 end = start + new Vector3(direction * slideDistance, 0, 0);
            float t = 0f;
            while (t < slideDuration)
            {
                currentPreviewInstance.transform.localPosition = Vector3.Lerp(start, end, t / slideDuration);
                t += Time.deltaTime;
                yield return null;
            }
            Destroy(currentPreviewInstance);
        }

        if (enableRandomOption && newIndex == characters.Length)
        {
            if (randomPreviewPrefab)
            {
                currentPreviewInstance = Instantiate(randomPreviewPrefab, previewArea);
                currentPreviewInstance.transform.localPosition = new Vector3(-direction * slideDistance, 0, 0);
                currentPreviewInstance.transform.localScale = Vector3.one;
            }
        }
        else
        {
            currentPreviewInstance = Instantiate(characters[newIndex], previewArea);
            currentPreviewInstance.transform.localPosition = new Vector3(-direction * slideDistance, 0, 0);
            currentPreviewInstance.transform.localScale = Vector3.one;
        }

        float tIn = 0f;
        Vector3 startIn = currentPreviewInstance.transform.localPosition;
        while (tIn < slideDuration)
        {
            currentPreviewInstance.transform.localPosition = Vector3.Lerp(startIn, Vector3.zero, tIn / slideDuration);
            tIn += Time.deltaTime;
            yield return null;
        }

        state = SelectionState.Idle;
        UpdateCharacterName();
    }

    void ClearPreview()
    {
        if (currentPreviewInstance)
        {
            Destroy(currentPreviewInstance);
            currentPreviewInstance = null;
        }
    }

    public string GetSelectedCharacterName()
    {
        return (characters != null && characters.Length > 0)
            ? characters[currentIndex].name
            : "";
    }
}
