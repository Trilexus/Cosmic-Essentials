using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class StructureDatabase
{
    public static List<Structure> AllStructures = new List<Structure>();
    public static Dictionary<AllowedLocation, List<Structure>> AllowedStructures = new Dictionary<AllowedLocation, List<Structure>>();

    public static void Initialize()
    {
        AllStructures = EntityManager.Instance.AllStructures;
        InitializeAllowedStructures();
    }

    private static void InitializeAllowedStructures()
    {
        AllStructures = EntityManager.Instance.AllStructures;
        foreach (var structure in AllStructures)
        {
            foreach (var location in structure.AllowedLocations)
            {
                if (!AllowedStructures.ContainsKey(location))
                {
                    AllowedStructures[location] = new List<Structure>();
                }
                AllowedStructures[location].Add(structure);
            }
        }
    }
}
