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

    [Header("Configuración de Inputs - Teclado")]
    [Tooltip("Tecla para seleccionar el siguiente personaje (teclado)")]
    public KeyCode nextKey = KeyCode.D;
    [Tooltip("Tecla para seleccionar el personaje anterior (teclado)")]
    public KeyCode previousKey = KeyCode.A;
    [Tooltip("Tecla para confirmar la selección (teclado)")]
    public KeyCode confirmKey = KeyCode.W;

    [Header("Configuración de Inputs - Mando")]
    [Tooltip("Nombre del eje horizontal (definido en el Input Manager) para el mando")]
    public string horizontalAxisName = "Horizontal";
    [Tooltip("Nombre del botón de confirmación (definido en el Input Manager) para el mando")]
    public string confirmButtonName = "Submit";
    [Tooltip("Retraso entre entradas (mando) para evitar entradas repetidas")]
    public float axisInputDelay = 0.3f;
    [Tooltip("Umbral del eje para considerar la entrada del mando")]
    public float gamepadThreshold = 0.5f;

    [Header("Configuración de Modo de Entrada")]
    [Tooltip("Selecciona desde el Inspector qué métodos de entrada estarán permitidos")]
    public InputMode allowedInput = InputMode.Both;

    private int currentIndex = 0;
    private GameObject currentPreviewInstance;
    private SelectionState state = SelectionState.Idle;
    private float axisInputTimer = 0f;

    public SelectionState State { get { return state; } }

    void Start()
    {
        if (characters != null && characters.Length > 0)
        {
            CreatePreviewImmediate(currentIndex);
            UpdateCharacterName();
        }
        else
        {
            Debug.LogError("No se han asignado prefabs de personajes en " + gameObject.name);
        }
    }

    void Update()
    {
        if (axisInputTimer > 0f)
            axisInputTimer -= Time.deltaTime;

        if (state == SelectionState.Idle)
        {
            // Procesar entrada por teclado si se permite
            if (allowedInput == InputMode.Both || allowedInput == InputMode.OnlyKeyboard)
            {
                if (Input.GetKeyDown(previousKey))
                {
                    int newIndex = (currentIndex - 1 + characters.Length) % characters.Length;
                    StartCoroutine(TransitionToCharacter(newIndex, -1));
                    currentIndex = newIndex;
                }
                else if (Input.GetKeyDown(nextKey))
                {
                    int newIndex = (currentIndex + 1) % characters.Length;
                    StartCoroutine(TransitionToCharacter(newIndex, 1));
                    currentIndex = newIndex;
                }
                if (Input.GetKeyDown(confirmKey))
                {
                    state = SelectionState.Confirmed;
                    UpdateCharacterName();
                }
            }

            // Procesar entrada por mando si se permite
            if (allowedInput == InputMode.Both || allowedInput == InputMode.OnlyGamepad)
            {
                float horizontalInput = Input.GetAxis(horizontalAxisName);
                if (axisInputTimer <= 0f)
                {
                    if (horizontalInput > gamepadThreshold)
                    {
                        int newIndex = (currentIndex + 1) % characters.Length;
                        StartCoroutine(TransitionToCharacter(newIndex, 1));
                        currentIndex = newIndex;
                        axisInputTimer = axisInputDelay;
                    }
                    else if (horizontalInput < -gamepadThreshold)
                    {
                        int newIndex = (currentIndex - 1 + characters.Length) % characters.Length;
                        StartCoroutine(TransitionToCharacter(newIndex, -1));
                        currentIndex = newIndex;
                        axisInputTimer = axisInputDelay;
                    }
                }
                if (Input.GetButtonDown(confirmButtonName))
                {
                    state = SelectionState.Confirmed;
                    UpdateCharacterName();
                }
            }
        }
    }

    /// <summary>
    /// Actualiza el texto del nombre utilizando el nombre del prefab y muestra "[READY]" si la selección fue confirmada.
    /// </summary>
    void UpdateCharacterName()
    {
        characterNameText.text = characters[currentIndex].name + (state == SelectionState.Confirmed ? " [READY]" : "");
    }

    /// <summary>
    /// Instancia el preview del personaje sin animación.
    /// </summary>
    /// <param name="index">Índice del personaje a mostrar</param>
    void CreatePreviewImmediate(int index)
    {
        if (currentPreviewInstance != null)
            Destroy(currentPreviewInstance);
        currentPreviewInstance = Instantiate(characters[index], previewArea);
        currentPreviewInstance.transform.localPosition = Vector3.zero;
        currentPreviewInstance.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Realiza la transición animada entre previews utilizando corutinas y Lerp.
    /// </summary>
    /// <param name="newIndex">Nuevo índice de personaje</param>
    /// <param name="direction">Dirección de la transición (1 para avanzar, -1 para retroceder)</param>
    IEnumerator TransitionToCharacter(int newIndex, int direction)
    {
        state = SelectionState.Transitioning;

        // Animación de salida del preview actual
        if (currentPreviewInstance != null)
        {
            Vector3 startPos = currentPreviewInstance.transform.localPosition;
            Vector3 endPos = startPos + new Vector3(direction * slideDistance, 0, 0);
            float elapsed = 0f;
            while (elapsed < slideDuration)
            {
                currentPreviewInstance.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / slideDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            currentPreviewInstance.transform.localPosition = endPos;
            Destroy(currentPreviewInstance);
        }

        // Instanciar el nuevo preview fuera de la vista (lado opuesto)
        GameObject newPreview = Instantiate(characters[newIndex], previewArea);
        newPreview.transform.localPosition = new Vector3(-direction * slideDistance, 0, 0);
        newPreview.transform.localScale = Vector3.one;

        // Animación de entrada del nuevo preview
        float elapsedIn = 0f;
        Vector3 startPosIn = newPreview.transform.localPosition;
        Vector3 targetPos = Vector3.zero;
        while (elapsedIn < slideDuration)
        {
            newPreview.transform.localPosition = Vector3.Lerp(startPosIn, targetPos, elapsedIn / slideDuration);
            elapsedIn += Time.deltaTime;
            yield return null;
        }
        newPreview.transform.localPosition = targetPos;
        currentPreviewInstance = newPreview;
        state = SelectionState.Idle;
        UpdateCharacterName();
    }

    /// <summary>
    /// Devuelve el nombre del personaje actualmente seleccionado (tomado del nombre del prefab).
    /// </summary>
    public string GetSelectedCharacterName()
    {
        return (characters != null && characters.Length > 0) ? characters[currentIndex].name : "";
    }
}
