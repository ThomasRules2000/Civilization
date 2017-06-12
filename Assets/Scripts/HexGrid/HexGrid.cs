using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public Text labelPrefab;
    Canvas gridCanvas;

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

        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        
        cells = new HexCell[width,height];

        map = generateMap(width, height, islandSizeMin, islandSizeMax, numIslands);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                CreateCell(x, z, map[x,z]);
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

    HexType[,] generateMap(int width, int height, int islandSizeMin, int islandSizeMax, int numIslands)
    {
        HexType[,] map = new HexType[width,height];

        HashSet<HexCoordinates> allIslandTiles = new HashSet<HexCoordinates>();

        for(int i = 0; i < numIslands; i++)
        {
            int centrex;
            int centrez;

            HexCoordinates centreCoords;

            do
            {
                centrex = Random.Range(0, width);
                centrez = Random.Range(0, height);
                centreCoords = new HexCoordinates(centrex, centrez);
            } while (allIslandTiles.Contains(centreCoords));

            //Debug.Log(centreCoords.ToString());

            int numIslandTiles = Random.Range(islandSizeMin, islandSizeMax+1);

            //Debug.Log(numIslandTiles);

            map[centrex, centrez] = HexType.types[HexType.typeKeys.plains];

            List<HexCoordinates> possibleTiles = new List<HexCoordinates>();
            HashSet<HexCoordinates> islandTiles = new HashSet<HexCoordinates>();

            islandTiles.Add(centreCoords);

            possibleTiles.AddRange(HexCoordinates.GetNeighbours(centreCoords));

            for (int j = 0; j < numIslandTiles && possibleTiles.Count > 0; j++)
            {
                int tileIndex;
                HexCoordinates coords;
                do
                {
                    tileIndex = Random.Range(0, possibleTiles.Count);
                    coords = possibleTiles[tileIndex];
                    //Debug.Log(coords.ToString());
                    possibleTiles.RemoveAt(tileIndex);
                } while (coords.X >= width || coords.X < 0 || coords.Z >= height || coords.Z < 0 || allIslandTiles.Contains(coords));
                islandTiles.Add(coords);
                map[coords.X, coords.Z] = HexType.types[HexType.typeKeys.plains];

                List<HexCoordinates> neighbours = HexCoordinates.GetNeighbours(coords);

                for(int hc = 0; hc<neighbours.Count; hc++)
                {
                    HexCoordinates neighbour = neighbours[hc];
                    if (islandTiles.Contains(neighbour))
                    {
                        neighbours.Remove(neighbour);
                    }
                }

                possibleTiles.AddRange(neighbours);
            }
            allIslandTiles.UnionWith(islandTiles);
        }

        int numAllIslandTiles = allIslandTiles.Count();

        HashSet<HexCoordinates> hillTiles = new HashSet<HexCoordinates>();
        for (int i = 0; i < numAllIslandTiles * fractionHills; i++) // Hill Generation
        {
            int hillIndex;
            HexCoordinates coords;
            do
            {
                hillIndex = Random.Range(0, allIslandTiles.Count);
                coords = allIslandTiles.ElementAt(hillIndex);
            } while (hillTiles.Contains(coords));

            //Debug.Log(coords.ToString());
            map[coords.X, coords.Z] = HexType.types[HexType.typeKeys.hill];
            hillTiles.Add(coords);
        }
        allIslandTiles.ExceptWith(hillTiles);

        HashSet<HexCoordinates> forestTiles = GenerateZones(out map, HexType.typeKeys.forest, fractionForest, forestSizeMin, forestSizeMax, numAllIslandTiles, allIslandTiles, map);
        allIslandTiles.ExceptWith(forestTiles);

        HashSet<HexCoordinates> desertTiles = GenerateZones(out map, HexType.typeKeys.desert, fractionDesert, desertSizeMin, desertSizeMax, numAllIslandTiles, allIslandTiles, map);
        allIslandTiles.ExceptWith(desertTiles);

        return map;
    }

    public HashSet<HexCoordinates> GenerateZones(out HexType[,] mapOut, HexType.typeKeys type, float fractionType, int typeSizeMin, int typeSizeMax, int numAllIslandTiles, HashSet<HexCoordinates> allIslandTiles, HexType[,] map)
    {
        mapOut = map;

        HashSet<HexCoordinates> typeTiles = new HashSet<HexCoordinates>();
        for (int i = 0; i < numAllIslandTiles * fractionType / (typeSizeMax - typeSizeMin + 1); i++) //Forest Generation
        {
            int typeSize = Random.Range(typeSizeMin, typeSizeMax);

            int typeCentreIndex;
            HexCoordinates centreCoords;
            do
            {
                typeCentreIndex = Random.Range(0, allIslandTiles.Count);
                centreCoords = allIslandTiles.ElementAt(typeCentreIndex);
            } while (typeTiles.Contains(centreCoords));
            typeTiles.Add(centreCoords);

            List<HexCoordinates> possibleTiles = new List<HexCoordinates>();
            possibleTiles.AddRange(HexCoordinates.GetNeighbours(centreCoords));

            for (int j = 1; j < typeSize; j++)
            {
                int tileIndex;
                HexCoordinates coords;
                do
                {
                    tileIndex = Random.Range(0, possibleTiles.Count);
                    coords = possibleTiles[tileIndex];
                } while (coords.X >= width || coords.X < 0 || coords.Z >= height || coords.Z < 0 || typeTiles.Contains(coords));
                typeTiles.Add(coords);

                mapOut[coords.X, coords.Z] = HexType.types[type];

                List<HexCoordinates> neighbours = HexCoordinates.GetNeighbours(coords);

                for (int hc = 0; hc < neighbours.Count; hc++)
                {
                    HexCoordinates neighbour = neighbours[hc];
                    if (typeTiles.Contains(neighbour))
                    {
                        neighbours.Remove(neighbour);
                    }
                }
                possibleTiles.AddRange(neighbours);
            }
        }

        return typeTiles;
    }

    public List<HexCell> path;
    void OnDrawGizmos()
    {
        if(cells != null)
        {
            foreach(HexCell c in cells)
            {
                Gizmos.color = Color.white;
                if (path != null && path.Contains(c))
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawCube(new Vector3((c.coordinates.X + c.coordinates.Z / 2f) * HexMetrics.innerRad * 2, 1, c.coordinates.Z * HexMetrics.outerRad * 1.5f), Vector3.one * 5);
            }
        }
    }
}
