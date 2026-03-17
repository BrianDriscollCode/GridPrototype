using UnityEngine;

public class UIManager : MonoBehaviour
{
    public enum EUICanvasSection
    {
        PLAYERTURN,
        PLAYERREACT
    }

    [SerializeField] GameObject playerTurnPanel;
    [SerializeField] GameObject playerReactPanel;

    private void Start()
    {
        playerTurnPanel.SetActive(false);
        playerReactPanel.SetActive(false);
    }

    public void SetCanvasActive(EUICanvasSection canvas)
    {
        switch (canvas)
        {
            case EUICanvasSection.PLAYERTURN:
                playerTurnPanel.SetActive(true);
                break;
            case EUICanvasSection.PLAYERREACT:
                playerReactPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void SetCanvasInactive(EUICanvasSection canvas)
    {
        switch (canvas)
        {
            case EUICanvasSection.PLAYERTURN:
                playerTurnPanel.SetActive(false);
                break;
            case EUICanvasSection.PLAYERREACT:
                playerReactPanel.SetActive(false);
                break;
            default:
                break;
        }
    }
}
