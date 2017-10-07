using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour {

    public HexGrid grid;
    public float speed = 50;

    int xMin = int.MaxValue;
    public int XMinimum
    {
        get
        {
            return xMin;
        }
        set
        {
            xMin = Mathf.Min(xMin, value);
        }
    }

    int xMax = 0;
    public int XMaximum
    {
        get
        {
            return xMax;
        }
        set
        {
            xMax = Mathf.Max(xMax, value);
        }
    }

    int zMin = int.MaxValue;
    public int ZMinimum
    {
        get
        {
            return zMin;
        }
        set
        {
            zMin = Mathf.Min(zMin, value);
        }
    }

    int zMax = 0;
    public int ZMaximum
    {
        get
        {
            return zMax;
        }
        set
        {
            zMax = Mathf.Max(zMax, value);
        }
    }

    Camera camera;

    Vector3 targetPos = Vector3.zero;

	// Use this for initialization
	void Start ()
    {
        camera = GetComponent<Camera>();

        xMin = grid.width + 1;
        //xMax = grid.width - 1;
        zMin = grid.height + 1;
        //zMax = grid.height - 1;

        UpdatePosition();
	}

    void Update()
    {
        transform.position = Vector3.Slerp(transform.position, targetPos, speed * Time.deltaTime);
    }

    /// <summary>
    /// Updates the position of the minimap camera
    /// </summary>
    public void UpdatePosition()
    {
        float xPos = (xMin + xMax) * HexMetrics.innerRad;
        float zPos = (zMin + zMax) * HexMetrics.outerRad * 0.75f;
        float xDelta = (xMax - xMin + 1) * HexMetrics.innerRad;
        float zDelta = (zMax - zMin + 1) * HexMetrics.outerRad * 0.75f;

        if(xDelta > zDelta * camera.aspect)
        {
            zDelta = xDelta / camera.aspect;
        }

        float yPos = zDelta / Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2);

        targetPos = new Vector3(xPos, yPos, zPos);
    }
}
