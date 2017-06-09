using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public Text labelPrefab;
    Canvas gridCanvas;

    public int width = 6;
    public int height = 6;

    public int islandSizeMin = 3;
    public int islandSizeMax = 5;
    public int numIslands = 1;

    public HexCell cellPrefab;

    public HexCell[,] cells;

    HexType[,] map;

    HexMesh hexMesh;
    void Awake()
    {
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

    public List<HexCoordinates> getNeighbours(HexCoordinates coords)
    {
        HexCoordinates offsetCoords = HexCoordinates.ToOffsetCoordinates(coords);

        List<HexCoordinates> neighbours = new List<HexCoordinates>();

        if (offsetCoords.X + 1 < width)
        {
            neighbours.Add(new HexCoordinates(coords.X + 1, coords.Z));
        }

        if (offsetCoords.X - 1 >= 0)
        {
            neighbours.Add(new HexCoordinates(coords.X - 1, coords.Z));
        }

        if (offsetCoords.Z + 1 < height)
        {
            neighbours.Add(new HexCoordinates(coords.X, coords.Z + 1));
        }

        if (offsetCoords.Z - 1 >= 0)
        {
            neighbours.Add(new HexCoordinates(coords.X, coords.Z - 1));
        }

        if (offsetCoords.X + 1 < width && offsetCoords.Z - 1 >= 0)
        {
            neighbours.Add(new HexCoordinates(coords.X + 1, coords.Z - 1));
        }

        if (offsetCoords.X - 1 >= 0 && offsetCoords.Z + 1 < height)
        {
            neighbours.Add(new HexCoordinates(coords.X - 1, coords.Z + 1));
        }

        return neighbours;
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

        Text label = Instantiate<Text>(labelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(pos.x, pos.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }

    HexType[,] generateMap(int width, int height, int islandSizeMin, int islandSizeMax, int numIslands)
    {
        HexType[,] map = new HexType[width,height];

        for(int i = 0; i < numIslands; i++)
        {
            int centrex = Random.Range(0, width);
            int centrez = Random.Range(0, height);

            HexCoordinates centreCoords = new HexCoordinates(centrex, centrez);

            //Debug.Log(centreCoords.ToString());

            int numIslandTiles = Random.Range(islandSizeMin, islandSizeMax+1);

            //Debug.Log(numIslandTiles);

            map[centrex, centrez] = HexType.types[HexType.typeKeys.plains];

            List<HexCoordinates> possibleTiles = new List<HexCoordinates>();
            List<HexCoordinates> islandTiles = new List<HexCoordinates>();

            //islandTiles.Add(centreCoords);

            possibleTiles.AddRange(getNeighbours(centreCoords));

            for (int j = 0; j < numIslandTiles && possibleTiles.Count > 0; j++)
            {
                int tileIndex = Random.Range(0,possibleTiles.Count);
                HexCoordinates coords = possibleTiles[tileIndex];
                //Debug.Log(coords.ToString());
                possibleTiles.RemoveAt(tileIndex);
                islandTiles.Add(coords);
                if (coords.X >= width || coords.X < 0 || coords.Z >= height || coords.Z < 0)
                {
                    continue;
                }
                map[coords.X, coords.Z] = HexType.types[HexType.typeKeys.plains];

                List<HexCoordinates> neighbours = getNeighbours(coords);

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
        }
        return map;
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
