using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class Mentat : MonoBehaviour
{

    public TextMeshProUGUI MentatAlertText;
    public TextMeshProUGUI MentatText;
    private DateTime startTimeAlert;
    public float displayTimeAlert = 5f;
    LocalizedString myLocalizedString = new LocalizedString("MentatAlerts", "InsufficientResources");
    string TranslatedValue => myLocalizedString.GetLocalizedString();
    public float typingSpeed = 0.002f;
    Coroutine myTypingCoroutine;
    string lastMessage;
    public delegate void ButtonYesHandler();
    public event ButtonYesHandler OnButtonYes;
    public delegate void ButtonNoHandler();
    public event ButtonNoHandler OnButtonNo;
    [SerializeField]
    public GameObject Buttons;
    private CanvasGroup ButtonsCanvasGroup;

    public delegate void OnNextPressedHandler();
    public event OnNextPressedHandler OnNextPressed;


    // Start is called before the first frame update
    void Start()
    {
        ButtonsCanvasGroup = Buttons.GetComponent<CanvasGroup>();
    }

    public void SetAlertText(string message)
    {
        myLocalizedString = new LocalizedString("MentatAlerts", message);
        MentatAlertText.text = TranslatedValue;
        startTimeAlert = DateTime.Now; 
    }
    public void SetText(string message)
    {
        myLocalizedString = new LocalizedString("MentatText", message);
        MentatText.text = TranslatedValue;
        startTimeAlert = DateTime.Now;
    }

    public void SetTextWithTyping(string message, string localisationTable, bool clearText)
    {
        SkipTpyping();
        if (clearText)
        {
            MentatText.text = "";
        }
        myLocalizedString = new LocalizedString(localisationTable, message);
        Debug.Log(TranslatedValue);
        lastMessage = TranslatedValue;
        myTypingCoroutine = StartCoroutine(TypeText(TranslatedValue));
        startTimeAlert = DateTime.Now;
    }

    public void SkipTpyping()
    {
        if (myTypingCoroutine != null)
        {
            StopCoroutine(myTypingCoroutine);
            myTypingCoroutine = null;
        } else
        {
            OnNextPressed?.Invoke();
        }
        MentatText.text = lastMessage;
    }

    public void ButtonYes()
    {
        OnButtonYes?.Invoke();
    }

    public void ButtonNo()
    {
        OnButtonNo?.Invoke();
    }

    public void HideButtons()
    {
        ButtonsCanvasGroup.alpha = 0;
        ButtonsCanvasGroup.interactable = false;
        ButtonsCanvasGroup.blocksRaycasts = false;
    }
    public void ShowButtons()
    {
        ButtonsCanvasGroup.alpha = 1;
        ButtonsCanvasGroup.interactable = true;
        ButtonsCanvasGroup.blocksRaycasts = true;
    }

    IEnumerator TypeText(string message)
    {
        Debug.Log("Typing");
        foreach (char letter in message.ToCharArray())
        {
            MentatText.text += letter;
            double startTime= Time.realtimeSinceStartupAsDouble;
            while (Time.realtimeSinceStartupAsDouble <= startTime + typingSpeed)
            {
                yield return null;
            }
            //yield return new WaitForSeconds(typingSpeed);
        }
        myTypingCoroutine = null;

    }

    // Update is called once per frame
    void Update()
    {
        TimeSpan timeSpanAlert = DateTime.Now - startTimeAlert;
        if (timeSpanAlert.TotalSeconds > displayTimeAlert)
        {
            MentatAlertText.text = "";
        }
    }



}
