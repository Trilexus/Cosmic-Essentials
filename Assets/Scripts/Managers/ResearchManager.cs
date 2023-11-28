using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ResearchManager : MonoBehaviour
{

    public List<ResearchNodeScriptableObject> ResearchNodeScriptableObjects;
    public List<ResearchNodeScriptableObject> ResearchNodeScriptableObjectsDone;
    public ResearchNodeScriptableObject ActiveResearchNodeScriptableObject;
    public GameObject ActiveResearchNodeGameObject;
    public static ResearchManager Instance;
    public int ResearchPoints = 0;

    public delegate void ResearchNodeDone(ResearchNode researchNode, ResearchNodeScriptableObject researchNodeScriptableObject);
    public event ResearchNodeDone OnResearchNodeDone;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetActiveResearchNode(ResearchNodeScriptableObject researchNode, GameObject ResearchNodeGameObject)
    {
        ActiveResearchNodeScriptableObject = researchNode;
        if (ActiveResearchNodeGameObject != null) ActiveResearchNodeGameObject.GetComponent<Image>().color = Color.white;
        ActiveResearchNodeGameObject = ResearchNodeGameObject;
        ActiveResearchNodeGameObject.GetComponent<Image>().color = Color.yellow;
    }

    public void AddResearchPoints(int researchPoints)
    {
        ResearchPoints += researchPoints;        
    }


    public List<ResearchNodeScriptableObject> GetAvailableResearchNodes()
    {
        List<ResearchNodeScriptableObject> availableResearchNodes = new List<ResearchNodeScriptableObject>();

        foreach (ResearchNodeScriptableObject researchNode in ResearchNodeScriptableObjects)
        {
            if (AreRequirementsMet(researchNode))
            {
                availableResearchNodes.Add(researchNode);
            }
        }

        return availableResearchNodes;
    }

    // Diese Methode überprüft, ob alle Voraussetzungen eines SO erfüllt sind.
    private bool AreRequirementsMet(ResearchNodeScriptableObject researchNode)
    {
        foreach (ResearchNodeScriptableObject requirement in researchNode.ResearchRequirements)
        {
            if (!ResearchNodeScriptableObjectsDone.Contains(requirement))
            {
                return false;
            }
        }

        return true;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DoResearch();
    }

    private void DoResearch()
    {
        if (ActiveResearchNodeScriptableObject != null && ResearchPoints > 0)
        {
            int remainingPoints = ActiveResearchNodeScriptableObject.ResearchNodeCost - ActiveResearchNodeScriptableObject.currentResearchProgress;
            int pointsToAdd = math.min(ResearchPoints, remainingPoints);
            ResearchPoints -= pointsToAdd;
            ActiveResearchNodeScriptableObject.currentResearchProgress += pointsToAdd;

            if (ActiveResearchNodeScriptableObject.currentResearchProgress >= ActiveResearchNodeScriptableObject.ResearchNodeCost)
            {
                ResearchNodeScriptableObjectsDone.Add(ActiveResearchNodeScriptableObject);
                ResearchNodeScriptableObjects.Remove(ActiveResearchNodeScriptableObject);
                ActiveResearchNodeGameObject.GetComponent<Image>().color = Color.green;
                OnResearchNodeDone?.Invoke(ActiveResearchNodeGameObject.GetComponent<ResearchNode>(), ActiveResearchNodeScriptableObject);
                ActiveResearchNodeGameObject = null;
                ActiveResearchNodeScriptableObject = null;

            }
        }
    }
}