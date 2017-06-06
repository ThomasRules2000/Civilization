using System.Collections;
using System.Collections.Generic;
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

    HexCell[] cells;

    HexType[,] map;

    HexMesh hexMesh;
    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        
        cells = new HexCell[height * width];

        map = generateMap(width, height, islandSizeMin, islandSizeMax, numIslands);

        for (int x = 0, i = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                CreateCell(x, z, i++, map[x,z]);
            }
        }
    }

    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    //Adds cells to array based on metrics, max width & max height
    void CreateCell(int x, int z, int i, HexType type)
    {
        Vector3 pos;
        pos.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRad * 2f);
        pos.y = 0f;
        pos.z = z * HexMetrics.outerRad * 1.5f;

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
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
            int centrey = -centrex - centrez;

            Debug.Log(centrex.ToString() + ", " + centrey.ToString() + "," + centrez.ToString());

            int numIslandTiles = Random.Range(islandSizeMin, islandSizeMax+1);

            Debug.Log(numIslandTiles);

            map[centrex, centrez] = HexType.types[HexType.typeKeys.plains];

            List<HexCoordinates> possibleTiles = new List<HexCoordinates>();
            List<HexCoordinates> islandTiles = new List<HexCoordinates>();

            islandTiles.Add(new HexCoordinates(centrex, centrez));

            if(centrex + 1 < width)
            {
                possibleTiles.Add(new HexCoordinates(centrex + 1, centrez));
            }

            if (centrex - 1 >= 0)
            {
                possibleTiles.Add(new HexCoordinates(centrex - 1, centrez));
            }

            if (centrez + 1 < height)
            {
                possibleTiles.Add(new HexCoordinates(centrex, centrez + 1));
            }

            if (centrez - 1 >= 0)
            {
                possibleTiles.Add(new HexCoordinates(centrex, centrez - 1));
            }

            if (centrex + 1 < width && centrez - 1 >= 0)
            {
                possibleTiles.Add(new HexCoordinates(centrex + 1, centrez - 1));
            }

            if (centrex - 1 >= 0 && centrez + 1 < height)
            {
                possibleTiles.Add(new HexCoordinates(centrex - 1, centrez + 1));
            }

            for (int j = 0; j < numIslandTiles && possibleTiles.Count > 0; j++)
            {
                int tileIndex = Random.Range(0,possibleTiles.Count-1);
                HexCoordinates coords = possibleTiles[tileIndex];
                Debug.Log(coords.ToString());
                possibleTiles.RemoveAt(tileIndex);
                islandTiles.Add(coords);
                map[coords.X, coords.Z] = HexType.types[HexType.typeKeys.plains];

                HexCoordinates xPos = new HexCoordinates(coords.X + 1, coords.Z);
                if (!islandTiles.Contains(xPos) && xPos.X < width)
                {
                    possibleTiles.Add(xPos);
                }

                HexCoordinates xNeg = new HexCoordinates(coords.X - 1, coords.Z);
                if (!islandTiles.Contains(xNeg) && xNeg.X >= 0)
                {
                    possibleTiles.Add(xNeg);
                }

                HexCoordinates zPos = new HexCoordinates(coords.X, coords.Z + 1);
                if (!islandTiles.Contains(zPos) && zPos.Z < height)
                {
                    possibleTiles.Add(zPos);
                }

                HexCoordinates zNeg = new HexCoordinates(coords.X, coords.Z - 1);
                if (!islandTiles.Contains(zNeg) && zNeg.Z >= 0)
                {
                    possibleTiles.Add(zNeg);
                }

                HexCoordinates yPos = new HexCoordinates(coords.X + 1, coords.Z - 1);
                if (!islandTiles.Contains(yPos) && yPos.X < width && yPos.Z >= 0)
                {
                    possibleTiles.Add(yPos);
                }

                HexCoordinates yNeg = new HexCoordinates(coords.X - 1, coords.Z + 1);
                if (!islandTiles.Contains(yNeg) && yNeg.X >= 0 && yNeg.Z < height)
                {
                    possibleTiles.Add(yNeg);
                }
            }


            /* Old Map Generator
             
            for (int j=1; j<=islandSize/2; j++)
            {
                int x = centrex;
                int y = centrey-j;
                int z = centrez+j;
                Debug.Log("Increment X, Decrement Z");
                for (; z != centrez; x++, z--) //inc x dec z to z=0
                {
                    if (x >= 0 && z >= 0 && x < width && z < height)
                    {
                        Debug.Log(x.ToString() + ", " + y.ToString() + ", " + z.ToString());
                        map[x, z] = HexType.types[HexType.typeKeys.plains];
                    }
                }
                Debug.Log("Increment Y, Decrement Z"); 
                for (; y != centrey; y++, z--) //inc y dec z to y=0
                {
                    if (x >= 0 && z >= 0 && x < width && z < height)
                    {
                        Debug.Log(x.ToString() + ", " + y.ToString() + ", " + z.ToString());
                        map[x, z] = HexType.types[HexType.typeKeys.plains];
                    }
                }
                Debug.Log("Increment Y, Decrement X");
                for (; x != centrex; y++, x--) //inc y dec x to x=0
                {
                    if (x >= 0 && z >= 0 && x < width && z < height)
                    {
                        Debug.Log(x.ToString() + ", " + y.ToString() + ", " + z.ToString());
                        map[x, z] = HexType.types[HexType.typeKeys.plains];
                    }
                }
                Debug.Log("Increment Z, Decrement X");
                for (; z != centrex; z++, x--) //inc z dec x to z=0
                {
                    if (x >= 0 && z >= 0 && x < width && z < height)
                    {
                        Debug.Log(x.ToString() + ", " + y.ToString() + ", " + z.ToString());
                        map[x, z] = HexType.types[HexType.typeKeys.plains];
                    }
                }
                Debug.Log("Increment Z, Decrement Y");
                for (; y != centrey; z++, y--) //inc z dec y to y=0
                {
                    if (x >= 0 && z >= 0 && x < width && z < height)
                    {
                        Debug.Log(x.ToString() + ", " + y.ToString() + ", " + z.ToString());
                        map[x, z] = HexType.types[HexType.typeKeys.plains];
                    }
                }
                Debug.Log("Increment X, Decrement Y");
                for (; x != centrex; x++, y--) //inc x dec y to x=0
                {
                    if (x >= 0 && z >= 0 && x < width && z < height)
                    {
                        Debug.Log(x.ToString() + ", " + y.ToString() + ", " + z.ToString());
                        map[x, z] = HexType.types[HexType.typeKeys.plains];
                    }
                }
            }
            */
        }

        return map;
    }
}
