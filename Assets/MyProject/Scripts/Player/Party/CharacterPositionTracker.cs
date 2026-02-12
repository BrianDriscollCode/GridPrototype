using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterPositionTracker : MonoBehaviour
{
    Dictionary<GameObject, Vector2Int> characterPositionList;
    List<GameObject> allCharacters;

    private void Start()
    {
        characterPositionList = new Dictionary<GameObject, Vector2Int>();
    }

    private void FixedUpdate()
    {
    }

    public void PrintCharacterPositionList()
    {
        foreach (KeyValuePair<GameObject, Vector2Int> kvp in characterPositionList)
        {
            Debug.Log($"{kvp.Key.name}: {kvp.Value}");
        }
    }

    public Dictionary<GameObject, Vector2Int> GetCharactersList()
    {
        allCharacters = GameObject.FindGameObjectsWithTag("Character").ToList();
        if (characterPositionList.Count() > 0)
            characterPositionList.Clear();
        foreach (GameObject c in allCharacters)
        {
            characterPositionList.Add(c, Vector2Int.RoundToInt(c.GetComponent<EntityGridLocation>().gridPos));
        }

        return characterPositionList;
    }

    public void UpdateCharacterLocations()
    {
        foreach (GameObject c in characterPositionList.Keys.ToList())
        {
            characterPositionList[c] = Vector2Int.RoundToInt(c.GetComponent<EntityGridLocation>().gridPos);
        }
    }
}

