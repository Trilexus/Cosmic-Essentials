using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResearchNode", menuName = "ScriptableObjects/ResearchNodeScriptableObject")]

public class ResearchNodeScriptableObject : ScriptableObject
{
    public string ResearchNodeName;
    public string ResearchNodeDescription;
    public Sprite ResearchNodeSprite;
    public int ResearchNodeLevel;
    public int ResearchNodeCost;
    public List<ResearchNodeScriptableObject> ResearchRequirements;
    public int currentResearchProgress = 0;
    public ResearchType ResearchType;
    public ResearchUpgrade researchUpgrade;
}


