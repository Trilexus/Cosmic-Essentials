using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStructureResourceUpgradee", menuName = "ScriptableObjects/StructureResourceUpgradeScriptableObject")]

public class StructureResourceUpgrade : ResearchUpgrade
{
    public List<StructureScriptableObject> structureScriptableObjects;
    public List<ResourceType> ResourcesType;
    public int ResourcesChangeAmount;
}
