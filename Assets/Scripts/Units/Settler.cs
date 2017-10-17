using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Settler : Unit
{
    void Start()
    {
        base.Start();
        actions.Add(new UnityAction(BuildCity));
    }

    public void BuildCity()
    {
        Vector3 pos = currentCell.transform.position;
        if (currentCell.isHill)
        {
            pos += Vector3.up * HexMetrics.hillHeight;
        }

        Instantiate(grid.defaultCity, pos, Quaternion.identity);
    }
}
