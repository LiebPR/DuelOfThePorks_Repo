// PlayerCharacterSelector.cs
// Opción Aleatorio como “?” (quinta opción)

using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerCharacterSelector : MonoBehaviour
{
    public enum SelectionState { Idle, Transitioning, Confirmed }
    public enum InputMode { Both, OnlyKeyboard, OnlyGamepad }

    [Header("Personajes (Prefabs)")]
    [Tooltip("Lista de prefabs de personajes disponibles para este jugador")]
    public GameObject[] characters;

    [Header("Opción Aleatoria")]
    [Tooltip("¿Incluir la opción Aleatorio al final de la lista?")]
    public bool enableRandomOption = true;
    [Tooltip("Etiqueta que se mostrará para la opción Aleatorio")]
    public string randomOptionLabel = "?";
    [Tooltip("Prefab para la vista previa de Aleatorio (ícono de interrogación)")]
    public GameObject randomPreviewPrefab;

    [Header("Referencias UI")]
    [Tooltip("Texto que muestra el nombre y estado del personaje seleccionado")]
    public TextMeshProUGUI characterNameText;
    [Tooltip("Contenedor para instanciar la vista previa del personaje")]
    public Transform previewArea;

    [Header("Configuración de Transición")]
    [Tooltip("Duración de la transición (slide) en segundos")]
    public float slideDuration = 0.5f;
    [Tooltip("Distancia de desplazamiento de la animación de transición")]
    public float slideDistance = 500f;

    [Header("Inputs - Teclado")]
    public KeyCode nextKey = KeyCode.D;
    public KeyCode previousKey = KeyCode.A;
    public KeyCode confirmKey = KeyCode.W;

    [Header("Inputs - Mando")]
    public string horizontalAxisName = "Horizontal";
    public string confirmButtonName = "Submit";
    public float axisInputDelay = 0.3f;
    public float gamepadThreshold = 0.5f;

    [Header("Modo de Entrada")]
    public InputMode allowedInput = InputMode.Both;

    private int currentIndex = 0;
    private int totalOptions;
    private GameObject currentPreviewInstance;
    private SelectionState state = SelectionState.Idle;
    private float axisInputTimer = 0f;

    public SelectionState State { get { return state; } }

    void Start()
    {
        totalOptions = characters.Length + (enableRandomOption ? 1 : 0);
        if (totalOptions > 0)
        {
            currentIndex = 0;
            CreatePreviewImmediate(currentIndex);
            UpdateCharacterName();
        }
        else
        {
            Debug.LogError("No hay opciones en el selector de " + gameObject.name);
        }
    }

    void Update()
    {
        if (axisInputTimer > 0f)
            axisInputTimer -= Time.deltaTime;

        if (state != SelectionState.Idle)
            return;

        // Teclado
        if (allowedInput == InputMode.Both || allowedInput == InputMode.OnlyKeyboard)
        {
            if (Input.GetKeyDown(previousKey))
                ChangeIndex((currentIndex - 1 + totalOptions) % totalOptions, -1);
            else if (Input.GetKeyDown(nextKey))
                ChangeIndex((currentIndex + 1) % totalOptions, 1);
            else if (Input.GetKeyDown(confirmKey))
                ConfirmSelection();
        }

        // Mando
        if (allowedInput == InputMode.Both || allowedInput == InputMode.OnlyGamepad)
        {
            float h = Input.GetAxis(horizontalAxisName);
            if (axisInputTimer <= 0f)
            {
                if (h > gamepadThreshold)
                {
                    ChangeIndex((currentIndex + 1) % totalOptions, 1);
                    axisInputTimer = axisInputDelay;
                }
                else if (h < -gamepadThreshold)
                {
                    ChangeIndex((currentIndex - 1 + totalOptions) % totalOptions, -1);
                    axisInputTimer = axisInputDelay;
                }
            }
            if (Input.GetButtonDown(confirmButtonName))
                ConfirmSelection();
        }
    }

    private void ChangeIndex(int newIndex, int direction)
    {
        StartCoroutine(TransitionToIndex(newIndex, direction));
        currentIndex = newIndex;
    }

    private void ConfirmSelection()
    {
        // Si confirma “?”, resuelve a uno de los personajes
        if (enableRandomOption && currentIndex == characters.Length)
        {
            int randIdx = Random.Range(0, characters.Length);
            currentIndex = randIdx;
            // Limpia preview “?” y muestra el elegido
            ClearPreview();
            CreatePreviewImmediate(randIdx);
        }
        state = SelectionState.Confirmed;
        UpdateCharacterName();
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
            // Muestra tu prefab de “?” en el centro
            if (randomPreviewPrefab != null)
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

        // Salida
        if (currentPreviewInstance != null)
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
            currentPreviewInstance = null;
        }

        // Entrada
        if (enableRandomOption && newIndex == characters.Length)
        {
            // Instancia “?” fuera de la vista
            if (randomPreviewPrefab != null)
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

        // Animación de entrada
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
        if (currentPreviewInstance != null)
        {
            Destroy(currentPreviewInstance);
            currentPreviewInstance = null;
        }
    }

    public string GetSelectedCharacterName()
    {
        return characters != null && characters.Length > 0
            ? characters[currentIndex].name
            : "";
    }
}
