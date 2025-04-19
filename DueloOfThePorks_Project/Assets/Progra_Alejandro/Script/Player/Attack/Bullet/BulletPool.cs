using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class BulletPool : MonoBehaviour
{
    //Instancia única (singelton
    public static BulletPool Instance { get; private set; }

    [SerializeField] private GameObject bulletPrefab;//Prefab para la bala
    [SerializeField] private int poolSize = 20;//Tamaño inicial de pool

    private Queue<GameObject> bulletPool = new Queue<GameObject>();//Cola para gestionar las bullet

    private void Awake()
    {
        //Verificamos que solo haya una instancia del bulletPool en la escena
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //Inicializamos el pool de balas
        InitializePool();
    }

    //Metodo que inicializa el pool creando una cantidad definida de balas inactivas.
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false); //Desactivamos la bala para que no esté en uso inicialmente
            bulletPool.Enqueue(bullet); //Añadiremos la bala al pool
        }
    }

    //Metodo para obtener una bala del pool
    public GameObject GetBullet()
    {
        if (bulletPool.Count > 0)
        {
            //Si hay balas disponibles, las sacamos del pool
            return bulletPool.Dequeue();
        }
        //Si el pool está vacío, opcionalmente podemos expandirlo
        //Crea una nueva bala y devuelve una referencia de ella
        Debug.LogWarning("Bullet pool empty!");
        return CreateNewBullet();
    }

    //Metodo para crear una bala en caso de que el pool esté vacío (expansion dinámica)
    private GameObject CreateNewBullet()
    {
        GameObject newBullet = Instantiate(bulletPrefab);
        newBullet.SetActive(false);
        return newBullet;
    }

    //Metodo para devolver una bala al pool después de ser ultilizada
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false); //Desactivamos la bala
        bulletPool.Enqueue(bullet); //La devolvemos al pool para su reutilización
    }
}
