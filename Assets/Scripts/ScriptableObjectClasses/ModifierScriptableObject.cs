using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewModifier", menuName = "ScriptableObjects/ModifierScriptableObject")]

public class ModifierScriptableObject : ScriptableObject
{
    public string ModifierName;
    public ModifierType ModifierType;
    //public AttributeType AttributeType;
    public Sprite ModifierSprite;
    public int ModifierLevel;
}

