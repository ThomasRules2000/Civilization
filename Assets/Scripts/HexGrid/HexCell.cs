using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public Color colour;
    public HexCoordinates coordinates;

    public float gCost;
    public float hCost;
    public HexCell parent;

    public float fCost
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
