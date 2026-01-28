using UnityEngine;

public class DebugMenuGUI : MonoBehaviour
{
    public bool show = true;

    public UserControlManager userControlManager;
    public GridManager gridManager;

    void Update()
    {
        // Toggle with backquote `
        //if (Input.GetKeyDown(KeyCode.BackQuote))
        //    show = !show;
    }

    void OnGUI()
    {
        if (!show) return;

                GUI.Box(new Rect(10, 10, 400, 140), "DEBUG MENU");

        float startY = 40;
        float rowHeight = 30;
        float labelWidth = 150;
        float valueWidth = 230;
        float leftMargin = 20;

        // Row 1: Current Control Mode
        GUI.Label(new Rect(leftMargin, startY, labelWidth, rowHeight), "Current Control Mode:");
        if (GUI.Button(new Rect(leftMargin + labelWidth, startY, valueWidth, rowHeight), 
            userControlManager.currentControlMode != null ? userControlManager.currentControlMode.name : "None"))
            Debug.Log("Control Mode clicked");

        // Row 2: Current State
        GUI.Label(new Rect(leftMargin, startY + rowHeight, labelWidth, rowHeight), "Current State:");
        if (GUI.Button(new Rect(leftMargin + labelWidth, startY + rowHeight, valueWidth, rowHeight), 
            userControlManager.currentStateString))
            Debug.Log("Current State clicked");

        // Row 3: Selected Character
        GUI.Label(new Rect(leftMargin, startY + rowHeight * 2, labelWidth, rowHeight), "Selected Character:");
        if (GUI.Button(new Rect(leftMargin + labelWidth, startY + rowHeight * 2, valueWidth, rowHeight), 
            userControlManager.selectedCharacter != null ? userControlManager.selectedCharacter.name : "None"))
            Debug.Log("Selected Character clicked");
    }
}

