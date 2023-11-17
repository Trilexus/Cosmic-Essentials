using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class OrderMenu : MonoBehaviour
{
    ResourceTransferDispatcher orderDispatcher;
    TextMeshProUGUI CostsInfoText;
    private HangarSlot hangarSlot;
    private List<HangarSlot> hangarSlots;
    [SerializeField]
    public GameObject AvailableTransportPanel;
    public GameObject AvailableTransporterForOrders;
    SpacefleetScriptableObject ActiveSpacefleetScriptableObject;


    private void Awake()
    {
         
    }
    // Start is called before the first frame update
    void Start()
    {
        orderDispatcher = new ResourceTransferDispatcher();
        CostsInfoText = GUIManager.Instance.orderCostsText;
        GUIManager.Instance.orderAmountSlider.maxValue = SpaceShip.maxStorage;
        EntityManager.Instance.OnSpacefleetChanged += UpdateMenuForAvailableSpaceFleet;
        //GUIManager.Instance.selectedCelestialBodyScript.SubscribeToHangarChanges(HangarChanged);
    }
    public void Update()
    {
        
    }

    public void UpdateMenuForAvailableSpaceFleet(List<SpacefleetScriptableObject> spacefleetScriptableObjects)
    {
        CelestialBody celestialBody =  GUIManager.Instance.selectedCelestialBodyScript;
        List<SpacefleetScriptableObject> spacefleetScriptableObjectsOnCelestialBody = EntityManager.Instance.GetAllSpacefleetScriptableObjectByTypes(SpacefleetType.SpaceShipTransporter);
        ActiveSpacefleetScriptableObject = spacefleetScriptableObjectsOnCelestialBody[0];
        foreach (SpacefleetScriptableObject spacefleetScriptableObject in spacefleetScriptableObjectsOnCelestialBody)
        {
            GameObject newSpacefleetMenuEntry = Instantiate(AvailableTransporterForOrders, AvailableTransportPanel.transform);
            newSpacefleetMenuEntry.transform.SetParent(AvailableTransportPanel.transform);
            Button button = newSpacefleetMenuEntry.GetComponent<Button>();
            TMP_Text text = newSpacefleetMenuEntry.GetComponentInChildren<TMP_Text>();
            AvailableTransporterButton availableTransporterButton = newSpacefleetMenuEntry.GetComponent<AvailableTransporterButton>();
            availableTransporterButton.spacefleetScriptableObject = spacefleetScriptableObject;
            text.text = Symbols.SpaceShip + " " + spacefleetScriptableObject.Name + "(" + spacefleetScriptableObject.MaxCargoSpace+")";
        }
        //dropdown.ClearOptions();
        //dropdown.AddOptions(spacefleetScriptableObjectsOnCelestialBody.ConvertAll(x => x.Name));
    }

    public void ChangeSpaceshipSettings(SpacefleetScriptableObject spacefleetScriptableObject)
    {
        GUIManager.Instance.orderAmountSlider.maxValue = spacefleetScriptableObject.MaxCargoSpace;
        GUIManager.Instance.orderAmountSlider.value = spacefleetScriptableObject.MaxCargoSpace;      
        ActiveSpacefleetScriptableObject = spacefleetScriptableObject;
    }

    public void UpdateCostsText()
    {
        int costs = ActiveSpacefleetScriptableObject.StartSpacePointsCosts;
        if (GUIManager.Instance.ToggleReturnToOrigin.isOn)
        {
            costs = ActiveSpacefleetScriptableObject.StartSpacePointsCosts * 2;
            CostsInfoText.text = costs.ToString();
        } else
        {
            CostsInfoText.text = costs.ToString();
        }
    }



    public void OnSliderChanged(float number)
    {
        if (GUIManager.Instance.orderAmountInputField.text != number.ToString())
        {
            GUIManager.Instance.orderAmountInputField.text = number.ToString();
        }
    }

    public void OnFieldChanged(string text)
    {
        if (GUIManager.Instance.orderAmountSlider.value.ToString() != text)
        {
            if (float.TryParse(text, out float number))
            {
                GUIManager.Instance.orderAmountSlider.value = number;
            }
        }
    }

    public void CreateOrder()
    {
        string ResourceType = GUIManager.Instance.orderTypeDropdown.options[GUIManager.Instance.orderTypeDropdown.value].text;
        int ResourceAmount = (int)GUIManager.Instance.orderAmountSlider.value;
        int repetitions = GUIManager.Instance.InputFieldOrderRepetitions.text == "" ? 1 : int.Parse(GUIManager.Instance.InputFieldOrderRepetitions.text);
        CelestialBody origin = GUIManager.Instance.selectedCelestialBody.GetComponent<CelestialBody>();
        CelestialBody destination = GUIManager.Instance.selectedCelestialBodyTarget.GetComponent<CelestialBody>();
        bool isPrioritized = GUIManager.Instance.ToggleIsPrioritized.isOn;
        bool onlyFullShipment = GUIManager.Instance.ToggleOnlyFullShipment.isOn;
        bool ReturnToOrigin = GUIManager.Instance.ToggleReturnToOrigin.isOn;
        bool isForever = GUIManager.Instance.ToggleIsForever.isOn;

        ResourceTransferOrder order = orderDispatcher.CreateOrderFromGui(ResourceType, ResourceAmount, origin, destination, repetitions, isPrioritized, onlyFullShipment, ReturnToOrigin, isForever);
        orderDispatcher.CreateOrderOnCelestialBody(order);  
    }

    public void OnCheckboxIsForeverValueChanged(bool isChecked)
    {
        GUIManager.Instance.InputFieldOrderRepetitions.interactable = !isChecked;
    }
}
