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

    // Update is called once per frame
    void Update()
    {
        TimeSpan timeSpanAlert = DateTime.Now - startTimeAlert;
        if (timeSpanAlert.TotalSeconds > displayTimeAlert)
        {
            MentatAlertText.text = "-_-";
        }
    }



}
