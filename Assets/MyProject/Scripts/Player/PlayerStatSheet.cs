using UnityEngine;

public class PlayerStatSheet : MonoBehaviour
{
    // Scriptable Object
    public CharacterStatSheetScriptableObject statSheet;

    // StateManagement Hook
    public bool turnComplete;

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

    public void Start()
    {
        turnComplete = false;

        health = statSheet.health;

        strength = statSheet.strength;
        intellect = statSheet.intellect;
        dexterity = statSheet.dexterity;
        wisdom = statSheet.wisdom;
        charisma = statSheet.charisma;

        movementPoints = statSheet.movementPoints;
        attackPoints = statSheet.attackPoints;

        fightingSpirit = statSheet.fightingSpirit;
    }
}
