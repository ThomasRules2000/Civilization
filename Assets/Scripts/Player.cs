using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Pathfinding pathfinding;
    Seeker seeker;

    private void Start()
    {
        pathfinding = gameObject.GetComponent<Pathfinding>();
        seeker = pathfinding.seeker.GetComponent<Seeker>();
    }

    void Update ()
    {
        if (Input.GetMouseButtonUp(1))
        {
            seeker.moveSeeker = true;
        }
        else if (Input.GetMouseButton(1))
        {
            seeker.moveSeeker = false;
            HandleInput();
        }
	}

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            TouchCell(hit.point);
        }
    }

    void TouchCell(Vector3 pos)
    {
        pos = transform.InverseTransformPoint(pos);
        HexCoordinates coords = (HexCoordinates.FromPosition(pos));
        //Debug.Log("Touched " + coords.ToString());
        pathfinding.target = coords;
        pathfinding.UpdatePath();
    }
}
