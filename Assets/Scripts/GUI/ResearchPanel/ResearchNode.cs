using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using System;
using UnityEngine.EventSystems;

public class ResearchNode : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI CostText;
    public GameObject progressBar;

    public ResearchNodeScriptableObject researchNodeData;
    LocalizedString myLocalizedString = new LocalizedString("Research", "");
    string TranslatedValue => myLocalizedString.GetLocalizedString();




    public void Initialize()
    {
        image.sprite = researchNodeData.ResearchNodeSprite;
        SetTitleText(researchNodeData.ResearchNodeName);
        SetDescriptionText(researchNodeData.ResearchNodeDescription);
        CostText.text = researchNodeData.ResearchNodeCost.ToString();
    }


    public void SetTitleText(string message)
    {
        myLocalizedString = new LocalizedString("Research", message);
        TitleText.text = TranslatedValue;
    }
    public void SetDescriptionText(string message)
    {
        myLocalizedString = new LocalizedString("Research", message);
        DescriptionText.text = TranslatedValue;
    }
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
            UpdateProgressBar();
    }

    public void UpdateProgressBar()
    {
        progressBar.GetComponent<Slider>().value = researchNodeData.currentResearchProgress / (researchNodeData.ResearchNodeCost / 100f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on " + researchNodeData.ResearchNodeName);
        ResearchManager.Instance.SetActiveResearchNode(researchNodeData, gameObject);
        progressBar.SetActive(true);
    }
}
