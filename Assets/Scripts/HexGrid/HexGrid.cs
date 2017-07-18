using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public Text labelPrefab;
    Canvas gridCanvas;
    public LineRenderer[] pathRenderer;

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
    public List<Civilization> civsInGame = new List<Civilization>();
    public Unit defaultUnit;

    public HexCell cellPrefab;

    public HexCell[,] cells;

    HexType[,] map;

    HexMesh hexMesh;
    void Awake()
    {
        //Error Checks
        if (fractionHills + fractionForest + fractionDesert > 1)
        {
            throw new System.Exception("Total Fractions of Tiles must be less than 1!");
        }
        if (numCivs > Civilizations.defaultCivsLength)
        {
            throw new System.Exception("Too Many Civs in Game! Must be less than " + Civilizations.defaultCivsLength);
        }

        pathRenderer = GetComponentsInChildren<LineRenderer>();

        Player player = GetComponent<Player>();

        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        
        cells = new HexCell[width,height];

        List<HexCoordinates> civStartPoints = new List<HexCoordinates>(numCivs);
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
            do
            {
                unit.UnitCivilization = Civilizations.civs[Random.Range(0, Civilizations.defaultCivsLength)];
            } while (civsInGame.Contains(unit.UnitCivilization));
            civsInGame.Add(unit.UnitCivilization);

            unit.name = unit.UnitCivilization.Nationality + " Unit";

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
            pathRenderer[0].positionCount = 0;
            return;
        }

        List<HexCell> tempPath = new List<HexCell>(); //Needed so that unit path is not modified, reducing tiles per turn
        tempPath.AddRange(path);
        if (currentCell != null)
        {
            tempPath.Insert(0, currentCell);
        }

        int breakPos = -1;
        float dist = 0;
        for(int i = 0; i < tempPath.Count - 1; i++)
        {
            dist = tempPath[i].transform.position.x - tempPath[i + 1].transform.position.x;
            if (Mathf.Abs(dist) > HexMetrics.innerRad * 6f)
            {
                breakPos = i;
                break;
            }
        }
        
        if(breakPos > 0)
        {
            List<Vector3> path1 = Pathfinding.toVector3(tempPath).Take(breakPos + 1).ToList();
            path1.Add(tempPath[breakPos + 1].transform.position + Vector3.right * width * HexMetrics.innerRad * 2f * Mathf.Sign(dist));

            pathRenderer[0].material.mainTextureScale = new Vector2((breakPos + 1) / 1.5f, 1);
            pathRenderer[0].positionCount = breakPos + 2;
            pathRenderer[0].SetPositions(path1.ToArray());

            pathRenderer[1].material.mainTextureScale = new Vector2((tempPath.Count - breakPos - 1) / 1.5f, 1);
            pathRenderer[1].positionCount = tempPath.Count - breakPos - 1;
            pathRenderer[1].SetPositions(Pathfinding.toVector3(tempPath).Skip(breakPos + 1).ToArray());
        }
        else
        {
            pathRenderer[0].material.mainTextureScale = new Vector2(tempPath.Count / 1.5f, 1);
            pathRenderer[0].positionCount = tempPath.Count;
            pathRenderer[0].SetPositions(Pathfinding.toVector3(tempPath));

            pathRenderer[1].positionCount = 0;
        }
    }
}
