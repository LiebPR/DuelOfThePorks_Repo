
// GameLoader.cs - Usa prefabs completos para el combate
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
            SetLayerRecursively(p1, LayerMask.NameToLayer("Player 1"));
        }

        if (p2Prefab != null)
        {
            GameObject p2 = Instantiate(p2Prefab, new Vector2(2, 0), Quaternion.identity);
            SetLayerRecursively(p2, LayerMask.NameToLayer("Player 2"));
        }
    }

    GameObject FindCombatPrefabByName(string name)
    {
        foreach (var prefab in combatPrefabs)
            if (prefab.name == name)
                return prefab;
        return null;
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
