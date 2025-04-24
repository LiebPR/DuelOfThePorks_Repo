using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInitializer : MonoBehaviour
{
    [Header("Identificadores")]
    public bool isPlayerOne = true;

    [Header("Respawn Settings")]
    public string respawnTag = "Player1Respawn";

    private LifeManager lifeManager;

    void Awake()
    {
        lifeManager = GetComponent<LifeManager>();
        if (lifeManager == null)
        {
            Debug.LogWarning("LifeManager no encontrado en " + gameObject.name);
            return;
        }

        AssignHeartPanel();// Buscar y asignar corazones
        AssignRespawnPoints();// Buscar y asignar puntos de respawn
        AssignOrbUI(); // Asignamos la barra de orbes también

    }

    private void AssignHeartPanel()
    {
        string panelName = isPlayerOne ? "UI_Player1" : "UI_Player2";
        GameObject panel = GameObject.Find(panelName);

        if (panel != null)
        {
            List<Image> hearts = new List<Image>();

            foreach (Transform child in panel.transform)
            {
                Image img = child.GetComponent<Image>();
                if (img != null)
                {
                    hearts.Add(img);
                    Debug.Log($"Corazón agregado: {child.name}");
                }
            }

            if (hearts.Count == 0)
            {
                Debug.LogWarning("No se encontraron corazones válidos en " + panelName);
            }

            // Orden por jerarquía visual
            Image[] heartImages = hearts.OrderBy(h => h.transform.GetSiblingIndex()).ToArray();
            lifeManager.SetHeartImages(heartImages);
        }
        else
        {
            Debug.LogError("No se encontró el panel de corazones: " + panelName);
        }
    }

    private void AssignRespawnPoints()
    {
        string respawnTagForPlayer = isPlayerOne ? "Player1Respawn" : "Player2Respawn";
        GameObject[] respawnObjects = GameObject.FindGameObjectsWithTag(respawnTagForPlayer);

        if (respawnObjects.Length == 0)
        {
            Debug.LogError("No se encontraron puntos de respawn con tag: " + respawnTagForPlayer);
            return;
        }

        Transform[] points = new Transform[respawnObjects.Length];
        for (int i = 0; i < respawnObjects.Length; i++)
        {
            points[i] = respawnObjects[i].transform;
        }

        lifeManager.SetRespawnPoints(points);
    }

    void AssignOrbUI()
    {
        string uiName = isPlayerOne ? "OrbUI_Player1" : "OrbUI_Player2";
        GameObject orbUIObject = GameObject.Find(uiName);

        if(orbUIObject != null)
        {
            OrbChargeUI ui = orbUIObject.GetComponent<OrbChargeUI>();
            PlayerOrbs orbs = GetComponent<PlayerOrbs>();
            if(ui != null && orbs != null)
            {
                orbs.SetOrbUI(ui);
            }
            else
            {
                Debug.LogWarning($"No se puede asignar Orb UI en {gameObject.name}: falstan componentes");
            }
        }
        else
        {
            Debug.LogError($"No se encontró el objeto de Orb UI: {uiName}");
        }
    }
}