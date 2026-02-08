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
    private List<MonoBehaviour> allControlActions;

    public InterfaceRaycastSelection interfaceRaycastSelection;

    public ECharacterPhase characterPhase;

    public InputSystem_Actions input;

    private MovementPointsManager movementPointsManager;

    private GameObject activeCharacter;

    private GridManager gridManager;

    public void EnterState(UserControlOrchestrator USO)
    {
        allControlActions = new List<MonoBehaviour>();
        userControlOrchestrator = USO;
        EventManager.ClickedTile += HandleTileClicked;
        EventManager.RightClickAttack += HandleBasicAttack;
        EventManager.FinishBasicMeeleAttack += HandleFinishBasicAttack;

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

        InitialiazeControlActions();
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
        CA_BasicMeeleAttack.ActionHandler();
    }

    public void FixedUpdate()
    {
        if (characterPhase == ECharacterPhase.IDLE)
        {
            //if (CA_IdleCharacter == null)
            //{
            //    CreateCA(E_CA_Type.IDLE_CHARACTER);
            //    DeleteCA(E_CA_Type.MOVE_CHARACTER);
            //}

            if (CA_IdleCharacter != null)
            {
                CA_IdleCharacter.Action();
            }
        }
        else if (characterPhase == ECharacterPhase.MOVE)
        {
            //if (CA_MoveCharacter == null)
            //{
            //    CreateCA(E_CA_Type.MOVE_CHARACTER);
            //    DeleteCA(E_CA_Type.IDLE_CHARACTER);
            //}
            
            if (CA_MoveCharacter != null)
            {
                CA_MoveCharacter.Action();
            }
        }
        else if (characterPhase == ECharacterPhase.ATTACK)
        {
            //if (CA_BasicMeeleAttack == null)
            //{
            //    CreateBasicMeeleAttackCA();
            //}

            if (CA_BasicMeeleAttack != null)
            {
                CA_BasicMeeleAttack.Action();
            }
        }
    }

    private void InitialiazeControlActions()
    {
        GameObject GO = userControlOrchestrator.gameObject;
        GameObject selectedCharacter = userControlOrchestrator.selectedCharacter;
        activeCharacter = selectedCharacter;

        CreateCA(E_CA_Type.HOVER_TILE_SELECTION);

        //CA_HoverCharacter = GO.AddComponent<CA_HoverCharacter>();
        //CA_HoverCharacter.userControlOrchestrator = userControlOrchestrator;

        CreateCA(E_CA_Type.MOVE_CHARACTER);
        selectedCharacter.GetComponent<PlayerAnim>().IdleAnimation();

        CreateCA(E_CA_Type.SELECT_TILE_WITH_CLICK);
        CreateCA(E_CA_Type.IDLE_CHARACTER);

        //CA_SelectCharacterWithClick = GO.AddComponent<CA_SelectCharacterWithClick>();
        //CA_SelectCharacterWithClick.userControlOrchestrator = userControlOrchestrator;

        CreateCA(E_CA_Type.BASIC_MEELE_ATTACK);
    }

    public List<MonoBehaviour> GetAllControlActions()
    {
        return allControlActions;
    }

    private void DestroyMultipleControlActions(List<MonoBehaviour> actions)
    {
        if (actions == null || allControlActions == null)
            return;

        for (int i = allControlActions.Count - 1; i >= 0; i--)
        {
            MonoBehaviour ca = allControlActions[i];

            if (ca != null && actions.Contains(ca))
            {
                DestroyComponent(ca);
                allControlActions.RemoveAt(i);

                // Clear the field reference if it matches
                if (ca == CA_HoverTileSelection) CA_HoverTileSelection = null;
                else if (ca == CA_HoverCharacter) CA_HoverCharacter = null;
                else if (ca == CA_IdleCharacter) CA_IdleCharacter = null;
                else if (ca == CA_MoveCharacter) CA_MoveCharacter = null;
                else if (ca == CA_SelectCharacterWithClick) CA_SelectCharacterWithClick = null;
                else if (ca == CA_SelectTileWithClick) CA_SelectTileWithClick = null;
                else if (ca == CA_BasicMeeleAttack) CA_BasicMeeleAttack = null;
            }
        }
    }

    // *** Not sure I ever want to do this 
    //
    //public void DestroyPrevControlActions(MonoBehaviour action)
    //{
    //    // go trhough a list and delete everything but the matching action
    //    if (allControlActions == null)
    //    {
    //        return;
    //    }

    //    for (int i = allControlActions.Count - 1; i >= 0; i--)
    //    {
    //        if (allControlActions[i] != action && allControlActions[i] != null)
    //        {
    //            DestroyComponent(allControlActions[i]);
    //            allControlActions.RemoveAt(i);
    //        }
    //    }
    //}

    public void DestroyAllControlActions()
    {
        DestroyComponent(CA_HoverTileSelection);
        DestroyComponent(CA_HoverCharacter);
        DestroyComponent(CA_MoveCharacter);
        DestroyComponent(CA_SelectTileWithClick);
        DestroyComponent(CA_IdleCharacter);
        DestroyComponent(CA_SelectCharacterWithClick);
    }

    // CA_MoveCharacter is managed with deletions and readding.
    private void CreateCA(E_CA_Type type)
    {
        GameObject GO = userControlOrchestrator.gameObject;
        GameObject selectedCharacter = userControlOrchestrator.selectedCharacter;

        if (type == E_CA_Type.MOVE_CHARACTER)
        {
            CA_MoveCharacter = GO.AddComponent<CA_MoveCharacter>();
            CA_MoveCharacter.userControlOrchestrator = userControlOrchestrator;
            CA_MoveCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
            CA_MoveCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
            allControlActions.Add(CA_MoveCharacter);
        }
        else if (type == E_CA_Type.IDLE_CHARACTER)
        {
            CA_IdleCharacter = GO.AddComponent<CA_IdleCharacter>();
            CA_IdleCharacter.userControlOrchestrator = userControlOrchestrator;
            CA_IdleCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
            CA_IdleCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
            allControlActions.Add(CA_IdleCharacter);
        }
        else if (type == E_CA_Type.BASIC_MEELE_ATTACK)
        {
            CA_BasicMeeleAttack = GO.AddComponent<CA_BasicMeeleAttack>();
            CA_BasicMeeleAttack.userControlOrchestrator = userControlOrchestrator;
            CA_BasicMeeleAttack.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
            CA_BasicMeeleAttack.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
            CA_BasicMeeleAttack.input = input;
            allControlActions.Add(CA_BasicMeeleAttack);
        }
        else if (type == E_CA_Type.SELECT_TILE_WITH_CLICK)
        {
            CA_SelectTileWithClick = GO.AddComponent<CA_SelectTileWithClick>();
            CA_SelectTileWithClick.userControlOrchestrator = userControlOrchestrator;
            CA_SelectTileWithClick.input = input;
            allControlActions.Add(CA_SelectTileWithClick);
        }
        else if (type == E_CA_Type.HOVER_TILE_SELECTION)
        {
            CA_HoverTileSelection = GO.AddComponent<CA_HoverTileSelection>();
            CA_HoverTileSelection.userControlOrchestrator = userControlOrchestrator;
            CA_HoverTileSelection.interfaceRaycastSelection = interfaceRaycastSelection;
            allControlActions.Add(CA_HoverTileSelection);
        }
    }

    private void CreateBasicMeeleAttackCA()
    {
        GameObject GO = userControlOrchestrator.gameObject;
        GameObject selectedCharacter = userControlOrchestrator.selectedCharacter;

        CA_BasicMeeleAttack = GO.AddComponent<CA_BasicMeeleAttack>();
        CA_BasicMeeleAttack.userControlOrchestrator = userControlOrchestrator;
        CA_BasicMeeleAttack.input = input;
        allControlActions.Add(CA_BasicMeeleAttack);
    }

    public void DeleteCA(E_CA_Type type)
    {
        switch (type)
        {
            case E_CA_Type.HOVER_TILE_SELECTION:
                if (CA_HoverTileSelection != null)
                {
                    allControlActions.Remove(CA_HoverTileSelection);
                    DestroyComponent(CA_HoverTileSelection);
                    CA_HoverTileSelection = null;
                }
                break;

            case E_CA_Type.HOVER_CHARACTER:
                if (CA_HoverCharacter != null)
                {
                    allControlActions.Remove(CA_HoverCharacter);
                    DestroyComponent(CA_HoverCharacter);
                    CA_HoverCharacter = null;
                }
                break;

            case E_CA_Type.IDLE_CHARACTER:
                if (CA_IdleCharacter != null)
                {
                    allControlActions.Remove(CA_IdleCharacter);
                    DestroyComponent(CA_IdleCharacter);
                    CA_IdleCharacter = null;
                }
                break;

            case E_CA_Type.MOVE_CHARACTER:
                if (CA_MoveCharacter != null)
                {
                    allControlActions.Remove(CA_MoveCharacter);
                    DestroyComponent(CA_MoveCharacter);
                    CA_MoveCharacter = null;
                }
                break;

            case E_CA_Type.SELECT_CHARACTER_WITH_CLICK:
                if (CA_SelectCharacterWithClick != null)
                {
                    allControlActions.Remove(CA_SelectCharacterWithClick);
                    DestroyComponent(CA_SelectCharacterWithClick);
                    CA_SelectCharacterWithClick = null;
                }
                break;

            case E_CA_Type.SELECT_TILE_WITH_CLICK:
                if (CA_SelectTileWithClick != null)
                {
                    allControlActions.Remove(CA_SelectTileWithClick);
                    DestroyComponent(CA_SelectTileWithClick);
                    CA_SelectTileWithClick = null;
                }
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
        if (characterPhase != ECharacterPhase.IDLE)
            return;

        List<GameObject> characterList = movementPointsManager.characters;
        GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);
        Vector2Int characterOriginalPos = gridManager.WorldToGridPosition(matchingCharacter.transform.position);

        if (matchingCharacter != null)
        {
            PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();

            if (playerStatSheet != null)
            {

                int distance = gridManager.GetTileDistance(characterOriginalPos, gridPos);

                // Validate and execute move
                if (distance <= playerStatSheet.movementPoints)
                {
                    playerStatSheet.movementPoints -= distance;
                    characterPhase = ECharacterPhase.MOVE;
                }
                else
                {
                    return;
                }
                 
                // this should be done in the character move CM
                //if (playerStatSheet.movementPoints <= 0 && playerStatSheet.attackPoints <= 0)
                //{
                //    userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
                //}
            }
        }

        characterPhase = ECharacterPhase.MOVE;
    }

    private void HandleBasicAttack()
    {
        if (characterPhase != ECharacterPhase.IDLE)
            return;

        List<GameObject> characterList = movementPointsManager.characters;
        GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);

        if (matchingCharacter != null)
        {
            PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();

            if (playerStatSheet != null && playerStatSheet.attackPoints > 0)
            {
                playerStatSheet.attackPoints -= 1;
                characterPhase = ECharacterPhase.ATTACK;
            }
            //// this should be done in the attack CM
            //if (playerStatSheet.movementPoints <= 0 && playerStatSheet.attackPoints <= 0)
            //{
            //    userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
            //}
        }
    }

    private void HandleFinishBasicAttack()
    {
        if (characterPhase == ECharacterPhase.ATTACK)
        {
            List<GameObject> characterList = movementPointsManager.characters;
            GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);

            if (matchingCharacter != null)
            {
                characterPhase = ECharacterPhase.IDLE;
            }
        }
    }
}

public class InfoObject
{
    public ECharacterPhase characterPhase;
}
