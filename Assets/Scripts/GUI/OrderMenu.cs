using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class OrderMenu : MonoBehaviour
{
    ResourceTransferDispatcher orderDispatcher;
    TextMeshProUGUI CostsInfoText;


    // Start is called before the first frame update
    void Start()
    {
        orderDispatcher = new ResourceTransferDispatcher();
        CostsInfoText = GUIManager.Instance.orderCostsText;
        GUIManager.Instance.orderAmountSlider.maxValue = SpaceShip.maxStorage;
    }
    public void Update()
    {
        
    }

    public void UpdateCostsText()
    {
        int costs = SpaceShip.SpaceShipStartSpacePointsCosts;
        if (GUIManager.Instance.ToggleReturnToOrigin.isOn)
        {
            costs = SpaceShip.SpaceShipStartSpacePointsCosts * 2;
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
