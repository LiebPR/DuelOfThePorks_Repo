using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Controla la UI de daño mostrando solo el número,
/// con dos modos:
///  - followPlayer: sigue automáticamente sobre la cabeza del jugador,
///                 con padding mundo y offset UI ajustables.
///  - estático: permanece fijo en el Canvas.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(HitDetector))]
public class DamageHandler : MonoBehaviour
{
    [Header("UI de Daño")]
    [Tooltip("TextMeshProUGUI que mostrará el número de daño.")]
    public TextMeshProUGUI damageText;

    [Header("Modo")]
    [Tooltip("Si está activo, sigue al jugador; si no, permanece fijo en Canvas.")]
    public bool followPlayer = true;

    [Header("Follow Configuración")]
    [Tooltip("Espacio extra (unidades mundo) por encima de la cabeza.")]
    public float followPadding = 0.2f;
    [Tooltip("Offset UI (anchoredPosition) tras conversión world->canvas.")]
    public Vector2 anchoredPosFollow = new Vector2(-63.7f, -726.2f);
    [Tooltip("SizeDelta en modo follow.")]
    public Vector2 sizeDeltaFollow = new Vector2(100f, 30f);
    [Tooltip("FontSize en modo follow.")]
    public float fontSizeFollow = 53f;

    [Header("Estático Configuración")]
    [Tooltip("AnchoredPosition en modo estático.")]
    public Vector2 anchoredPosStatic = new Vector2(472f, -328f);
    [Tooltip("SizeDelta en modo estático.")]
    public Vector2 sizeDeltaStatic = new Vector2(100f, 30f);
    [Tooltip("FontSize en modo estático.")]
    public float fontSizeStatic = 150f;

    private RectTransform _rt;
    private Camera _cam;
    private BoxCollider2D _bc;
    private SpriteRenderer _sr;
    private bool _initialized;

    void Initialize()
    {
        if (!_initialized)
        {
            if (damageText == null)
            {
                var hd = GetComponent<HitDetector>();
                if (hd != null)
                {
                    string tag = hd.isPlayerOne ? "percentagePlayer1" : "percentagePlayer2";
                    var go = GameObject.FindGameObjectWithTag(tag);
                    if (go != null)
                        damageText = go.GetComponent<TextMeshProUGUI>();
                }
                if (damageText == null) return;
            }

            _rt = damageText.rectTransform;
            _cam = Camera.main;
            _bc = GetComponent<BoxCollider2D>();
            if (_bc == null) _sr = GetComponent<SpriteRenderer>();
            _initialized = true;
        }

        if (followPlayer)
        {
            _rt.sizeDelta = sizeDeltaFollow;
            damageText.fontSize = fontSizeFollow;
        }
        else
        {
            _rt.sizeDelta = sizeDeltaStatic;
            damageText.fontSize = fontSizeStatic;
            _rt.anchoredPosition = anchoredPosStatic;
            damageText.enabled = true;
        }
    }

    void Awake() => Initialize();
    void OnEnable() => Initialize();
    void OnValidate() => Initialize();

    void LateUpdate()
    {
        if (!_initialized || damageText == null || _rt == null) return;

        if (followPlayer)
        {
            float headY;
            if (_bc != null)
                headY = _bc.bounds.max.y;
            else if (_sr != null)
                headY = transform.position.y + _sr.bounds.extents.y;
            else
                headY = transform.position.y;

            Vector3 worldPos = new Vector3(transform.position.x, headY + followPadding, transform.position.z);
            Vector3 screenPos = _cam.WorldToScreenPoint(worldPos);
            bool visible = screenPos.z >= 0f;
            damageText.enabled = visible;
            if (!visible) return;

            RectTransform canvasRect = damageText.canvas.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                damageText.canvas.renderMode == RenderMode.ScreenSpaceCamera
                    ? damageText.canvas.worldCamera
                    : null,
                out Vector2 localPoint
            );
            _rt.anchoredPosition = localPoint + anchoredPosFollow;
        }
    }

    /// <summary>
    /// Actualiza solo el número redondeado.
    /// </summary>
    public void UpdateHealthDisplay(float damagePercentage)
    {
        if (damageText != null)
            damageText.text = Mathf.RoundToInt(damagePercentage).ToString();
    }

#if UNITY_EDITOR
    void MarkDirty() { if (!Application.isPlaying) EditorUtility.SetDirty(this); }
#endif
}
