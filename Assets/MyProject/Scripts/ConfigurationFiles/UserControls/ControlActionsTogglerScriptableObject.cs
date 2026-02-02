using UnityEngine;

[CreateAssetMenu(fileName = "ControlActionsTogglerScriptableObject", menuName = "Scriptable Objects/ControlActionsTogglerScriptableObject")]
public class ControlActionsTogglerScriptableObject : ScriptableObject
{
    public bool enableHoverTileSelection;
    public bool enableMoveCharacter;
    public bool enableSelectTileWithClick;
    public bool enableIdleCharacter;
    public bool enableCharacterHover;
    public bool enableCharacterClick;
}
