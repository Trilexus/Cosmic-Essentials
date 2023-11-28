using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class TopPanel : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI areasText;
    [SerializeField]
    TextMeshProUGUI populationText;
    [SerializeField]
    TextMeshProUGUI ecoText;

    [SerializeField]
    TextMeshProUGUI foodText;
    [SerializeField]
    TextMeshProUGUI metalText;
    [SerializeField]
    TextMeshProUGUI energyText;
    [SerializeField]
    TextMeshProUGUI spacePointsText;
    [SerializeField]
    TextMeshProUGUI researchPointsLocalText;
    [SerializeField]
    TextMeshProUGUI researchPointsGlobalText;
    CelestialBody selectedCelestialBodyScript;
    StringBuilder stringBuilder = new StringBuilder();

    // Start is called before the first frame update
    void Start()
    {
        ResetPanel();
    }

    public void ResetPanel()
    {
        areasText.text = Symbols.Area;
        populationText.text = Symbols.population;
        ecoText.text = Symbols.ecoInfo;

        foodText.text = Symbols.Food;
        metalText.text = Symbols.Metal;
        energyText.text = Symbols.Energy;
        spacePointsText.text = Symbols.SpacePoint;
    }

    public void UpdatePanelInfos(CelestialBody celestialBodyScript)
    {
        selectedCelestialBodyScript = celestialBodyScript;
        areasText.text = GetAreaInfoString();
        populationText.text = GetPopulationInfoString();
        ecoText.text = GetEcoInfoString();

        foodText.text = GetResourceInfoString(ResourceType.Food);
        metalText.text = GetResourceInfoString(ResourceType.Metal);
        energyText.text = GetResourceInfoString(ResourceType.Energy);
        spacePointsText.text = GetResourceInfoString(ResourceType.SpacePoints);
        researchPointsLocalText.text = GetResourceInfoString(ResourceType.ResearchPoints);
    }

    public string GetResourceInfoString(ResourceType resourceType)
    {
        if (resourceType != ResourceType.ResearchPoints)
        {
            string symbol = Symbols.GetSymbol(resourceType);
            string storage = (selectedCelestialBodyScript.GetResourceCount(resourceType, DataTypeResource.Storage)).ToString();
            string production = (selectedCelestialBodyScript.GetResourceCount(resourceType, DataTypeResource.Production)).ToString();
            string consumption = (selectedCelestialBodyScript.GetResourceCount(resourceType, DataTypeResource.Consumption)).ToString();
            stringBuilder.Clear();
            stringBuilder.Append($"{symbol}:{storage}({production}/{consumption})");
        }
        else
        {
            string symbol = Symbols.GetSymbol(resourceType);
            string storage = (selectedCelestialBodyScript.GetResourceCount(resourceType, DataTypeResource.Storage)).ToString();
            string production = (selectedCelestialBodyScript.GetResourceCount(resourceType, DataTypeResource.Production)).ToString();
            string consumption = (selectedCelestialBodyScript.GetResourceCount(resourceType, DataTypeResource.Consumption)).ToString();
            stringBuilder.Clear();
            stringBuilder.Append($"{symbol}:{production}/{consumption}");
        }
        
        return stringBuilder.ToString();
    }

    public string GetGlobalResourceInfoString()
    {
        string symbol = Symbols.GetSymbol(ResourceType.ResearchPoints);
        string totalResearchPoints = ResearchManager.Instance.ResearchPoints.ToString();
        stringBuilder.Clear();
        stringBuilder.Append($"{symbol}:{totalResearchPoints}");
        return stringBuilder.ToString();
    }

    public string GetAreaInfoString()
    {
        string symbol = Symbols.Area;
        string totalAreas = (selectedCelestialBodyScript.maxAreas).ToString();
        string developedAreas =  (selectedCelestialBodyScript.Areas.Count).ToString();
        stringBuilder.Clear();
        stringBuilder.Append($"{symbol}:{developedAreas}/{totalAreas}");
        return stringBuilder.ToString();
    }

    public string GetPopulationInfoString()
    {
        string symbol = Symbols.population;
        string currentPopulation = (selectedCelestialBodyScript.population.CurrentPopulation).ToString();
        string totalLivingSpace = (selectedCelestialBodyScript.population._maxPopulation).ToString();
        stringBuilder.Clear();
        stringBuilder.Append($"{symbol}:{currentPopulation}/{totalLivingSpace}");
        return stringBuilder.ToString();
    }

    public string GetEcoInfoString()
    {
        string ecoSymbol = Symbols.ecoInfo;
        stringBuilder.Clear();
        if (selectedCelestialBodyScript is Planet planet)
        {
            string ecoColor = planet.CalculateEcoIndexColor();
            ecoSymbol = $"{ecoColor}{Symbols.ecoInfo}:</color>";

            stringBuilder.Append($"{ecoSymbol} {(planet.ecoIndex).ToString()}%");
            stringBuilder.Append($"({planet.ecoIndexChangeValue:+0.##;-0.##;0})");
            stringBuilder.Append($"({Symbols.Area} <color=\"green\">{planet.sumOfEcoImpactFactorsForFreeAreas:+0.##;-0.##;0}</color>)");
            stringBuilder.Append($"({Symbols.population} <color=\"red\">{planet.sumOfEcoImpactFactorsForPopulation:+0.##;-0.##;0}</color>)");
            stringBuilder.Append($"({Symbols.buildings} <color=\"red\">{planet.sumOfNegativeEcoImpactFactorsForStructures:+0.##;-0.##;0}</color>/");
            stringBuilder.Append($"<color=\"green\">{planet.sumOfPositiveEcoImpactFactorsForStructures:+0.##;-0.##;0}</color>)");


        }
        else
        {
            stringBuilder.Append($"{ecoSymbol}:---");
        }
        return stringBuilder.ToString();
    }


    // Update is called once per frame
    void Update()
    {
        researchPointsGlobalText.text = GetGlobalResourceInfoString();

        if (GUIManager.Instance.selectedCelestialBody != null)
        {
            UpdatePanelInfos(selectedCelestialBodyScript);
        }
    }


}
