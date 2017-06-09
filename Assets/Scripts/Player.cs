using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Camera mainCamera;
    public float zoomSpeed = 5000;
    public float maxZoomedOut = 500;
    public float maxZoomedIn = 50;

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
        else
        {
            float d = Input.GetAxis("Mouse ScrollWheel");
            if (d > 0f)
            {
                if (mainCamera.transform.position.y - zoomSpeed*Time.deltaTime > maxZoomedIn)
                {
                    mainCamera.transform.Translate(Vector3.Slerp(transform.position, new Vector3(transform.position.x, transform.position.y - zoomSpeed, transform.position.z), Time.deltaTime),Space.World);
                }
            }
            else if (d < 0f)
            {
                if (mainCamera.transform.position.y + zoomSpeed*Time.deltaTime < maxZoomedOut)
                {
                    mainCamera.transform.Translate(Vector3.Slerp(transform.position, new Vector3(transform.position.x, transform.position.y + zoomSpeed, transform.position.z), Time.deltaTime),Space.World);
                }
            }
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
