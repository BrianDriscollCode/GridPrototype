using UnityEngine;


// * - preceded by a star symbol means "this is what the name represents.
//     If there is no star symbol, it is verbatim.

//Name convention: IUSO_*GameState_*InterfaceState_State
public interface IUSO_State
{
    void EnterState(UserControlOrchestrator USO);
    void ExitState();
    void SuspendState();
    void ResumeState();
    public void Update();
    public void FixedUpdate();
    void DeleteCA(E_CA_Type type);
    public InfoObject GetStateInfo();

    public ECharacterPhase GetCharacterPhase();
    public void SetCharacterPhase(ECharacterPhase phase);
}
