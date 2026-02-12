using System.Collections.Generic;
using UnityEngine;

public class DebugMenuGUI : MonoBehaviour
{
    public bool show = true;

    //public UserControlManager userControlManager;
    public UserControlOrchestrator userControlOrchestrator;
    public GridManager gridManager;

    public string characterPhaseString;
    public string stateString;

    void Update()
    {
        // Toggle with backquote `
        //if (Input.GetKeyDown(KeyCode.BackQuote))
        //    show = !show;
        characterPhaseString = userControlOrchestrator.userControlState.GetStateInfo().characterPhase.ToString();
        stateString = userControlOrchestrator.stateString;
    }

    void OnGUI()
    {
        if (!show) return;

        // Calculate dynamic height based on control actions count
        int controlActionsCount = GetControlActionsCount();
        float rowHeight = 30;
        float boxHeight = 230 + (controlActionsCount * rowHeight); // Increased for new row

        GUI.Box(new Rect(10, 10, 400, boxHeight), "DEBUG MENU");

        float startY = 40;  
        float labelWidth = 150;
        float valueWidth = 230;
        float leftMargin = 20;

        GUI.Label(new Rect(leftMargin, startY, labelWidth, rowHeight), "Current Control Mode:");
        if (GUI.Button(new Rect(leftMargin + labelWidth, startY, valueWidth, rowHeight),
            userControlOrchestrator.stateString != null ? stateString : "None"))
            Debug.Log("State String");

        GUI.Label(new Rect(leftMargin, startY + rowHeight, labelWidth, rowHeight), "CharacterPhase:");
        if (GUI.Button(new Rect(leftMargin + labelWidth, startY + rowHeight, valueWidth, rowHeight),
            userControlOrchestrator.userControlState.GetStateInfo() != null ? characterPhaseString : "None"))
            Debug.Log("Current State clicked");

        GUI.Label(new Rect(leftMargin, startY + rowHeight * 2, labelWidth, rowHeight), "chracter Pos:");
        if (GUI.Button(new Rect(leftMargin + labelWidth, startY + rowHeight * 2, valueWidth, rowHeight),
            userControlOrchestrator.selectedCharacter != null ? userControlOrchestrator.selectedCharacter.transform.position.ToString() : "no character selected"))
            Debug.Log("Selected Character clicked");

        GUI.Label(new Rect(leftMargin, startY + rowHeight * 3, labelWidth, rowHeight), "Hovered Character:");
        if (GUI.Button(new Rect(leftMargin + labelWidth, startY + rowHeight * 3, valueWidth, rowHeight),
            userControlOrchestrator.selectedCharacter != null ? gridManager.WorldToGridPosition(userControlOrchestrator.selectedCharacter.transform.position).ToString() : "None"))
            Debug.Log("Hovered Character clicked");

        GUI.Label(new Rect(leftMargin, startY + rowHeight * 4, labelWidth, rowHeight), "Turn: ");
        if (GUI.Button(new Rect(leftMargin + labelWidth, startY + rowHeight * 4, valueWidth, rowHeight),
            userControlOrchestrator.selectedCharacter != null ? gridManager.WorldToGridPosition(userControlOrchestrator.selectedCharacter.transform.position).ToString() : "None"))
            Debug.Log("Hovered Character clicked");

        // Row 6: Target
        GUI.Label(new Rect(leftMargin, startY + rowHeight * 5, labelWidth, rowHeight), "Target:");
        string targetInfo = GetTargetInfo();
        if (GUI.Button(new Rect(leftMargin + labelWidth, startY + rowHeight * 5, valueWidth, rowHeight), targetInfo))
            Debug.Log("Target Info: " + targetInfo);

        // Row 7: Selected Character Info
        GUI.Label(new Rect(leftMargin, startY + rowHeight * 6, labelWidth, rowHeight), "Selected Character:");
        string selectedCharInfo = GetSelectedCharacterInfo();
        if (GUI.Button(new Rect(leftMargin + labelWidth, startY + rowHeight * 6, valueWidth, rowHeight), selectedCharInfo))
            Debug.Log("Selected Character Info: " + selectedCharInfo);

        // Row 8+: All Control Actions (multiple rows)
        DrawControlActionsList(leftMargin, startY + rowHeight * 7, labelWidth, valueWidth, rowHeight);
    }

    private string GetTargetInfo()
    {
        if (userControlOrchestrator.target == null)
            return "None";

        GameObject target = userControlOrchestrator.target;
        Vector2Int gridPos = gridManager.WorldToGridPosition(target.transform.position);
        
        return $"{target.name} @ {gridPos}";
    }

    private string GetSelectedCharacterInfo()
    {
        if (userControlOrchestrator.selectedCharacter == null)
            return "None";

        GameObject character = userControlOrchestrator.selectedCharacter;
        string characterName = character.name;
        
        PlayerStatSheet stats = character.GetComponent<PlayerStatSheet>();
        if (stats != null)
        {
            return $"{characterName} (MP:{stats.movementPoints} AP:{stats.attackPoints})";
        }
        
        return characterName;
    }

    private void DrawControlActionsList(float leftMargin, float startY, float labelWidth, float valueWidth, float rowHeight)
    {
        GUI.Label(new Rect(leftMargin, startY, labelWidth, rowHeight), "Control Actions:");

        if (userControlOrchestrator.userControlState is IUSO_Battle_PlayerTurn_State battleState)
        {
            List<MonoBehaviour> actions = battleState.GetAllControlActions();

            if (actions == null || actions.Count == 0)
            {
                GUI.Label(new Rect(leftMargin + labelWidth, startY, valueWidth, rowHeight), "None");
                return;
            }

            for (int i = 0; i < actions.Count; i++)
            {
                string actionName = actions[i] != null ? actions[i].GetType().Name : "null";
                float yPos = startY + (i * rowHeight);

                if (GUI.Button(new Rect(leftMargin + labelWidth, yPos, valueWidth, rowHeight), actionName))
                    Debug.Log($"Control Action [{i}]: {actionName}");
            }
        }
        else
        {
            GUI.Label(new Rect(leftMargin + labelWidth, startY, valueWidth, rowHeight), "N/A");
        }
    }

    private int GetControlActionsCount()
    {
        if (userControlOrchestrator.userControlState is IUSO_Battle_PlayerTurn_State battleState)
        {
            List<MonoBehaviour> actions = battleState.GetAllControlActions();
            return actions != null ? actions.Count : 0;
        }
        return 0;
    }

    private string GetControlActionsString()
    {
        if (userControlOrchestrator.userControlState is IUSO_Battle_PlayerTurn_State battleState)
        {
            List<MonoBehaviour> actions = battleState.GetAllControlActions();
            if (actions == null || actions.Count == 0)
                return "None";

            return string.Join(", ", actions.ConvertAll(action => action != null ? action.GetType().Name : "null"));
        }
        return "N/A";
    }
}

