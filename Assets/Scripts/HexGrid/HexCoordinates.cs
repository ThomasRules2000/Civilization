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
            return -x - z;
        }
    }

    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z) //Calculates hex coords from array pos
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public static HexCoordinates FromOffsetCoordinates(HexCoordinates coords)
    {
        return new HexCoordinates(coords.X - (coords.Z) / 2, coords.Z);
    }

    public static HexCoordinates ToOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x + z / 2, z);
    }

    public static HexCoordinates ToOffsetCoordinates(HexCoordinates coords)
    {
        return new HexCoordinates(coords.X + (coords.Z / 2), coords.Z);
    }

    public static HexCoordinates FromPosition(Vector3 pos) //Get hex coords at position
    {
        float x = pos.x / (HexMetrics.innerRad * 2f);
        float y = -x;
        float offset = pos.z / (HexMetrics.outerRad * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0) //Rounding Errors near the edge of cells, so reconstruct wrong value from other 2
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new HexCoordinates(iX, iZ);
    }

    public static List<HexCoordinates> GetNeighbours(HexCoordinates coords)
    {
        List<HexCoordinates> neighbours = new List<HexCoordinates>();

        neighbours.Add(new HexCoordinates(coords.X, coords.Z + 1));     //NE
        neighbours.Add(new HexCoordinates(coords.X + 1, coords.Z));     //E
        neighbours.Add(new HexCoordinates(coords.X + 1, coords.Z - 1)); //SE
        neighbours.Add(new HexCoordinates(coords.X, coords.Z - 1));     //SW
        neighbours.Add(new HexCoordinates(coords.X - 1, coords.Z));     //W
        neighbours.Add(new HexCoordinates(coords.X - 1, coords.Z + 1)); //NW

        return neighbours;
    }

    public static List<HexCoordinates> GetTwoTileRad(HexCoordinates coords)
    {
        List<HexCoordinates> neighbours = new List<HexCoordinates>();
        neighbours.AddRange(GetNeighbours(coords)); //Single Tile Dist

        neighbours.Add(new HexCoordinates(coords.X, coords.Z + 2));      //NE
        neighbours.Add(new HexCoordinates(coords.X + 1, coords.Z + 1));  //NEE
        neighbours.Add(new HexCoordinates(coords.X + 2, coords.Z));      //E
        neighbours.Add(new HexCoordinates(coords.X + 2, coords.Z - 1));  //SEE
        neighbours.Add(new HexCoordinates(coords.X + 2, coords.Z - 2));  //SE
        neighbours.Add(new HexCoordinates(coords.X + 1, coords.Z - 2));  //S
        neighbours.Add(new HexCoordinates(coords.X, coords.Z - 2));      //SW
        neighbours.Add(new HexCoordinates(coords.X - 1, coords.Z - 1));  //SWW
        neighbours.Add(new HexCoordinates(coords.X - 2, coords.Z));      //W
        neighbours.Add(new HexCoordinates(coords.X - 2, coords.Z + 1));  //NWW
        neighbours.Add(new HexCoordinates(coords.X - 2, coords.Z + 2));  //NW
        neighbours.Add(new HexCoordinates(coords.X - 1, coords.Z + 2));  //N

        return neighbours;
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

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

