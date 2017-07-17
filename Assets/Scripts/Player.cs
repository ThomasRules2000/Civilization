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

    public float camTranslationSpeed = 20;
    public float mouseScrollArea = 20;

    public Transform cameraRig;
    Camera[] cameras;

    float rotationStep;

    int turnNo = 1;
    public Text turnCounterText;
    public Text civNameText;

    HexGrid grid;

    public Unit unit;
    List<Unit> units = new List<Unit>();

    Civilization civ;
    public Civilization PlayerCivilization
    {
        get
        {
            return civ;
        }
        set
        {
            civ = value;
            civNameText.text = civ.ToString();
        }
    }

    private void Start()
    {
        unit = GetComponentInChildren<Unit>();
        units.AddRange(GetComponentsInChildren<Unit>());

        grid = GetComponent<HexGrid>();

        turnCounterText.text = "Turn: " + turnNo;


        cameras = cameraRig.GetComponentsInChildren<Camera>();
        rotationStep = zoomSpeed * (topRotation-bottomRotation) / (maxZoomedOut - maxZoomedIn);
        cameraRig.transform.position = new Vector3(unit.transform.position.x, maxZoomedIn, unit.transform.position.z - (maxZoomedIn / Mathf.Tan(Mathf.Deg2Rad * bottomRotation)));
        cameraRig.transform.rotation = Quaternion.Euler(bottomRotation, cameraRig.transform.eulerAngles.y, cameraRig.transform.eulerAngles.z);

        cameras[0].transform.localPosition = Vector3.zero;
        cameras[0].transform.localRotation = Quaternion.identity;

        cameras[1].transform.localPosition = Vector3.left * grid.width * HexMetrics.innerRad * 2;
        cameras[1].transform.localRotation = Quaternion.identity;
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
                HexCoordinates currentPos = HexCoordinates.ToOffsetCoordinates(HexCoordinates.FromPosition(unit.transform.position));
                grid.UpdateLine(unit.Path, grid.cells[currentPos.X, currentPos.Z]);
            }
        }

        //Mouse Wheel Zoom
        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        if (scrollAxis > 0f)
        {
            //Zoom In
            if (cameraRig.transform.position.y - zoomSpeed >= maxZoomedIn)
            {
                Vector2 camMovement = Vector2.zero;

                Ray inputRay = cameras[0].ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(inputRay, out hit))
                {
                    camMovement = new Vector2((hit.point.x - cameraRig.transform.position.x) * zoomSpeed / panMult, (hit.point.z - cameraRig.transform.position.z) * zoomSpeed / panMult);
                }
                else
                {
                    inputRay = cameras[1].ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(inputRay, out hit))
                    {
                        camMovement = new Vector2((hit.point.x - cameraRig.transform.position.x) * zoomSpeed / panMult, (hit.point.z - cameraRig.transform.position.z) * zoomSpeed / panMult);
                    }
                }

                cameraRig.transform.Translate(new Vector3(camMovement.x, -zoomSpeed, camMovement.y), Space.World);
                cameraRig.transform.rotation = Quaternion.Euler(cameraRig.transform.eulerAngles.x - rotationStep, cameraRig.transform.eulerAngles.y, cameraRig.transform.eulerAngles.z);
            }
        }
        else if (scrollAxis < 0f)
        {
            //Zoom Out
            Vector2 camMovement = Vector2.zero;

            Ray inputRay = cameras[0].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                camMovement = new Vector2((cameraRig.transform.position.x - hit.point.x) * zoomSpeed / panMult, (cameraRig.transform.position.z - hit.point.z) * zoomSpeed / panMult);
            }
            else
            {
                inputRay = cameras[1].ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(inputRay, out hit))
                {
                    camMovement = new Vector2((cameraRig.transform.position.x - hit.point.x) * zoomSpeed / panMult, (cameraRig.transform.position.z - hit.point.z) * zoomSpeed / panMult);
                }
            }

            if (cameraRig.transform.position.y + zoomSpeed <= maxZoomedOut)
            {
                cameraRig.transform.Translate(camMovement.x, +zoomSpeed, camMovement.y, Space.World);
                cameraRig.transform.rotation = Quaternion.Euler(cameraRig.transform.eulerAngles.x + rotationStep, cameraRig.transform.eulerAngles.y, cameraRig.transform.eulerAngles.z);
            }
        }

        float horizAxis = Input.GetAxis("Horizontal") * Time.deltaTime * camTranslationSpeed * (cameraRig.transform.position.y - maxZoomedIn / 2);
        float vertAxis = Input.GetAxis("Vertical") * Time.deltaTime * camTranslationSpeed * (cameraRig.transform.position.y - maxZoomedIn / 2);

        float mPosX = Input.mousePosition.x;
        float mPosY = Input.mousePosition.y;
        if(mPosX >= 0 && mPosX < mouseScrollArea)
        {
            horizAxis = -Time.deltaTime * camTranslationSpeed * (cameraRig.transform.position.y - maxZoomedIn / 2);
        }
        else if(mPosX < Screen.width && mPosX >= Screen.width-mouseScrollArea)
        {
            horizAxis = Time.deltaTime * camTranslationSpeed * (cameraRig.transform.position.y - maxZoomedIn / 2);
        }

        if (mPosY >= 0 && mPosY < mouseScrollArea)
        {
            vertAxis = -Time.deltaTime * camTranslationSpeed * (cameraRig.transform.position.y - maxZoomedIn / 2);
        }
        else if (mPosY < Screen.height && mPosY >= Screen.height - mouseScrollArea)
        {
            vertAxis = Time.deltaTime * camTranslationSpeed * (cameraRig.transform.position.y - maxZoomedIn / 2);
        }

        cameraRig.transform.Translate(horizAxis, 0f, vertAxis, Space.World);
	}

    public void NextTurn()
    {
        foreach(Unit u in units) //Check units aren't moving before next turn
        {
            if (u.moveUnit)
            {
                return;
            }
        }
        turnNo++;
        turnCounterText.text = "Turn: " + turnNo;
        foreach(Unit u in units)
        {
            u.moveUnit = true;
            u.canMoveThisTurn = u.tilesPerTurn;
        }
        HexCoordinates currentPos = HexCoordinates.ToOffsetCoordinates(HexCoordinates.FromPosition(unit.transform.position));
        grid.UpdateLine(unit.Path, grid.cells[currentPos.X, currentPos.Z]);
    }

    bool HandleInput(inputMethods method)
    {
        Ray inputRay = cameras[0].ScreenPointToRay(Input.mousePosition);
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
        else
        {
            inputRay = cameras[1].ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out hit))
            {
                if (method == inputMethods.doPath)
                {
                    return DoPath(hit.point);
                }
                else if (method == inputMethods.selectUnit)
                {
                    return SelectUnit(hit.transform);
                }
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
            Unit transUnit = trans.GetComponent<Unit>();
            if(transUnit.UnitCivilization == civ)
            {
                unit = transUnit;
                return true;
            }
            else
            {
                return false;
            }   
        }
        else
        {
            return false;
        }
    }
}
