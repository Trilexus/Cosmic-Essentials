using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifierProductivityFactor : Modifier
{
    float ProductivityFactor;
    // Start is called before the first frame update
    public ModifierProductivityFactor(ModifierProductivityFactorScriptableObject modifierProductivityFactorScriptableObject) : base(modifierProductivityFactorScriptableObject)
    {
        ProductivityFactor = modifierProductivityFactorScriptableObject.ProductivityFactor;
    }

    public override void ApplyModifier(float ProductivityRate)
    {

    }

}
