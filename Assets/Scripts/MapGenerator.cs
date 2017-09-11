using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MapGenerator {
    /// <summary>
    /// Generates the map and returns an array of HexTypes
    /// </summary>
    /// <param name="width">in tiles</param>
    /// <param name="height">in tiles</param>
    /// <param name="islandSizeMin">Min tiles for island</param>
    /// <param name="islandSizeMax">Max ties for island</param>
    /// <param name="numIslands">Number of islands to generate</param>
    /// <param name="fractionHills"></param>
    /// <param name="fractionForest"></param>
    /// <param name="forestSizeMin">Min tiles for forests</param>
    /// <param name="forestSizeMax">Max tiles for forests</param>
    /// <param name="fractionDesert"></param>
    /// <param name="desertSizeMin">Min tiles for deserts</param>
    /// <param name="desertSizeMax">Max tiles for deserts</param>
    /// <param name="tundraHeight">How many tiles from top and bottom</param>
    /// <param name="numCivs"></param>
    /// <param name="civStartPoints">Returned starting positions</param>
    /// <param name="hillCoords">Returned coords for hills</param>
    /// <returns></returns>
    public static HexType[,] GenerateMap(int width, int height, int islandSizeMin, int islandSizeMax, int numIslands, float fractionHills, float fractionForest, int forestSizeMin, int forestSizeMax, float fractionDesert, int desertSizeMin, int desertSizeMax, int tundraHeight, int numCivs, out List<HexCoordinates> civStartPoints, out List<HexCoordinates> hillCoords)
    {
        HexType[,] map = new HexType[width, height];

        HashSet<HexCoordinates> allIslandTiles = new HashSet<HexCoordinates>();

        for (int i = 0; i < numIslands; i++)
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

            int numIslandTiles = Random.Range(islandSizeMin, islandSizeMax + 1);

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
                } while (coords.Z >= height || coords.Z < 0 || allIslandTiles.Contains(coords));

                int xVal = coords.X;
                if(xVal >= width)
                {
                    xVal -= width;
                }
                else if(xVal < 0)
                {
                    xVal += width;
                }

                coords = new HexCoordinates(xVal, coords.Z);

                islandTiles.Add(coords);

                if (coords.Z < tundraHeight - 1 || coords.Z > height - tundraHeight)
                {
                    map[coords.X, coords.Z] = HexType.types[HexType.typeKeys.tundra];
                }
                else if((coords.Z == tundraHeight - 1 || coords.Z == height - tundraHeight) && Random.value < 0.5f)
                {
                    map[coords.X, coords.Z] = HexType.types[HexType.typeKeys.tundra];
                }
                else
                {
                    map[coords.X, coords.Z] = HexType.types[HexType.typeKeys.plains];
                }

                List<HexCoordinates> neighbours = HexCoordinates.GetNeighbours(coords);

                int hc = 0;
                while(hc < neighbours.Count)
                {
                    HexCoordinates neighbour = neighbours[hc];
                    if (islandTiles.Contains(neighbour))
                    {
                        neighbours.RemoveAt(hc);
                    }
                    else
                    {
                        hc++;
                    }
                }

                possibleTiles.AddRange(neighbours);
            }
            allIslandTiles.UnionWith(islandTiles);

            foreach(HexCoordinates possTile in possibleTiles)
            {
                int xVal = possTile.X;
                if (xVal >= width)
                {
                    xVal -= width;
                }
                else if (xVal < 0)
                {
                    xVal += width;
                }

                if (possTile.Z >= 0 && possTile.Z < height && map[xVal,possTile.Z] == null)
                {
                    map[xVal, possTile.Z] = HexType.types[HexType.typeKeys.coast];
                }
            }
        }

        int numAllIslandTiles = allIslandTiles.Count;

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
            hillTiles.Add(coords);
        }
        hillCoords = hillTiles.ToList();

        HashSet<HexCoordinates> desertTiles = GenerateZones(out map, width, height, HexType.typeKeys.desert, fractionDesert, desertSizeMin, desertSizeMax, numAllIslandTiles, allIslandTiles, map);
        allIslandTiles.ExceptWith(desertTiles);

        HashSet<HexCoordinates> forestTiles = GenerateZones(out map, width, height, HexType.typeKeys.forest, fractionForest, forestSizeMin, forestSizeMax, numAllIslandTiles, allIslandTiles, map);
        allIslandTiles.ExceptWith(forestTiles);

        civStartPoints = getStartPoints(allIslandTiles, numCivs, width, height);

        return map;
    }
    /// <summary>
    /// Generates zones such as forests and deserts
    /// </summary>
    /// <param name="mapOut">The updated map</param>
    /// <param name="width">in tiles</param>
    /// <param name="height">in tiles</param>
    /// <param name="type">of tile</param>
    /// <param name="fractionType"></param>
    /// <param name="typeSizeMin">Min tiles per area</param>
    /// <param name="typeSizeMax">Max tiles per area</param>
    /// <param name="numAllIslandTiles"></param>
    /// <param name="allIslandTiles"></param>
    /// <param name="map">input map</param>
    /// <returns></returns>
    static HashSet<HexCoordinates> GenerateZones(out HexType[,] mapOut, int width, int height, HexType.typeKeys type, float fractionType, int typeSizeMin, int typeSizeMax, int numAllIslandTiles, HashSet<HexCoordinates> allIslandTiles, HexType[,] map)
    {
        mapOut = map;

        HashSet<HexCoordinates> typeTiles = new HashSet<HexCoordinates>();
        for (int i = 0; i < numAllIslandTiles * fractionType / (typeSizeMax - typeSizeMin + 1); i++) //Type Generation
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

                int xVal = coords.X;
                if (xVal >= width)
                {
                    xVal -= width;
                }
                else if (xVal < 0)
                {
                    xVal += width;
                }

                coords = new HexCoordinates(xVal, coords.Z);

                typeTiles.Add(coords);

                if(map[xVal,coords.Z] == HexType.types[HexType.typeKeys.plains])
                {
                    mapOut[xVal, coords.Z] = HexType.types[type];
                }
                
                List<HexCoordinates> neighbours = HexCoordinates.GetNeighbours(coords);

                int hc = 0;
                while (hc < neighbours.Count)
                {
                    HexCoordinates neighbour = neighbours[hc];
                    if (typeTiles.Contains(neighbour))
                    {
                        neighbours.RemoveAt(hc);
                    }
                    else
                    {
                        hc++;
                    }
                }
                possibleTiles.AddRange(neighbours);
            }
        }

        return typeTiles;
    }

    static List<HexCoordinates> getStartPoints(HashSet<HexCoordinates> islandTiles, int numCivs, int width, int height)
    {
        List<HexCoordinates> startPoints = new List<HexCoordinates>(numCivs);
        for(int i = 0; i < numCivs; i++)
        {
            HexCoordinates newPoint;
            do
            {
                newPoint = islandTiles.ElementAt<HexCoordinates>(Random.Range(0, islandTiles.Count));
            }
            while ((startPoints.Intersect(HexCoordinates.GetNTileRad(newPoint,4)).Count() > 0) || startPoints.Contains(newPoint));
            startPoints.Add(newPoint);
        }
        return startPoints;
    }
}
