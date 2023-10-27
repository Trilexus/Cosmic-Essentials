using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.SocialPlatforms;
using UnityEngine.Android;

public class Planet : CelestialBody
{
    [SerializeField]
    protected int ecoIndex = 100;
    protected float minEcoIndex = 0;
    protected float maxEcoIndex = 100;
    int ecoIndexChangeValue = 0;

    const int RED_THRESHOLD = 30;
    const int YELLOW_THRESHOLD = 70;
    const int FREE_AREA_ECOIMPACTFACTOR = 5;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        //celestialBodyInfo = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        SetupPopulation();
        CalculateEcoIndex();
        UpdateInfoText();
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateInfoText();
    }

    protected override void Tick()
    {
        CalculateProductivityRate();
        base.Tick();
        CalculateEcoIndex();
        CalculatePopulation();
        UpdateInfoText();
    }
    public void CalculateProductivityRate()
    {
        // Mehr Einwohner pro Area = bessere ProductivityRate
        float ProductivityPopulationFactor = (float)population.CurrentPopulation / Areas.Count;
        ProductivityPopulationFactor = Mathf.Clamp(ProductivityPopulationFactor, MinProductivityRate, MaxProductivityRate);
        // Besserer EcoIndex = bessere ProductivityRate
        float ProductivityEcoIndexFactor = ecoIndex / 50;
        // Zu wenig Ressourcen = schlechtere ProductivityRate. Ausreichende Ressourcen = normale ProductivityRate
        float ProductivityresourceFactor1 = (ResourceStorageCelestialBody[ResourceType.Metal].StorageQuantity < 0 ? 0.15f : 0.5f); //TODO: Magic Number
        float ProductivityresourceFactor2 = (ResourceStorageCelestialBody[ResourceType.Energy].StorageQuantity < 0 ? 0.15f : 0.5f); //TODO: Magic Number
        float ProductivityresourceFactor = 1 + ProductivityresourceFactor1 + ProductivityresourceFactor2;

        int factorCount = 3;
        ProductivityRate = (ProductivityPopulationFactor + ProductivityEcoIndexFactor + ProductivityresourceFactor) / factorCount;
        ProductivityRate = (float)Math.Round(Mathf.Clamp(ProductivityRate, MinProductivityRate, MaxProductivityRate),2,MidpointRounding.AwayFromZero) ;        
    }

    //UpdateInfoText is called from Update() in CelestialBody and is used to update the text in the child gameobjetc CelestialBodyInfos
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
        string ecoColor = CalculateEcoIndexColor();        
        int DevelopedAreas = Areas.Count();
        sb.Clear();
        
        // Eco Index and Population and productivity rate info
        string ecoInfo = $"{ecoColor}\uf06c: {ecoIndex}%({ecoIndexChangeValue:+0.##;-0.##;0})</color>";
        string populationInfo = $"\ue533: {population.CurrentPopulation}";
        string productivityInfo = $"\uf201: {ProductivityRate}";


        // Farm, Mine, and Reactor info
        string areaInfo = $"{Symbols.AreaSymbol} {DevelopedAreas}/{maxAreas}";
        string farmInfo = $"{Symbols.FarmSymbol} {farms}/{farmsInConstruction}";
        string mineInfo = $"{Symbols.MineSymbol} {mines}/{minesInConstruction}";
        string reactorInfo = $"{Symbols.ReactorSymbol} {reactors}/{reactorsInConstruction}";
        string spaceportInfo = $"{Symbols.SpaceportSymbol} {spaceports}/{spaceportsInConstruction}";

        // Resource Info
        string resourceStorageFood = $"{Symbols.FoodSymbol} {ResourceStorageCelestialBody[ResourceType.Food].StorageQuantity}";
        string resourceStorageMetal = $"{Symbols.MetalSymbol} {ResourceStorageCelestialBody[ResourceType.Metal].StorageQuantity}";
        string resourceStorageEnergy = $"{Symbols.EnergySymbol} {ResourceStorageCelestialBody[ResourceType.Energy].StorageQuantity}";
        string resourceStorageSpacePoint = $"{Symbols.SpacePointSymbol} {ResourceStorageCelestialBody[ResourceType.SpacePoints].StorageQuantity}";
        string SpaceShipsAvailable = $"{Symbols.SpaceShipSymbol} {SpaceShipTransporterAvailable}";

        string resourceFoodProduction = $"({ResourceStorageCelestialBody[ResourceType.Food].ProductionQuantity})";
        string resourceMetalProduction = $"({ResourceStorageCelestialBody[ResourceType.Metal].ProductionQuantity})";
        string resourceEnergyProduction = $"({ResourceStorageCelestialBody[ResourceType.Energy].ProductionQuantity})";
        string resourceSpacePointProduction = $"({ResourceStorageCelestialBody[ResourceType.SpacePoints].ProductionQuantity})";
        
        string resourceFoodConsumption = $"({ResourceStorageCelestialBody[0].ConsumptionQuantity})";
        string resourceMetalConsumption = $"({ResourceStorageCelestialBody[ResourceType.Metal].ConsumptionQuantity})";
        string resourceEnergyConsumption = $"({ResourceStorageCelestialBody[ResourceType.Energy].ConsumptionQuantity})";
        string resourceSpacePointConsumption = $"({ResourceStorageCelestialBody[ResourceType.SpacePoints].ConsumptionQuantity})";

        sb.AppendLine($"{areaInfo} {populationInfo}");
        sb.AppendLine($"{ecoInfo} {productivityInfo}");
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

    private void CalculateEcoIndex()
    {
        // Most structures have a negative impact on the ecoIndex
        int sumOfEcoImpactFactorsForStructures = Areas.Sum(area => area.structure.EcoImpactFactor);
        // Population has a negative impact on the ecoIndex
        int sumOfEcoImpactFactorsForPopulation = population.EcoImpactFactor;
        // Free areas have a positive impact on the ecoIndex
        int sumOfEcoImpactFactorsForFreeAreas = (maxAreas - Areas.Count()) * FREE_AREA_ECOIMPACTFACTOR;
        ecoIndexChangeValue = sumOfEcoImpactFactorsForFreeAreas + sumOfEcoImpactFactorsForStructures + sumOfEcoImpactFactorsForPopulation;

        // If EcoIndex is in the yellow or red range, there is a bonus to allow Eco Index to settle into a range
        if (ecoIndex < YELLOW_THRESHOLD)
        {
            int reducerDampingFactor = 5; // Used to moderate the impact of the dampingFactor
            int dampingFactor = (YELLOW_THRESHOLD - ecoIndex) / reducerDampingFactor;
            ecoIndexChangeValue += dampingFactor; // Ensure ecoIndexChangeValue is not negative
            //ecoIndexChangeValue = Math.Clamp(ecoIndexChangeValue, 0, YELLOW_THRESHOLD);
        }
        ecoIndex += ecoIndexChangeValue;
        ecoIndex = (int)Math.Clamp(ecoIndex, minEcoIndex, maxEcoIndex);

    }

    private string CalculateEcoIndexColor()
    {
          if (ecoIndex < RED_THRESHOLD)
        {
            return "<color=\"red\">";
        }
        else if (ecoIndex < YELLOW_THRESHOLD)
        {
            return "<color=\"yellow\">";
        }
        else
        {
            return "<color=\"green\">";
        }
    }

    private void SetupPopulation()
    {
        population._maxPopulation = maxAreas;
        population.RED_THRESHOLD = RED_THRESHOLD;
    }
    public void CalculatePopulation()
    {
        population.UpdatePopulation(ResourceStorageCelestialBody[0].StorageQuantity, ecoIndex);
    }

}
