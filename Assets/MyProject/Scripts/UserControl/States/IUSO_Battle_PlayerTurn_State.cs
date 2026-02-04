using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    private UserControlOrchestrator userControlOrchestrator;

    private CA_HoverTileSelection CA_HoverTileSelection;
    private CA_HoverCharacter CA_HoverCharacter;
    private CA_IdleCharacter CA_IdleCharacter;
    private CA_MoveCharacter CA_MoveCharacter;
    private CA_SelectCharacterWithClick CA_SelectCharacterWithClick;
    private CA_SelectTileWithClick CA_SelectTileWithClick;
    private CA_BasicMeeleAttack CA_BasicMeeleAttack;

    public InterfaceRaycastSelection interfaceRaycastSelection;

    public ECharacterPhase characterPhase;

    public InputSystem_Actions input;

    private MovementPointsManager movementPointsManager;

    private GameObject activeCharacter;

    private GridManager gridManager;

    public void EnterState(UserControlOrchestrator USO)
    {
        userControlOrchestrator = USO;
        EventManager.ClickedTile += HandleTileClicked;
        EventManager.RightClickAttack += HandleBasicAttack;

        ManagerRegistry managerRegistry = GameObject.FindAnyObjectByType<ManagerRegistry>();

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
        }

        interfaceRaycastSelection = userControlOrchestrator.interfaceRaycastSelection;
        characterPhase = ECharacterPhase.IDLE;
        input = userControlOrchestrator.input;

        GameObject GO = userControlOrchestrator.gameObject;
        GameObject selectedCharacter = userControlOrchestrator.selectedCharacter;
        activeCharacter = selectedCharacter;

        CA_HoverTileSelection = GO.AddComponent<CA_HoverTileSelection>();
        CA_HoverTileSelection.userControlOrchestrator = userControlOrchestrator;
        CA_HoverTileSelection.interfaceRaycastSelection = interfaceRaycastSelection;

        //CA_HoverCharacter = GO.AddComponent<CA_HoverCharacter>();
        //CA_HoverCharacter.userControlOrchestrator = userControlOrchestrator;

        //CA_MoveCharacter = GO.AddComponent<CA_MoveCharacter>();
        //CA_MoveCharacter.userControlOrchestrator = userControlOrchestrator;
        //CA_MoveCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
        //CA_MoveCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
        CreateMoveCA();
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

        CA_BasicMeeleAttack = GO.AddComponent<CA_BasicMeeleAttack>();
        CA_BasicMeeleAttack.userControlOrchestrator = userControlOrchestrator;
        CA_BasicMeeleAttack.input = input;
    }

    public void ExitState()
    {
        EventManager.ClickedTile -= HandleTileClicked;
        EventManager.RightClickAttack -= HandleBasicAttack;

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


    public void Update()
    {
        CA_HoverTileSelection.Action();
        CA_SelectTileWithClick.Action();
        CA_BasicMeeleAttack.Action();
    }

    public void FixedUpdate()
    {
        if (characterPhase == ECharacterPhase.IDLE && CA_IdleCharacter != null)
        {
            CA_IdleCharacter.Action();
        }
        else if (characterPhase == ECharacterPhase.MOVE)
        {
            if (CA_MoveCharacter == null)
            {
                CreateMoveCA();
            }
            CA_MoveCharacter.Action();
        }
    }

    // CA_MoveCharacter is managed with deletions and readding.
    private void CreateMoveCA()
    {
        GameObject GO = userControlOrchestrator.gameObject;
        GameObject selectedCharacter = userControlOrchestrator.selectedCharacter;

        CA_MoveCharacter = GO.AddComponent<CA_MoveCharacter>();
        CA_MoveCharacter.userControlOrchestrator = userControlOrchestrator;
        CA_MoveCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
        CA_MoveCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
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
            UnityEngine.Object.Destroy(component);
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

    private void HandleTileClicked(Vector2Int gridPos)
    {

        List<GameObject> characterList = movementPointsManager.characters;
        GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);
        Vector2Int characterOriginalPos = gridManager.WorldToGridPosition(matchingCharacter.transform.position);

        if (matchingCharacter != null)
        {
            PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();

            if (playerStatSheet != null)
            {
                Debug.Log($"Movement Points: {playerStatSheet.movementPoints}");

                int distance = gridManager.GetTileDistance(characterOriginalPos, gridPos);


                Debug.Log("Tile Distance: " + gridManager.GetTileDistance(characterOriginalPos, gridPos).ToString());
                // Why is start pos 1,0 every time? now 5,0
                Debug.Log("Start Position: " + characterOriginalPos.ToString());
                Debug.Log("Destinatio: " + gridPos);

                // Validate and execute move
                if (distance <= playerStatSheet.movementPoints)
                {
                    playerStatSheet.movementPoints -= distance;
                    characterPhase = ECharacterPhase.MOVE;
                }
                else
                {
                    Debug.Log("Insufficient movement points!");
                    return;
                }

                if (playerStatSheet.movementPoints <= 0)
                {
                    userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
                }
            }
        }

        characterPhase = ECharacterPhase.MOVE;
    }

    private void HandleBasicAttack()
    {
        List<GameObject> characterList = movementPointsManager.characters;
        GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);

        if (matchingCharacter != null)
        {
            PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();

            if (playerStatSheet != null)
            {
                playerStatSheet.attackPoints -= 1;
            }
        }
    }
}

public class InfoObject
{
    public ECharacterPhase characterPhase;
}
