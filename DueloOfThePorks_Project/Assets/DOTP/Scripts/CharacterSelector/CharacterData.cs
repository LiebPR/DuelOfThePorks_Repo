using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character")]
public class CharacterData : ScriptableObject
{
    public string characterName; // Nombre del personaje
    public Sprite characterSprite; // Imagen del personaje
    public GameObject characterPrefab; // Prefab del personaje en el juego
}
