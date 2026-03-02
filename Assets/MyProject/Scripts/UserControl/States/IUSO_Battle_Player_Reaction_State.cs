using UnityEngine;

public class IUSO_Battle_Player_Reaction_State : IUSO_State
{
    private UserControlOrchestrator userControlOrchestrator;

    public void EnterState(UserControlOrchestrator USO)
    {
        userControlOrchestrator = USO;
        
        // TODO: Initialize reaction-specific control actions
        // e.g., CA_ReactionChoice, CA_CounterAttack, etc.
    }

    public void ExitState()
    {
        // TODO: Clean up reaction-specific control actions
        
        // Clear references
        userControlOrchestrator = null;
    }

    public void Update()
    {
        // TODO: Handle reaction input per frame
    }

    public void FixedUpdate()
    {
        // TODO: Handle reaction physics/state updates
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