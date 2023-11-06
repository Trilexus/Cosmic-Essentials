using JetBrains.Annotations;
using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

public class StructureMenuEntry : MonoBehaviour
{

    public StructureScriptableObject structureData; // Dein ScriptableObject
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
    // Start is called before the first frame update
    void Start()
    {
        GetAndSetStructureInfoTextAndImage();
    }

    // Update is called once per frame
    void Update()
    {
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
        lastunderConstructionBuildingCount = 0;
        underConstructionBuildingCount = 0;
        foreach (Transform child in BuildProgress.transform)
        {
            Destroy(child.gameObject);
        }   
    }

    public void UpdateInfoText()
    {
        List<Area> Areas = GUIManager.Instance.selectedCelestialBodyScript.Areas;
        int completedBuildingCount = Areas.Count(I => I.structure.Type == structureData.Type && I.constructionProgress >= OneHundredPercent);
        lastunderConstructionBuildingCount = underConstructionBuildingCount;
        underConstructionBuildingCount = Areas.Count(I => I.structure.Type == structureData.Type && I.constructionProgress < OneHundredPercent);
        if (lastunderConstructionBuildingCount != underConstructionBuildingCount) { CreateBuildPrograssItems(); }
        int buildingCount = completedBuildingCount + underConstructionBuildingCount;

        sb.Clear();
        sb.AppendLine($"{buildingCount}");
        BuildingCounter.text = sb.ToString();
    }

    public void CreateBuildPrograssItems()
    {
        if (BuildProgress.transform.childCount < underConstructionBuildingCount)
        {
            for (int i = BuildProgress.transform.childCount; i < underConstructionBuildingCount; i++)
            {
                GameObject go = Instantiate(BuildProgressItem, BuildProgress.transform);
                go.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else if (BuildProgress.transform.childCount > underConstructionBuildingCount)
        {
            for (int i = BuildProgress.transform.childCount; i > underConstructionBuildingCount; i--)
            {
                Destroy(BuildProgress.transform.GetChild(i - 1).gameObject);
            }
        }
    }

    public void GetAndSetStructureInfoTextAndImage()
    {
        Image.sprite = structureData.Sprite;
        sb.Clear();
        //sb.AppendLine($"BuildCosts:");
        foreach (Resource cost in structureData.Costs)
        {
            string symbol = Symbols.GetSymbol(cost.ResourceType);

            sb.Append($"{symbol} {cost.Quantity} ");
        }
        //sb.AppendLine($"Throughput:");
        BuildingInfosCosts.text = sb.ToString();
        sb.Clear();

        string livingSpace = $"{Symbols.Apartments} {structureData.LivingSpace}";


        sb.Append($"{livingSpace} ");
        string producedResource = "";
        string consumedResource = "";
        foreach (Resource resource in structureData.Resources)
        {
            
            color = CalculateResourceColor(resource.Quantity);
            string symbol = Symbols.GetSymbol(resource.ResourceType);
            string quantity = $"{color} {symbol} {resource.Quantity}</color>";
            if (resource.Quantity < 0)
            {
                consumedResource += $"{quantity} ";
            } else
            {
                producedResource += $"{quantity} ";
            }
            
        }
        sb.AppendLine($"{producedResource}");
        sb.AppendLine($"{consumedResource}");
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
}
