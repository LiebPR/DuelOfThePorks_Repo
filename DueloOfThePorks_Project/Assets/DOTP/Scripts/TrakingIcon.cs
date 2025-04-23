using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrakingIcon : MonoBehaviour
{
    [SerializeField] LayerMask IconLayer;
    [SerializeField] float followSpeed = 5f;
    [SerializeField] float searchInterval = 1f;
    [SerializeField] float verticalOffset = 1.5f;

    Transform target;
    float searchTimer;

    private void Update()
    {
        searchTimer -= Time.deltaTime;

        // Reintenta encontrar target si no hay uno, o si el target está inactivo o ha cambiado de layer
        if (target == null || !target.gameObject.activeInHierarchy || ((1 << target.gameObject.layer) & IconLayer.value) == 0)
        {
            if (searchTimer <= 0f)
            {
                FindTarget();
                searchTimer = searchInterval;
            }
        }

        // Seguir al target si es válido
        if (target != null && target.gameObject.activeInHierarchy)
        {
            Vector3 targetPos = new Vector3(target.position.x, target.position.y + verticalOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }

    void FindTarget()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.activeInHierarchy && ((1 << obj.layer) & IconLayer.value) != 0)
            {
                target = obj.transform;
                transform.position = new Vector3(target.position.x, target.position.y + verticalOffset, transform.position.z);
                break;
            }
        }
    }
}