using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageHandler : MonoBehaviour
{
    public TextMeshProUGUI damageText;

    public void UpdateHealthDisplay(float damagePercentage)
    {
        if(damageText != null)
        {
            damageText.text = $"Daño: {Mathf.RoundToInt(damagePercentage)}%";
        }
    }
}
