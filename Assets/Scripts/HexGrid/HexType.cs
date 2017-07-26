﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HexType
{
    public Color colour;
    public int movementCost;
    public bool isWater;

    public HexType(Color col, int cost, bool water)
    {   
        colour = col;
        movementCost = cost;
        isWater = water;
    }

    public enum typeKeys //Allows for intellisense for dictionary
    {
        plains,
        ocean,
        forest,
        desert,
        city,
        coast,
        tundra
    }

    public static Dictionary<typeKeys, HexType> types = new Dictionary<typeKeys, HexType> //Todo - add more tile types
    {
        { typeKeys.plains,  new HexType(Color.green,  1, false) },
        { typeKeys.ocean,   new HexType(Color.blue,   1, true)  },
        { typeKeys.forest,  new HexType(new Color(0.133f,0.545f,0.133f),  2, false)  },
        { typeKeys.desert,  new HexType(new Color(1f, 0.714f, 0.009f), 1, false) },
        { typeKeys.city,    new HexType(Color.grey,   1, false) },
        { typeKeys.coast,   new HexType(new Color(0.298f, 0.298f, 1), 1, true)  },
        { typeKeys.tundra,  new HexType(new Color(0.439f, 0.427f, 0.384f), 1, false) }
    };

    public override string ToString()
    {
        typeKeys key = types.FirstOrDefault(x => x.Value == this).Key;
        return System.Enum.GetName(typeof(typeKeys), key);
    }
}
