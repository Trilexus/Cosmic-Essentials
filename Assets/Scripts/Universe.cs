using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Universe :MonoBehaviour
{
    [SerializeField]
    private List<PlanetarySystem> planetarySystems;

    [SerializeField]
    public float tickDuration = 5.0f;

    private IEnumerator TickCoroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(tickDuration);
            OnTick();
        }
    }

    public void Start()
    {
        CollectAllSpaceSystems();
        StartCoroutine(TickCoroutine());

    }

    public void CollectAllSpaceSystems()
    {
        planetarySystems = GameObject.FindObjectsOfType<PlanetarySystem>().ToList();

    }

    private void OnTick()
    {
        // Deine Tick-Berechnungen hier
        // Debug.Log("Tick occurred");
    }
}
