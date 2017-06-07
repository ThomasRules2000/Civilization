using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Cube Coordinates
[System.Serializable]
public struct HexCoordinates
{
    [SerializeField] 
    private int x, z;

    public int X { get { return x; } }

    public int Z { get { return z; } }

    public int Y
    {
        get
        {
            return -X - Z;
        }
    }

    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public static HexCoordinates FromOffsetCoordinates(HexCoordinates coords)
    {
        return new HexCoordinates(coords.X - (coords.Z) / 2, coords.Z);
    }

    public static List<HexCoordinates> FromOffsetCoordinates(List<HexCoordinates> list)
    {
        List<HexCoordinates> returnList = new List<HexCoordinates>();
        foreach (HexCoordinates coords in list)
        {
            returnList.Add(new HexCoordinates(coords.X - (coords.Z) / 2, coords.Z));
        }
        return returnList;
    }

    public static HexCoordinates ToOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x + z / 2, z);
    }

    public static HexCoordinates ToOffsetCoordinates(HexCoordinates coords)
    {
        return new HexCoordinates(coords.X + (coords.Z / 2), coords.Z);
    }

    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }
}

