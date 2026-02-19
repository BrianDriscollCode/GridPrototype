using UnityEngine;

public class PlayerStatSheet : MonoBehaviour
{
    // Scriptable Object
    public CharacterStatSheetScriptableObject statSheet;

    // StateManagement Hook
    public bool turnComplete;

    // Living Stats
    public int health;
    public int maxHealth;

    // Attribute Stats
    public int strength;
    public int intellect;
    public int dexterity;
    public int wisdom;
    public int charisma;

    // action points
    public int movementPoints;
    public int maxMovementPoints;
    public int attackPoints;
    public int maxAttackPoints;

    // tertiary stats
    public int fightingSpirit;

    public void Start()
    {
        turnComplete = false;

        health = statSheet.health;
        maxHealth = statSheet.maxHealth;

        strength = statSheet.strength;
        intellect = statSheet.intellect;
        dexterity = statSheet.dexterity;
        wisdom = statSheet.wisdom;
        charisma = statSheet.charisma;

        movementPoints = statSheet.movementPoints;
        maxMovementPoints = statSheet.maxMovementPoints;
        attackPoints = statSheet.attackPoints;
        maxAttackPoints = statSheet.maxAttackPoints;

        fightingSpirit = statSheet.fightingSpirit; 
    }
}
