using System.Collections;
using UnityEngine;

public class ThroughtPlatform : MonoBehaviour
{
    private Collider2D platformCollider;

    [Header("Configuración de la Plataforma")]
    [SerializeField] float fallTime = 0.3f;

    [Header("Detección del Jugador")]
    public Transform player;
    [SerializeField] LayerMask playerLayer;

    [Header("Configuración del OverlapBox")]
    [SerializeField] Vector2 checkBoxSize = new Vector2(2f, 0.5f);
    [SerializeField] Vector2 checkBoxOffset = new Vector2(0f, 0.5f); 

    float keyPressTime = -1f;
    [SerializeField] float keyPressLimit = 0.5f; 

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (IsPlayerAbove())
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                keyPressTime = Time.time; 
            }

            if (keyPressTime > 0 && Time.time - keyPressTime <= keyPressLimit && Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(DisableColliderTemporarily());
                keyPressTime = -1f; 
            }
        }
    }

    IEnumerator DisableColliderTemporarily()
    {
        platformCollider.enabled = false;
        yield return new WaitForSeconds(fallTime);
        platformCollider.enabled = true;
    }

    bool IsPlayerAbove()
    {
        if (player == null) return false;

        Vector2 checkPosition = (Vector2)transform.position + checkBoxOffset;
        Collider2D hit = Physics2D.OverlapBox(checkPosition, checkBoxSize, 0f, playerLayer);

        return hit != null && hit.transform == player;
    }

 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 checkPosition = (Vector2)transform.position + checkBoxOffset;
        Gizmos.DrawWireCube(checkPosition, checkBoxSize);
    }
}


