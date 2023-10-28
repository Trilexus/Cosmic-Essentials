using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class OrderMenu : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnSliderChanged(float number)
    {
        if (GUIManager.Instance.orderAmountInputField.text != number.ToString())
        {
            GUIManager.Instance.orderAmountInputField.text = number.ToString();
        }
    }

    public void OnFieldChanged(string text)
    {
        if (GUIManager.Instance.orderAmountSlider.value.ToString() != text)
        {
            if (float.TryParse(text, out float number))
            {
                GUIManager.Instance.orderAmountSlider.value = number;
            }
        }
    }
}
