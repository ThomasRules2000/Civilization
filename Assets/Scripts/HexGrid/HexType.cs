using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HexType
{
    public Color colour;
    public Color fovColour;
    public int movementCost;
    public bool isWater;

    public HexType(Color col, Color fov, int cost, bool water)
    {   
        colour = col;
        fovColour = fov;
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

    /// <summary>
    /// A dictionary of all types a tile can be
    /// </summary>
    public static Dictionary<typeKeys, HexType> types = new Dictionary<typeKeys, HexType> //Todo - add more tile types
    {
        { typeKeys.plains,  new HexType(Color.green, new Color(0, 0.8f, 0),  1, false) },
        { typeKeys.ocean,   new HexType(new Color(0, 0.5f, 1f), new Color(0, 0.4f, 0.8f),  1, true)  },
        { typeKeys.forest,  new HexType(new Color(0.133f,0.545f,0.133f), new Color(0.106f,0.436f,0.106f), 2, false)  },
        { typeKeys.desert,  new HexType(new Color(1, 0.714f, 0.009f), new Color(0.8f, 0.571f, 0.007f), 1, false) },
        { typeKeys.city,    new HexType(Color.grey, new Color(0.4f, 0.4f, 0.4f),   1, false) },
        { typeKeys.coast,   new HexType(new Color(0.2f, 0.6f, 1), new Color(0.16f, 0.48f, 0.8f), 1, true)  },
        { typeKeys.tundra,  new HexType(new Color(0.439f, 0.427f, 0.384f), new Color(0.351f, 0.342f, 0.307f), 1, false) }
    };

    public override string ToString()
    {
        typeKeys key = types.FirstOrDefault(x => x.Value == this).Key;
        return System.Enum.GetName(typeof(typeKeys), key);
    }
}
