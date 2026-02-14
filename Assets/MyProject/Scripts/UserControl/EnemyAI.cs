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

            playerParty = characterRegisterManager.playerParty;
            enemyParty = characterRegisterManager.enemyParty;
        } 
    }
    public void StartTurn(IUSO_Battle_EnemyTurn_State battleState)
    {
        enemyBattleState = battleState;
    }




}
