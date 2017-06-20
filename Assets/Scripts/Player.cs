using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float zoomSpeed = 18;
    public float panMult = 180;
    public float maxZoomedOut = 500;
    public float maxZoomedIn = 50;
    public float topRotation = 90;
    public float bottomRotation = 60;

    float rotationStep;

    int turnNo = 1;
    public Text turnCounterText;

    HexGrid grid;

    Unit unit;
    List<Unit> units = new List<Unit>();

    private void Start()
    {
        unit = GetComponentInChildren<Unit>();
        units.AddRange(GetComponentsInChildren<Unit>());

        grid = GetComponent<HexGrid>();

        turnCounterText.text = "Turn: " + turnNo;

        rotationStep = zoomSpeed * (topRotation-bottomRotation) / (maxZoomedOut - maxZoomedIn);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,maxZoomedOut,Camera.main.transform.position.z);
        Camera.main.transform.rotation = Quaternion.Euler(topRotation, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
    }

    void Update ()
    {
        //Space Next Turn
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextTurn();
        }
        
        //Right Mouse Click Pathfinding
        if (Input.GetMouseButtonUp(1))
        {
            unit.moveUnit = true;
        }
        else if (Input.GetMouseButton(1))
        {
            unit.moveUnit = false;
            HandleInput(inputMethods.doPath);
        }

        //Left Mouse Click Unit Selection
        if (Input.GetMouseButton(0))
        { 
            bool success = HandleInput(inputMethods.selectUnit);
            if (success)
            {
                grid.path = unit.Path;
                HexCoordinates currentPos = HexCoordinates.ToOffsetCoordinates(HexCoordinates.FromPosition(unit.transform.position));
                grid.UpdateLine(unit.Path, grid.cells[currentPos.X, currentPos.Z]);
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

    public void NextTurn()
    {
        turnNo++;
        turnCounterText.text = "Turn: " + turnNo;
        foreach(Unit u in units)
        {
            u.canMoveThisTurn = u.tilesPerTurn;
        }
        HexCoordinates currentPos = HexCoordinates.ToOffsetCoordinates(HexCoordinates.FromPosition(unit.transform.position));
        grid.UpdateLine(unit.Path, grid.cells[currentPos.X, currentPos.Z]);
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
        unit.target = coords;
        unit.UpdatePath();
        return true;
    }

    bool SelectUnit(Transform trans)
    {
        if(trans.GetComponent<Unit>() != null)
        {
            unit = trans.GetComponent<Unit>();
            return true;
        }
        else
        {
            return false;
        }
    }
}
