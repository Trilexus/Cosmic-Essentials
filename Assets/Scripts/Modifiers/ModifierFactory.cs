using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//https://en.wikipedia.org/wiki/Factory_method_pattern
//My first factory! yay
public class ModifierFactory
{
    public static Modifier CreateModifier(ModifierScriptableObject modifierScriptableObject)
    {
        switch (modifierScriptableObject)
        {
            case ModifierResourceProductionFactorScriptableObject modifierResourceProductionFactorScriptableObject:
                return new ModifierResourceProductionFactor(modifierResourceProductionFactorScriptableObject);
            case ModifierProductivityFactorScriptableObject modifierProductivityFactorScriptableObject:
                return new ModifierProductivityFactor(modifierProductivityFactorScriptableObject);
            case ModifierEcoFactorScriptableObject modifierEcoFactorScriptableObject:
                return new ModifierEcoFactor(modifierEcoFactorScriptableObject);
            default:
                throw new ArgumentException("Unbekannter ModifierScriptableObject-Typ", nameof(modifierScriptableObject));

        }
    }
}
