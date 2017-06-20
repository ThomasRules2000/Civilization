using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public const float outerRad = 10f;
    public const float innerRad = outerRad * 0.866025404f; //*sqrt(3)/2

    public const float hillTopRatio = 0.7f; //Size of hill relative to rest of cells
    public const float hillHeight = 2f; //Height of hills
    const float hillOuterRad = outerRad * hillTopRatio;
    const float hillInnerRad = innerRad * hillTopRatio;

    public const int chunkSizeX = 5; 
    public const int chunkSizeZ = 5;

    static Vector3[] corners =
    {
        new Vector3(0f, 0f, outerRad),
        new Vector3(innerRad, 0f, 0.5f*outerRad),
        new Vector3(innerRad, 0f, -0.5f*outerRad),
        new Vector3(0f, 0f, -outerRad),
        new Vector3(-innerRad, 0f, -0.5f*outerRad),
        new Vector3(-innerRad, 0f, 0.5f*outerRad),
        new Vector3(0f, 0f, outerRad) //So that tringle mesh loops back without out of range
    };

    static Vector3[] hillCorners =
    {
        new Vector3(0f, 0f, hillOuterRad),
        new Vector3(hillInnerRad, 0f, 0.5f * hillOuterRad),
        new Vector3(hillInnerRad, 0f, -0.5f * hillOuterRad),
        new Vector3(0f, 0f, -hillOuterRad),
        new Vector3(-hillInnerRad, 0f, -0.5f * hillOuterRad),
        new Vector3(-hillInnerRad, 0f, 0.5f * hillOuterRad),
        new Vector3(0f, 0f, hillOuterRad) //Loopback
    };

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstHillCorner(HexDirection direction)
    {
        return hillCorners[(int)direction];
    }

    public static Vector3 GetSecondHillCorner(HexDirection direction)
    {
        return hillCorners[(int)direction + 1];
    }
}