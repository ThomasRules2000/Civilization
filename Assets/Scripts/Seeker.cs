using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : MonoBehaviour {

    public float movementSpeed = 5;
    public bool moveSeeker = false;
    public HexCoordinates target;

    HexGrid grid;
    List<HexCell> path;
	// Use this for initialization
	void Start ()
    {
        grid = gameObject.GetComponentInParent<HexGrid>();
	}
	
	// Update is called once per frame

	public void Update()
    {
        if (path != null && path.Count != 0 && moveSeeker) //Follow Path
        {
            if(transform.position.x == path[0].transform.position.x && transform.position.z == path[0].transform.position.z)
            {
                path.RemoveAt(0);
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
            HexCoordinates.ToOffsetCoordinates(target), true, true);
        grid.path = path;
    }
}
