using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public float movementSpeed = 5;
    public int tilesPerTurn = 3;
    public int canMoveThisTurn = 3;
    public bool moveUnit = false;
    public HexCoordinates target;

    HexGrid grid;
    List<HexCell> path;

    Player player;

    public List<HexCell> Path
    {
        get
        {
            return path;
        }
    }

	// Use this for initialization
	void Start ()
    {
        grid = gameObject.GetComponentInParent<HexGrid>();
        player = gameObject.GetComponentInParent<Player>();
	}
	
	// Update is called once per frame

	public void Update()
    {
        if (path != null && path.Count != 0 && canMoveThisTurn > 0 && moveUnit) //Follow Path
        {
            if(transform.position.x == path[0].transform.position.x && transform.position.z == path[0].transform.position.z)
            {
                canMoveThisTurn -= path[0].Type.movementCost;
                //Debug.Log(path[0].Type.ToString() + " " + path[1].Type.ToString());
                if(path.Count > 1 && canMoveThisTurn > 1 && path[0].Type.isWater != path[1].Type.isWater)
                {
                    canMoveThisTurn = path[0].Type.movementCost + 1;
                }
                HexCell currentCell = path[0];
                path.RemoveAt(0);
                if(player.unit == this)
                {
                    grid.UpdateLine(path, currentCell);
                }
            }
            else
            {
                float step = movementSpeed * Time.deltaTime / path[0].Type.movementCost;
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(path[0].transform.position.x, transform.position.y, path[0].transform.position.z), step);
            }                       
        }
        else //Move to centre of cell
        {
            float step = movementSpeed * Time.deltaTime * 0.5f;
            HexCoordinates currentCoords = HexCoordinates.ToOffsetCoordinates(HexCoordinates.FromPosition(transform.position));
            Vector3 centre = grid.cells[currentCoords.X, currentCoords.Z].transform.position;
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(centre.x, transform.position.y, centre.z), step);
        }
    }

    public void UpdatePath()
    {
        path = Pathfinding.FindPath(new HexCoordinates(Mathf.RoundToInt(transform.localPosition.x / (HexMetrics.innerRad * 2f)), Mathf.RoundToInt(transform.localPosition.z / (HexMetrics.outerRad * 1.5f))),
            HexCoordinates.ToOffsetCoordinates(target), true, true, tilesPerTurn);
        HexCoordinates currentCoords = HexCoordinates.ToOffsetCoordinates(HexCoordinates.FromPosition(transform.localPosition));
        if(grid.cells[currentCoords.X, currentCoords.Z].Type.isWater != path[0].Type.isWater)
        {
            canMoveThisTurn = 1;
        }
        grid.path = path;
        grid.UpdateLine(path, grid.cells[currentCoords.X, currentCoords.Z]);
    }
}
