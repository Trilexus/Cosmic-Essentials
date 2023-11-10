using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpacefleetMenuEntry : MonoBehaviour
{

    public SpacefleetScriptableObject spaceFleetData; // Dein ScriptableObject
    public Image Image;
    public TextMeshProUGUI BuildingCounter;
    public TextMeshProUGUI BuildingName;
    public TextMeshProUGUI BuildingInfosCosts;
    public TextMeshProUGUI BuildingInfosThroughput;
    public StringBuilder sb = new StringBuilder();
    [SerializeField]
    GameObject BuildProgress;
    [SerializeField]
    GameObject BuildProgressItem;
    string color;
    int OneHundredPercent = 100;

    int lastunderConstructionBuildingCount = 0;
    int underConstructionBuildingCount = 0;

    internal void GetAndSetSpaceFleetInfoTextAndImage()
    {
        Image.sprite = spaceFleetData.Sprite;
        sb.Clear();
        string resourceSummary = "";
        foreach (Resource cost in spaceFleetData.Costs)
        {
            string symbol = Symbols.GetSymbol(cost.ResourceType);
            string nonbreakingSpace = "\u00A0";
            string quantity = $"{cost.Quantity}";
            resourceSummary += $" {symbol}{nonbreakingSpace}{quantity} ";
        }
        sb.Append($"{resourceSummary}");
        string livingSpace = $" {Symbols.Apartments} {spaceFleetData.LivingSpace}";
        sb.AppendLine($"{livingSpace}");
        BuildingInfosCosts.text = sb.ToString();
        sb.Clear();

        BuildingName.text = spaceFleetData.Name;
        BuildingInfosThroughput.text = sb.ToString();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


