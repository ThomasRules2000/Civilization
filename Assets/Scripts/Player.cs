﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float zoomSpeed = 1000;
    public float maxZoomedOut = 500;
    public float maxZoomedIn = 50;
    public float topRotation = 90;
    public float bottomRotation = 60;

    float rotationStep;

    Pathfinding pathfinding;
    Seeker seeker;

    private void Start()
    {
        pathfinding = gameObject.GetComponent<Pathfinding>();
        seeker = pathfinding.seeker.GetComponent<Seeker>();

        rotationStep = zoomSpeed * (topRotation-bottomRotation) / (maxZoomedOut - maxZoomedIn);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,maxZoomedOut,Camera.main.transform.position.z);
        Camera.main.transform.rotation = Quaternion.Euler(topRotation, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
    }

    void Update ()
    {
        //Mouse Click Pathfinding
        if (Input.GetMouseButtonUp(1))
        {
            seeker.moveSeeker = true;
        }
        else if (Input.GetMouseButton(1))
        {
            seeker.moveSeeker = false;
            HandleInput();
        }

        //Mouse Wheel Zoom
        float d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            //Zoom In
            if (Camera.main.transform.position.y - zoomSpeed > maxZoomedIn)
            {
                Camera.main.transform.Translate(new Vector3(0, -zoomSpeed, zoomSpeed / (rotationStep * 40)), Space.World);
                Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x - rotationStep, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            }
        }
        else if (d < 0f)
        {
            //Zoom Out
            if (Camera.main.transform.position.y + zoomSpeed < maxZoomedOut)
            {
                Camera.main.transform.Translate(new Vector3(0, zoomSpeed, -zoomSpeed / (rotationStep * 40)), Space.World);
                Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x + rotationStep, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
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
