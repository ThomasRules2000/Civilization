using System.Collections;
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

            City city = Instantiate(grid.defaultCity, pos, Quaternion.identity, grid.transform);
            city.currentOwner = city.originalOwner = UnitCivilization;
            city.CityName = "A City";
            Destroy(this.gameObject);
        } 
    }
}
