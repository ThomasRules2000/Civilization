﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Settler : Unit
{
    new void Start()
    {
        base.Start();
        actions.Add(new UnityAction(Settle));
        actionNames.Add("Settle");
    }

    public void Settle()
    {
        Vector3 pos = currentCell.transform.position;
        if (!currentCell.Type.isWater)
        {
            if (currentCell.isHill)
            {
                pos += Vector3.up * HexMetrics.hillHeight;
            }

            Instantiate(grid.defaultCity, pos, Quaternion.Euler(-90,0,0));
            Destroy(this.gameObject);
        } 
    }
}