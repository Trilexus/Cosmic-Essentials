using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Rendering;

public class TabCelestialBodyInfo : MonoBehaviour
{
    [SerializeField]
    GameObject BuffInfoEntryPrefab;
    [SerializeField]
    GameObject InfoMenuContent;
    [SerializeField]
    TextMeshProUGUI CBNameText;
    [SerializeField]
    Image CBImage;
    [SerializeField]
    TextMeshProUGUI CBMainInfosText_L;
    [SerializeField]
    TextMeshProUGUI CBMainInfosText_R;
    StringBuilder StringBuilder = new StringBuilder();
    CelestialBody LastCelestialBody;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateInfos(CelestialBody celestialBody)
    {
        SetInfoText(celestialBody);
    }

    public void SetInfoText(CelestialBody celestialBody)
    {
        if (celestialBody is Planet planet)
        {
            StringBuilder.Clear();
            StringBuilder.AppendLine($"{StyleSheet.H1}Planetary overview{StyleSheet.H1End}");
            StringBuilder.AppendLine($"{StyleSheet.BulletPoint}Celestial Body Type: {planet.AllowedLocation}{StyleSheet.BulletPointEnd}");
            StringBuilder.AppendLine($"{StyleSheet.BulletPoint}Building areas: {celestialBody.maxAreas}{StyleSheet.BulletPointEnd}");
            StringBuilder.AppendLine($"Each undeveloped building site generates {planet.freeAreaImpactFactor} EcoPoints.");
            StringBuilder.AppendLine($"{StyleSheet.H2}Storage Capacity{StyleSheet.H1End}");
            StringBuilder.AppendLine($"{StyleSheet.BulletPoint}Maximum {planet.CurrentResourceStorageLimit} units per resource{StyleSheet.BulletPointEnd}");
            StringBuilder.AppendLine($"{StyleSheet.H2}Population{StyleSheet.H1End}");
            StringBuilder.AppendLine($"{StyleSheet.BulletPoint}Current Inhabitants: {planet.population.CurrentPopulation}{StyleSheet.BulletPointEnd}");
            StringBuilder.AppendLine($"{StyleSheet.BulletPoint}Maximum Housing Capacity: {planet.population._maxPopulation}{StyleSheet.BulletPointEnd}");
            StringBuilder.AppendLine($"{StyleSheet.BulletPoint}Growth Percent: {planet.population.GrowthPercent}%{StyleSheet.BulletPointEnd}");

            CBMainInfosText_L.text = StringBuilder.ToString();
            StringBuilder.Clear();
            celestialBody.BaseResourceProduction.ForEach(resourceProduction =>
            {
                string quantity = resourceProduction.Quantity > 0 ? $"+{resourceProduction.Quantity}" : $"-{resourceProduction.Quantity}";
                StringBuilder.AppendLine($"{Symbols.GetSymbol(resourceProduction.ResourceType)}: {quantity}");
                CBMainInfosText_R.text = StringBuilder.ToString();
            });
        }
    }

    public void CreateMenuForCelestialBody(CelestialBody celestialBody)
    {
        LastCelestialBody = celestialBody;
        celestialBody.OnTickDone += UpdateInfos;
        CBNameText.text = celestialBody.Name;
        CBImage.sprite = celestialBody.ChildSpriteRenderer.sprite;
        
        SetInfoText(celestialBody);
        
        
        celestialBody.modifiers.ForEach(modifier =>
        {
        GameObject newInfoMenuEntry = Instantiate(BuffInfoEntryPrefab, InfoMenuContent.transform);
            switch (modifier)
            {
                case ModifierResourceProductionFactor modifierResourceProductionFactor:
                    newInfoMenuEntry.transform.SetParent(InfoMenuContent.transform);
                    newInfoMenuEntry.GetComponent<BuffEntry>().Initialize(modifierResourceProductionFactor);
                    break;
                case ModifierEcoFactor modifierEcoFactor:
                    newInfoMenuEntry.transform.SetParent(InfoMenuContent.transform);
                    newInfoMenuEntry.GetComponent<BuffEntry>().Initialize(modifierEcoFactor);
                    break;
                case ModifierProductivityFactor modifierProductivityFactor:
                    newInfoMenuEntry.transform.SetParent(InfoMenuContent.transform);
                    newInfoMenuEntry.GetComponent<BuffEntry>().Initialize(modifierProductivityFactor);
                    break;
            }            
        });

    }



    public void ClearMenu()
    {
        LastCelestialBody.OnTickDone -= UpdateInfos;
        foreach (Transform entry in InfoMenuContent.transform)
        {
            Destroy(entry.gameObject);
        }
        CBMainInfosText_L.text = "";
        CBNameText.text = "";
        CBMainInfosText_R.text = "";
    }
}