using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production : MonoBehaviour
{
    int requiredProduction;
    int containedProduction;

    int ProductionRemaining
    {
        get
        {
            return requiredProduction - containedProduction;
        }
    }
}
