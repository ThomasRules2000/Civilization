using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production : MonoBehaviour
{
    public int requiredProduction;
    public int containedProduction;

    public int ProductionRemaining
    {
        get
        {
            return requiredProduction - containedProduction;
        }
    }

    public void Produce()
    {
        //Defined so can be used by UnitProduction and ImprovementProductions later
    }
}
