using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureBuildProgressPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject scrollViewStructureOverviewContent;
    [SerializeField]
    private GameObject structureInBuildingProgressPrefab;
    private CelestialBody selectedCelestialBodyScript;


    // Start is called before the first frame update
    void Start()
    {
        InizialiseContent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void InizialiseContent()
    {
        GUIManager.Instance.OnSelectedCelestialBodyChanged += CreateStructureView;
        GUIManager.Instance.OnDeselectCelestialBody += ClearStructurePanel;
    }

    public void CreateStructureView(CelestialBody celestialBody)
    {
        Debug.Log("CreateStructureView");
        FillStructureOverview(celestialBody);
        selectedCelestialBodyScript = celestialBody;
        selectedCelestialBodyScript.OnStartStructureBuild += CreateNode;
    }

    public void CreateNode(Area area)
    {
        GameObject structureInBuildingProgress = Instantiate(structureInBuildingProgressPrefab, scrollViewStructureOverviewContent.transform);
        structureInBuildingProgress.GetComponent<StructureBuildProgressEntry>().Initialize(area);
        structureInBuildingProgress.transform.SetParent(scrollViewStructureOverviewContent.transform);
    }

    public void FillStructureOverview(CelestialBody celestialBody) 
    {
        List<Area> areas = celestialBody.Areas;
        foreach (Area area in areas.Where(i => i.ConstructionProgress < 100))
        {
            CreateNode(area);
        }
    }


    private void ClearStructurePanel()
    {
        selectedCelestialBodyScript.OnStartStructureBuild -= CreateNode;
        foreach (Transform child in scrollViewStructureOverviewContent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
