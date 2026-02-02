using UnityEngine;

public enum ECharacterPhase
{
    IDLE,
    MOVE,
    ATTACK,
    DEFEND,
    REACT
}
public class IUSO_Battle_PlayerTurn_State : IUSO_State
{
    private CA_HoverTileSelection CA_HoverTileSelection;
    private CA_HoverCharacter CA_HoverCharacter;
    private CA_IdleCharacter CA_IdleCharacter;
    private CA_MoveCharacter CA_MoveCharacter;
    private CA_SelectCharacterWithClick CA_SelectCharacterWithClick;
    private CA_SelectTileWithClick CA_SelectTileWithClick;

    public InterfaceRaycastSelection interfaceRaycastSelection;

    public ECharacterPhase characterPhase;

    public InputSystem_Actions input;

    public void EnterState(UserControlOrchestrator userControlOrchestrator)
    {
        EventManager.ClickedTile += HandleTileClicked;

        interfaceRaycastSelection = userControlOrchestrator.interfaceRaycastSelection;
        characterPhase = ECharacterPhase.IDLE;
        input = userControlOrchestrator.input;

        GameObject GO = userControlOrchestrator.gameObject;
        GameObject selectedCharacter = userControlOrchestrator.selectedCharacter;

        CA_HoverTileSelection = GO.AddComponent<CA_HoverTileSelection>();
        CA_HoverTileSelection.userControlOrchestrator = userControlOrchestrator;
        CA_HoverTileSelection.interfaceRaycastSelection = interfaceRaycastSelection;

        //CA_HoverCharacter = GO.AddComponent<CA_HoverCharacter>();
        //CA_HoverCharacter.userControlOrchestrator = userControlOrchestrator;

        //CA_MoveCharacter = GO.AddComponent<CA_MoveCharacter>();
        //CA_MoveCharacter.userControlOrchestrator = userControlOrchestrator;
        //CA_MoveCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
        //CA_MoveCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
        CreateMoveCA(userControlOrchestrator);
        selectedCharacter.GetComponent<PlayerAnim>().IdleAnimation();

        CA_SelectTileWithClick = GO.AddComponent<CA_SelectTileWithClick>();
        CA_SelectTileWithClick.userControlOrchestrator = userControlOrchestrator;
        CA_SelectTileWithClick.input = input;

        CA_IdleCharacter = GO.AddComponent<CA_IdleCharacter>();
        CA_IdleCharacter.userControlOrchestrator = userControlOrchestrator;
        CA_IdleCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
        CA_IdleCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();

        //CA_SelectCharacterWithClick = GO.AddComponent<CA_SelectCharacterWithClick>();
        //CA_SelectCharacterWithClick.userControlOrchestrator = userControlOrchestrator;
    }

    public void ExitState(UserControlOrchestrator userControlOrchestrator)
    {
        EventManager.ClickedTile -= HandleTileClicked;

        // Clean up all components
        DestroyComponent(CA_HoverTileSelection);
        DestroyComponent(CA_HoverCharacter);
        DestroyComponent(CA_MoveCharacter);
        DestroyComponent(CA_SelectTileWithClick);
        DestroyComponent(CA_IdleCharacter);
        DestroyComponent(CA_SelectCharacterWithClick);

        // Clear references
        interfaceRaycastSelection = null; // only ref no component - this is good
        CA_HoverTileSelection = null;
        CA_HoverCharacter = null;
        CA_MoveCharacter = null;
        CA_SelectTileWithClick = null;
        CA_IdleCharacter = null;
        CA_SelectCharacterWithClick = null;
    }

    //** Does not work on regular c# classes, needs monobehavior
    //private void OnEnable()
    //{
    //    EventManager.ClickedTile += HandleTileClicked;
    //    //Debug.Log("Listener: subscribed to ClickedTile");
    //}

    //private void OnDisable()
    //{
    //    EventManager.ClickedTile -= HandleTileClicked;
    //    //Debug.Log("Listener: unsubscribed from ClickedTile");
    //}

    private void HandleTileClicked(Vector2Int gridPos)
    {
        Debug.Log("Handle tile clicked");
        characterPhase = ECharacterPhase.MOVE;
    }

    public void Update(UserControlOrchestrator userControlOrchestrator)
    {
        CA_HoverTileSelection.Action();
        CA_SelectTileWithClick.Action();
    }

    private void CreateMoveCA(UserControlOrchestrator userControlOrchestrator)
    {
        GameObject GO = userControlOrchestrator.gameObject;
        GameObject selectedCharacter = userControlOrchestrator.selectedCharacter;

        CA_MoveCharacter = GO.AddComponent<CA_MoveCharacter>();
        CA_MoveCharacter.userControlOrchestrator = userControlOrchestrator;
        CA_MoveCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
        CA_MoveCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
    }

    public void FixedUpdate(UserControlOrchestrator userControlOrchestrator)
    {
        if (characterPhase == ECharacterPhase.IDLE && CA_IdleCharacter != null)
        {
            CA_IdleCharacter.Action();
        }
        else if (characterPhase == ECharacterPhase.MOVE)
        {
            if (CA_MoveCharacter == null)
            {
                CreateMoveCA(userControlOrchestrator);
            }
            CA_MoveCharacter.Action();
        }
    }

    public void DeleteCA(E_CA_Type type)
    {
        switch (type)
        {
            case E_CA_Type.HOVER_TILE_SELECTION:
                DestroyComponent(CA_HoverTileSelection);
                CA_HoverTileSelection = null;
                break;

            case E_CA_Type.HOVER_CHARACTER:
                DestroyComponent(CA_HoverCharacter);
                CA_HoverCharacter = null;
                break;

            case E_CA_Type.IDLE_CHARACTER:
                DestroyComponent(CA_IdleCharacter);
                CA_IdleCharacter = null;
                break;

            case E_CA_Type.MOVE_CHARACTER:
                DestroyComponent(CA_MoveCharacter);
                CA_MoveCharacter = null;
                break;

            case E_CA_Type.SELECT_CHARACTER_WITH_CLICK:
                DestroyComponent(CA_SelectCharacterWithClick);
                CA_SelectCharacterWithClick = null;
                break;

            case E_CA_Type.SELECT_TILE_WITH_CLICK:
                DestroyComponent(CA_SelectTileWithClick);
                CA_SelectTileWithClick = null;
                break;
        }
    }
    private void DestroyComponent(Component component)
    {
        if (component != null)
            Object.Destroy(component);
    }

    public InfoObject GetStateInfo()
    {
        return new InfoObject { characterPhase = this.characterPhase };
    }


    public ECharacterPhase GetCharacterPhase()
    {
        return characterPhase;
    }

    public void SetCharacterPhase(ECharacterPhase phase)
    {
        characterPhase = phase;
    }
}

public class InfoObject
{
    public ECharacterPhase characterPhase;
}
