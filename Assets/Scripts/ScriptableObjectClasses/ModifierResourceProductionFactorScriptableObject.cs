using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewModifierResourceProductionFactor", menuName = "ScriptableObjects/ModifierResourceProductionFactorScriptableObject")]
public class ModifierResourceProductionFactorScriptableObject : ModifierScriptableObject
{
    public ResourceType ResourceType;
    public float ResourceProductionFactor;
}
