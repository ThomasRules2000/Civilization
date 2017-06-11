using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    public float hillHeight = 2f;
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
            for (int i = 0; i < 6; i++)
            {
                Vector3 v2 = centre + HexMetrics.corners[i];
                Vector3 v3 = centre + HexMetrics.corners[i + 1];

                HexCoordinates neighbour = HexCoordinates.ToOffsetCoordinates(neighbours[i]);
                if (neighbour.X >= 0 && neighbour.X < grid.width && neighbour.Z >= 0 && neighbour.Z < grid.height)
                {
                    if (grid.cells[neighbour.X, neighbour.Z].Type == HexType.types[HexType.typeKeys.hill])
                    {
                        HexCoordinates prevNeighbour;
                        if (i == 0)
                        {
                            prevNeighbour = HexCoordinates.ToOffsetCoordinates(neighbours[5]);
                        }
                        else
                        {
                            prevNeighbour = HexCoordinates.ToOffsetCoordinates(neighbours[i - 1]);
                        }

                        HexCoordinates nextNeighbour;
                        if (i == 5)
                        {
                            nextNeighbour = HexCoordinates.ToOffsetCoordinates(neighbours[0]);
                        }
                        else
                        {
                            nextNeighbour = HexCoordinates.ToOffsetCoordinates(neighbours[i + 1]);
                        }

                        if (prevNeighbour.X >= 0 && prevNeighbour.X < grid.width && prevNeighbour.Z >= 0 && prevNeighbour.Z < grid.height)
                        {
                            if (grid.cells[prevNeighbour.X, prevNeighbour.Z].Type == HexType.types[HexType.typeKeys.hill])
                            {
                                v2 += Vector3.up * hillHeight;
                            }
                        }

                        if (nextNeighbour.X >= 0 && nextNeighbour.X < grid.width && nextNeighbour.Z >= 0 && nextNeighbour.Z < grid.height)
                        {
                            if (grid.cells[nextNeighbour.X, nextNeighbour.Z].Type == HexType.types[HexType.typeKeys.hill])
                            {
                                v3 += Vector3.up * hillHeight;
                            }
                        }
                    }
                }
                AddTriangle(centre + Vector3.up * hillHeight, v2, v3);
                AddTriangleColour(cell.colour);
                linePoints.Add(v2);
            }
        }
        else
        {
            //Add 6 triangles
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(centre, centre + HexMetrics.corners[i], centre + HexMetrics.corners[i + 1]);
                AddTriangleColour(cell.colour);
                linePoints.Add(centre + HexMetrics.corners[i]);
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
