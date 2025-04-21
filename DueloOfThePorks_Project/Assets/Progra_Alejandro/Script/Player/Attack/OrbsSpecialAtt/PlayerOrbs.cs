using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrbs : MonoBehaviour
{
    [SerializeField] int currentOrbs = 0;
    [SerializeField] int maxOrbs = 3;

    [SerializeField] OrbChargeUI orbChargeUI;

    private void Start()
    {
        if(orbChargeUI != null)
        {
            orbChargeUI.SetVisible(true);
            orbChargeUI.UpdateFill(currentOrbs, maxOrbs);
        }
    }
    public bool CanPickUpOrb()
    {
        return currentOrbs < maxOrbs;
    }

    public void AddOrb()
    {
        if (CanPickUpOrb())
        {
            currentOrbs++;
            UpdateUI();
        }
    }

    public bool CanUseSpecialAttack()
    {
        return currentOrbs >= maxOrbs;
    }

    public void ConsumeOrbs()
    {
        currentOrbs = 0;
        UpdateUI();

        if(orbChargeUI != null)
        {
            orbChargeUI.ResetBar();
            orbChargeUI.SetVisible(false); //true si quieres que quede la barra vacía
        }
    }

    public void RemoveOrb()
    {
        Debug.Log($"[PlayerOrbs.RemoveOrb] llamando en {gameObject.name}, orbs antes: {currentOrbs}");
        if(currentOrbs > 0)
        {
            currentOrbs--;
            UpdateUI();

            if(orbChargeUI != null)
            {
                orbChargeUI.FlashRed(); //La orbe de orbes parpadea al perder 1
            }
        }
    }

    void UpdateUI()
    {
        if(orbChargeUI != null)
        {
            orbChargeUI.UpdateFill(currentOrbs, maxOrbs);
        }
    }
}
