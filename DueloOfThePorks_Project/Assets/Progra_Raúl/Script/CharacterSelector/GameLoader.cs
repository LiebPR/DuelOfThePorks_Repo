// GameLoader.cs - corregido con validación de layers "Player1" y "Player2"
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    public GameObject[] combatPrefabs;

    void Start()
    {
        string p1Name = PlayerPrefs.GetString("Player1Character");
        string p2Name = PlayerPrefs.GetString("Player2Character");

        GameObject p1Prefab = FindCombatPrefabByName(p1Name);
        GameObject p2Prefab = FindCombatPrefabByName(p2Name);

        if (p1Prefab != null)
        {
            GameObject p1 = Instantiate(p1Prefab, new Vector2(-2, 0), Quaternion.identity);
            SetLayerRecursivelySafe(p1, "Player1");
        }

        if (p2Prefab != null)
        {
            GameObject p2 = Instantiate(p2Prefab, new Vector2(2, 0), Quaternion.identity);
            SetLayerRecursivelySafe(p2, "Player2");
        }
    }

    GameObject FindCombatPrefabByName(string name)
    {
        foreach (var prefab in combatPrefabs)
            if (prefab.name == name)
                return prefab;
        return null;
    }

    void SetLayerRecursivelySafe(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);

        if (layer < 0 || layer > 31)
        {
            Debug.LogError($"Layer '{layerName}' no existe. Crealo en Edit > Project Settings > Tags and Layers.");
            return;
        }

        SetLayerRecursively(obj, layer);
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}