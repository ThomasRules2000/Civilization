using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public Transform seeker;
    public HexCoordinates target;
    public int landToWaterCost = 2;

    HexGrid grid;

    void Awake()
    {
        grid = gameObject.GetComponent<HexGrid>();
    }

    public void UpdatePath()
    {
        FindPath( new HexCoordinates(Mathf.RoundToInt(seeker.localPosition.x / (HexMetrics.innerRad * 2f)), Mathf.RoundToInt(seeker.localPosition.z / (HexMetrics.outerRad * 1.5f))),
            HexCoordinates.ToOffsetCoordinates(target), true, true);
    }

    void FindPath(HexCoordinates startPos, HexCoordinates targetPos, bool canWaterTravel, bool canLandTravel) //A* Algorithm
    {
        //Debug.Log(targetPos.ToString());
        HexCell startNode = grid.cells[startPos.X, startPos.Z];
        HexCell targetNode = grid.cells[targetPos.X, targetPos.Z];

        List<HexCell> openSet = new List<HexCell>();
        HashSet<HexCell> closedSet = new HashSet<HexCell>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            HexCell currentNode = openSet[0];

            for(int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach(HexCoordinates neighbourCoords in (grid.getNeighbours(currentNode.coordinates)))
            {
                //Debug.Log("Neighbour: " + neighbourCoords.ToString());
                HexCoordinates neighbourOffsetCoords = (HexCoordinates.ToOffsetCoordinates(neighbourCoords));
                //Debug.Log(neighbourCoords.ToString() + " ==> " + neighbourOffsetCoords.ToString());
                if (neighbourOffsetCoords.X >= grid.width || neighbourOffsetCoords.X < 0 || neighbourOffsetCoords.Z >= grid.height || neighbourOffsetCoords.Z < 0)
                {
                    continue;
                }
                HexCell neighbour = grid.cells[neighbourOffsetCoords.X, neighbourOffsetCoords.Z];
                if(((!canWaterTravel && neighbour.Type.isWater || !canLandTravel && !neighbour.Type.isWater) && !(neighbour.Type == HexType.types[HexType.typeKeys.city])) || closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newMovementCost = currentNode.gCost + neighbour.Type.movementCost;
                if(currentNode.Type.isWater != neighbour.Type.isWater)
                {
                    newMovementCost += landToWaterCost;
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
                }
            }
        }
    }

    void RetracePath(HexCell startNode, HexCell endNode)
    {
        List<HexCell> path = new List<HexCell>();
        HexCell currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    float GetDistance(HexCoordinates nodeA, HexCoordinates nodeB)
    {
        return Mathf.Sqrt(((nodeA.X - nodeB.X) * (nodeA.X - nodeB.X)) + (nodeA.Y - nodeB.Y) * (nodeA.Y - nodeB.Y) + (nodeA.Z - nodeB.Z) * (nodeA.Z - nodeB.Z));
    }
}
