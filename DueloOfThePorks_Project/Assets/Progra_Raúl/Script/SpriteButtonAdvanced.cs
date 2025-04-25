using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SpriteButtonAdvanced : MonoBehaviour
{
    [Header("Botón Settings")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool quitGame = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSourceHover;
    private AudioSource audioSourceClick;

    [Header("Visual Settings")]
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    [SerializeField] private float hoverDarkenAmount = 0.8f;

    [Header("Texto Settings")]
    [SerializeField] private TextMeshPro hoverText;
    [SerializeField] private string textOnHover = "EXIT";

    private bool isHovering = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (hoverText != null)
            hoverText.text = "";

        // Crear automáticamente los AudioSources para hover y click
        if (hoverSound != null)
        {
            audioSourceHover = gameObject.AddComponent<AudioSource>();
            audioSourceHover.clip = hoverSound;
            audioSourceHover.playOnAwake = false;
        }

        if (clickSound != null)
        {
            audioSourceClick = gameObject.AddComponent<AudioSource>();
            audioSourceClick.clip = clickSound;
            audioSourceClick.playOnAwake = false;
        }
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (!isHovering)
            {
                isHovering = true;
                OnHoverEnter();
            }

            if (Input.GetMouseButtonDown(0))
            {
                OnClick();
            }
        }
        else
        {
            if (isHovering)
            {
                isHovering = false;
                OnHoverExit();
            }
        }
    }

    void OnHoverEnter()
    {
        if (audioSourceHover != null)
            audioSourceHover.Play();

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor * hoverDarkenAmount;

        if (hoverText != null)
            hoverText.text = textOnHover;
    }

    void OnHoverExit()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        if (hoverText != null)
            hoverText.text = "";
    }

    void OnClick()
    {
        if (audioSourceClick != null)
            audioSourceClick.Play();

        if (quitGame)
            QuitGame();
        else if (!string.IsNullOrEmpty(sceneToLoad))
            LoadScene();
    }

    void LoadScene()
    {
        Debug.Log($"Cargando escena: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
