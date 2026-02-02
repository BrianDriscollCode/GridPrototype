using UnityEngine;


// * - preceded by a star symbol means "this is what the name represents.
//     If there is no star symbol, it is verbatim.

//Name convention: IUSO_*GameState_*InterfaceState_State
public interface IUSO_State
{
    void EnterState(UserControlOrchestrator userControlOrchestrator);
    void ExitState(UserControlOrchestrator userControlOrchestrator);
    public void Update(UserControlOrchestrator userControlOrchestrator);
    public void FixedUpdate(UserControlOrchestrator userControlOrchestrator);
    void DeleteCA(E_CA_Type type);
    public InfoObject GetStateInfo();

    public ECharacterPhase GetCharacterPhase();
    public void SetCharacterPhase(ECharacterPhase phase);
}
