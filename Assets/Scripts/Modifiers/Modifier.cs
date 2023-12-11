using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Modifier
{ 
    public string ModifierName;
    public ModifierType ModifierType;
    //public AttributeType AttributeType;
    public Sprite ModifierSprite;
    public int ModifierLevel;

    public Modifier(ModifierScriptableObject ModifierScriptableObject)    
    {
        ModifierName = ModifierScriptableObject.ModifierName;
        ModifierType = ModifierScriptableObject.ModifierType;
        //AttributeType = ModifierScriptableObject.AttributeType;
        ModifierSprite = ModifierScriptableObject.ModifierSprite;
        ModifierLevel = ModifierScriptableObject.ModifierLevel;
    }

    public virtual void ApplyModifier(Dictionary<ResourceType, ResourceStorage> ResourceStorageCelestialBody)
    {

    }
    public virtual void ApplyModifier(float ProductivityRate)
    {

    }
    public virtual void ApplyModifier(int ecoIndex)
    {

    }
}

public enum ModifierType
{
    Buff,
    Debuff
}

public enum AttributeType
{
    ResourceProductionFactor,
    ProductivityFactor,
    EcoFactor,

}
