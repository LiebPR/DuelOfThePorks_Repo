using UnityEngine;
using TMPro; // Importar TextMeshPro
using System.Collections;

public class DamagePercentage : MonoBehaviour
{
    [Header("Porcentaje de daño")]
    public float damagePercentage = 0f;
    [SerializeField] private float minDamageIncrease = 5f; // Mínimo incremento
    [SerializeField] private float maxDamageIncrease = 15f; // Máximo incremento

    [Header("UI")]
    public TextMeshProUGUI damageText; // Referencia al texto en pantalla

    [Header("Knowtime")]
    public float knowTime = 2f; // Tiempo en segundos que el porcentaje se mantendrá antes de ser modificado

    [Header("Empuje")]
    public float pushForce = 5f; // Fuerza de empuje
    public float pushMultiplier = 1.5f; // Multíplicador de empuje cuando el daño llega a 100%

    [Header("Muerte del Jugador")]
    public float deathChanceFactor = 100f; // Factor de multiplicación para la probabilidad de muerte

    private float internalDamagePercentage = 0f; // Segundo porcentaje interno
    private float deathThreshold = 0f; // Umbral de muerte que será aleatorio
    private bool isInternalPercentageActive = false; // Controla si el segundo porcentaje está activo

    private Rigidbody rb; // Referencia al Rigidbody del enemigo
    private Transform player; // Referencia al jugador (para aplicar el empuje hacia él)

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtener el Rigidbody del enemigo
        player = GameObject.FindGameObjectWithTag("Player").transform; // Buscar al jugador en la escena
        UpdateDamageText();
    }

    public void TakeDamage()
    {
        StartCoroutine(ApplyDamageOverTime());
    }

    private IEnumerator ApplyDamageOverTime()
    {
        yield return new WaitForSeconds(knowTime);

        // Después de esperar el tiempo, el daño se aplica según el ataque realizado
        UpdateDamageText(); // Actualiza la UI después del tiempo determinado
    }

    public void UpdateDamageText()
    {
        if (damageText != null)
        {
            // Redondea el porcentaje de daño a un número entero y actualiza el texto
            damageText.text = $"Daño: {Mathf.RoundToInt(damagePercentage)}%";
        }
    }

    // Métodos de ataques que aumentan el daño con un rango aleatorio
    public void Attack1() { ApplyRandomDamage(5f, 10f); }
    public void Attack2() { ApplyRandomDamage(8f, 15f); }
    public void Attack3() { ApplyRandomDamage(12f, 20f); }
    public void Attack4() { ApplyRandomDamage(15f, 25f); }

    // Aplica un incremento aleatorio basado en el ataque realizado
    private void ApplyRandomDamage(float minIncrease, float maxIncrease)
    {
        // Genera un aumento de daño aleatorio dentro del rango especificado
        float increase = Random.Range(minIncrease, maxIncrease);
        damagePercentage += increase;

        // Debug: Verificar el porcentaje de daño después de aplicar el incremento
        Debug.Log($"Nuevo porcentaje de daño: {damagePercentage}%");

        // Si el daño alcanza o supera 100 y está entre 100 y 200, el jugador muere
        if (damagePercentage >= 100f)
        {
            if (!isInternalPercentageActive)
            {
                // Activa el segundo porcentaje cuando el daño alcance el 100%
                internalDamagePercentage = Random.Range(0f, 50f); // Inicia un segundo porcentaje aleatorio
                deathThreshold = Random.Range(100f, 200f); // Establece un umbral aleatorio de muerte entre 100 y 200
                isInternalPercentageActive = true;
            }
        }

        // Si el segundo porcentaje está activo, incrementa las posibilidades de muerte
        if (isInternalPercentageActive)
        {
            internalDamagePercentage += Random.Range(0f, 1f); // Incrementa el porcentaje interno

            // Si el daño total está en el rango de 100 a 200 y el porcentaje interno es suficientemente alto, el jugador muere
            if (damagePercentage >= 100f && damagePercentage <= 200f)
            {
                float deathChance = Mathf.Clamp01(internalDamagePercentage / deathChanceFactor); // Calcula las probabilidades de muerte
                if (Random.Range(0f, 1f) < deathChance)
                {
                    Die();
                }
            }
        }

        // Verifica el empuje
        ApplyPush(); // Empuje aumentado si está en el rango de daño
        UpdateDamageText(); // Actualiza la UI inmediatamente
    }

    // Aplica empuje al jugador cuando el porcentaje de daño es 100 o más
    private void ApplyPush()
    {
        if (player == null) return; // Si no encontramos al jugador, no hacemos nada

        Vector3 pushDirection = (player.position - transform.position).normalized; // Dirección hacia el jugador
        float appliedPushForce = damagePercentage >= 100f ? pushForce * pushMultiplier : pushForce; // Aplica empuje aumentado si el daño es mayor o igual a 100%

        // Aplicar la fuerza al Rigidbody del jugador
        player.GetComponent<Rigidbody>().AddForce(pushDirection * appliedPushForce, ForceMode.Impulse);
    }

    // Método que maneja la "muerte" del jugador
    private void Die()
    {
        // Aquí puedes agregar la lógica de muerte, por ejemplo:
        Debug.Log("El jugador ha muerto debido a daño excesivo.");

        // Opcional: Puedes desactivar el objeto del jugador
        gameObject.SetActive(false); // Esto desactiva al jugador

        // O puedes agregar una animación de muerte, o cualquier otro comportamiento
        // Por ejemplo, podrías mostrar una UI de Game Over, etc.
    }
}
