using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MapGenerator {

    public static HexType[,] GenerateMap(int width, int height, int islandSizeMin, int islandSizeMax, int numIslands, float fractionHills, float fractionForest, int forestSizeMin, int forestSizeMax, float fractionDesert, int desertSizeMin, int desertSizeMax, int numCivs, out List<HexCoordinates> civStartPoints)
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
                } while (allIslandTiles.Contains(coords));

                int xVal = coords.X;
                if(xVal >= width)
                {
                    xVal -= width;
                }
                else if(xVal < 0)
                {
                    xVal += width;
                }

                int zVal = coords.Z;
                if (zVal >= height)
                {
                    zVal -= height;
                }
                else if (zVal < 0)
                {
                    zVal += height;
                }

                coords = new HexCoordinates(xVal, zVal);

                islandTiles.Add(coords);
                map[coords.X, coords.Z] = HexType.types[HexType.typeKeys.plains];

                List<HexCoordinates> neighbours = HexCoordinates.GetNeighbours(coords);

                for (int hc = 0; hc < neighbours.Count; hc++)
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

            foreach(HexCoordinates possTile in possibleTiles)
            {
                //HexCoordinates possTileOffset = HexCoordinates.ToOffsetCoordinates(possTile);
                if (possTile.X >= 0 && possTile.X < width && possTile.Z >= 0 && possTile.Z < height && map[possTile.X,possTile.Z] == null)
                {
                    map[possTile.X, possTile.Z] = HexType.types[HexType.typeKeys.coast];
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
            map[coords.X, coords.Z] = HexType.types[HexType.typeKeys.hill];
            hillTiles.Add(coords);
        }
        allIslandTiles.ExceptWith(hillTiles);

        HashSet<HexCoordinates> desertTiles = GenerateZones(out map, width, height, HexType.typeKeys.desert, fractionDesert, desertSizeMin, desertSizeMax, numAllIslandTiles, allIslandTiles, map);
        allIslandTiles.ExceptWith(desertTiles);

        HashSet<HexCoordinates> forestTiles = GenerateZones(out map, width, height, HexType.typeKeys.forest, fractionForest, forestSizeMin, forestSizeMax, numAllIslandTiles, allIslandTiles, map);
        allIslandTiles.ExceptWith(forestTiles);

        civStartPoints = getStartPoints(allIslandTiles, numCivs, width, height);

        return map;
    }

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

    static HashSet<HexCoordinates> GenerateEncompassingZones(out HexType[,] mapOut, int width, int height, HexType.typeKeys type, float fractionType, int typeSizeMin, int typeSizeMax, int minDistFromEquator, int maxDistFromEquator, int numAllIslandTiles, HashSet<HexCoordinates> allIslandTiles, HexType[,] map)
    {
        mapOut = map;

        HashSet<HexCoordinates> typeTiles = new HashSet<HexCoordinates>();
        for(int i = 0; i < numAllIslandTiles * fractionType / (typeSizeMax - typeSizeMin + 1); i++)
        {
            int zoneSize = Random.Range(typeSizeMin, typeSizeMax);
        }

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

        HashSet<HexCoordinates> thisTileNeighbours = allIslandTiles;
        thisTileNeighbours.IntersectWith(HexCoordinates.GetNeighbours(centreCoords));
        foreach(HexCoordinates hc in thisTileNeighbours)
        {

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
            while ((startPoints.Intersect(HexCoordinates.GetTwoTileRad(newPoint)).Count() > 0) || startPoints.Contains(newPoint));
            startPoints.Add(newPoint);
        }
        return startPoints;
    }
}
