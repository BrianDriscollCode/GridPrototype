using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;
public class IUSO_Battle_EnemyTurn_State : IUSO_State
{   
    // Receives global ref of enemyAI from orchestrator
    // rethink this?
    public EnemyAI enemyAI;

    public ECharacterPhase characterPhase;
    private GameObject activeEnemyCharacter;
    private GameObject targetPlayerCharacter;


    private GridManager gridManager;
    private MovementPointsManager movementPointsManager;
    private UserControlOrchestrator userControlOrchestrator;
    private CharacterRegisterManager characterRegisterManager;


    private CA_IdleCharacter CA_IdleCharacter;
    private CA_MoveCharacter CA_MoveCharacter;
    private CA_BasicMeeleAttack CA_BasicMeeleAttack;
    private List<MonoBehaviour> allControlActions;

    private InputSystem_Actions input;


    private List<GameObject> playerParty;
    private List<GameObject> enemyParty;

    public void EnterState(UserControlOrchestrator UCO)
    {
        userControlOrchestrator = UCO;
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
            managerObj = null;

            managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<CharacterRegisterManager>() != null);
            if (managerObj != null)
            {
                characterRegisterManager = managerObj.GetComponent<CharacterRegisterManager>();
            }
            managerObj = null;

            playerParty = characterRegisterManager.playerParty;
            enemyParty = characterRegisterManager.enemyParty;
            input = userControlOrchestrator.input;
        }

        InitializeControlActions();
        enemyAI.StartTurn(this);
    }


    // What does the enemy need to make a decision
    // PlayerPositions

    public void ExitState()
    {
        
    }
    public void Update()
    {

    }

    public void FixedUpdate()
    {
    
    }

    public void InitializeControlActions()
    {

    }

    // Factory pattern WOULD BE better, but need to focus on prototype
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
    }

    public void DeleteCA(E_CA_Type type)
    {

    }

    public InfoObject GetStateInfo()
    {
        return new InfoObject{ characterPhase = this.characterPhase };
    }

    public ECharacterPhase GetCharacterPhase()
    {
        return this.characterPhase;
    }

    public void SetCharacterPhase(ECharacterPhase phase)
    {

    }
}