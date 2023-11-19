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
    private ResourceTransferOrder buildStationMeterialOrder;
    private bool orderPlaced = false;



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
        orderPlaced = false;

    }
    public void StartBuilding()
    {
        Debug.Log("startBuilding");
        belongingtoPlanet = GUIManager.Instance.selectedCelestialBody;
        belongingtoPlanetScript = belongingtoPlanet.GetComponent<Planet>();
        constructionProgress = progressIncrement;
        buildStationMeterialOrder = new ResourceTransferOrder(orderSpacePoints, belongingtoPlanetScript, this, 4, true, true,true,false);
        belongingtoPlanetScript.AddResourceTransferOrder(buildStationMeterialOrder);
        orderPlaced = true;
    }

    public void ProcessBuilding()
    {
        if (ResourceStorageCelestialBody.TryGetValue(ResourceType.SpacePoints, out var spacePointsStorage) && spacePointsStorage.StorageQuantity >= requiredResourceAmountStep)
        {
            constructionProgress += progressIncrement;
            spacePointsStorage.StorageQuantity -= requiredResourceAmountStep;
            spriteMask.transform.localScale = new Vector3(constructionProgress / 100f, constructionProgress / 100f, 1);
        } else if (!belongingtoPlanetScript.ResourceTransferOrders.Contains(buildStationMeterialOrder) && !orderPlaced)
        {
            //belongingtoPlanetScript.ResourceTransferOrders.Add(buildStationMeterialOrder);
            orderPlaced = true;
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
        int spaceShipTransporterAvailable = hangarManager.GetSpaceFleetCount(true) + hangarManager.GetSpaceFleetCount(false);


        // Farm, Mine, and Reactor info
        string areaInfo = $"{Symbols.Area} {DevelopedAreas}/{maxAreas}";
        string farmInfo = $"{Symbols.Farm} {farms}/{farmsInConstruction}";
        string mineInfo = $"{Symbols.Mine} {mines}/{minesInConstruction}";
        string reactorInfo = $"{Symbols.Reactor} {reactors}/{reactorsInConstruction}";
        string spaceportInfo = $"{Symbols.Spaceport} {spaceports}/{spaceportsInConstruction}";

        // Resource Info
        string resourceStorageFood = $"{Symbols.Food} {ResourceStorageCelestialBody[ResourceType.Food].StorageQuantity}";
        string resourceStorageMetal = $"{Symbols.Metal} {ResourceStorageCelestialBody[ResourceType.Metal].StorageQuantity}";
        string resourceStorageEnergy = $"{Symbols.Energy} {ResourceStorageCelestialBody[ResourceType.Energy].StorageQuantity}";
        string resourceStorageSpacePoint = $"{Symbols.SpacePoint} {ResourceStorageCelestialBody[ResourceType.SpacePoints].StorageQuantity}";
        string SpaceShipsAvailable = $"{Symbols.SpaceShip} {spaceShipTransporterAvailable}";


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
        sb.AppendLine($"{SpaceShipsAvailable}");

        finalString = sb.ToString();
        celestialBodyInfoRight.SetText(finalString);
    }
}
