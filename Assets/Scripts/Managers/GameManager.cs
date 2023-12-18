using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour
{
    public GameObject WelcomeMesageBox;
    public static GameManager Instance;
    public delegate void ChangeLanguageDelegate();
    public static event ChangeLanguageDelegate ChangeLanguageEvent;

    public void ToggleWelcomeBox()
    {
        WelcomeMesageBox.SetActive(!WelcomeMesageBox.activeSelf);
    }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void PauseResumeGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        } else
        {
            Time.timeScale = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PauseResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeLanguage(string language)
    {
        if (language == "English")
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }else if (language == "German")
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        }
        ChangeLanguageEvent?.Invoke();
    }
}
