using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class Symbols
{
    public const string Area = "\uf0ac";
    public const string Farm = "\uf722";
    public const string Mine = "\uf275";
    public const string Reactor = "\uf7ba";
    public const string Spaceport = "\uf7c0";

    public const string Food = "\ue2cd";
    public const string Metal = "\uf468";
    public const string Energy = "\uf0e7";
    public const string SpacePoint = "\uf7bf";    
    public const string SpaceShip = "\uf135";

    public const string prioritized = "\uf06a";
    public const string infinity = "\uf534";
    public const string rightLeft = "\uf362";
    public const string box = "\uf466";
    public const string clipboardCheck = "\uf46c";

    public static string GetSymbol(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Food:
                return Food;
            case ResourceType.Metal:
                return Metal;
            case ResourceType.Energy:
                return Energy;
            case ResourceType.SpacePoints:
                return SpacePoint;
            default:
                return "-";
        }
    }
}
