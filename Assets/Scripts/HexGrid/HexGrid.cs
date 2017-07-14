﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public Text labelPrefab;
    Canvas gridCanvas;
    public LineRenderer pathRenderer;

    public bool showCoords = true;

    public int width = 6;
    public int height = 6;

    public int islandSizeMin = 3;
    public int islandSizeMax = 5;
    public int numIslands = 1;
    public float fractionHills = 0.2f;
    public float fractionForest = 0.2f;
    public float fractionDesert = 0.2f;

    public float hillHeight = 2f;
    public int forestSizeMin = 1;
    public int forestSizeMax = 10;
    public int desertSizeMin = 1;
    public int desertSizeMax = 10;
    public int maxDesertDistFromEquator = 5;

    public int numCivs = 2;
    public Unit defaultUnit;

    public HexCell cellPrefab;

    public HexCell[,] cells;

    HexType[,] map;

    HexMesh hexMesh;
    void Awake()
    {
        if (fractionHills + fractionForest + fractionDesert > 1)
        {
            throw new System.Exception("Total Fractions of Tiles must be less than 1!");
        }

        pathRenderer = GetComponent<LineRenderer>();

        Player player = GetComponent<Player>();

        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        
        cells = new HexCell[width,height];

        HexCoordinates[] civStartPoints = new HexCoordinates[numCivs];
        map = MapGenerator.GenerateMap(width, height, islandSizeMin, islandSizeMax, numIslands, fractionHills, fractionForest, forestSizeMin, forestSizeMax, fractionDesert, desertSizeMin, desertSizeMax, numCivs, out civStartPoints);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                CreateCell(x, z, map[x,z]);
            }
        }

        for(int i = 0; i < numCivs; i++)
        {
            Debug.Log(civStartPoints[i].ToString());
            Vector3 cellPos = cells[civStartPoints[i].X, civStartPoints[i].Z].transform.position;
            Unit unit = Instantiate<Unit>(defaultUnit, cellPos + Vector3.up, Quaternion.identity, transform);
            unit.UnitCivilization = Civilizations.civs[Random.Range(0, Civilizations.defaultCivsLength)];
            if(i == 0)
            {
                player.unit = unit;
                player.PlayerCivilization = unit.UnitCivilization;
            }
            else
            {
                unit.gameObject.AddComponent<AI>();
            }
        }
    }

    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    public int MaxSize
    {
        get
        {
            return width * height;
        }
    }

    //Adds cells to array based on metrics, max width & max height
    void CreateCell(int x, int z, HexType type)
    {
        Vector3 pos;
        pos.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRad * 2f);
        pos.y = 0f;
        pos.z = z * HexMetrics.outerRad * 1.5f;

        HexCell cell = cells[x,z] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = pos;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.name = "Hex Cell " + cell.coordinates.ToString();
        if (type != null)
        {
            cell.Type = type;
        }
        else
        {
            cell.Type = HexType.types[HexType.typeKeys.ocean];
        }

        if (showCoords)
        {
            Text label = Instantiate<Text>(labelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
        }     
    }

    public void UpdateLine(List<HexCell> path, HexCell currentCell)
    {
        if(path == null)
        {
            pathRenderer.positionCount = 0;
            return;
        }

        List<HexCell> tempPath = new List<HexCell>(); //Needed so that unit path is not modified, reducing tiles per turn
        tempPath.AddRange(path);
        if (currentCell != null)
        {
            tempPath.Insert(0, currentCell);
        }
        pathRenderer.material.mainTextureScale = new Vector2(tempPath.Count / 1.5f, 1);
        pathRenderer.positionCount = tempPath.Count;
        pathRenderer.SetPositions(Pathfinding.toVector3(tempPath));
    }
}
