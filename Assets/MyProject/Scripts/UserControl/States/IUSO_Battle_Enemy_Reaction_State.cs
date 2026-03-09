using UnityEngine;
using UnityEngine.Windows;

public class IUSO_Battle_Enemy_Reaction_State : IUSO_State
{
    private UserControlOrchestrator userControlOrchestrator;
    private InputSystem_Actions input;
    private EnemyAI enemyAI;

    public void EnterState(UserControlOrchestrator USO)
    {
        userControlOrchestrator = USO;
        input = userControlOrchestrator.input;
        enemyAI = userControlOrchestrator.enemyAI;
        
        // TODO: Initialize reaction-specific control actions
        // e.g., CA_ReactionChoice, CA_CounterAttack, etc.
    }

    public void SuspendState() 
    {
    }
    public void ResumeState() 
    { 

    }

    public void ExitState()
    {
        // No animator manipulation here — PlayerTurn.ResumeState() handles it
        //userControlOrchestrator = null;
    }

    public void Update()
    {
        // Guard against null input
        if (input == null)
        {
            input = userControlOrchestrator.input;
            Debug.LogError("Input is null in Reaction State Update!");
        }
            
        if (input.Player.Exit.WasPressedThisFrame())
        {
            userControlOrchestrator.PopState();
        }
    }

    public void FixedUpdate()
    {
        // TODO: Handle reaction physics/state updates
        //Debug.Log("RUNNING REACTION");

        
    }

    public void DeleteCA(E_CA_Type type)
    {
        // TODO: Implement control action deletion if needed
        // Similar pattern to IUSO_Battle_PlayerTurn_State.DeleteCA()
    }

    public InfoObject GetStateInfo()
    {
        return new InfoObject { characterPhase = ECharacterPhase.REACT };
    }

    public ECharacterPhase GetCharacterPhase()
    {
        return ECharacterPhase.REACT;
    }

    public void SetCharacterPhase(ECharacterPhase phase)
    {
        // Reaction state typically stays in REACT phase
        // Can add validation or state switching logic if needed
    }
}