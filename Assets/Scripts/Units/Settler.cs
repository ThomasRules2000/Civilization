using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settler : Unit
{
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
