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
    public int ecoIndex = 100;
    protected float minEcoIndex = 0;
    protected float maxEcoIndex = 100;
    public int ecoIndexChangeValue = 0;

    const int RED_THRESHOLD = 30;
    const int YELLOW_THRESHOLD = 70;
    public int freeAreaImpactFactor;

    public int sumOfNegativeEcoImpactFactorsForStructures;
    public int sumOfPositiveEcoImpactFactorsForStructures;
    public int sumOfEcoImpactFactorsForPopulation;
    public int sumOfEcoImpactFactorsForFreeAreas;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        //celestialBodyInfo = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        SetupPopulation();
        CalculateEcoIndex();
        //CalculatePopulation();
        CalculateProductivityRate();
        UpdateInfoText();
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!isPaused)
        {
            UpdateInfoText();
        }
    }




    protected override void Tick()
    {
        if (!isPaused)
        {
            CalculateEcoIndex();
            CalculateProductivityRate();
            base.Tick();
            CalculatePopulation();
            UpdateInfoText();
        }
    }
    public void CalculateProductivityRate()
    {
        // Mehr Einwohner pro area (Geb�ude) = bessere ProductivityRate
        float ProductivityPopulationFactor = (float)population.CurrentPopulation / Areas.Count;
        ProductivityPopulationFactor = Mathf.Clamp(ProductivityPopulationFactor, MinProductivityRate, MaxProductivityRate);
        if (population.CurrentPopulation > population._maxPopulation)
        {
            ProductivityPopulationFactor = -0.5f; //Too little living space has an impact on efficiency.
        }else if(population.CurrentPopulation <= 0)
        {
            ProductivityPopulationFactor = -1;
        }

        // Besserer EcoIndex = bessere ProductivityRate
        float ProductivityEcoIndexFactor = ecoIndex / 100f;//TODO: Magic Number
        if (ecoIndex < RED_THRESHOLD)
        {
            ProductivityEcoIndexFactor = -1;
        }else if (ecoIndex < YELLOW_THRESHOLD)
        {
            ProductivityEcoIndexFactor = -0.5f;
        } else
        {
            ProductivityEcoIndexFactor = 1f;       
        }

        // Zu wenig Ressourcen = schlechtere ProductivityRate. Ausreichende Ressourcen = normale ProductivityRate
        float ProductivityResourceFactor1 = (ResourceStorageCelestialBody[ResourceType.Metal].StorageQuantity < 0 ? 0f : 0.5f); //TODO: Magic Number
        float ProductivityResourceFactor2 = (ResourceStorageCelestialBody[ResourceType.Energy].StorageQuantity < 0 ? 0f : 0.5f); //TODO: Magic Number
        float ProductivityresourceFactor = ProductivityResourceFactor1 + ProductivityResourceFactor2;

        float factorCount = 2;
        ProductivityRate = (ProductivityPopulationFactor + ProductivityEcoIndexFactor + ProductivityresourceFactor) / factorCount;

        ProductivityRate = (float)Math.Round(Mathf.Clamp(ProductivityRate, MinProductivityRate, MaxProductivityRate),2,MidpointRounding.AwayFromZero) ;

    }

    //UpdateInfoText is called from Update() in CelestialBody and is used to update the text in the child gameobjetc CelestialBodyInfos
    public override void UpdateInfoText()
    {
        int farms = Areas.Count(I => I.structure.Name == "Farm" && I.ConstructionProgress >= OneHundredPercent);
        int mines = Areas.Count(I => I.structure.Name == "Mine" && I.ConstructionProgress >= OneHundredPercent);
        int reactors = Areas.Count(I => I.structure.Name == "Reactor" && I.ConstructionProgress >= OneHundredPercent);
        int spaceports = Areas.Count(I => I.structure.Name == "Spaceport" && I.ConstructionProgress >= OneHundredPercent);
        int apartments = Areas.Count(I => I.structure.Name == "Apartments" && I.ConstructionProgress >= OneHundredPercent);
        int farmsInConstruction = Areas.Count(I => I.structure.Name == "Farm" && I.ConstructionProgress < OneHundredPercent);
        int minesInConstruction = Areas.Count(I => I.structure.Name == "Mine" && I.ConstructionProgress < OneHundredPercent);
        int reactorsInConstruction = Areas.Count(I => I.structure.Name == "Reactor" && I.ConstructionProgress < OneHundredPercent);
        int spaceportsInConstruction = Areas.Count(I => I.structure.Name == "Spaceport" && I.ConstructionProgress < OneHundredPercent);
        int apartmentsInConstruction = Areas.Count(I => I.structure.Name == "Apartments" && I.ConstructionProgress < OneHundredPercent);
        string ecoColor = CalculateEcoIndexColor();        
        int DevelopedAreas = Areas.Count();
        sb.Clear();
        
        // Eco Index and Population and productivity rate info
        string ecoInfo = $"{ecoColor}\uf06c: {ecoIndex}%({ecoIndexChangeValue:+0.##;-0.##;0})</color>";
        string populationInfo = $"\ue533: {population.CurrentPopulation} / {population._maxPopulation}";
        string productivityInfo = $"\uf201: {ProductivityRate}";
        int spaceShipTransporterAvailable = hangarManager.GetSpaceFleetCount(true) + hangarManager.GetSpaceFleetCount(false);
        string SpaceShipsAvailable = $"{Symbols.SpaceShip} {spaceShipTransporterAvailable}";


        // Farm, Mine, and Reactor info
        string areaInfo = $"{Symbols.Area} {DevelopedAreas}/{maxAreas}";
        string farmInfo = $"{Symbols.Farm} {farms}/{farmsInConstruction}";
        string mineInfo = $"{Symbols.Mine} {mines}/{minesInConstruction}";
        string reactorInfo = $"{Symbols.Reactor} {reactors}/{reactorsInConstruction}";
        string spaceportInfo = $"{Symbols.Spaceport} {spaceports}/{spaceportsInConstruction}";
        string apartmentsInfo = $"{Symbols.Apartments} {apartments}/{apartmentsInConstruction}";

        

        sb.AppendLine($"{areaInfo} {apartmentsInfo} {populationInfo}");
        sb.AppendLine($"{ecoInfo} {productivityInfo}");
        string finalString = sb.ToString();
        celestialBodyInfoTop.SetText(finalString);

        sb.Clear();
        sb.AppendLine($"{farmInfo} {CollectResourceInfos(ResourceType.Food)}");
        sb.AppendLine($"{mineInfo} {CollectResourceInfos(ResourceType.Metal)}");
        sb.AppendLine($"{reactorInfo} {CollectResourceInfos(ResourceType.Energy)}");
        sb.AppendLine($"{spaceportInfo} {CollectResourceInfos(ResourceType.SpacePoints)}");


        sb.AppendLine($"{SpaceShipsAvailable}");

        finalString = sb.ToString();
        celestialBodyInfoRight.SetText(finalString);
    }

    public string CollectResourceInfos(ResourceType resourceType)
    {
        string resourceSymbol = $"{Symbols.GetSymbol(resourceType)}";
        int resourceStorage = ResourceStorageCelestialBody[resourceType].StorageQuantity;
        int resourceProduction = ResourceStorageCelestialBody[resourceType].ProductionQuantity;
        int resourceConsumption = ResourceStorageCelestialBody[resourceType].ConsumptionQuantity;
        string returnString = $"{Symbols.GetSymbol(resourceType)} {resourceStorage}({resourceProduction})({resourceConsumption})";
        if (resourceStorage == 0 && resourceProduction == 0 && resourceConsumption == 0)
        {
            returnString = "";
        }
        return returnString;
    }

    private void CalculateEcoIndex()
    {
        // Most structures have a negative impact on the ecoIndex
        sumOfNegativeEcoImpactFactorsForStructures = Areas.Where(area => area.structure.EcoImpactFactor < 0).Sum(area => area.structure.EcoImpactFactor);
        // Some buildings have a positive impact on the ecoIndex
        sumOfPositiveEcoImpactFactorsForStructures = Areas.Where(area => area.ConstructionProgress >= 100 && area.structure.EcoImpactFactor > 0).Sum(area => area.structure.EcoImpactFactor);
        // Population has a negative impact on the ecoIndex
        sumOfEcoImpactFactorsForPopulation = population.EcoImpactFactor;
        // Free areas have a positive impact on the ecoIndex
        sumOfEcoImpactFactorsForFreeAreas = (maxAreas - Areas.Count()) * freeAreaImpactFactor;
        //int 
        ecoIndexChangeValue = sumOfEcoImpactFactorsForFreeAreas + sumOfNegativeEcoImpactFactorsForStructures + sumOfPositiveEcoImpactFactorsForStructures + sumOfEcoImpactFactorsForPopulation;

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

    public string CalculateEcoIndexColor()
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

    public int CalcMaxPopulation()
    {
        int maxPopulation = 0;
        foreach(var apartments in Areas)
        {
            if (apartments.ConstructionProgress >= 100)
            {
                maxPopulation += apartments.structure.LivingSpace;
            }
        }
        return maxPopulation;
    }

    private void SetupPopulation()
    {        
        population.RED_THRESHOLD = RED_THRESHOLD;
        population._maxPopulation = CalcMaxPopulation();
    }
    public void CalculatePopulation()
    {
        population.UpdatePopulation(ResourceStorageCelestialBody[0].StorageQuantity, ecoIndex, CalcMaxPopulation());
    }

}
