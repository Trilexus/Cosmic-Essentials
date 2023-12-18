using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class MessagePanel : MonoBehaviour
{
    public GameObject MessageEntryPrefab;
    public TextMeshProUGUI MessageText;
    public TextDataScriptableObject Messages;
    public int CurrentMessage = 0;
    public int LastMessages;
    StringBuilder stringBuilder = new StringBuilder();
    //Regex from https://forum.unity.com/threads/using-text-of-a-tmp_text-component-to-set-a-unicode-value-shows-up-as-a-string-literal.686578/
    //Replace \uXXXX and \UXXXXXXXX with the correct unicode character
    private Regex m_RegexExpression = new Regex(@"(?<!\\)(?:\\u[0-9a-fA-F]{4}|\\U[0-9a-fA-F]{8})");
    [SerializeField]
    private bool useLocalisaztion;
    [SerializeField]
    private string LocalisatzionTableName;

    // Start is called before the first frame update
    void Start()
    {
        LastMessages = Messages.texts.Count - 1;
        if (useLocalisaztion)
        {
            UpdateMessageWithLocalisation();
        }else
        {
            UpdateMessage();
        }
        GameManager.ChangeLanguageEvent += UpdateCurrentText;
    }


    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCurrentText()
    {
        if (useLocalisaztion)
        {
            UpdateMessageWithLocalisation();
        }
        else
        {
            UpdateMessage();
        }
    }
    public void NextMessage()
    {
        if (CurrentMessage < LastMessages)
        {
            CurrentMessage++;
        } else
        {
            CloseMessage();
        }
        if (useLocalisaztion)
        {
            UpdateMessageWithLocalisation();
        }
        else
        {
            UpdateMessage();
        }
    }
    public void PreviousMessage()
    {
        if (CurrentMessage > 0)
        {
            CurrentMessage--;            
        }
        if (useLocalisaztion)
        {
            UpdateMessageWithLocalisation();
        }
        else
        {
            UpdateMessage();
        }
    }

    public void UpdateMessage()
    {
        stringBuilder.Clear();
        string message = ReplaceEscape(Messages.texts[CurrentMessage]);
        stringBuilder.Append($"{message}");
        MessageText.text = stringBuilder.ToString(); 
    }

    public void UpdateMessageWithLocalisation()
    {
        stringBuilder.Clear();
        LocalizedString myLocalizedString = new LocalizedString(LocalisatzionTableName, Messages.texts[CurrentMessage]);
        stringBuilder.Append($"{myLocalizedString.GetLocalizedString()}");
        MessageText.text = stringBuilder.ToString();
    }

    public void CloseMessage()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }

    private string ReplaceEscape(string message)
    {
        message = m_RegexExpression.Replace(message,
            match =>
            {
                if (match.Value.StartsWith("\\U"))
                    return char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\U", ""), NumberStyles.HexNumber));

                return char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\u", ""), NumberStyles.HexNumber));
            });

        return message;
    }
}
