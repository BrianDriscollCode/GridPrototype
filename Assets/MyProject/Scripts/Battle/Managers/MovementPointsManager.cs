using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class MovementPointsManager : MonoBehaviour
{
    public ManagerRegistry managerRegistry;
    public CharacterRegisterManager characterRegisterManager;

    public List<GameObject> characters;
   

    private bool isCharacterListPopulated;

    private void Start()
    {
        managerRegistry = GameObject.FindAnyObjectByType<ManagerRegistry>();

        // Make this a helper method within ManagerRegistry
        if (managerRegistry != null)
        {
            GameObject managerObject = managerRegistry.managerList.Find(obj => obj.GetComponent<CharacterRegisterManager>() != null);

            if (managerObject != null)
            {
                characterRegisterManager = managerObject.GetComponent<CharacterRegisterManager>();
            }
        }
    }

    private void Update()
    {
       if (!isCharacterListPopulated)
        {
            isCharacterListPopulated = true;
            characters = characterRegisterManager.characters;
        }
    }


    private void OnEnable()
    {
        EventManager.ClickedTile += HandleTileClicked;
        //Debug.Log("Listener: subscribed to ClickedTile");
    }

    private void OnDisable()
    {
        EventManager.ClickedTile -= HandleTileClicked;
        //Debug.Log("Listener: unsubscribed from ClickedTile");
    }

    public void HandleTileClicked(Vector2Int gridPos)
    {

    }

    public bool IsWithinMovementPointsLimit(int playerMovementPoints, int cost)
    {
        return playerMovementPoints > cost ? true : false;
    }

    /// <summary>
    /// Calculates Chebyshev distance (diagonals count as 1 tile)
    /// Example: (0,0) to (2,2) = 2 tiles (moving diagonally)
    /// </summary>
    public int CalculateMovementCost(Vector2Int currentLocation, Vector2Int destinationLocation)
    {
        int deltaX = Mathf.Abs(destinationLocation.x - currentLocation.x);
        int deltaY = Mathf.Abs(destinationLocation.y - currentLocation.y);

        int distance = Mathf.Max(deltaX, deltaY);

        Debug.Log($"Movement from ({currentLocation.x},{currentLocation.y}) to ({destinationLocation.x},{destinationLocation.y}) = {distance} tiles (with diagonals)");

        return distance;
    }


}
