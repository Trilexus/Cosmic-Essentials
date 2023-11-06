using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;
    public GameObject CelestialBodyMenu;
    public GameObject CelestialBodyBuildInfo;
    public CelestialBodyInfo CelestialBodyInfoScript;

    public Vector3 celestialBodyMenuOffset;
    public Vector3 celestialBodyInfoOffset;

    public GameObject selectedCelestialBody;
    public CelestialBody selectedCelestialBodyScript;
    public GameObject selectedCelestialBodyTarget;
    public GameObject selectedPlanetarySystem;
    public GameObject ActiveCelestialBodyMarker; // shows the selected celestial body
    public GameObject ActiveCelestialBodyTarget; // shows the celestial body target for orders

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

    public GameObject ScrollViewOrdersOverviewContent;
    public GameObject OrderInfoEntryPrefab;
    public GameObject Mentat;
    public Mentat MentatScript;

    public GameObject StructureMenu;
    public StructureMenu StructureMenuScript;


    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // Singleton
            DontDestroyOnLoad(this.gameObject); // Singleton
            StructureMenuScript = StructureMenu.GetComponent<StructureMenu>();
            CelestialBodyInfoScript = CelestialBodyBuildInfo.GetComponent<CelestialBodyInfo>();
            MentatScript = Mentat.GetComponent<Mentat>();
        }
        else
        {
            Destroy(this.gameObject); // Singleton
        }
    }

    public void SetActiveCelestialBody(GameObject selectedCelestialBody)
    {
        if (this.selectedCelestialBody == null)
        {
            this.selectedCelestialBody = selectedCelestialBody;
            selectedCelestialBodyScript = selectedCelestialBody.GetComponent<CelestialBody>();
            orderOriginImage.sprite = selectedCelestialBodyScript.ChildSpriteRenderer.sprite;
            ActiveCelestialBodyMarker.transform.position = selectedCelestialBody.transform.position;
            ActiveCelestialBodyMarker.SetActive(true);
            SetActivePlanetarySystem(selectedCelestialBody.transform.parent.gameObject);
            FillOrdersOverview();
            UpdateCelestialBodyInfos();
            StructureMenuScript.CreateMenuForCelestialBody(selectedCelestialBodyScript.AllowedLocation);
        } else if (this.selectedCelestialBody == selectedCelestialBody) {
            this.selectedCelestialBody = null;
            orderOriginImage.sprite = celestialBodyDefaultImage;
            orderTargetImage.sprite = celestialBodyDefaultImage;
            ActiveCelestialBodyMarker.SetActive(false);
            ActiveCelestialBodyTarget.SetActive(false);
            StructureMenuScript.ClearMenu();
        } else
        {
            selectedCelestialBodyTarget = selectedCelestialBody;
            orderTargetImage.sprite = selectedCelestialBodyTarget.GetComponent<CelestialBody>().ChildSpriteRenderer.sprite;
            ActiveCelestialBodyTarget.transform.position = selectedCelestialBody.transform.position;
            ActiveCelestialBodyTarget.SetActive(true);
        }
    }


    public void FillOrdersOverview()
    {
        foreach (Transform child in ScrollViewOrdersOverviewContent.transform)
        {
            Destroy(child.gameObject);
        }
        List<ResourceTransferOrder> Orders = selectedCelestialBody.GetComponent<CelestialBody>().ResourceTransferOrders;
        foreach (ResourceTransferOrder order in Orders)
        {
            GameObject orderEntry = Instantiate(OrderInfoEntryPrefab, ScrollViewOrdersOverviewContent.transform);
            orderEntry.GetComponent<OrderInfoEntry>().Initialize(order, Orders);
            orderEntry.transform.SetParent(ScrollViewOrdersOverviewContent.transform);
        }
    }



    private void SetActivePlanetarySystem(GameObject selectedPlanetarySystem)
    {
        if (selectedPlanetarySystem != this.selectedPlanetarySystem)
        {
            DeactivateCurrentPlanetarySystem();
            this.selectedPlanetarySystem = selectedPlanetarySystem;
            selectedPlanetarySystem.GetComponent<PlanetarySystem>().IsActivePlanetarySystem = true;
        }        
    }

    private void DeactivateCurrentPlanetarySystem()
    {
        if (selectedPlanetarySystem != null)
        {
            selectedPlanetarySystem.GetComponent<PlanetarySystem>().IsActivePlanetarySystem = false;
        }
    }

    public void MoveCelestialBodyMenu(GameObject selectedCelestialBody)
    {
        RectTransform rect = CelestialBodyMenu.GetComponent<RectTransform>();
        celestialBodyMenuOffset = new Vector3(rect.rect.width / 2, rect.rect.height / -2, 0);
        Vector3 newPosition = selectedCelestialBody.transform.position;
        newPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, newPosition);
        Vector3 newPositionWithOffset = newPosition + celestialBodyMenuOffset;
        rect.transform.position = newPositionWithOffset;
        CelestialBodyMenu.SetActive(true);
        UpdateCelestialBodyInfos();
    }
    
    public void UpdateCelestialBodyInfos()
    {
        CelestialBodyInfoScript.UpdateCelestialBodyInfos();
    }

    public void HideCelestialBodyMenu()
    {
        CelestialBodyMenu.SetActive(false);
    }
    public void ShowCelestialBodyMenu()
    {
        CelestialBodyMenu.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
