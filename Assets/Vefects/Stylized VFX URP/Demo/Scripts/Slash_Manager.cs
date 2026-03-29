using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash_Manager : MonoBehaviour
{
    public float delay;
    private float reinitializeDelay;

    public GameObject magicAttacksManager;

    public Transform spawnOffset;
    bool usingSlashCircle;

    public GameObject[] FXList_Slash;
    public GameObject[] FXList_SlashCircle;
    public GameObject[] FXList_Piercing;
    GameObject[] currentFXList;

    int currentFXElement;

    InputSystem_Actions input;

    // Start is called before the first frame update
    void Awake()
    {
        reinitializeDelay = delay;
        currentFXList = FXList_Slash;

    }


    private void Start()
    {
        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;
    }

    // Update is called once per frame
    void Update()
    {
        if(delay > 0)
        {
            delay -= Time.deltaTime;
        }

        if(delay <= 0)
        {
            DoTheSlash(currentFXList[currentFXElement]);
            delay = reinitializeDelay;
        }

        if(magicAttacksManager != null)
        {
            ChangeEffect();
        }

        InputsFXElement();
        InputsFXType();

        if(usingSlashCircle)
        {
            //SlashCircle();
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


    void InputsFXType()
    {
        if (input.Player.B.IsPressed())
        {
            currentFXList = FXList_Slash;
        }

        if (input.Player.N.IsPressed())
        {
            currentFXList = FXList_SlashCircle;
        }

        if (input.Player.M.IsPressed())
        {
            currentFXList = FXList_Piercing;
        }

    }


    void InputsFXElement()
    {
        if (input.Player.Right.IsPressed())
        {
            if (currentFXElement < currentFXList.Length - 1)
            {
                currentFXElement += 1;
            }

            else if (currentFXElement >= currentFXList.Length - 1)
            {
                currentFXElement = 0;
            }

        }

        if (input.Player.Left.IsPressed())
        {
            if (currentFXElement > 0)
            {
                currentFXElement -= 1;
            }

            else if (currentFXElement <= 0)
            {
                currentFXElement = currentFXList.Length - 1;
            }

        }
    }

    void DoTheSlash(GameObject FX)
    {
        Instantiate(FX, spawnOffset.position, spawnOffset.rotation);

    }
}
