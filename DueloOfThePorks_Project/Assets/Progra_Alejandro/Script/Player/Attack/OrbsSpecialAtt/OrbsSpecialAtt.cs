using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbsSpecialAtt : MonoBehaviour, IDamageable
{
    [SerializeField] enum OrbOwner { None, Player1, Player2}
    [SerializeField] OrbOwner currentOwner = OrbOwner.None;
    
    GameObject lastHitter;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
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
            StartCoroutine(DestroyReturn());
        }
    }

    IEnumerator DestroyReturn()
    {
        anim.SetTrigger("Destroy");

        yield return new WaitForSeconds(0.5714286f);

        Destroy(gameObject);
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
