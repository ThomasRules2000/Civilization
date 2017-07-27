using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {

    public bool fadeOut = false;
    HexGrid grid;
    Renderer renderer;
    float alphaPerSecond;

    void Start()
    {
        grid = transform.parent.GetComponentInParent<HexGrid>();
        renderer = GetComponent<Renderer>();

        alphaPerSecond = 1f / grid.cloudFadeTime;
    }

    // Update is called once per frame
    void Update ()
    {
        if (fadeOut)
        {
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, renderer.material.color.a - alphaPerSecond * Time.deltaTime);
        }
	}
}
