using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public Color colour;
    public HexCoordinates coordinates;

    public int gCost;
    public int hCost;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    private HexType _type;
    public HexType Type {
        get
        {
            return _type;
        }
        set
        {
            _type = value;
            colour = value.colour;
        }
    }
}
