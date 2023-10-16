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
    [SerializeField]
    public ResourceType ResourceType; 


    public void Initialize(string name, int quantity)
    {
        this.Name = name;
        this.Quantity = quantity;
        if (Enum.TryParse(name, out ResourceType resourceType))
        {
            this.ResourceType = resourceType;  // Setze das Enum-Feld
        }
        else
        {
            throw new ArgumentException($"Invalid resource name: {name}");
        }
    }
}
