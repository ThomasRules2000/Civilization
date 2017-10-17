using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    int turnNo = 1;
    public Text turnCounterText;
    public Text civNameText;

    HexGrid grid;

    public GUIControl guiControl;
    public CameraRig cameraRig;

    public Unit unit;
    public List<Unit> units = new List<Unit>();

    public List<City> cities = new List<City>();

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

    void Start()
    {
        unit = GetComponentInChildren<Unit>();
        //units.AddRange(GetComponentsInChildren<Unit>());

        grid = GetComponent<HexGrid>();

        turnCounterText.text = "Turn: " + turnNo;       
    }

    void Update()
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
                guiControl.updateButtons(unit.actions, unit.actionNames);
            }
        }

        //Mouse Wheel Zoom
        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        if (scrollAxis > 0f)
        {
            cameraRig.ZoomIn();
        }
        else if (scrollAxis < 0f)
        {
            //Zoom Out
            cameraRig.ZoomOut();
        }
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
        RaycastHit hit;
        if(cameraRig.Raycast(Input.mousePosition, out hit))
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
