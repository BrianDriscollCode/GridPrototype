using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{

    public GameObject magicAttacksManager;

    public Transform spawnOffset;
    bool usingSlashCircle;

    public GameObject[] FXList_Slash;
    GameObject[] currentFXList;

    int currentFXElement;

    InputSystem_Actions input;

    [SerializeField] Transform characterTransform;

    // Start is called before the first frame update
    void Awake()
    {
        currentFXList = FXList_Slash;
    }


    private void Start()
    {
        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;

        currentFXList = FXList_Slash;
        currentFXElement = 0;
    }

    // Update is called once per frame
    void Update()
    {


        if (magicAttacksManager != null)
        {
            ChangeEffect();
        }

        if (input.Player.TrueDown.WasPressedThisFrame())
        {
            DoTheSlash(currentFXList[currentFXElement], characterTransform);
        }
    }

    void ChangeEffect()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        if (input.Player.Interact.IsPressed())
        {
            magicAttacksManager.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    public void DoTheSlash(GameObject FX, Transform offset)
    {
        GameObject instantiatedFX = Instantiate(FX, offset.position, offset.rotation);
    }
}
