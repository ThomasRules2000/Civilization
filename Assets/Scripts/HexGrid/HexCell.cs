using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour, IHeapItem<HexCell>
{
    public Color colour;

    bool isVisible;
    public bool IsVisible
    {
        get
        {
            return isVisible;
        }
        set
        {
            if(isVisible == value)
            {
                return;
            }

            if (value)
            {
                colour = type.colour;
            }
            else
            {
                colour = type.fovColour;
            }

            isVisible = value;

            if(militaryUnit != null && militaryUnit.UnitCivilization != player.PlayerCivilization)
            {
                militaryUnit.IsVisible = value;
            }
            if(passiveUnit != null && passiveUnit.UnitCivilization != player.PlayerCivilization)
            {
                passiveUnit.IsVisible = value;
            }
        }
    }

    Player player;

    public HexCoordinates coordinates;
    public HexGridChunk chunk;
    public bool isHill;

    public Cloud cloud;

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

    private HexType type;
    public HexType Type {
        get
        {
            return type;
        }
        set
        {
            type = value;
            if (isVisible)
            {
                colour = value.colour;
            }
            else
            {
                colour = value.fovColour;
            }
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

    void Start()
    {
        player = transform.parent.GetComponentInParent<Player>();
    }

    void Refresh()
    {
        chunk.Refresh();
    }
}
