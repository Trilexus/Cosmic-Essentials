using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStation : CelestialBody
{
    public int constructionProgress; // 0 -> 100 percent 

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateInfoText();
    }

    public override void UpdateInfoText()
    {
        Debug.Log("UpdateInfoText: " + constructionProgress);
    }

    public void startBuilding()
    {
        constructionProgress = 1;
    }
}
