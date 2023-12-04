using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

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


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateMenuForCelestialBody(CelestialBody celestialBody)
    {
        CBNameText.text = celestialBody.Name;
        CBImage.sprite = celestialBody.ChildSpriteRenderer.sprite;
        StringBuilder.Clear();
        StringBuilder.AppendLine($"Building areas({Symbols.Area}): {celestialBody.maxAreas}");
        if (celestialBody is Planet planet)
        {
            StringBuilder.AppendLine($"Celestial Body: {planet.AllowedLocation}<sprite name='bolt-solid'>");
            StringBuilder.AppendLine($"Each undeveloped building site ({Symbols.Area}) generates {planet.freeAreaImpactFactor} EcoPoints ({Symbols.ecoInfo})");
            StringBuilder.AppendLine($"Storage Capacity ({Symbols.box}): {planet.CurrentResourceStorageLimit} Units per Resource.");
            StringBuilder.AppendLine($"Current Inhabitants ({Symbols.population}): {planet.population.CurrentPopulation}; Maximum Housing Capacity ({Symbols.Apartments}): {planet.population._maxPopulation}. ");
            
            CBMainInfosText_L.text = StringBuilder.ToString();
        }
        celestialBody.BaseResourceProduction.ForEach(resourceProduction =>
        {
            StringBuilder.Clear();
            string quantity = resourceProduction.Quantity > 0 ? $"+{resourceProduction.Quantity}" : $"-{resourceProduction.Quantity}";
            StringBuilder.AppendLine($"{Symbols.GetSymbol(resourceProduction.ResourceType)}: {quantity}");
            CBMainInfosText_R.text += StringBuilder.ToString();
        });
        
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
        foreach (Transform entry in InfoMenuContent.transform)
        {
            Destroy(entry.gameObject);
        }
        CBMainInfosText_L.text = "";
        CBNameText.text = "";
        CBMainInfosText_R.text = "";
    }
}