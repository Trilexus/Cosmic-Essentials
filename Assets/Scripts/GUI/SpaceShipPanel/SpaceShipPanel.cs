using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


public class SpaceShipPanel : MonoBehaviour
{
    
    bool SliderChanged = false;

    [System.Serializable]
    public class SliderInputFieldPair
    {
        public ResourceType ResourceType;
        public Slider ResourceSlider;
        public TMP_InputField ResourceInputField;
        
    }
    [SerializeField]
    Image SpaceShipImage;
    [SerializeField]
    Image TargetImage;
    [SerializeField]
    TextMeshProUGUI TargetName;
    [SerializeField]
    TextMeshProUGUI SpaceShipName;
    [SerializeField]
    TextMeshProUGUI SpaceShipDescription;
    private SpacefleetScriptableObject activeSpaceShip;
    [SerializeField]
    Button LaunchButton;

    [SerializeField]
    List <SliderInputFieldPair> ResourceSliderInputFieldPairs;
    [SerializeField]
    Dictionary<ResourceType, SliderInputFieldPair> ResourceSliderInputFieldPairsDict = new Dictionary<ResourceType, SliderInputFieldPair>();

    [SerializeField]
    SpacefleetScriptableObject SpacefleetScriptableObject;
    [SerializeField]
    CelestialBody CelestialBodyTarget;

