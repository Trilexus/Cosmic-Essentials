using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System;
using Unity.VisualScripting;

public class Planet : CelestialBody
{

    [SerializeField]
    protected float ecoIndex = 1;
    protected float minEcoIndex = 0;
    protected float maxEcoIndex = 1;
    float ecoIndexChangeValue = 0f;
    private StringBuilder sb = new StringBuilder();
    const float RED_THRESHOLD = 0.3f;
    const float YELLOW_THRESHOLD = 0.7f;
    const float MAX_ECOINDEX = 1f;
    const float FREE_AREA_ECOIMPACTFACTOR = 0.05f;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        celestialBodyInfo = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
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
        float ProductivityEcoIndexFactor = ecoIndex;
        // Zu wenig Ressourcen = schlechtere ProductivityRate. Ausreichende Ressourcen = normale ProductivityRate
        float ProductivityresourceFactor1 = (ResourceStorageCelestialBody[1].Quantity < 0 ? 0.15f : 0.5f);
        float ProductivityresourceFactor2 = (ResourceStorageCelestialBody[2].Quantity < 0 ? 0.15f : 0.5f);
        float ProductivityresourceFactor = 1 + ProductivityresourceFactor1 + ProductivityresourceFactor2;

        int factorCount = 3;
        ProductivityRate = (ProductivityPopulationFactor + ProductivityEcoIndexFactor + ProductivityresourceFactor) / factorCount;
        ProductivityRate = Mathf.Clamp(ProductivityRate, MinProductivityRate, MaxProductivityRate);        
    }

    //UpdateInfoText is called from Update() in CelestialBody and is used to update the text in the child gameobjetc CelestialBodyInfos
    public override void UpdateInfoText()
    {
        int farms = Areas.Count(I => I.structure.Name == "Farm" && I.constructionProgress >= 100);
        int mines = Areas.Count(I => I.structure.Name == "Mine" && I.constructionProgress >= 100);
        int reactors = Areas.Count(I => I.structure.Name == "Reactor" && I.constructionProgress >= 100);
        int farmInConstruction = Areas.Count(I => I.structure.Name == "Farm" && I.constructionProgress < 100);
        int minesInConstruction = Areas.Count(I => I.structure.Name == "Mine" && I.constructionProgress < 100);
        int reactorsInConstruction = Areas.Count(I => I.structure.Name == "Reactor" && I.constructionProgress < 100);
        string ecoColor = CalculateEcoIndexColor();
        int ecoIndexInt = (int)(ecoIndex * 100);
        int ecoIndexChangeValueInt = (int)(ecoIndexChangeValue * 100f);
        int DevelopedAreas = Areas.Count();
        sb.Clear();
        sb.AppendFormat("{7}\uf06c: {11}%({14:+0.##;-0.##;0})</color> \n\uf0ac: {13}/{12} \ue533: {6}\n \uf722: {0}/{1} \uf275: {2}/{3} \uf7ba: {4}/{5}\n\ue2cd: {8} \uf468: {9} \uf0e7: {10}", farms, farmInConstruction, mines, minesInConstruction, reactors, reactorsInConstruction, population.CurrentPopulation,ecoColor, ResourceStorageCelestialBody[0].Quantity, ResourceStorageCelestialBody[1].Quantity, ResourceStorageCelestialBody[2].Quantity, ecoIndexInt,maxAreas,DevelopedAreas, ecoIndexChangeValueInt);
        celestialBodyInfo.SetText(sb.ToString());
    }

    private void CalculateEcoIndex()
    {
        float sumOfEcoImpactFactorsForStructures = Areas.Sum(area => area.structure.EcoImpactFactor);
        float sumOfEcoImpactFactorsForFreeAreas = (maxAreas - Areas.Count()) * FREE_AREA_ECOIMPACTFACTOR;
        float sumOfEcoImpactFactorsForPopulation = population.EcoImpactFactor;
        ecoIndexChangeValue = sumOfEcoImpactFactorsForFreeAreas - (sumOfEcoImpactFactorsForStructures + sumOfEcoImpactFactorsForPopulation);    
        ecoIndexChangeValue = (float)Math.Round(ecoIndexChangeValue, 2, MidpointRounding.AwayFromZero);
        ecoIndex = ecoIndex + ecoIndexChangeValue;
        ecoIndex = Math.Clamp(ecoIndex,minEcoIndex,maxEcoIndex);
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
        population.UpdatePopulation(ResourceStorageCelestialBody[0].Quantity, ecoIndex);
    }

}
