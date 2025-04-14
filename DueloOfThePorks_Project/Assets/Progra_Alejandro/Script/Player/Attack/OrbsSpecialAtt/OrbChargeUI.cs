using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbChargeUI : MonoBehaviour
{
    [SerializeField] Image fillImage;

    Coroutine flashRoutine;

    private void Awake()
    {
        if(fillImage == null)
        {
            fillImage = transform.GetChild(0).GetComponent<Image>();//Agarrar el hijo automaticamente
        }
    }

    //Llama esto cuando recolectas una orbe
    public void UpdateFill(float currentOrbs, float maxOrbs)
    {
        float fillAmount = currentOrbs / maxOrbs;
        fillImage.fillAmount = fillAmount;
    }

    public void ResetBar()
    {
        fillImage.fillAmount = 0f;
    }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public void FlashRed()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRedRoutine());
    }

    IEnumerator FlashRedRoutine()
    {
        Color originalColor = fillImage.color;
        for (int i = 0; i < 2; i++)
        {
            //Desvanecer la opacidad baja(0.3f) de forma suave
            float elapsedTime = 0f;
            float duration = 0.1f; //Duración para devanecerse

            while(elapsedTime < duration)
            {
                fillImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0.3f, elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;
            while(elapsedTime < duration)
            {
                fillImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0.3f, 1f, elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        fillImage.color = originalColor;
    }
}
