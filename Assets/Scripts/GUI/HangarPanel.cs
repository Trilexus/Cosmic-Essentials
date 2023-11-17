using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarPanel : MonoBehaviour
{
    private GameObject scrollViewHangarOverviewContent;
    private GameObject spaceShipEntryPrefab;
    private CelestialBody selectedCelestialBodyScript;
    // Start is called before the first frame update
    void Start()
    {
        InitializedScript();
    }

    public void InitializedScript()
    {
        scrollViewHangarOverviewContent = GUIManager.Instance.ScrollViewHangarOverviewContent;
        spaceShipEntryPrefab = GUIManager.Instance.SpaceShipEntryPrefab;
        GUIManager.Instance.OnSelectedCelestialBodyChanged += CreateHangarView;
        GUIManager.Instance.OnDeselectCelestialBody += DestroyHangarView;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateHangarView(CelestialBody celestialBody)
    {
        Debug.Log("CreateHangarView");
        FillHangarOverview(celestialBody);
        selectedCelestialBodyScript.SubscribeToHangarChanges(UpdateHangar);
    }
    public void UpdateHangar(HangarSlot hangarSlot)
    {
        Debug.Log("UpdateHangar");
        FillHangarOverview(selectedCelestialBodyScript);
    }

    public void DestroyHangarView()
    {
        Debug.Log("DestroyHangarView");
        ClearHangarPanel();
        selectedCelestialBodyScript.UnSubscribeToHangarChanges(UpdateHangar);
    }


    public void FillHangarOverview(CelestialBody selectedCelestialBodyScript)
    {
        this.selectedCelestialBodyScript = selectedCelestialBodyScript;
        ClearHangarPanel();
        List<HangarSlot> hangar = selectedCelestialBodyScript.PerformHangarOperation(manager => manager.GetHangarSlots());
        foreach (HangarSlot slot in hangar)
        {
            GameObject SpaceShip = Instantiate(spaceShipEntryPrefab, scrollViewHangarOverviewContent.transform);
            SpaceShip.GetComponent<SpaceShipEntry>().Initialize(slot);
            SpaceShip.transform.SetParent(scrollViewHangarOverviewContent.transform);
        }
    }

    private void ClearHangarPanel()
    {
        foreach (Transform child in scrollViewHangarOverviewContent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
