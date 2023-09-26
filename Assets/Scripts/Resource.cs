using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public int Quantity;

    public Resource(string name, int quantity)
    {
        this.Name = name;
        this.Quantity = quantity;
    }
}
