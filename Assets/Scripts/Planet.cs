using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System;

public class Planet : CelestialBody
{
    [SerializeField]
    protected float ecoIndex = 1;
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
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateInfoText();
    }

    protected override void Tick()
    {
        base.Tick();
        CalculateEcoIndex();
        //CalculatePopulation();
        UpdateInfoText();
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
        //int farmInConstruction = Areas.Count(I => I.structure.Name == "Farm");
        //int minesInConstruction = Areas.Count(I => I.structure.Name == "Mine");
        //int reactorsInConstruction = Areas.Count(I => I.structure.Name == "Reactor");
        string ecoColor = CalculateEcoIndexColor();
        int ecoIndexInt = (int)(ecoIndex * 100);
        int ecoIndexChangeValueInt = (int)(ecoIndexChangeValue * 100);
        int DevelopedAreas = Areas.Count();
        sb.Clear();
        sb.AppendFormat("\uf0ac: {13}/{12} \ue533: {6} - {7}\uf06c: {11}%({14:+0.##;-0.##;0})</color> \n \uf722: {0}/{1} \uf275: {2}/{3} \uf7ba: {4}/{5}\n\ue2cd: {8} \uf468: {9} \uf0e7: {10}", farms, farmInConstruction, mines, minesInConstruction, reactors, reactorsInConstruction, population,ecoColor, ResourceStorage[0].Quantity, ResourceStorage[1].Quantity, ResourceStorage[2].Quantity, ecoIndexInt,maxAreas,DevelopedAreas, ecoIndexChangeValueInt);
        celestialBodyInfo.SetText(sb.ToString());
    }

    private void CalculateEcoIndex()
    {
        float sumOfEcoImpactFactors = Areas.Sum(area => area.structure.EcoImpactFactor);
        Debug.Log("sumOfEcoImpactFactors:" + sumOfEcoImpactFactors);
        float sumOfEcoImpactFactorsForFreeAreas = (maxAreas - Areas.Count()) * FREE_AREA_ECOIMPACTFACTOR;
        Debug.Log("sumOfEcoImpactFactorsForFreeAreas:" + sumOfEcoImpactFactorsForFreeAreas);
        ecoIndexChangeValue = sumOfEcoImpactFactorsForFreeAreas - sumOfEcoImpactFactors;
        ecoIndex = ecoIndex + ecoIndexChangeValue;
        ecoIndex = Math.Min(MAX_ECOINDEX, ecoIndex);
        Debug.Log(ecoIndex);
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

}
