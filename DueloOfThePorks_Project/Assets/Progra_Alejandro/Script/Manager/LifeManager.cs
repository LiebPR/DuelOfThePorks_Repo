using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] int maxVidas = 3;
    int vidas;

    [SerializeField] Transform[] puntosDeRespawn;
    [SerializeField] HitDetector hitDetector; //Reinicia el daño
    

    private void Start()
    {
        vidas = maxVidas;
        if(hitDetector == null)
        {
            hitDetector = GetComponent<HitDetector>();
        }
        hitDetector.damagePercentage = 0f;
        hitDetector.damageHandler?.UpdateHealthDisplay(0f);
    }
    public void Die()
    {
        vidas--;
        if(vidas <= 0)
        {
            Debug.Log($"{(hitDetector.isPlayerOne ? "Player1" : "Player2")} se ha quedado sin vidas. GAME OVER. ");
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"{(hitDetector.isPlayerOne ? "Player1" : "Player2")} pierde una vida. Respawneando...");
            Respawn();
        }
    }

    void Respawn()
    {
        //Reseteamos el daño
        hitDetector.damagePercentage = 0f;

        //Respawn aleatorio
        int index = Random.Range(0, puntosDeRespawn.Length);
        transform.position = puntosDeRespawn[index].position;

        //Actualiza visualmente el porcentaje
        if (hitDetector.damageHandler != null)
        {
            hitDetector.damageHandler?.UpdateHealthDisplay(hitDetector.damagePercentage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("DeathZone"))
        {
            Debug.Log("Zona de muerte tocada, perdiendo vida");
            Die();
        }
    }
}
