using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

public class StructureMenuEntry : MenuEntry
{

    public StructureScriptableObject structureData; // Dein ScriptableObject
    string color;

    // Start is called before the first frame update
    void Start()
    {

        GetAndSetStructureInfoTextAndImage();
    }

    // Update is called once per frame
    void Update()
    {
        SetBuildableColor();
        if (GUIManager.Instance.selectedCelestialBody != null)
        {
            UpdateInfoText();
        } else
        {
            ResetCounterToDefault();
        }
    }

    public void ResetCounterToDefault()
    {
        BuildingCounter.text = "---";
        lastunderConstructionCount = 0;
        underConstructionCount = 0;
        foreach (Transform child in BuildProgress.transform)
        {
            Destroy(child.gameObject);
        }   
    }

    public void UpdateInfoText()
    {
        List<Area> Areas = GUIManager.Instance.selectedCelestialBodyScript.Areas;
        int completedBuildingCount = Areas.Count(I => I.structure.Name == structureData.Name && I.ConstructionProgress >= OneHundredPercent);
        lastunderConstructionCount = underConstructionCount;
        underConstructionCount = Areas.Count(I => I.structure.Name == structureData.Name && I.ConstructionProgress < OneHundredPercent);
        if (lastunderConstructionCount != underConstructionCount) { CreateBuildProgressItems(); }
        int buildingCount = completedBuildingCount + underConstructionCount;

        sb.Clear();
        sb.AppendLine($"{buildingCount}");
        BuildingCounter.text = sb.ToString();
    }

    public void CreateBuildProgressItems()
    {
        if (BuildProgress.transform.childCount < underConstructionCount)
        {
            for (int i = BuildProgress.transform.childCount; i < underConstructionCount; i++)
            {
                GameObject go = Instantiate(BuildProgressItem, BuildProgress.transform);
                go.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else if (BuildProgress.transform.childCount > underConstructionCount)
        {
            for (int i = BuildProgress.transform.childCount; i > underConstructionCount; i--)
            {
                Destroy(BuildProgress.transform.GetChild(i - 1).gameObject);
            }
        }
    }

    public void GetAndSetStructureInfoTextAndImage()
    {
        Image.sprite = structureData.Sprite;
        sb.Clear();
        string resourceSummary = "";
        string nonbreakingSpace = "\u00A0";
        foreach (ResourceScriptableObject cost in structureData.Costs)
        {
            string symbol = Symbols.GetSymbol(cost.ResourceType);
            string quantity = $"{cost.Quantity}";
            resourceSummary += $"{symbol}{nonbreakingSpace}{quantity} ";
        }
        sb.Append($"{resourceSummary}");
        //sb.AppendLine($"Throughput:");
        sb.Append($"{Symbols.population}{nonbreakingSpace}{structureData.CostsPopulation}");
        BuildingInfosCosts.text = sb.ToString();
        sb.Clear();

        string livingSpace = $"{Symbols.Apartments}{nonbreakingSpace}{structureData.LivingSpace}";


        sb.Append($"{livingSpace} ");
        string producedResource = "";
        string consumedResource = "";
        foreach (ResourceScriptableObject resource in structureData.Resources)
        {
            
            color = CalculateResourceColor(resource.Quantity);
            string symbol = Symbols.GetSymbol(resource.ResourceType);
            string quantity = $"{color}{symbol} {resource.Quantity}</color>";
            if (resource.Quantity < 0)
            {
                consumedResource += $"{quantity} ";
            } else
            {
                producedResource += $"{quantity} ";
            }            
        }
        if (structureData.StorageCapacity > 0)
        {
            string storageCapacity = $"{Symbols.box} {structureData.StorageCapacity}";
            sb.Append($"{storageCapacity}");
        }
        color = CalculateResourceColor(structureData.EcoImpactFactor);
        string ecoFactor = $"{color}{Symbols.ecoInfo} {structureData.EcoImpactFactor}</color>";

        sb.AppendLine($"{producedResource}");
        sb.AppendLine($"{consumedResource}");
        sb.AppendLine($"{ecoFactor}");
        BuildingName.text = structureData.Name;
        BuildingInfosThroughput.text = sb.ToString();
    }

    private string CalculateResourceColor(int amount)
    {
        if (amount < 0)
        {
            return "<color=\"red\">";
        }else
        {
            return "<color=\"green\">";
        }
    }

    public void UpdateBuildingCounter(int count)
    {
        BuildingCounter.text = count.ToString();
    }

    public void SetBuildableColor()
    {
        if (IsBuildable(structureData.StructureRequirements))
        {
            Image.color = Color.white;
        }
        else
        {
            Image.color = Color.red;
        }
    }
}
