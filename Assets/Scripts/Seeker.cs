using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : MonoBehaviour {

    public float movementSpeed = 5;
    public bool moveSeeker = false;

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
        path = grid.path;
        if (path != null && path.Count != 0 && moveSeeker)
        {
            if(transform.position.x == path[0].transform.position.x && transform.position.z == path[0].transform.position.z)
            {
                path.RemoveAt(0);
            }
            else
            {
                float step = movementSpeed * Time.deltaTime / path[0].Type.movementCost;
                HexCoordinates currentCoords = HexCoordinates.FromPosition(transform.position);
                HexCell currentCell = grid.cells[currentCoords.X, currentCoords.Z];
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(path[0].transform.position.x, transform.position.y, path[0].transform.position.z), step);
            }                       
        }
    }
}
