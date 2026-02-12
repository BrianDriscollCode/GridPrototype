using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class IUSO_Battle_EnemyTurn_State : IUSO_State
{
    public ECharacterPhase characterPhase;
    private GameObject activeCharacter;
    private GridManager gridManager;
    private MovementPointsManager movementPointsManager;

    private UserControlOrchestrator userControlOrchestrator;

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
        }
    }


    public void ExitState()
    {

    }
    public void Update()
    {

    }

    public void FixedUpdate()
    {

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