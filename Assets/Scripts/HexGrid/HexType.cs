using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        hill
    }

    public static Dictionary<typeKeys, HexType> types = new Dictionary<typeKeys, HexType> //Todo - add more tile types
    {
        { typeKeys.plains,  new HexType(Color.green,  1, false) },
        { typeKeys.ocean,   new HexType(Color.blue,   1, true)  },
        { typeKeys.forest,  new HexType(new Color(0.133f,0.545f,0.133f),  4, false)  },
        { typeKeys.desert,  new HexType(new Color(1f, 0.714f, 0.009f), 1, false) },
        { typeKeys.city,    new HexType(Color.grey,   1, false) },
        { typeKeys.hill,    new HexType(Color.green,  2, false) }
    };

    public override string ToString()
    {
        return System.Enum.GetName(typeof(typeKeys), this);
    }
}
