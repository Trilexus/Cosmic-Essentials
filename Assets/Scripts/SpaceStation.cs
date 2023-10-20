using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpaceStation : CelestialBody
{
    public int constructionProgress; // 0 -> 100 percent
    [SerializeField]
    const int requiredResourceAmount = 500;
    const int requiredResourceAmountStep = 100;
    [SerializeField]
    const int progressIncrement = 20;
    [SerializeField]
    private GameObject spriteMask; // shows the construction progress 0 -> 1 
    private GameObject belongingtoPlanet;
    private Planet belongingtoPlanetScript;
    private Vector3 PlanetOffset = new Vector3(1.5f, 1.5f, 0);
    [SerializeField]
    private List<ResourceStorage> orderSpacePoints;
    private bool isResourceTransferOrderActive = false;


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

    public override void UnloadTheSpaceShips()
    {
        base.UnloadTheSpaceShips();
        isResourceTransferOrderActive = false;
    }
    public void StartBuilding()
    {
        Debug.Log("startBuilding");
        belongingtoPlanet = GUIManager.Instance.selectedCelestialBody;
        belongingtoPlanetScript = belongingtoPlanet.GetComponent<Planet>();
        constructionProgress = progressIncrement;
        belongingtoPlanetScript.ResourceTransferOrders.Add(new ResourceTransferOrder(orderSpacePoints, belongingtoPlanetScript, this));
        isResourceTransferOrderActive = true;
    }

    public void ProcessBuilding()
    {
        Debug.Log("ProcessBuilding");
        if (ResourceStorageCelestialBody.TryGetValue(ResourceType.SpacePoints, out var spacePointsStorage) && spacePointsStorage.StorageQuantity >= requiredResourceAmountStep)
        {
            Debug.Log("ProcessBuilding: if");
            Debug.Log("ProcessBuilding: spacePointsStorage.StorageQuantity: " + spacePointsStorage.StorageQuantity);
            isResourceTransferOrderActive = false;
            constructionProgress += progressIncrement;
            spacePointsStorage.StorageQuantity -= requiredResourceAmountStep;
            spriteMask.transform.localScale = new Vector3(constructionProgress / 100f, constructionProgress / 100f, 1);
        } else if (!isResourceTransferOrderActive)
        {
            belongingtoPlanetScript.ResourceTransferOrders.Add(new ResourceTransferOrder(orderSpacePoints, belongingtoPlanetScript, this));
            isResourceTransferOrderActive = true;
        }
    }


    public void setPositionNearCelestialObject(GameObject celestialObject)
    {
        Debug.Log("setPositionNearCelestialObject");
        transform.position = celestialObject.transform.position + PlanetOffset;
    }

    public override void UpdateInfoText()
    {
        int farms = Areas.Count(I => I.structure.Name == "Farm" && I.constructionProgress >= OneHundredPercent);
        int mines = Areas.Count(I => I.structure.Name == "Mine" && I.constructionProgress >= OneHundredPercent);
        int reactors = Areas.Count(I => I.structure.Name == "Reactor" && I.constructionProgress >= OneHundredPercent);
        int spaceports = Areas.Count(I => I.structure.Name == "Spaceport" && I.constructionProgress >= OneHundredPercent);
        int farmsInConstruction = Areas.Count(I => I.structure.Name == "Farm" && I.constructionProgress < OneHundredPercent);
        int minesInConstruction = Areas.Count(I => I.structure.Name == "Mine" && I.constructionProgress < OneHundredPercent);
        int reactorsInConstruction = Areas.Count(I => I.structure.Name == "Reactor" && I.constructionProgress < OneHundredPercent);
        int spaceportsInConstruction = Areas.Count(I => I.structure.Name == "Spaceport" && I.constructionProgress < OneHundredPercent);
        int DevelopedAreas = Areas.Count();
        sb.Clear();

        // Eco Index and Population and productivity rate info
        string populationInfo = $"\ue533: {population.CurrentPopulation}";
        string productivityInfo = $"\uf201: {ProductivityRate}";


        // Farm, Mine, and Reactor info
        string areaInfo = $"\uf0ac: {DevelopedAreas}/{maxAreas}";
        string farmInfo = $"\uf722: {farms}/{farmsInConstruction}";
        string mineInfo = $"\uf275: {mines}/{minesInConstruction}";
        string reactorInfo = $"\uf7ba: {reactors}/{reactorsInConstruction}";
        string spaceportInfo = $"\uf135: {spaceports}/{spaceportsInConstruction}";

        // Resource Info
        string resourceStorageFood = $"\ue2cd: {ResourceStorageCelestialBody[ResourceType.Food].StorageQuantity}";
        string resourceStorageMetal = $"\uf468: {ResourceStorageCelestialBody[ResourceType.Metal].StorageQuantity}";
        string resourceStorageEnergy = $"\uf0e7: {ResourceStorageCelestialBody[ResourceType.Energy].StorageQuantity}";
        string resourceStorageSpacePoint = $"\uf7bf: {ResourceStorageCelestialBody[ResourceType.SpacePoints].StorageQuantity}";

        string resourceFoodProduction = $"({ResourceStorageCelestialBody[ResourceType.Food].ProductionQuantity})";
        string resourceMetalProduction = $"({ResourceStorageCelestialBody[ResourceType.Metal].ProductionQuantity})";
        string resourceEnergyProduction = $"({ResourceStorageCelestialBody[ResourceType.Energy].ProductionQuantity})";
        string resourceSpacePointProduction = $"({ResourceStorageCelestialBody[ResourceType.SpacePoints].ProductionQuantity})";

        string resourceFoodConsumption = $"({ResourceStorageCelestialBody[0].ConsumptionQuantity})";
        string resourceMetalConsumption = $"({ResourceStorageCelestialBody[ResourceType.Metal].ConsumptionQuantity})";
        string resourceEnergyConsumption = $"({ResourceStorageCelestialBody[ResourceType.Energy].ConsumptionQuantity})";
        string resourceSpacePointConsumption = $"({ResourceStorageCelestialBody[ResourceType.SpacePoints].ConsumptionQuantity})";

        sb.AppendLine($"{areaInfo} {populationInfo}");
        string finalString = sb.ToString();
        celestialBodyInfoTop.SetText(finalString);

        sb.Clear();
        sb.AppendLine($"{farmInfo} {resourceStorageFood}{resourceFoodProduction}{resourceFoodConsumption}");
        sb.AppendLine($"{mineInfo} {resourceStorageMetal}{resourceMetalProduction}{resourceMetalConsumption}");
        sb.AppendLine($"{reactorInfo} {resourceStorageEnergy}{resourceEnergyProduction}{resourceEnergyConsumption}");
        sb.AppendLine($"{spaceportInfo} {resourceStorageSpacePoint}{resourceSpacePointProduction}{resourceSpacePointConsumption}");

        finalString = sb.ToString();
        celestialBodyInfoRight.SetText(finalString);
    }
}
