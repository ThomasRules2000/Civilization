using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float zoomSpeed = 18;
    public float panMult = 180;
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
        seeker = GetComponentInChildren<Seeker>();

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
            HandleInput(inputMethods.doPath);
        }

        if (Input.GetMouseButtonUp(0))
        {
            seeker.moveSeeker = true;
        }
        else if (Input.GetMouseButton(0))
        { 
            bool success = HandleInput(inputMethods.selectUnit);
            if (success)
            {
                seeker.moveSeeker = false;
                HandleInput(inputMethods.doPath);
            }          
        }

        //Mouse Wheel Zoom
        float d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            //Zoom In
            if (Camera.main.transform.position.y - zoomSpeed > maxZoomedIn)
            {
                Vector2 camMovement = Vector2.zero;

                Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(inputRay, out hit))
                {
                    camMovement = new Vector2((hit.point.x - Camera.main.transform.position.x) * zoomSpeed / panMult, (hit.point.z - Camera.main.transform.position.z) * zoomSpeed / panMult);
                }

                Camera.main.transform.Translate(new Vector3(camMovement.x, -zoomSpeed, camMovement.y), Space.World);
                Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x - rotationStep, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            }
        }
        else if (d < 0f)
        {
            //Zoom Out
            Vector2 camMovement = Vector2.zero;

            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                camMovement = new Vector2((Camera.main.transform.position.x - hit.point.x) * zoomSpeed / panMult, (Camera.main.transform.position.z - hit.point.z) * zoomSpeed / panMult);
            }

            if (Camera.main.transform.position.y + zoomSpeed < maxZoomedOut)
            {
                Camera.main.transform.Translate(new Vector3(camMovement.x, +zoomSpeed, camMovement.y), Space.World);
                Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x + rotationStep, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            }
        }
	}

    bool HandleInput(inputMethods method)
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            if(method == inputMethods.doPath)
            {
                return DoPath(hit.point);
            }
            else if(method == inputMethods.selectUnit)
            {
                return SelectUnit(hit.transform);
            }
        }
        return false;
    }

    public enum inputMethods
    {
        doPath,
        selectUnit
    }

    bool DoPath(Vector3 pos)
    {
        pos = transform.InverseTransformPoint(pos);
        HexCoordinates coords = (HexCoordinates.FromPosition(pos));
        //Debug.Log("Touched " + coords.ToString());
        seeker.target = coords;
        seeker.UpdatePath();
        return true;
    }

    bool SelectUnit(Transform unit)
    {
        if(unit.GetComponent<Seeker>() != null)
        {
            seeker = unit.GetComponent<Seeker>();
            return true;
        }
        else
        {
            return false;
        }
    }
}
