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
    void Start()
    {
        GUIMarkerRectTransform = GUIMarker.GetComponent<RectTransform>();
        GUIMarkerCanvasGroup = GUIMarker.GetComponent<CanvasGroup>();
        mentat = GUIManager.Instance.MentatScript;
        WelcomePanelInstance = Instantiate(WelcomePanelStory,canvas.transform);

    }

    public void Update()
    {
        if (!WelcomePanelInstance.activeSelf && tutorialStep == 0)
        {
            //Destroy(WelcomePanelInstance);
            StartTutorial();
            tutorialStep++;
        }
    }

    public void StartTutorial()
    {
        SetMenatText("MentatMessage01");
    }

    public void SetMenatText(string message)
    {
        stringBuilder.Clear();
        LocalizedString myLocalizedString = new LocalizedString(LocalisatzionTableName, message);
        stringBuilder.Append($"{myLocalizedString.GetLocalizedString()}");
        mentat.SetTextWithTyping(message, LocalisatzionTableName);
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
}
