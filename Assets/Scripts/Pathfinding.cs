using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public static int landToWaterCost = 2;

    static HexGrid grid;

    void Awake()
    {
        grid = gameObject.GetComponent<HexGrid>();
    }

    public static List<HexCell> FindPath(HexCoordinates startPos, HexCoordinates targetPos, bool canWaterTravel, bool canLandTravel, int tilesPerTurn, Civilization unitCiv, bool isMilitary) //A* Algorithm
    {
        //Debug.Log(targetPos.ToString());
        HexCell startNode = grid.cells[startPos.X, startPos.Z];
        HexCell targetNode = grid.cells[targetPos.X, targetPos.Z];

        Heap<HexCell> openSet = new Heap<HexCell>(grid.MaxSize);
        HashSet<HexCell> closedSet = new HashSet<HexCell>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            HexCell currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                List<HexCell> path = RetracePath(startNode, targetNode);
                return path;
            }

            foreach(HexCoordinates neighbourCoords in (HexCoordinates.GetNeighbours(currentNode.coordinates)))
            {
                //Debug.Log("Neighbour: " + neighbourCoords.ToString());
                HexCoordinates neighbourOffsetCoords = (HexCoordinates.ToOffsetCoordinates(neighbourCoords));
                //Debug.Log(neighbourCoords.ToString() + " ==> " + neighbourOffsetCoords.ToString());

                if (neighbourOffsetCoords.Z >= grid.height || neighbourOffsetCoords.Z < 0)
                {
                    continue;
                }

                int xOffset = neighbourOffsetCoords.X;
                if (xOffset >= grid.width)
                {
                    xOffset -= grid.width;
                }
                else if (xOffset < 0)
                {
                    xOffset += grid.width;
                }

                HexCell neighbour = grid.cells[xOffset, neighbourOffsetCoords.Z];
                if(((!canWaterTravel && neighbour.Type.isWater || !canLandTravel && !neighbour.Type.isWater) && !(neighbour.Type == HexType.types[HexType.typeKeys.city]))
                    || (neighbour.unitCiv != null && neighbour.unitCiv != unitCiv) || (isMilitary && neighbour.militaryUnit != null) || (!isMilitary && neighbour.passiveUnit != null) 
                    || closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newMovementCost = currentNode.gCost + neighbour.Type.movementCost;
                if(currentNode.Type.isWater != neighbour.Type.isWater)
                {
                    newMovementCost += tilesPerTurn - (newMovementCost % tilesPerTurn); //End turn when crossing to land from water or vice versa
                }

                if (newMovementCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCost;
                    neighbour.hCost = GetDistance(neighbour.coordinates, targetPos);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        return null; //If no path found
    }

    static List<HexCell> RetracePath(HexCell startNode, HexCell endNode)
    {
        List<HexCell> path = new List<HexCell>();
        HexCell currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path;
    }

    static float GetDistance(HexCoordinates nodeA, HexCoordinates nodeB)
    {
        float normalDist = Mathf.Max(nodeA.X - nodeB.X, nodeA.Y - nodeB.Y, nodeA.Z - nodeB.Z);

        //If x near left edge, a-b is -ve so add width
        float xLowDist = Mathf.Max(nodeA.X - nodeB.X + grid.width, nodeA.Y - nodeB.Y, nodeA.Z - nodeB.Z);

        //If x near right edge, a-b is +ve so subtract width
        float xHighDist = Mathf.Max(nodeA.X - nodeB.X - grid.width, nodeA.Y - nodeB.Y, nodeA.Z - nodeB.Z);

        return Mathf.Min(normalDist, xLowDist, xHighDist);
    }

    public static Vector3[] toVector3(List<HexCell> path)
    {
        Vector3[] v3List = new Vector3[path.Count];
        for(int i = 0; i < path.Count; i++)
        {
            v3List[i] = path[i].transform.position;
        }
        return v3List;
    }
}
