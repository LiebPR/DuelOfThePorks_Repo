using UnityEngine;

public class ShrinkingZone : MonoBehaviour
{
    public float shrinkRate = 0.5f;
    public float minSize = 2f;

    private void Update()
    {
        if (transform.localScale.x > minSize)
        {
            Vector3 shrinkAmount = Vector3.one * shrinkRate * Time.deltaTime;
            transform.localScale -= shrinkAmount;
        }
    }
}