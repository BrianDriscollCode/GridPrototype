using UnityEngine;

public class PlayerStateHelper
{
    public void CheckAvailableTilesHelper(GameObject character, GridManager gridManager, bool reactionState = false)
    {
        int characterMovePoints;
        Vector2Int characterGridPos;

        if (reactionState)
        {
            characterMovePoints = Mathf.FloorToInt(character.GetComponent<PlayerStatSheet>().movementPoints / 2f);
            characterGridPos = gridManager.WorldToGridPosition(character.GetComponent<EntityGridLocation>().pos);
        }
        else
        {
            characterMovePoints = character.GetComponent<PlayerStatSheet>().movementPoints;
            characterGridPos = gridManager.WorldToGridPosition(character.GetComponent<EntityGridLocation>().pos);
        }
           

        gridManager.CheckAvailableMoveTilesAndHighlight(characterMovePoints, characterGridPos);
    }
}