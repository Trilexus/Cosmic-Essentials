using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifierResourceProductionFactor : Modifier
{
    public ResourceType ResourceType;
    public float ResourceProductionFactor;

    // Start is called before the first frame update
    public ModifierResourceProductionFactor(ModifierResourceProductionFactorScriptableObject modifierScriptableObject) : base(modifierScriptableObject)
    {
        ResourceType = modifierScriptableObject.ResourceType;
        ResourceProductionFactor = modifierScriptableObject.ResourceProductionFactor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyModifier(Dictionary<ResourceType, ResourceStorage> ResourceStorageCelestialBody)
    {
        if (ModifierType == ModifierType.Buff)
        {
            if (ResourceStorageCelestialBody.TryGetValue(ResourceType, out var resourceStorage))
            {
                resourceStorage.ProductionQuantity = (int)(resourceStorage.ProductionQuantity * ResourceProductionFactor);
            }
        }
        else if (ModifierType == ModifierType.Debuff)
        {
            if (ResourceStorageCelestialBody.TryGetValue(ResourceType, out var resourceStorage))
            {
                resourceStorage.ConsumptionQuantity = (int)(resourceStorage.ConsumptionQuantity * ResourceProductionFactor);
            }
        } 
    }
    
}
