using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchPanel : MonoBehaviour
{
    public GameObject ResearchContent;
    public GameObject ResearchNodePrefab;
    // Start is called before the first frame update
    void Start()
    {
        FillContentWithNodes();
        ResearchManager.Instance.OnResearchNodeDone += RefreshAfterResearchDone;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FillContentWithNodes()
    {
        ClearContent();
        List<ResearchNodeScriptableObject> availableResearchNodes = ResearchManager.Instance.GetAvailableResearchNodes();
        foreach (ResearchNodeScriptableObject researchNode in availableResearchNodes)
        {
            GameObject researchNodeGameObject = Instantiate(ResearchNodePrefab, ResearchContent.transform);
            ResearchNode researchNodeScript = researchNodeGameObject.GetComponent<ResearchNode>();
            researchNodeScript.researchNodeData = researchNode;
            researchNodeScript.Initialize();
        }
    }

    public void RefreshAfterResearchDone(ResearchNode researchNode, ResearchNodeScriptableObject researchNodeScriptableObject)
    {
        FillContentWithNodes();
    }

    public void RemovePanel(GameObject panel)
    {
        Destroy(panel);
    }

    public void ClearContent() {
        foreach (Transform child in ResearchContent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
