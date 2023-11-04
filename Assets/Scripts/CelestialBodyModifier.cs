using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyModifier : MonoBehaviour
{
    IModifierType modifierType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum MoidifierType
{
    ResourceMultiplier,
    PopulationCapShift,
    PopulationGrowthRateMultiplier,
    ConstructionRateMultiplier,
    ProductivityRateMultiplier,
}
