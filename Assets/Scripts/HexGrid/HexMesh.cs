using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{

    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colours;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colours = new List<Color>();
    }

    //Create triangles for grid of cells
    public void Triangulate(HexCell[] cells)
    {
        //Clear Mesh 
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colours.Clear();

        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.colors = colours.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();
    }

    void Triangulate(HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;

        //Add 6 triangles
        for (int i = 0; i<6; i++)
        {
            AddTriangle(center, center + HexMetrics.corners[i], center + HexMetrics.corners[i+1]);
            AddTriangleColour(cell.colour);
        }      
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
