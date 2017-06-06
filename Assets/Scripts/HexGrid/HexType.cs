using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexType
{
    public Color colour;

    public HexType(Color col)
    {   
        colour = col;
    }

    public enum typeKeys //Allows for intellisense for dictionary
    {
        plains,
        ocean,
        forest,
        desert
    }

    public static Dictionary<typeKeys, HexType> types = new Dictionary<typeKeys, HexType>
    {
        { typeKeys.plains,  new HexType(Color.white)  },
        { typeKeys.ocean,   new HexType(Color.blue)   },
        { typeKeys.forest,  new HexType(Color.green)  },
        { typeKeys.desert,  new HexType(Color.yellow) }
    };
}
