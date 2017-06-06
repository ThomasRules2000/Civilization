using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    HexGrid grid;

    private void Awake()
    {
        grid = gameObject.GetComponent<HexGrid>();
    }

    void FindPath(HexCoordinates startPos, HexCoordinates targetPos, bool canWaterTravel, bool canLandTravel)
    {
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
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                return;
            }

            foreach(HexCoordinates neighbourCoords in grid.getNeighbours(currentNode.coordinates))
            {
                HexCell neighbour = grid.cells[neighbourCoords.X, neighbourCoords.Z];
                if(((!canWaterTravel && neighbour.Type.isWater || !canLandTravel && !neighbour.Type.isWater) && !(neighbour.Type == HexType.types[HexType.typeKeys.city])) || closedSet.Contains(neighbour))
                {
                    continue;
                }

                float newMovementCost = currentNode.gCost + neighbour.Type.movementCost;
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
    }

    float GetDistance(HexCoordinates nodeA, HexCoordinates nodeB)
    {
        return Mathf.Max(Mathf.Abs(nodeA.X - nodeB.X), Mathf.Abs(nodeA.Y - nodeB.Y), Mathf.Abs(nodeA.Z - nodeB.Z));
    }
}
