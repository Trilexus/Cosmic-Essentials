using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Population { 

    [SerializeField]
    public int CurrentPopulation;
    public int _maxPopulation;
    [SerializeField]
    private int _minPopulation;
    [SerializeField]
    private float _populationGrowthCounter;
    [SerializeField]
    private int _growthThresholdTicks = 10;
    public float RED_THRESHOLD;
    [SerializeField]
    private int _ecoImpactFactor = -1;
    public int EcoImpactFactor => _ecoImpactFactor * CurrentPopulation;

    public Population(int CurrentPopulation, int maxPopulation, int growthThresholdTicks, float RED_THRESHOLD)
    {
        
        this.CurrentPopulation = CurrentPopulation;
        this.RED_THRESHOLD = RED_THRESHOLD;
        _maxPopulation = maxPopulation;
        _growthThresholdTicks = growthThresholdTicks;
    }

    public void UpdatePopulation(int foodAvailable, float ecoecoIndex)
    {
        bool isfoodAvailable = foodAvailable > 0;
        if (isfoodAvailable && ecoecoIndex > RED_THRESHOLD)
        {
            _populationGrowthCounter = _populationGrowthCounter + (ecoecoIndex);
            if (_populationGrowthCounter >= _growthThresholdTicks)
            {
                if (CurrentPopulation < _maxPopulation)
                {
                    CurrentPopulation++;
                }
                _populationGrowthCounter = 0;
            }
        } else if (isfoodAvailable && ecoecoIndex <= RED_THRESHOLD)
        {
            _populationGrowthCounter = 0;
        } else if (foodAvailable < 0)
        {
            //food not available
            if (CurrentPopulation > _minPopulation)
            {
                CurrentPopulation--;
            }
        }
    }
}