    // Start is called before the first frame update
    void Start()
    {
        SpaceShipEntry.OnClickEvent += SetActiveSpaceShip;
        foreach (SliderInputFieldPair sliderInputFieldPair in ResourceSliderInputFieldPairs)
        {
            ResourceSliderInputFieldPairsDict.Add(sliderInputFieldPair.ResourceType, sliderInputFieldPair);
        }
        GUIManager.Instance.OnSelectedCelestialBodyTargetChanged += SetTarget;
        GUIManager.Instance.OnDeselectCelestialBody += ResetMenu;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activeSpaceShip != null && !LaunchButton.interactable)
        {
            ActivateGUI();
        } else if (LaunchButton.interactable)
        {
            DeActivateGUI();
        }
    }

    public void ResetMenu()
    {
        Debug.Log("ResetMenu");
        activeSpaceShip = null;
        SpaceShipImage.sprite = null;
        SpaceShipName.text = "";
        SpaceShipDescription.text = "";
        CelestialBodyTarget = null;
        TargetImage.sprite = null;
        TargetName.text = "";
        foreach (SliderInputFieldPair sliderInputFieldPair in ResourceSliderInputFieldPairs)
        {
            sliderInputFieldPair.ResourceSlider.value = 0;
            sliderInputFieldPair.ResourceInputField.text = "0";
        }
    }

    public void LaunchSpaceship()
    {
        CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBodyScript;

        if (activeSpaceShip == null) { return; }
        if (CelestialBodyTarget == null) { return; }
        SpaceShip spaceShip = celestialBody.PerformHangarOperation(hangar => hangar.GetSpaceShip(activeSpaceShip, true, false));
        ResourceTransferDispatcher resourceTransferDispatcher = new ResourceTransferDispatcher();
        resourceTransferDispatcher.LoadSpaceShip(spaceShip, activeSpaceShip.ResourceStorage);
        spaceShip.StartJourneyFlyToTarget(celestialBody, CelestialBodyTarget);
        ResetMenu();
    }

    public void SetActiveSpaceShip(SpacefleetScriptableObject spacefleetScriptableObject)
    {
        activeSpaceShip = spacefleetScriptableObject;
        SpaceShipImage.sprite = spacefleetScriptableObject.Sprite;
        SpaceShipName.text = spacefleetScriptableObject.Name;
        SpaceShipDescription.text = Symbols.fuel + spacefleetScriptableObject.Fuel + "/" + spacefleetScriptableObject.MaxFuel;
        SetMaxSliderInputValues(spacefleetScriptableObject.CargoSpace);
        foreach (ResourceStorage resourceStorrage in spacefleetScriptableObject.ResourceStorage.Values)
        {
            ResourceSliderInputFieldPairsDict[resourceStorrage.ResourceType].ResourceSlider.maxValue = spacefleetScriptableObject.CargoSpace;
            ResourceSliderInputFieldPairsDict[resourceStorrage.ResourceType].ResourceSlider.value = resourceStorrage.StorageQuantity;
            ResourceSliderInputFieldPairsDict[resourceStorrage.ResourceType].ResourceInputField.text = resourceStorrage.StorageQuantity.ToString();
        }
    }

    public void ActivateGUI()
    {
        foreach (SliderInputFieldPair sliderInputFieldPair in ResourceSliderInputFieldPairs)
        {
            sliderInputFieldPair.ResourceInputField.interactable = true;
            sliderInputFieldPair.ResourceSlider.interactable = true;
        }
        LaunchButton.interactable = true;
        
    }
    public void DeActivateGUI()
    {
        foreach (SliderInputFieldPair sliderInputFieldPair in ResourceSliderInputFieldPairs)
        {
            sliderInputFieldPair.ResourceInputField.interactable = false;
            sliderInputFieldPair.ResourceSlider.interactable = false;
        }
        LaunchButton.interactable = false;

    }

    public void SetTarget(CelestialBody celestialBodyTarget)
    { 
        //ResetMenu();
        if (celestialBodyTarget == null)
        {            
            return;
        }
        Debug.Log("SetTarget");
        CelestialBodyTarget = celestialBodyTarget;
        TargetImage.sprite = CelestialBodyTarget.ChildSpriteRenderer.sprite;
        TargetName.text = CelestialBodyTarget.Name;
    }

    public void SliderChangedSetField(Slider slider)
    {
        if (!SliderChanged && activeSpaceShip != null)
        {
            SliderChanged = true;
        } else
        {
            return;
        }
        int sliderValue = (int)slider.value;
        ResourceType resourceType = ResourceSliderInputFieldPairs.Where(x => x.ResourceSlider == slider).FirstOrDefault().ResourceType;
        
        //Berechnen, wie viel noch transportiert werden kann (Wie viel Platz ist noch im Spaceship)
        int MaxTransportAmnount = CalculateMaxTransportAmnount(resourceType);

        if (MaxTransportAmnount < sliderValue)
        {
            sliderValue = MaxTransportAmnount;

        }

        
        //Wenn die Differenz größer als 0 ist, dann wird die Differenz von der Ressource auf dem Planeten abgezogen
        //if (difference > 0)
        //{
            int ResourceAvailable = CalculateResourceToLoad(resourceType, sliderValue);
            LoadUnloadResourceToShip(resourceType, ResourceAvailable);
        //}
        int newSliderValue = activeSpaceShip.ResourceStorage[resourceType].StorageQuantity;
        slider.value = newSliderValue;

        int inputFieldValue = int.Parse(ResourceSliderInputFieldPairs.Where(x => x.ResourceSlider == slider).FirstOrDefault().ResourceInputField.text);
        if (inputFieldValue != newSliderValue)
        {
            ResourceSliderInputFieldPairs.Where(x => x.ResourceSlider == slider).FirstOrDefault().ResourceInputField.text = newSliderValue.ToString();
        }
        SliderChanged = false;
    }

    public void InputFieldChangedSetSlider(TMP_InputField tMP_InputField)
    {
        if (activeSpaceShip == null)
        {            
            return;
        }
        int ifValue = int.Parse(tMP_InputField.text);
        int sliderValue = (int)ResourceSliderInputFieldPairs.Where(x => x.ResourceInputField == tMP_InputField).FirstOrDefault().ResourceSlider.value;
        if (ifValue != sliderValue)
        {
            ResourceSliderInputFieldPairs.Where(x => x.ResourceInputField == tMP_InputField).FirstOrDefault().ResourceSlider.value = ifValue;
        }

    }

    //Berechnen, wie viel noch transportiert werden kann (Wie viel Platz ist noch im Spaceship)
    public int CalculateMaxTransportAmnount(ResourceType resourceType)
    {   
        int transportAmount = 0;
        foreach (SliderInputFieldPair sliderInputFieldPair in ResourceSliderInputFieldPairs.Where(i => i.ResourceType != resourceType))
        {
            transportAmount += (int)sliderInputFieldPair.ResourceSlider.value;
        }
        int difference = activeSpaceShip.CargoSpace - transportAmount;
        Debug.Log("Difference: " + difference);
        return difference;
    }

    public int LoadUnloadResourceToShip(ResourceType resourceType, int amount)
    {
        CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBodyScript;
        activeSpaceShip.ResourceStorage[resourceType].StorageQuantity += amount;
        celestialBody.ResourceStorageCelestialBody[resourceType].StorageQuantity -= amount;
        return amount;
    } 

    public int CalculateResourceToLoad(ResourceType resourceType, int amount)
    {
        CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBodyScript;
        int storrageInShip = activeSpaceShip.ResourceStorage[resourceType].StorageQuantity;
        Debug.Log("Storrage in Ship: " + storrageInShip);
        amount -= storrageInShip;
        int availableAmount = celestialBody.ResourceStorageCelestialBody[resourceType].StorageQuantity;
        if (availableAmount >= amount)
        {
            return amount;
        }else
        {
            return availableAmount;
        }
    }

    public int XCheckResourceAvailable(ResourceType resourceType, int amount)
    {
        CelestialBody celestialBody = GUIManager.Instance.selectedCelestialBodyScript;
        int storrageInShip = activeSpaceShip.ResourceStorage[resourceType].StorageQuantity;
        int storrageOnPlanet = celestialBody.ResourceStorageCelestialBody[resourceType].StorageQuantity;
        amount -= storrageInShip;

        if (storrageOnPlanet >= amount)
        {
            return amount;
        }
        else
        {
            return storrageOnPlanet;
        }
    }

    public void SetMaxSliderInputValues(int amount)
    {
        foreach (SliderInputFieldPair sliderInputFieldPair in ResourceSliderInputFieldPairs)
        {
            sliderInputFieldPair.ResourceSlider.maxValue = amount;
        }
    }
}
