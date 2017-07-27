using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour {

    public HexGrid grid;

	// Use this for initialization
	void Start ()
    {
        float xPos = (grid.width - 1) * HexMetrics.innerRad;
        float zPos = (grid.height - 1) * HexMetrics.outerRad * 0.75f;

        Debug.Log(xPos.ToString() + ", " + zPos.ToString());

        transform.position = new Vector3(xPos, 700, zPos);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
