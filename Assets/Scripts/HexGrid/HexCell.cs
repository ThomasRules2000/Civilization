using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour, IHeapItem<HexCell>
{
    public Color colour;
    public HexCoordinates coordinates;
    public bool isHill;

    public Civilization unitCiv = null;
    public Unit militaryUnit;
    public Unit passiveUnit;

    public float gCost;
    public float hCost;
    public HexCell parent;
    int heapIndex;

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

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(HexCell toCompare)
    {
        int compare = fCost.CompareTo(toCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(toCompare.hCost);
        }
        return -compare;
    }
}
