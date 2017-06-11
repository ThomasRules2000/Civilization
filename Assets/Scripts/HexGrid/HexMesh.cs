using System.Collections;
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
            //Add 6 triangles, centre is higher as hill
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                Vector3 v2 = centre + HexMetrics.GetFirstCorner(d);
                Vector3 v3 = centre + HexMetrics.GetSecondCorner(d);

                HexCoordinates neighbour = HexCoordinates.ToOffsetCoordinates(neighbours[(int)d]);
                if (neighbour.X >= 0 && neighbour.X < grid.width && neighbour.Z >= 0 && neighbour.Z < grid.height)
                {
                    if (grid.cells[neighbour.X, neighbour.Z].Type == HexType.types[HexType.typeKeys.hill])
                    {
                        HexCoordinates prevNeighbour;
                        if (d == HexDirection.NE)
                        {
                            prevNeighbour = HexCoordinates.ToOffsetCoordinates(neighbours[5]);
                        }
                        else
                        {
                            prevNeighbour = HexCoordinates.ToOffsetCoordinates(neighbours[(int)d - 1]);
                        }

                        HexCoordinates nextNeighbour;
                        if (d == HexDirection.NW)
                        {
                            nextNeighbour = HexCoordinates.ToOffsetCoordinates(neighbours[0]);
                        }
                        else
                        {
                            nextNeighbour = HexCoordinates.ToOffsetCoordinates(neighbours[(int)d + 1]);
                        }

                        if (prevNeighbour.X >= 0 && prevNeighbour.X < grid.width && prevNeighbour.Z >= 0 && prevNeighbour.Z < grid.height)
                        {
                            if (grid.cells[prevNeighbour.X, prevNeighbour.Z].Type == HexType.types[HexType.typeKeys.hill])
                            {
                                v2 += Vector3.up * grid.hillHeight;
                            }
                        }

                        if (nextNeighbour.X >= 0 && nextNeighbour.X < grid.width && nextNeighbour.Z >= 0 && nextNeighbour.Z < grid.height)
                        {
                            if (grid.cells[nextNeighbour.X, nextNeighbour.Z].Type == HexType.types[HexType.typeKeys.hill])
                            {
                                v3 += Vector3.up * grid.hillHeight;
                            }
                        }
                    }
                }
                AddTriangle(centre + Vector3.up * grid.hillHeight, v2, v3);
                AddTriangleColour(cell.colour);
                linePoints.Add(v2);
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
