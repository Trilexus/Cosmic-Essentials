using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class OrderMenu : MonoBehaviour
{
    ResourceTransferDispatcher orderDispatcher;


    // Start is called before the first frame update
    void Start()
    {
        orderDispatcher = new ResourceTransferDispatcher();
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
        CelestialBody origin = GUIManager.Instance.selectedCelestialBody.GetComponent<CelestialBody>();
        CelestialBody destination = GUIManager.Instance.selectedCelestialBodyTarget.GetComponent<CelestialBody>();
        int repetitions = GUIManager.Instance.orderAmountInputField.text == "" ? 0 : int.Parse(GUIManager.Instance.orderAmountInputField.text);
        bool isPrioritized = GUIManager.Instance.ToggleIsPrioritized.isOn;
        bool onlyFullShipment = GUIManager.Instance.ToggleOnlyFullShipment.isOn;
        bool ReturnToOrigin = GUIManager.Instance.ToggleReturnToOrigin.isOn;

        ResourceTransferOrder order = orderDispatcher.CreateOrderFromGui(ResourceType, ResourceAmount, origin, destination, repetitions, isPrioritized, onlyFullShipment, ReturnToOrigin);
        orderDispatcher.CreateOrderOnCelestialBody(order);
    }
}
