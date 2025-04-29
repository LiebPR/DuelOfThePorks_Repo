using UnityEngine;

public class GameLoader : MonoBehaviour
{
    [Header("Prefabs de combate disponibles")]
    public GameObject[] combatPrefabs;

    [Header("Posiciones de spawn")]
    public Vector2 player1Start = new Vector2(-2, 0);
    public Vector2 player2Start = new Vector2(2, 0);

    void Start()
    {
        // Leemos lo que guardó el selector; puede ser un nombre o "?"
        string p1Name = PlayerPrefs.GetString("Player1Character");
        string p2Name = PlayerPrefs.GetString("Player2Character");

        // Si era "?", elegimos un prefab al azar solo ahora, al cargar la partida
        if (p1Name == "?")
            p1Name = combatPrefabs[Random.Range(0, combatPrefabs.Length)].name;
        if (p2Name == "?")
            p2Name = combatPrefabs[Random.Range(0, combatPrefabs.Length)].name;

        // Guardamos de nuevo para que el resto del juego sepa quién salió
        PlayerPrefs.SetString("Player1Character", p1Name);
        PlayerPrefs.SetString("Player2Character", p2Name);
        PlayerPrefs.Save();

        // Buscamos y generamos los prefabs correspondientes
        GameObject p1Prefab = FindCombatPrefabByName(p1Name);
        GameObject p2Prefab = FindCombatPrefabByName(p2Name);

        if (p1Prefab != null)
        {
            GameObject p1 = Instantiate(p1Prefab, player1Start, Quaternion.identity);
            SetLayerRecursivelySafe(p1, "Player1");
        }

        if (p2Prefab != null)
        {
            GameObject p2 = Instantiate(p2Prefab, player2Start, Quaternion.identity);
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
            Debug.LogError($"Layer '{layerName}' no existe. Créalo en Edit > Project Settings > Tags and Layers.");
            return;
        }
        SetLayerRecursively(obj, layer);
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
