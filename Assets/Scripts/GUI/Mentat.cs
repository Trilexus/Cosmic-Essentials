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


    // Start is called before the first frame update
    void Start()
    {
        
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

    public void SetTextWithTyping(string message, string localisationTable)
    {
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
        }
        MentatText.text = lastMessage;

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
