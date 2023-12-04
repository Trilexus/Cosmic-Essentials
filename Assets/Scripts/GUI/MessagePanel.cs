using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

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


    // Start is called before the first frame update
    void Start()
    {
        stringBuilder.Append($"{Messages.texts[CurrentMessage]}");
        LastMessages = Messages.texts.Count - 1;
        MessageText.text = stringBuilder.ToString();
    }


    

    // Update is called once per frame
    void Update()
    {
        
    }
    public void NextMessage()
    {
        if (CurrentMessage < LastMessages)
        {
            CurrentMessage++;
            UpdateMessage();
        }
    }
    public void PreviousMessage()
    {
        if (CurrentMessage > 0)
        {
            CurrentMessage--;
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
