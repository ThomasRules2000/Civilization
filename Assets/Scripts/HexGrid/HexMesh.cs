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

    MeshCollider meshCollider;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
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

        if(cell.isHill)
        {
            Vector3 hillCentre = new Vector3(centre.x, centre.y + HexMetrics.hillHeight, centre.z);
            Color shadedCol = new Color(cell.colour.r * 0.8f, cell.colour.g * 0.8f, cell.colour.b * 0.8f);

            //Add 6 (smaller) triangles
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                Vector3 firstGroundCorner = HexMetrics.GetFirstCorner(d);
                Vector3 secondGroundCorner = HexMetrics.GetSecondCorner(d);
                Vector3 firstHillCorner = HexMetrics.GetFirstHillCorner(d);
                Vector3 secondHillCorner = HexMetrics.GetSecondHillCorner(d);
                 
                //Smaller Central Triangles
                AddTriangle(hillCentre, hillCentre + firstHillCorner, hillCentre + secondHillCorner);
                AddTriangleColour(cell.colour);
                linePoints.Add(hillCentre + HexMetrics.GetFirstHillCorner(d) + Vector3.up * 0.1f);

                //Side triangles
                AddTriangle(hillCentre + firstHillCorner, centre + firstGroundCorner, centre + secondGroundCorner);
                AddTriangle(centre + secondGroundCorner, hillCentre + secondHillCorner, hillCentre + firstHillCorner);

                if(d <= HexDirection.E)
                {
                    AddTriangleColour(shadedCol);
                    AddTriangleColour(shadedCol);
                }
                else
                {
                    AddTriangleColour(cell.colour);
                    AddTriangleColour(cell.colour);
                }
                

                //Lines up the hill
                LineRenderer connectorLine = Instantiate<LineRenderer>(linePrefab);
                connectorLine.transform.name = cell.coordinates.ToString() + " " + System.Enum.GetName(typeof(HexDirection), d) + " Line";
                connectorLine.transform.parent = transform;
                connectorLine.positionCount = 2;
                connectorLine.SetPositions(new Vector3[]
                {
                    hillCentre + HexMetrics.GetFirstHillCorner(d) + Vector3.up * 0.1f,
                    centre + HexMetrics.GetFirstCorner(d) + Vector3.up * 0.1f
                });
            }
        }
        else
        {
            //Add 6 triangles
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                AddTriangle(centre, centre + HexMetrics.GetFirstCorner(d), centre + HexMetrics.GetSecondCorner(d));
                AddTriangleColour(cell.colour);
                linePoints.Add(centre + HexMetrics.GetFirstCorner(d) + Vector3.up * 0.1f);
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
