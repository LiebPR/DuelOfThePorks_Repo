using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbsSpecialAtt : MonoBehaviour, IDamageable
{
    [SerializeField] enum OrbOwner { None, Player1, Player2}
    [SerializeField] OrbOwner currentOwner = OrbOwner.None;
    
    GameObject lastHitter;
    Animator anim;
    SpriteRenderer spriteRenderer;

    [SerializeField] float orbLifetime = 10f;
    [SerializeField] float warningTime = 3f;
    bool isDestroy = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }
    private void Start()
    {
        StartCoroutine(LifetimeRoutine());
    }

    public void ReciveDamage(float damage)
    {
        if (isDestroy) return;

        GameObject hitter = GetLastHitter();
        if (hitter == null) return;

        PlayerOrbs pLayerOrbs = hitter.GetComponent<PlayerOrbs>();
        if(pLayerOrbs != null && pLayerOrbs.CanPickUpOrb())
        {
            pLayerOrbs.AddOrb();
            isDestroy = true;
            StopAllCoroutines(); //Detener la destruccion automatica si fuer recogida
            StartCoroutine(DestroyReturn());
        }
    }

    IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(orbLifetime - warningTime);
        StartCoroutine(FadeWarning());

        yield return new WaitForSeconds(warningTime);
        StartCoroutine(DestroyReturn());
    }

    IEnumerator FadeWarning()
    {
        Color originalColor = spriteRenderer.color;
        float fadeDuration = 0.2f;
        int flashCount = Mathf.FloorToInt(warningTime / (fadeDuration * 2));

        for(int i = 0; i < flashCount; i++)
        {
            yield return StartCoroutine(FadeToAlpha(0.3f, fadeDuration));
            yield return StartCoroutine(FadeToAlpha(1f, fadeDuration));
        }

        spriteRenderer.color = originalColor;
    }

    IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        float startAlpha = spriteRenderer.color.a;
        float time = 0f;

        while(time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, targetAlpha);
    }

    IEnumerator DestroyReturn()
    {
        anim.SetTrigger("Destroy");

        yield return new WaitForSeconds(1.5f);

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
