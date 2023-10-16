using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStation : CelestialBody
{
    public int constructionProgress; // 0 -> 100 percent
    [SerializeField]
    const int requiredResourceAmount = 500;
    [SerializeField]
    const int progressIncrement = 20;
    [SerializeField]
    private GameObject spriteMask; // shows the construction progress 0 -> 1 
    private GameObject belongingtoPlanet;
    private Planet belongingtoPlanetScript;
    private Vector3 PlanetOffset = new Vector3(1.5f, 1.5f, 0);

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateInfoText();
    }
    protected override void Tick()
    {
        base.Tick();
        if (constructionProgress > 0 && constructionProgress < 100 ) { ProcessBuilding(); }
    }


    public override void UpdateInfoText()
    {
        //Debug.Log("UpdateInfoText: " + constructionProgress);
    }

    public void StartBuilding()
    {
        Debug.Log("startBuilding");
        belongingtoPlanet = GUIManager.Instance.selectedCelestialBody;
        belongingtoPlanetScript = belongingtoPlanet.GetComponent<Planet>();
        constructionProgress = progressIncrement;
    }

    public void ProcessBuilding()
    {
        if (ResourceStorageCelestialBody.TryGetValue(ResourceType.SpacePoints, out var resourceStorage) && resourceStorage.ProductionQuantity >= requiredResourceAmount)
        {
            constructionProgress += progressIncrement;
            resourceStorage.ProductionQuantity -= requiredResourceAmount;
            spriteMask.transform.localScale = new Vector3(constructionProgress / 100f, constructionProgress / 100f, 1);
        }
    }


    public void setPositionNearCelestialObject(GameObject celestialObject)
    {
        Debug.Log("setPositionNearCelestialObject");
        transform.position = celestialObject.transform.position + PlanetOffset;
    }
}
