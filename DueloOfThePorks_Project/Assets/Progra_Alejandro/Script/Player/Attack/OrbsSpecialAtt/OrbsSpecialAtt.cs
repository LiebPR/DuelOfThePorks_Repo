using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbsSpecialAtt : MonoBehaviour, IDamageable
{
    [SerializeField] enum OrbOwner { None, Player1, Player2}
    [SerializeField] OrbOwner currentOwner = OrbOwner.None;
    
    GameObject lastHitter;
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ReciveDamage(float damage)
    {
        //Identificamos quién la golpeó
        GameObject hitter = GetLastHitter();
        if (hitter == null) return;

        PlayerOrbs pLayerOrbs = hitter.GetComponent<PlayerOrbs>();
        if(pLayerOrbs != null && pLayerOrbs.CanPickUpOrb())
        {
            pLayerOrbs.AddOrb();
            Destroy(gameObject);
        }
    }

    public void SetLastHitter(GameObject hitter)
    {
        lastHitter = hitter;
    }

    GameObject GetLastHitter()
    {
        return lastHitter;
    }
}
