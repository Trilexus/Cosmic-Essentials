using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextData", menuName = "ScriptableObjects/TextDataScriptableObject", order = 1)]

public class TextDataScriptableObject : ScriptableObject
{
    [TextArea(5, 30)]
    public List<string> texts;
}