using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpacefleetObject", menuName = "ScriptableObjects/SpacefleetScriptableObject")]
public class SpacefleetScriptableObject : EntityScriptableObject
{

    
    public List<Resource> ResourcesCargo;
    public SpacefleetType Type; // Enum, was für ein Schiff oder Raumstation ist es?
    public int MaxCargoSpace; // Wie viel Frachtraum bietet es?
    public int MaxSpeed; // Wie schnell ist es?
    public int MaxRange; // Wie weit kann es fliegen?
    public int MaxFuel; // Wie viel Treibstoff kann es mitnehmen?

}
