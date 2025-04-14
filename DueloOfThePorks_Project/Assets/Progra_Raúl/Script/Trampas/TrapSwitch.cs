using UnityEngine;

public class TrapSwitch : MonoBehaviour
{
    public GameObject fireTrap;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & (1 << LayerMask.NameToLayer("Player1")) | (1 << LayerMask.NameToLayer("Player2"))) != 0)
        {
            fireTrap.GetComponent<FireTrap>()?.Activate();
        }
    }
}