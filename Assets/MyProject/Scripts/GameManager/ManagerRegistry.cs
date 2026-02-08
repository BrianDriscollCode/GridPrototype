using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ManagerRegistry : MonoBehaviour
{
    public List<GameObject> managerList;

    public void Start()
    {
        managerList = GameObject.FindGameObjectsWithTag("Manager").ToList();
    }
}
