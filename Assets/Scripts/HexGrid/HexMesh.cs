﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    public LineRenderer linePrefab;

    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colours;
    HexGrid grid;

    MeshCollider meshCollider;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        grid = GetComponentInParent<HexGrid>();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colours = new List<Color>();
    }

    //Create triangles for grid of cells
    public void Triangulate(HexCell[,] cells)
    {
        //Clear Mesh 
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colours.Clear();

        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for(int j = 0; j < cells.GetLength(1); j++)
            {
                Triangulate(cells[i,j], HexCoordinates.GetNeighbours(cells[i,j].coordinates));
            }
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.colors = colours.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }

    void Triangulate(HexCell cell, List<HexCoordinates> neighbours)
    {
        Vector3 centre = cell.transform.localPosition;

        List<Vector3> linePoints = new List<Vector3>();

        if(cell.Type == HexType.types[HexType.typeKeys.hill])
        {
            Vector3 hillCentre = new Vector3(centre.x, centre.y + HexMetrics.hillHeight, centre.z);
            //Add 6 (smaller) triangles
            for(HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                Vector3 firstGroundCorner = HexMetrics.GetFirstCorner(d);
                Vector3 secondGroundCorner = HexMetrics.GetSecondCorner(d);
                Vector3 firstHillCorner = HexMetrics.GetFirstHillCorner(d);
                Vector3 secondHillCorner = HexMetrics.GetSecondHillCorner(d);

                HexCoordinates neighbourCoords = HexCoordinates.ToOffsetCoordinates(neighbours[(int)d]);
                HexCell thisNeighbour = null;
                if (neighbourCoords.X >= 0 && neighbourCoords.X < grid.width && neighbourCoords.Z >= 0 && neighbourCoords.Z < grid.height)
                {
                    thisNeighbour = grid.cells[neighbourCoords.X, neighbourCoords.Z];
                }
                 
                AddTriangle(hillCentre, hillCentre + firstHillCorner, hillCentre + secondHillCorner);
                AddTriangleColour(cell.colour);
                linePoints.Add(hillCentre + HexMetrics.GetFirstHillCorner(d));

                if(thisNeighbour == null || thisNeighbour.Type != HexType.types[HexType.typeKeys.hill])
                {
                    AddTriangle(hillCentre + firstHillCorner, centre + firstGroundCorner, centre + secondGroundCorner);
                    AddTriangleColour(cell.colour);
                    AddTriangle(centre + secondGroundCorner, hillCentre + secondHillCorner, hillCentre + firstHillCorner);
                    AddTriangleColour(cell.colour);
                }
                else
                {
                    AddTriangle(hillCentre + firstHillCorner, hillCentre + firstGroundCorner, hillCentre + secondGroundCorner);
                    AddTriangleColour(cell.colour);
                    AddTriangle(hillCentre + secondGroundCorner, hillCentre + secondHillCorner, hillCentre + firstHillCorner);
                    AddTriangleColour(cell.colour);
                }
            }
        }
        else
        {
            //Add 6 triangles
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                AddTriangle(centre, centre + HexMetrics.GetFirstCorner(d), centre + HexMetrics.GetSecondCorner(d));
                AddTriangleColour(cell.colour);
                linePoints.Add(centre + HexMetrics.GetFirstCorner(d));
            }
        }

        LineRenderer outline = Instantiate<LineRenderer>(linePrefab);
        outline.transform.name = cell.coordinates.ToString() + "Outline";
        outline.transform.parent = transform;
        outline.positionCount = 6;
        outline.SetPositions(linePoints.ToArray());
    }

    //Create corners
    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    //Colours
    void AddTriangleColour(Color colour)
    {
        colours.Add(colour);
        colours.Add(colour);
        colours.Add(colour);
    }
}
