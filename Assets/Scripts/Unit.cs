using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public float movementSpeed = 5;
    public int tilesPerTurn = 3;
    public int canMoveThisTurn = 3;
    public bool moveUnit = false;
    public HexCoordinates target;
    public bool isMilitary = false;
    Civilization civ;
    public HexCell currentCell;

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

    public Civilization UnitCivilization
    {
        get
        {
            return civ;
        }
        set
        {
            civ = value;
            GetComponent<Renderer>().material.color = civ.PrimaryColour;
        }
    }

	// Use this for initialization
	void Start ()
    {
        grid = GetComponentInParent<HexGrid>();
        player = GetComponentInParent<Player>();
    }
	
	// Update is called once per frame

	public void Update()
    {
        if (path != null && path.Count != 0 && canMoveThisTurn > 0 && moveUnit) //Follow Path
        {
            if(transform.position.x == path[0].transform.position.x && transform.position.z == path[0].transform.position.z) //If at node
            {
                if(civ == player.PlayerCivilization)
                {
                    grid.RevealMap(HexCoordinates.FromPosition(transform.position), 3);
                }

                canMoveThisTurn -= path[0].Type.movementCost;
                //Debug.Log(path[0].Type.ToString() + " " + path[1].Type.ToString());
                if(path.Count > 1 && path[0].Type.isWater != path[1].Type.isWater)
                {
                    canMoveThisTurn = 1;
                }
                HexCell currentCell = path[0];
                path.RemoveAt(0);
                if(player.unit == this)
                {
                    grid.UpdateLine(path, currentCell);
                }
                if(path.Count > 0 && ((path[0].unitCiv != null && path[0].unitCiv != civ) || (isMilitary && path[0].militaryUnit != null) || (!isMilitary && path[0].passiveUnit != null)))
                {
                    UpdatePath();
                }
            }
            else
            {
                float step = movementSpeed * Time.deltaTime / path[0].Type.movementCost;

                float dist = transform.position.x - path[0].transform.position.x;

                if (Mathf.Abs(dist) > HexMetrics.innerRad * 6f) //More than 3 tiles to deal with iffy pathfinding
                {
                    if(grid.width - Mathf.Abs(dist) <= step)
                    {
                        transform.localPosition += Vector3.left * grid.width * HexMetrics.innerRad * 2f * Mathf.Sign(dist);
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-path[0].transform.position.x, transform.position.y, path[0].transform.position.z), step);
                    }
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[0].transform.position.x, transform.position.y, path[0].transform.position.z), step);
                }
            }                       
        }
        else //Move to centre of cell
        {
            currentCell.militaryUnit = null;
            currentCell.unitCiv = null;
            moveUnit = false; //For check script
            float step = movementSpeed * Time.deltaTime * 0.5f;
            HexCoordinates currentCoords = HexCoordinates.ToOffsetCoordinates(HexCoordinates.FromPosition(transform.position));
            currentCell = grid.cells[currentCoords.X, currentCoords.Z];
            Vector3 centre = currentCell.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(centre.x, transform.position.y, centre.z), step);
            currentCell.unitCiv = civ;
            currentCell.militaryUnit = this;
        }
    }

    public void UpdatePath()
    {
        path = Pathfinding.FindPath(new HexCoordinates(Mathf.RoundToInt(transform.localPosition.x / (HexMetrics.innerRad * 2f)), Mathf.RoundToInt(transform.localPosition.z / (HexMetrics.outerRad * 1.5f))),
            HexCoordinates.ToOffsetCoordinates(target), true, true, tilesPerTurn, civ, isMilitary);
        HexCoordinates currentCoords = HexCoordinates.ToOffsetCoordinates(HexCoordinates.FromPosition(transform.localPosition));
        if (grid.cells[currentCoords.X, currentCoords.Z].Type.isWater != path[0].Type.isWater && canMoveThisTurn > 1)
        {
            canMoveThisTurn = 1;
        }
        if(player.unit == this)
        {
            grid.UpdateLine(path, grid.cells[currentCoords.X, currentCoords.Z]);
        }
    }
}
