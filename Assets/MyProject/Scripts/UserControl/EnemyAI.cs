using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI decision-making component for enemy turns.
/// Initialized by UserControlOrchestrator with all dependencies.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    // Dependencies injected by orchestrator
    private IUSO_Battle_EnemyTurn_State state;
    private GridManager gridManager;
    private MovementPointsManager movementPointsManager;
    private CharacterRegisterManager characterRegisterManager;

    private List<GameObject> playerParty;
    private List<GameObject> enemyParty;

    private int attackDistance = 1;
    private bool isInitialized = false;

    // AI-specific public accessible state
    public GameObject currentEnemy { get; private set; }
    public GameObject currentTarget { get; private set; }

    /// <summary>
    /// Called by UserControlOrchestrator to inject dependencies
    /// </summary>
    public void Initialize(
        GridManager grid,
        MovementPointsManager movePoints,
        CharacterRegisterManager charRegister,
        IUSO_Battle_EnemyTurn_State battleState)
    {
        gridManager = grid;
        movementPointsManager = movePoints;
        characterRegisterManager = charRegister;
        state = battleState;

        if (characterRegisterManager != null)
        {
            playerParty = characterRegisterManager.playerParty;
            enemyParty = characterRegisterManager.enemyParty;
        }

        isInitialized = true;
        //Debug.Log"EnemyAI initialized successfully");
    }

    /// <summary>
    /// Main entry point - called by IUSO_Battle_EnemyTurn_State
    /// </summary>
    public void ExecuteTurn()
    {
        if (!isInitialized)
        {
            Debug.LogError("EnemyAI.ExecuteTurn() called before initialization!");
            return;
        }

        SelectActiveEnemy();

        if (currentEnemy == null)
        {
            Debug.LogWarning("No valid enemy to act");
            return;
        }

        SelectTarget();

        if (currentTarget == null)
        {
            Debug.LogWarning("No valid target found");
            return;
        }

        //Debug.Log$"Enemy Turn: {currentEnemy.name} {currentTarget.name}");

        // Decide and execute action
        if (IsTargetInAttackRange())
        {
            ExecuteAttack();
        }
        else
        {
            ExecuteMove();
        }
    }

    /// <summary>
    /// Continue AI decision-making after a previous action completes
    /// Called by IUSO_Battle_EnemyTurn_State after movement finishes
    /// </summary>
    //public void ContinueTurn()
    //{
    //    if (!isInitialized)
    //    {
    //        Debug.LogError("EnemyAI.ContinueTurn() called before initialization!");
    //        return;
    //    }

    //    if (currentEnemy == null || currentTarget == null)
    //    {
    //        Debug.LogWarning("Cannot continue turn - no active enemy or target");
    //        return;
    //    }

    //    PlayerStatSheet stats = currentEnemy.GetComponent<PlayerStatSheet>();
    //    if (stats == null) return;

    //    // Check if enemy has any actions left
    //    if (stats.attackPoints <= 0 && stats.movementPoints <= 0)
    //    {
    //        //Debug.Log$"{currentEnemy.name} has no actions remaining");
    //        stats.turnComplete = true;
    //        return;
    //    }

    //    // Re-evaluate position after movement
    //    if (IsTargetInAttackRange() && stats.attackPoints > 0)
    //    {
    //        ExecuteAttack();
    //    }
    //    else
    //    {
    //        //Debug.Log$"{currentEnemy.name} cannot attack - either out of range or no attack points");
    //        stats.turnComplete = true;
    //    }
    //}

    public void SelectActiveEnemy()
    {
        foreach (GameObject enemy in enemyParty)
        {
            PlayerStatSheet stats = enemy.GetComponent<PlayerStatSheet>();
            if (stats == null) continue;

            if ((stats.attackPoints > 0 || stats.movementPoints > 0) && !stats.turnComplete)
            {
                currentEnemy = enemy;
                //Debug.Log$"Selected enemy: {currentEnemy.name}");
                return;
            }
        }

        currentEnemy = null;
    }

    private void SelectTarget()
    {
        currentTarget = FindNearestPlayer();
    }

    private bool IsTargetInAttackRange()
    {
        if (currentEnemy == null || currentTarget == null) return false;

        Vector2Int enemyPos = gridManager.characterPositionList[currentEnemy];
        Vector2Int targetPos = gridManager.characterPositionList[currentTarget];

        return gridManager.GetTileDistance(enemyPos, targetPos) <= attackDistance;
    }

    private void ExecuteAttack()
    {
        //Debug.Log$"{currentEnemy.name} attacking {currentTarget.name}");
        // TODO: Implement attack

        state.characterPhase = ECharacterPhase.ATTACK;
    }

    private void ExecuteMove()
    {
        //Debug.Log$"{currentEnemy.name} moving towards {currentTarget.name}");
        GameObject destinationTile = ChooseBestMoveTile();

        if (destinationTile != null)
        {
            //Debug.Log$"Moving to: {destinationTile.name}");
            // TODO: Execute movement

            PlayerClickControls enemyControls = currentEnemy.GetComponent<PlayerClickControls>();

            float offset = gridManager.cellSize / 2f;
            Vector3 offsetVector = new Vector3(offset, 0f, offset);

            //Vector2Int fromGridPos = gridManager.WorldToGridPosition(currentEnemy.transform.position);
            //int fromDestinationX = fromGridPos.x;
            //int fromDestinationY = fromGridPos.y;
            //Vector3 fromPosVector = new Vector3(fromDestinationX, 0, fromDestinationY);
            //Vector3 worldPosFromPos = gridManager.GridToWorldPosition(fromGridPos.x, fromGridPos.y);
            //enemyControls.SetFromPos(worldPosFromPos + offsetVector);
            //Vector3 newSetFromPos = new Vector3(currentEnemy.transform.position.x, currentEnemy.tr, currentEnemy.transform.position.z);
            enemyControls.SetFromPos(currentEnemy.transform.position);

            //Vector2Int destinationGridPos = gridManager.WorldToGridPosition(destinationTile.transform.position);
            //int destinationX = destinationGridPos.x;
            //int destinationY = destinationGridPos.y;
            //Vector3 destinationPosVector = new Vector3(destinationX, 0, destinationY);
            //Vector3 destinationToPos = gridManager.GridToWorldPosition(destinationGridPos.x, destinationGridPos.y);
            //enemyControls.SetToPos(destinationToPos + offsetVector);
            float tileHeightOffset = 1.5f;
            Vector3 newSetToPos = new Vector3(destinationTile.transform.position.x, tileHeightOffset, destinationTile.transform.position.z);
            enemyControls.SetToPos(newSetToPos);

            Vector3 storedFromPos = enemyControls.GetFromPos();
            Vector3 storedToPos = enemyControls.GetToPos();

            if (storedFromPos != null && storedToPos != null)
            {
                EventManager.OnMoveEnemy();
            }
            else
            {
                //Debug.Log"one or more stored positions are null");
            }
        }
    }

    public GameObject ChooseBestMoveTile()
    {
        List<GameObject> availableTiles = FindReachableTilesNearTarget();

        if (availableTiles.Count == 0) return null;

        // Find closest tiles to enemy position
        Vector2Int enemyPos = gridManager.WorldToGridPosition(currentEnemy.transform.position);
        List<GameObject> closestTiles = new List<GameObject>();
        int minDistance = int.MaxValue;

        foreach (GameObject tile in availableTiles)
        {
            Vector2Int tilePos = gridManager.WorldToGridPosition(tile.transform.position);
            int distance = gridManager.GetTileDistance(enemyPos, tilePos);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestTiles.Clear();
                closestTiles.Add(tile);
            }
            else if (distance == minDistance)
            {
                closestTiles.Add(tile);
            }
        }

        // Pick random from equally good options
        return closestTiles.Count > 0
            ? closestTiles[Random.Range(0, closestTiles.Count)]
            : null;
    }

    public GameObject FindNearestPlayer()
    {
        if (currentEnemy == null || !gridManager.characterPositionList.ContainsKey(currentEnemy))
            return null;

        Vector2Int enemyPos = gridManager.characterPositionList[currentEnemy];
        GameObject closestPlayer = null;
        int shortestDistance = int.MaxValue;

        foreach (GameObject player in playerParty)
        {
            if (gridManager.characterPositionList.ContainsKey(player))
            {
                Vector2Int playerPos = gridManager.characterPositionList[player];
                int distance = gridManager.GetTileDistance(enemyPos, playerPos);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestPlayer = player;
                }
            }
        }

        return closestPlayer;
    }

    public List<GameObject> FindReachableTilesNearTarget()
    {
        List<GameObject> availableTiles = new List<GameObject>();

        if (currentEnemy == null || currentTarget == null ||
            !gridManager.characterPositionList.ContainsKey(currentEnemy) ||
            !gridManager.characterPositionList.ContainsKey(currentTarget))
        {
            return availableTiles;
        }

        Vector2Int enemyPos = gridManager.characterPositionList[currentEnemy];
        Vector2Int targetPos = gridManager.characterPositionList[currentTarget];

        PlayerStatSheet stats = currentEnemy.GetComponent<PlayerStatSheet>();
        if (stats == null) return availableTiles;

        int movementPoints = stats.movementPoints;

        // BFS from target outward
        Queue<Vector2Int> toCheck = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        toCheck.Enqueue(targetPos);
        visited.Add(targetPos);

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0),
            new Vector2Int(1, 1), new Vector2Int(1, -1),
            new Vector2Int(-1, -1), new Vector2Int(-1, 1)
        };

        while (toCheck.Count > 0)
        {
            Vector2Int currentPos = toCheck.Dequeue();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int adjacentPos = currentPos + dir;

                if (visited.Contains(adjacentPos)) continue;
                visited.Add(adjacentPos);

                if (!gridManager.IsValidGridPosition(adjacentPos.x, adjacentPos.y)) continue;
                if (!gridManager.HasTileAt(adjacentPos.x, adjacentPos.y)) continue;
                if (!gridManager.IsTileAccessible(adjacentPos.x, adjacentPos.y)) continue;
                if (gridManager.IsGridPosOccupied(adjacentPos)) continue;

                int moveCost = movementPointsManager.CalculateMovementCost(enemyPos, adjacentPos);

                if (moveCost <= movementPoints)
                {
                    GameObject tile = gridManager.GetTile(adjacentPos.x, adjacentPos.y);
                    if (tile != null && !availableTiles.Contains(tile))
                    {
                        availableTiles.Add(tile);
                    }
                }
                else
                {
                    toCheck.Enqueue(adjacentPos);
                }
            }
        }

        return availableTiles;
    }
}