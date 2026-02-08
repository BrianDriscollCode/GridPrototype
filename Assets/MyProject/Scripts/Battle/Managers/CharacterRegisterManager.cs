using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterRegisterManager : MonoBehaviour
{
    public List<GameObject> characters;
    public List<GameObject> playerParty;
    public List<GameObject> enemyParty;

    private void Start()
    {
        characters = GameObject.FindGameObjectsWithTag("Character").ToList();
    
        foreach (GameObject c in characters)
        {
            PartyTag partyTagGO = c.GetComponent<PartyTag>();
            
            if (partyTagGO.partyTag == PartyTag.CharacterPartyTag.PLAYER)
            {
                playerParty.Add(c);
            }

            if (partyTagGO.partyTag == PartyTag.CharacterPartyTag.ENEMY)
            {
                enemyParty.Add(c);
            }
        }
    }

}
