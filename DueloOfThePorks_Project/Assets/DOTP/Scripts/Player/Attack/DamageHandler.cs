using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageHandler : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    HitDetector hitDetectorDam;

    private void Awake()
    {
        hitDetectorDam = GetComponent<HitDetector>();

        if(damageText == null)
        {
            string tagTosearch = hitDetectorDam != null && hitDetectorDam.isPlayerOne ? "percentagePlayer1" : "percentagePlayer2";
            GameObject damageTextObj = GameObject.FindGameObjectWithTag(tagTosearch);

            if(damageTextObj != null)
            {
                damageText = damageTextObj.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning($"No se encontró un objeto con el tag{tagTosearch} para asignar el porcentaje de daño.");
            }
        }
    }

    public void UpdateHealthDisplay(float damagePercentage)
    {
        if(damageText != null)
        {
            damageText.text = $"{Mathf.RoundToInt(damagePercentage)}%";
        }
    }
}
