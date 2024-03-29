using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    SpacefleetScriptableObject ActiveSpacefleetScriptableObject;
    [SerializeField]
    private Button AutoButton;
    private bool AutoMode = false;

    CelestialBody originCelestialBody;
    CelestialBody targetCelestialBody;


    public Sprite celestialBodyDefaultImage;
    public Image orderOriginImage;
    public Image orderTargetImage;
    public TMP_Dropdown orderTypeDropdown;
    public Slider orderAmountSlider;
    public TMP_InputField orderAmountInputField;
    public TMP_InputField InputFieldOrderRepetitions;
    public Button orderCreateButton;
    public Toggle ToggleIsPrioritized;
    public Toggle ToggleOnlyFullShipment;
    public Toggle ToggleReturnToOrigin;
    public Toggle ToggleIsForever;
    public TextMeshProUGUI orderCostsText;


    private void Awake()
    {
         
    }
    // Start is called before the first frame update
    void Start()
    {
        AutoButton.image.color = Color.green;
        orderDispatcher = new ResourceTransferDispatcher();
        CostsInfoText = GUIManager.Instance.orderCostsText;
        GUIManager.Instance.orderAmountSlider.maxValue = SpaceShip.CargoSpace;
        EntityManager.Instance.OnSpacefleetChanged += UpdateMenuForAvailableSpaceFleet;
        UpdateMenuForAvailableSpaceFleet(EntityManager.Instance.GetSpacefleetScriptableObjectForCelestialBody());
        ChangeSpaceshipSettings(ActiveSpacefleetScriptableObject, true);
        //GUIManager.Instance.selectedCelestialBodyScript.SubscribeToHangarChanges(HangarChanged);

        GUIManager.Instance.OnSelectedCelestialBodyChanged += SetOrigin;
        GUIManager.Instance.OnSelectedCelestialBodyTargetChanged += SetTarget;
        GUIManager.Instance.OnDeselectCelestialBody += UnsetOriginAndTarget;
    }
    public void Update()
    {
        
    }

    public void UpdateMenuForAvailableSpaceFleet(List<SpacefleetScriptableObject> spacefleetScriptableObjects)
    {
        //CelestialBody celestialBody =  GUIManager.Instance.selectedCelestialBodyScript;
        List<SpacefleetScriptableObject> spacefleetScriptableObjectsOnCelestialBody = EntityManager.Instance.GetAllSpacefleetScriptableObjectByTypes(SpacefleetType.SpaceShipTransporter);
        ActiveSpacefleetScriptableObject = spacefleetScriptableObjectsOnCelestialBody.Last();
        foreach (SpacefleetScriptableObject spacefleetScriptableObject in spacefleetScriptableObjectsOnCelestialBody)
        {
            GameObject newSpacefleetMenuEntry = Instantiate(AvailableTransporterForOrders, AvailableTransportPanel.transform);
            newSpacefleetMenuEntry.transform.SetParent(AvailableTransportPanel.transform);
            Button button = newSpacefleetMenuEntry.GetComponent<Button>();
            TMP_Text text = newSpacefleetMenuEntry.GetComponentInChildren<TMP_Text>();
            AvailableTransporterButton availableTransporterButton = newSpacefleetMenuEntry.GetComponent<AvailableTransporterButton>();
            availableTransporterButton.spacefleetScriptableObject = spacefleetScriptableObject;
            text.text = Symbols.SpaceShip + " " + spacefleetScriptableObject.Name + "\n("+ Symbols.cargoBox + " " + spacefleetScriptableObject.CargoSpace +")";
        }
    }

    public void SetActiveButtonColor(GameObject buttonGameObject)
    {
          foreach (Transform child in AvailableTransportPanel.transform)
        {
            Button button = child.GetComponent<Button>();
            button.image.color = Color.white;
        }
        Button button1 = buttonGameObject.GetComponent<Button>();
        button1.image.color = Color.green;
    }


    public void ChangeSpaceshipSettings(SpacefleetScriptableObject spacefleetScriptableObject, bool auto)
    {
        GUIManager.Instance.orderAmountSlider.maxValue = spacefleetScriptableObject.CargoSpace;
        GUIManager.Instance.orderAmountSlider.value = spacefleetScriptableObject.CargoSpace;      
        ActiveSpacefleetScriptableObject = spacefleetScriptableObject;
        AutoMode = auto;
        Debug.Log("ChangeSpaceshipSettings - AutoMode :" + AutoMode);
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
        originCelestialBody = GUIManager.Instance.selectedCelestialBody.GetComponent<CelestialBody>();
        targetCelestialBody = GUIManager.Instance.selectedCelestialBodyTarget.GetComponent<CelestialBody>();
        bool isPrioritized = GUIManager.Instance.ToggleIsPrioritized.isOn;
        bool onlyFullShipment = GUIManager.Instance.ToggleOnlyFullShipment.isOn;
        bool ReturnToOrigin = GUIManager.Instance.ToggleReturnToOrigin.isOn;
        bool isForever = GUIManager.Instance.ToggleIsForever.isOn;
        ResourceTransferOrder order;
        Debug.Log("CreateOrder - AutoMode :" + AutoMode);
        if (AutoMode)
        {
            order = orderDispatcher.CreateOrderFromGui(ResourceType, ResourceAmount, originCelestialBody, targetCelestialBody, repetitions, isPrioritized, onlyFullShipment, ReturnToOrigin, isForever);
        }
        else
        {
            order = orderDispatcher.CreateOrderFromGui(ResourceType, ResourceAmount, originCelestialBody, targetCelestialBody, repetitions, isPrioritized, onlyFullShipment, ReturnToOrigin, isForever, ActiveSpacefleetScriptableObject);
        }
        orderDispatcher.CreateOrderOnCelestialBody(order);  
    }


    public void SetOrigin(CelestialBody origin)
    {
        originCelestialBody = origin;
        orderOriginImage.sprite = originCelestialBody.ChildSpriteRenderer.sprite;
    }
    public void SetTarget(CelestialBody target)
    {
        targetCelestialBody = target;
        orderTargetImage.sprite = targetCelestialBody.ChildSpriteRenderer.sprite;
        orderCreateButton.interactable = true;
    }

    public void UnsetOriginAndTarget()
    {
        orderCreateButton.interactable = false;
        originCelestialBody = null;
        targetCelestialBody = null;
        orderOriginImage.sprite = celestialBodyDefaultImage;
        orderTargetImage.sprite = celestialBodyDefaultImage;
    }

    public void OnCheckboxIsForeverValueChanged(bool isChecked)
    {
        GUIManager.Instance.InputFieldOrderRepetitions.interactable = !isChecked;
    }
}
