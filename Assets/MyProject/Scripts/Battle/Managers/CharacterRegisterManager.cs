using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterRegisterManager : MonoBehaviour
{
    public List<GameObject> characters;

    private void Start()
    {
        characters = GameObject.FindGameObjectsWithTag("Character").ToList();
    
        foreach (GameObject c in characters)
        {
            Debug.Log("Character has entered the arena: " + c);
        }
    }

}
