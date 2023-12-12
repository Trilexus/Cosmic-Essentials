using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpacefleetMenuEntry : MenuEntry
{

    public SpacefleetScriptableObject spaceFleetData; // Dein ScriptableObject
    string color;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetBuildableColor();
        if (spaceFleetData.Type == SpacefleetType.SpaceShipTransporter)
        {
            UpdateSpaceShipTransporterCounter();
        } else if (spaceFleetData.Type == SpacefleetType.SpaceStation)
        {
            UpdateSpaceStationCounter();
        }
    }



    internal void GetAndSetSpaceFleetInfoTextAndImage()
    {
        Image.sprite = spaceFleetData.Sprite;
        BuildingName.text = spaceFleetData.Name;
        sb.Clear();
        string resourceSummary = "";
        foreach (ResourceScriptableObject cost in spaceFleetData.Costs)
        {
            string symbol = Symbols.GetSymbol(cost.ResourceType);
            string nonbreakingSpace = "\u00A0";
            string quantity = $"{cost.Quantity}";
            resourceSummary += $"{symbol}{nonbreakingSpace}{quantity} ";
        }
        sb.AppendLine($"{resourceSummary}");
        string livingSpace = $"{Symbols.population} {spaceFleetData.LivingSpace}";
        sb.AppendLine($"{livingSpace}");
        string cargoSpace = $"{Symbols.cargoBox} {spaceFleetData.CargoSpace}";
        sb.AppendLine($"{cargoSpace}");
        BuildingInfosCosts.text = sb.ToString();
        sb.Clear();
        string requirements = GetStructureRequirementsString();
        BuildingInfosThroughput.text = requirements;
    }

    public void SetBuildableColor()
    {
        if (IsBuildable(spaceFleetData.StructureRequirements))
        {
            Image.color = Color.white;
        }
        else
        {
            Image.color = Color.red;
        }
    }

    public string GetStructureRequirementsString()
    {
        sb.Clear();
            foreach (StructureScriptableObject structureRequirement in spaceFleetData.StructureRequirements)
        {
            sb.AppendLine(structureRequirement.Name);            
        }
        return sb.ToString();
    }

    public void UpdateSpaceShipTransporterCounter()
    {
        CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBodyScript;
        int countReady = celestialBody.PerformHangarOperation(manager => manager.GetSpaceFleetCount(spaceFleetData, true, false));
        countReady += celestialBody.PerformHangarOperation(manager => manager.GetSpaceFleetCount(spaceFleetData, true, true));
        int countInConstruction = celestialBody.PerformHangarOperation(manager => manager.GetSpaceFleetCount(spaceFleetData, false,true));
        countInConstruction += celestialBody.PerformHangarOperation(manager => manager.GetSpaceFleetCount(spaceFleetData, false, false));
        BuildingCounter.text = countReady + "/" + countInConstruction;
    }

    private void UpdateSpaceStationCounter()
    {
        int count = GUIManager.Instance.selectedPlanetarySystemScript.celestialBodies.Where(I => I is SpaceStation  && I.isActiveAndEnabled).Count();
        BuildingCounter.text = count.ToString();

    }
}