using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    HexCell[,] cells;

    HexMesh hexMesh;

    void Awake()
    {
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[HexMetrics.chunkSizeX,HexMetrics.chunkSizeZ];
    }

    void Start ()
    {
        hexMesh.Triangulate(cells);
	}

    public void AddCell(int x, int z, HexCell cell)
    {
        cells[x,z] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
		//cell.uiRect.SetParent(gridCanvas.transform, false);
    }

    public void Refresh()
    {
        hexMesh.Triangulate(cells);
    }
}
