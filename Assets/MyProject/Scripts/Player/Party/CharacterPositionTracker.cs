using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterPositionTracker : MonoBehaviour
{
    [SerializeField] private List<GameObject> characterKeys = new List<GameObject>();
    [SerializeField] private List<Vector2Int> characterPositions = new List<Vector2Int>();
    [SerializeField] private List<GameObject> allCharacters;

    // This is the list that communicates with other components in scene
    public Dictionary<GameObject, Vector2Int> characterPositionList;

    private void Awake()
    {
        characterPositionList = new Dictionary<GameObject, Vector2Int>();
    }

    private void FixedUpdate()
    {
        UpdateInspectorView();
    }

    private void UpdateInspectorView()
    {
        characterKeys.Clear();
        characterPositions.Clear();
        
        foreach (var kvp in characterPositionList)
        {
            characterKeys.Add(kvp.Key);
            characterPositions.Add(kvp.Value);
        }
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

        if (characterPositionList != null)
        {
            if (characterPositionList.Count() > 0)
                characterPositionList.Clear();
        }
        
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

