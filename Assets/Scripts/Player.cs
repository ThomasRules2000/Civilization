using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Pathfinding pathfinding;

    private void Start()
    {
        pathfinding = gameObject.GetComponent<Pathfinding>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetMouseButton(1))
        {
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
        HexCoordinates coords = HexCoordinates.FromPosition(pos);
        Debug.Log("Touched " + coords.ToString());
        pathfinding.target = coords;
    }
}
