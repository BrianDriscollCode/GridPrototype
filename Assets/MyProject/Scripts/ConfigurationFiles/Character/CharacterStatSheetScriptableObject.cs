using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatSheetScriptableObject", menuName = "Scriptable Objects/CharacterStatSheetScriptableObject")]
public class CharacterStatSheetScriptableObject : ScriptableObject
{
    // Living Stats
    public int health;

    // Attribute Stats
    public int strength;
    public int intellect;
    public int dexterity;
    public int wisdom;
    public int charisma;

    // action points
    public int movementPoints;
    public int attackPoints;

    // tertiary stats
    public int fightingSpirit;
}