using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyAI : MonoBehaviour
{
    public ManagerRegistry managerRegistry;
    public MovementPointsManager movementPointsManager;
    public CharacterRegisterManager characterRegisterManager;
    public IUSO_Battle_EnemyTurn_State enemyBattleState;

    public GridManager gridManager;

    private List<GameObject> playerParty;
    private List<GameObject> enemyParty;

    public GameObject currentEnemy;
    public GameObject currentTarget;

    // Temp ingame vars
    int attackDistance = 1;

    private void Start()
    {
        managerRegistry = GameObject.FindAnyObjectByType<ManagerRegistry>();

        if (managerRegistry != null)
        {
            GameObject managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<MovementPointsManager>() != null);
            if (managerObj != null)
            {
                movementPointsManager = managerObj.GetComponent<MovementPointsManager>();
            }
            managerObj = null;


            managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<GridManager>() != null);
            if (managerObj != null)
            {
                gridManager = managerObj.GetComponent<GridManager>();
            }
            managerObj = null;

            managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<CharacterRegisterManager>() != null);
            if (managerObj != null)
            {
                characterRegisterManager = managerObj.GetComponent<CharacterRegisterManager>();
            }
            managerObj = null;

            // Gets all enemy and player characters in party
            playerParty = characterRegisterManager.playerParty;
            enemyParty = characterRegisterManager.enemyParty;

            SetCurrentEnemy();
            currentTarget = FindNearestTarget();
        } 
    }

    private void RunEnemyProtoMove()
    {
        // Enemy already has target, this is a very temporary system

        List<GameObject> availableTilesByTarget = FindNearestAvailableTilesWithinMoveDistance();

        Dictionary<GameObject, int> tilesByDistance = new Dictionary<GameObject, int>();

        foreach (GameObject tile in availableTilesByTarget)
        {
            Vector2Int tilePos = gridManager.WorldToGridPosition(tile.transform.position);
            Vector2Int enemyPos = gridManager.WorldToGridPosition(currentEnemy.transform.position);
            tilesByDistance.Add(tile, gridManager.GetTileDistance(tilePos, enemyPos));
        }

        List<GameObject> closestTiles = new List<GameObject>();
        int closestVal = int.MaxValue;

        foreach (KeyValuePair<GameObject, int> tilePair in tilesByDistance)
        {
            if (tilePair.Value <= closestVal)
            {
                if (tilePair.Value != closestVal)
                {
                    closestTiles.Clear();
                }

                closestVal = tilePair.Value;
                closestTiles.Add(tilePair.Key);
            }
        }

        GameObject chosenTile;
        if (closestTiles.Count > 1)
        {
            // Choose random tile from closestTiles
            int randomIndex = UnityEngine.Random.Range(0, closestTiles.Count);
            chosenTile = closestTiles[randomIndex];

            EnableMove();
        }
        else if (closestTiles.Count == 1)
        {
            chosenTile = closestTiles[0];

            EnableMove();
        }
        else
        {
            // No tiles available
            Debug.LogWarning("No closest tiles found");
            SignalMoveEnded();
            return;
        }



        // TODO: Use chosenTile for movement
        Debug.Log($"Chosen tile: {chosenTile.name}");
    }

    private void EnableMove()
    {

        SignalMoveEnded();
    }

    private void SignalMoveEnded()
    {

    }

    public void StartTurn(IUSO_Battle_EnemyTurn_State battleState)
    {
        enemyBattleState = battleState;
        RunEnemyProtoMove();
    }

    // For Prototype, needs to be refactored for more serious considerations
    private void SetCurrentEnemy()
    {
        foreach (GameObject enemy in enemyParty)
        {
            PlayerStatSheet statSheet = enemy.GetComponent<PlayerStatSheet>();

            int AP = statSheet.attackPoints;
            int MP = statSheet.movementPoints;
            bool turnComplete = statSheet.turnComplete;

            if (AP > 0 || MP > 0 && !turnComplete)
            {
                currentEnemy = enemy;
                return;
            }
        }
    }

    public bool AnyPlayersWithinAttackDistance()
    {
        return false;
    }

    public GameObject FindNearestTarget()
    {
        if (currentEnemy == null || !gridManager.characterPositionList.ContainsKey(currentEnemy))
            return null;

        Vector2Int enemyGridPosition = gridManager.characterPositionList[currentEnemy];
        GameObject closestPlayer = null;
        int shortestDistance = int.MaxValue;

        foreach (GameObject playerCharacter in playerParty)
        {
            if (gridManager.characterPositionList.ContainsKey(playerCharacter))
            {
                Vector2Int playerGridPosition = gridManager.characterPositionList[playerCharacter];
                int distance = gridManager.GetTileDistance(enemyGridPosition, playerGridPosition);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestPlayer = playerCharacter;
                }
            }
        }

        return closestPlayer;
    }

    public void RunAttack()
    {

    }

    public List<GameObject> FindNearestAvailableTilesWithinMoveDistance()
    {
        List<GameObject> availableTiles = new List<GameObject>();

        if (currentEnemy == null || currentTarget == null)
        {
            Debug.LogWarning("Current enemy or target is null");
            return availableTiles;
        }

        if (!gridManager.characterPositionList.ContainsKey(currentEnemy) ||
            !gridManager.characterPositionList.ContainsKey(currentTarget))
        {
            Debug.LogWarning("Enemy or target not found in character position list");
            return availableTiles;
        }

        Vector2Int enemyPos = gridManager.characterPositionList[currentEnemy];
        Vector2Int targetPos = gridManager.characterPositionList[currentTarget];

        PlayerStatSheet enemyStats = currentEnemy.GetComponent<PlayerStatSheet>();
        if (enemyStats == null)
        {
            Debug.LogWarning("Enemy missing PlayerStatSheet component");
            return availableTiles;
        }

        int enemyMovementPoints = enemyStats.movementPoints;

        // BFS to find tiles around the target, expanding outward
        Queue<Vector2Int> toCheck = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        // Start with the target position
        toCheck.Enqueue(targetPos);
        visited.Add(targetPos);

        // 8 directions: up, down, left, right, and 4 diagonals
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // North
            new Vector2Int(1, 0),   // East
            new Vector2Int(0, -1),  // South
            new Vector2Int(-1, 0),  // West
            new Vector2Int(1, 1),   // NE
            new Vector2Int(1, -1),  // SE
            new Vector2Int(-1, -1), // SW
            new Vector2Int(-1, 1)   // NW
        };

        int whileRun = 0;
        

        while (toCheck.Count > 0)
        {
            whileRun += 1;
            Debug.Log("While Loop run: " + whileRun);

            Vector2Int currentPos = toCheck.Dequeue();

            int tileCheck = 0;
            // Check all adjacent tiles
            foreach (Vector2Int dir in directions)
            {
                tileCheck += 1;
                Debug.Log("Tile check run: " + tileCheck);
                Vector2Int adjacentPos = currentPos + dir;

                // Skip if already visited
                if (visited.Contains(adjacentPos))
                    continue;

                visited.Add(adjacentPos);

                // Check if the tile is valid and exists
                if (!gridManager.IsValidGridPosition(adjacentPos.x, adjacentPos.y))
                    continue;

                if (!gridManager.HasTileAt(adjacentPos.x, adjacentPos.y))
                    continue;

                // Check if tile is accessible (not blocked)
                if (!gridManager.IsTileAccessible(adjacentPos.x, adjacentPos.y))
                    continue;

                // Check if tile is occupied by another character
                if (gridManager.IsGridPosOccupied(adjacentPos))
                    continue;

                // Calculate movement cost from enemy to this tile
                int movementCost = movementPointsManager.CalculateMovementCost(enemyPos, adjacentPos);

                Debug.Log(tileCheck + " - Movement Cost: " + movementCost);
                Debug.Log(tileCheck + " - Enemy Movement Points: " + enemyMovementPoints);

                // If within movement range, add to available tiles
                if (movementCost <= enemyMovementPoints)
                {
                    GameObject tile = gridManager.GetTile(adjacentPos.x, adjacentPos.y);
                    if (tile != null && !availableTiles.Contains(tile))
                    {
                        availableTiles.Add(tile);
                    }
                }
                else
                {
                    // Still expand search outward to find tiles that might be reachable
                    // (closer tiles around the target that we haven't checked yet)
                    toCheck.Enqueue(adjacentPos);
                }
            }
        }

        return availableTiles;
    }

    //public List<GameObject> FindNearestAvailableTilesWithinMoveDistance()
    //{

    //    // Get adjacent tiles of target

    //    // Check distance using the grid manager and see if 
    //    // enemy has enough move points to reach any of those tiles

    //    // If no, find adjacent tiles of the adjacent tiles of the target

    //    // Check distance again and if enemy has enough move points to reach any

    //    // repeat this until matches are found, store the matches


    //    // A* Pathfinding to closest adjacent tile of target


    //    List<GameObject> GO = new List<GameObject>();
    //    return GO;
    //}
    public Vector2Int ChooseTile()
    {
        Vector2Int v2i = new Vector2Int();
        return v2i;
    }

    public void move()
    {

    }

}
