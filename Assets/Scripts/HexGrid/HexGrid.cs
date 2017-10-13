using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public Text labelPrefab;
    //Canvas gridCanvas;
    public LineRenderer[] pathRenderer;

    //public bool showCoords = true;

    public int width = 6;
    public int height = 6;

    public int chunkCountX = 10;
    public int chunkCountZ = 10;
    public HexGridChunk chunkPrefab;

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
    public int tundraHeight = 3;

    public List<Cloud> clouds;
    public float cloudFadeTime;

    public int numCivs = 2;
    public List<Civilization> civsInGame = new List<Civilization>();
    public Transform defaultUnit;

    public City defaultCity;

    public HexCell cellPrefab;

    public HexCell[,] cells;
    public HexGridChunk[,] chunks;

    HexType[,] map;

    public MinimapCamera minimapCamera;

    //HexMesh hexMesh;
    void Awake()
    {
        //Error Checks
        if (fractionForest + fractionDesert > 1)
        {
            throw new System.Exception("Total Fractions of Types must be less than 1!");
        }
        if (numCivs > Civilizations.defaultCivsLength)
        {
            throw new System.Exception("Too Many Civs in Game! Must be less than " + Civilizations.defaultCivsLength);
        }

        pathRenderer = GetComponentsInChildren<LineRenderer>();

        Player player = GetComponent<Player>();

        //gridCanvas = GetComponentInChildren<Canvas>();
        //hexMesh = GetComponentInChildren<HexMesh>();

        width = chunkCountX * HexMetrics.chunkSizeX;
        height = chunkCountZ * HexMetrics.chunkSizeZ;

        chunks = new HexGridChunk[chunkCountX, chunkCountZ];
        cells = new HexCell[width,height];

        List<HexCoordinates> civStartPoints = new List<HexCoordinates>(numCivs);
        List<HexCoordinates> hillCoords = new List<HexCoordinates>();
        map = MapGenerator.GenerateMap(width, height, islandSizeMin, islandSizeMax, numIslands, fractionHills, fractionForest, forestSizeMin, forestSizeMax, fractionDesert, desertSizeMin, desertSizeMax, tundraHeight, numCivs, out civStartPoints, out hillCoords);

        for (int x = 0; x < chunkCountX; x++) //Chunks
        {
            for (int z = 0; z < chunkCountZ; z++)
            {
                HexGridChunk chunk = chunks[x, z] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }

        for (int x = 0; x < width; x++) //Cells
        {
            for (int z = 0; z < height; z++)
            {
                if(hillCoords.Contains(new HexCoordinates(x, z)))
                {
                    CreateCell(x, z, map[x, z], true);
                }
                else
                {
                    CreateCell(x, z, map[x, z], false);
                }        
            }
        }

        for(int i = 0; i <= numCivs; i++)
        {
            HexCell cell = cells[civStartPoints[i].X, civStartPoints[i].Z];
            Vector3 cellPos = cell.transform.position;
            Transform unitTrans = Instantiate<Transform>(defaultUnit, cellPos + Vector3.up, Quaternion.identity, transform);
            Settler settler = unitTrans.gameObject.AddComponent<Settler>();
            do
            {
                settler.UnitCivilization = Civilizations.civs[Random.Range(0, Civilizations.defaultCivsLength)];
            } while (civsInGame.Contains(settler.UnitCivilization));
            civsInGame.Add(settler.UnitCivilization);

            Debug.Log(settler.UnitCivilization.ToString() + ": " + civStartPoints[i].ToString());

            settler.name = settler.UnitCivilization.Nationality + " Settler";
            settler.currentCell = cell;

            if(i == 0)
            {
                player.unit = settler;
                player.PlayerCivilization = settler.UnitCivilization;
                //unit.UpdateVisiblility(new HexCoordinates(5,5), unit.transform.position, 3);
                RevealMap(cell.coordinates, 3);     
            }
            else
            {
                settler.gameObject.AddComponent<UnitAI>();
            }
            Debug.Log("End of adding a Unit");
        }
    }

    public int MaxSize
    {
        get
        {
            return width * height;
        }
    }

    /// <summary>
    /// Adds cells to array based on metrics, max width & max height
    /// </summary>
    /// <param name="x">X coord of cell in array</param>
    /// <param name="z">Z coord of cell in array</param>
    /// <param name="type"></param>
    /// <param name="isHill"></param>
    void CreateCell(int x, int z, HexType type, bool isHill)
    {
        Vector3 pos;
        pos.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRad * 2f);
        pos.y = 0f;
        pos.z = z * HexMetrics.outerRad * 1.5f;

        HexCell cell = cells[x,z] = Instantiate<HexCell>(cellPrefab);
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
        cell.isHill = isHill;

        /*if (showCoords)
        {
            Text label = Instantiate<Text>(labelPrefab);
            //label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
        }*/

        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX,chunkZ];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX, localZ, cell);
    }

    /// <summary>
    /// Updates the drawn line showing the current unit's path
    /// </summary>
    /// <param name="path"></param>
    /// <param name="currentCell">Cell the unit is in</param>
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

    public void RevealMap(HexCoordinates centreCoords, int radius)
    {
        List<HexGridChunk> toUpdate = new List<HexGridChunk>();

        HexCoordinates offsetCentreCoords = HexCoordinates.ToOffsetCoordinates(centreCoords);

        foreach (HexCoordinates coords in HexCoordinates.GetNTileRad(centreCoords, radius))
        {
            HexCoordinates offsetCoords = HexCoordinates.ToOffsetCoordinates(coords);

            if (offsetCoords.Z >= height || offsetCoords.Z < 0)
            {
                continue;
            }

            int xVal = offsetCoords.X;
            if (xVal >= width)
            {
                xVal -= width;
            }
            else if (xVal < 0)
            {
                xVal += width;
            }

            cells[xVal, offsetCoords.Z].IsVisible = true;
            if (!toUpdate.Contains(cells[xVal, offsetCoords.Z].chunk))
            {
                toUpdate.Add(cells[xVal, offsetCoords.Z].chunk);
            }
        }

        foreach(HexGridChunk chunk in toUpdate)
        {
            chunk.Refresh();
        }
    }
}
