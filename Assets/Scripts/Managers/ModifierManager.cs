using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifierManager : MonoBehaviour
{
    public static ModifierManager Instance;
    public ModifierFactory ModifierFactory = new ModifierFactory();

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }else 
        {
            Destroy(gameObject);
        }
    }

    public List<Modifier> CreateModifiers(List<ModifierScriptableObject> modifierScriptableObjects)
    {
        List<Modifier> modifiers = new List<Modifier>();
        foreach (ModifierScriptableObject modifierScriptableObject in modifierScriptableObjects)
        {
            modifiers.Add(ModifierFactory.CreateModifier(modifierScriptableObject));
        }
        return modifiers;
    }

    public void ApplyModifiers(List<Modifier> modifiers, Dictionary<ResourceType, ResourceStorage> ResourceStorageCelestialBody)
    {
        foreach (Modifier modifier in modifiers.Where(modifier => modifier is ModifierResourceProductionFactor))
        {
            modifier.ApplyModifier(ResourceStorageCelestialBody);
        }
    }

    public void ApplyModifiers(List<Modifier> modifiers, float ProductivityRate)
    {
        foreach (Modifier modifier in modifiers.Where(modifier => modifier is ModifierProductivityFactor))
        {
            modifier.ApplyModifier(ProductivityRate);
        }
    }

    public void ApplyModifiers(List<Modifier> modifiers, int ecoIndex)
    {
        foreach (Modifier modifier in modifiers.Where(modifier => modifier is ModifierEcoFactor))
        {
            modifier.ApplyModifier(ecoIndex);
        }
    }

}
