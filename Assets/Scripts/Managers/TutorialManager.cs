using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> GUIElements;
    [SerializeField]
    private GameObject GUIMarker;
    private RectTransform GUIMarkerRectTransform;
    private CanvasGroup GUIMarkerCanvasGroup;
    public int currentElement = 0;
    [SerializeField]
    private string LocalisatzionTableName = "TutorialTexts";
    StringBuilder stringBuilder = new StringBuilder();
    Mentat mentat;
    [SerializeField]
    GameObject WelcomePanelStory;
    GameObject WelcomePanelInstance;
    [SerializeField]
    Canvas canvas;
    int tutorialStep = 0;
    int storyStep = 0;
    private bool yesPressed = false;
    private bool noPressed = false;
    private bool nextPressed = false;
    [SerializeField]
    private GameObject MainPlanet;
    private Planet MainPlanetScript;
    private Animator MainPlanetAnimator;
    private TabsMenu PanelTabsMenuTopScript;
    private float timer = 0.0f;
    private float interval = 0.6f;
    public GameObject CelestialBodyToReachWithSpaceShip;
    


    void Start()
    {
        SpaceShip.OnReachedCelestialBodyEvent += WinConditionsFulfilled;
        MainPlanetScript = MainPlanet.GetComponent<Planet>();
        MainPlanetAnimator = MainPlanet.GetComponentInChildren<Animator>();
        GUIMarkerRectTransform = GUIMarker.GetComponent<RectTransform>();
        GUIMarkerCanvasGroup = GUIMarker.GetComponent<CanvasGroup>();
        mentat = GUIManager.Instance.MentatScript;
        WelcomePanelStory.SetActive(true);
        WelcomePanelInstance = WelcomePanelStory;
        mentat.OnButtonYes += OnButtonYes;
        mentat.OnButtonNo += OnButtonNo;
        mentat.OnNextPressed += NextPressed;
        GameObject PanelTabsMenuTop = GUIElements[1];
        PanelTabsMenuTopScript = PanelTabsMenuTop.GetComponent<TabsMenu>();
    }
    private void OnDestroy()
    {
        mentat.OnButtonYes -= OnButtonYes;
        mentat.OnButtonNo -= OnButtonNo;
        mentat.OnNextPressed -= NextPressed;
        SpaceShip.OnReachedCelestialBodyEvent -= WinConditionsFulfilled;
    }

    private void LogTutorialStep()
    {
        timer += Time.deltaTime; // Zeit seit dem letzten Update hinzufügen

        if (timer >= interval)
        {
            // Führen Sie hier Ihren Befehl aus
            Debug.Log(tutorialStep);

            timer = 0.0f; // Timer zurücksetzen
        }
    }
    
    public void WinConditionsFulfilled(GameObject planet, SpaceShip spaceShip)
    {
        if (CelestialBodyToReachWithSpaceShip == planet && storyStep < 500)
        {
            storyStep = 500;
        }
    }

    public void Update()
    {
        TutorialSteps();
        StorySteps();
    }

    private void StorySteps()
    {
        switch (storyStep)
        {
            case 500:
                SetMenatText("YouWin", true);
                storyStep++;
                BlinkGUIElement("MentatImage");
                break;
        }
    }
    void TutorialSteps()
    {
        //LogTutorialStep();
        switch (tutorialStep)
        {
            case 0:
                if (!WelcomePanelInstance.activeSelf)
                {
                    Time.timeScale = 1;
                    //Destroy(WelcomePanelInstance);
                    SetTutorialText(true);
                    tutorialStep++;
                }
                break;
            case 1:
                if (yesPressed)
                {
                    FreezTimeOnMainPlanet();
                    SetTutorialText(true);
                    tutorialStep = 999; // 999 is waiting for the player to select the main planet
                    mentat.HideButtons();
                    resetButtons();
                    GUIManager.Instance.OnSelectedCelestialBodyChanged += NextTutorialStep_SelectedCelestialBodyChanged;
                }
                else if (noPressed)
                {
                    tutorialStep = 900;
                    resetButtons();
                }
                break;
            case 2:
                SetTutorialText(true);
                tutorialStep++;
                //mentat.ShowButtons();
                resetButtons();
                break;
            case 3:
                if (nextPressed)
                {
                    SetTutorialText(true);
                    tutorialStep = 999; // 999 is waiting for the player to select Structure Build Menu
                    PanelTabsMenuTopScript.onTabActivate += NextTutorialStep_StructureMenuActivated;
                    BlinkGUIElement("ButtonStructures");
                }
                break;
            case 4:
                SetTutorialText(true);
                tutorialStep++;
                resetButtons();
                HideGUIMarker();
                break;
            case 5:
                if (nextPressed)
                {
                    SetTutorialText(true);
                    tutorialStep++;
                    resetButtons();
                }
                break;
            case 6:
                if (nextPressed)
                {
                    SetTutorialText(true);
                    tutorialStep++;
                    resetButtons();
                }
                break;
            case 7:
                if (nextPressed)
                {
                    SetTutorialText(true);
                    tutorialStep++;
                    resetButtons();
                }
                break;
            case 8:
                if (nextPressed)
                {
                    SetTutorialText(true);
                    tutorialStep++;
                    resetButtons();
                }
                break;
            case 9:
                if (nextPressed)
                {
                    SetTutorialText(true);
                    tutorialStep++;
                    resetButtons();
                    tutorialStep = 950; // 950 Ends Tutorial
                }
                break;
            case 900:
                SetTutorialText(true);
                UnFreezTimeOnMainPlanet();
                mentat.HideButtons();
                mentat.OnButtonYes -= OnButtonYes;
                mentat.OnButtonNo -= OnButtonNo;
                tutorialStep++;
                break;
            case 950:
                SetTutorialText(true);
                UnFreezTimeOnMainPlanet();
                tutorialStep++;
                break;
        }
    }

    public void NextTutorialStep_SelectedCelestialBodyChanged(CelestialBody celestialBody)
    {
        if (celestialBody.gameObject == MainPlanet) 
        {
            GUIManager.Instance.OnSelectedCelestialBodyChanged -= NextTutorialStep_SelectedCelestialBodyChanged;
            tutorialStep  =2;
        }
    }

    public void NextTutorialStep_StructureMenuActivated(GameObject gameObject)
    {
        GameObject StructureMenu = GUIElements[6];
        if (gameObject == StructureMenu)
        {
            PanelTabsMenuTopScript.onTabActivate -= NextTutorialStep_StructureMenuActivated;
            tutorialStep = 4;
        }
    }

    public void SetTutorialText(bool clearText)
    {
        SetMenatText("MentatMessage" + tutorialStep.ToString("D3"), clearText);
    }

    public void FreezTimeOnMainPlanet()
    {
        MainPlanetScript.isPaused = true;
        MainPlanetAnimator.enabled = false;
    }
    public void UnFreezTimeOnMainPlanet()
    {
        MainPlanetScript.isPaused = false;
        MainPlanetAnimator.enabled = true;
    }

    public void SetMenatText(string message, bool clearText)
    {
        stringBuilder.Clear();
        LocalizedString myLocalizedString = new LocalizedString(LocalisatzionTableName, message);
        stringBuilder.Append($"{myLocalizedString.GetLocalizedString()}");
        mentat.SetTextWithTyping(message, LocalisatzionTableName, clearText);
    }

    public void NextPosition()
    {
        RectTransform rectTransform = GUIElements[currentElement].GetComponent<RectTransform>();
        GUIMarkerRectTransform.SetParent(GUIElements[currentElement].transform.parent);
        GUIMarkerRectTransform.anchoredPosition = rectTransform.anchoredPosition;
        GUIMarkerRectTransform.sizeDelta = rectTransform.sizeDelta;
        GUIMarkerRectTransform.anchorMin = rectTransform.anchorMin;
        GUIMarkerRectTransform.anchorMax = rectTransform.anchorMax;
        GUIMarkerRectTransform.pivot = rectTransform.pivot;
        PressButton();
        if (currentElement < GUIElements.Count - 1)
        {
            currentElement++;
        }
        else
        {
            currentElement = 0;
        }
    }

    public void BlinkGUIElement(string name) 
    {
        GameObject GUIElement = GUIElements.Find(I => I.name == name);
        RectTransform rectTransform = GUIElement.GetComponent<RectTransform>();
        GUIMarkerRectTransform.SetParent(GUIElement.transform.parent);
        GUIMarkerRectTransform.anchoredPosition = rectTransform.anchoredPosition;
        GUIMarkerRectTransform.sizeDelta = rectTransform.sizeDelta;
        GUIMarkerRectTransform.anchorMin = rectTransform.anchorMin;
        GUIMarkerRectTransform.anchorMax = rectTransform.anchorMax;
        GUIMarkerRectTransform.pivot = rectTransform.pivot;
        ShowGUIMarker();
    }

    public void ShowGUIMarker()
    {
        GUIMarkerCanvasGroup.alpha = 1;
    }

    public void HideGUIMarker()
    {
           GUIMarkerCanvasGroup.alpha = 0;
    }

    public void PressButton()
    {
        Button button = GUIElements[currentElement].GetComponent<Button>();
        if (button != null)
        {
            button.onClick.Invoke();
        }
    }

    public void OnButtonYes()
    {
        yesPressed = true;
        noPressed = false;
    }
    public void OnButtonNo()
    {
        yesPressed = false;
        noPressed = true;
    }
    public void resetButtons()
    {
        yesPressed = false;
        noPressed = false;
        nextPressed = false;
    }

    public void NextPressed()
    {
        nextPressed = true;
    }

}
