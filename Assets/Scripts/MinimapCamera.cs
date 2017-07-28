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

        float yPos = zPos / Mathf.Tan(Mathf.PI / 6);

        transform.position = new Vector3(xPos, yPos, zPos);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
