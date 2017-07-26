using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

    HexGrid grid;
    Unit unit;

    // Use this for initialization
    void Start ()
    {
        grid = GetComponentInParent<HexGrid>();
        unit = GetComponent<Unit>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(unit.Path == null || unit.Path.Count == 0)
        {
            unit.target = new HexCoordinates(Random.Range(0, grid.width - 1), Random.Range(0, grid.height - 1));
            unit.UpdatePath();
        }
	}
}
