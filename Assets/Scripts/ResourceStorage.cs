using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public int Quantity;

    public ResourceStorage(string name, int quantity)
    {
        this.Name = name;
        this.Quantity = quantity;
    }
}
