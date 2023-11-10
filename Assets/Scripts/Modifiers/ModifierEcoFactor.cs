using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifierEcoFactor : Modifier
{
    ModifierEcoFactorScriptableObject ScriptableObject;
    public int EcoFactor;

    // Start is called before the first frame update
    public ModifierEcoFactor(ModifierEcoFactorScriptableObject modifierEcoFactorScriptableObject) : base(modifierEcoFactorScriptableObject)
    {
        ScriptableObject = modifierEcoFactorScriptableObject;
        EcoFactor = ScriptableObject.EcoFactor;
    }
    
    public override void ApplyModifier(int ecoIndex)
    {

    }
}
