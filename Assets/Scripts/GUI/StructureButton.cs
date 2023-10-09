using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureButton : MonoBehaviour
{
    public StructureScriptableObject structureData; // Dein ScriptableObject
    public string structureName;

    public void Build()
    {
        // Dein Code, z.B. den EntityManager ansprechen
        if (structureData != null)
        {
            EntityManager.Instance.BuildStructure(structureData);
        } else if (structureName.Equals("Spacestation"))
        {
            EntityManager.Instance.BuildSpaceStation();
        }
    }
    public void Demolish()
    {
        // Dein Code, z.B. den EntityManager ansprechen
        EntityManager.Instance.DemolishStructure(structureData);
    }
}
