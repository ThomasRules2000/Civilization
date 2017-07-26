using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {

    public bool fadeOut = false;
    HexGrid grid;
    Renderer[] renderers;
    float alphaPerSecond;

    void Start()
    {
        grid = transform.parent.GetComponentInParent<HexGrid>();
        renderers = GetComponentsInChildren<Renderer>();

        alphaPerSecond = 1f / grid.cloudFadeTime;
    }

    // Update is called once per frame
    void Update ()
    {
        if (fadeOut)
        {
            for(int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = new Color(renderers[i].material.color.r, renderers[i].material.color.g, renderers[i].material.color.b, renderers[i].material.color.a - alphaPerSecond * Time.deltaTime);
            }
        }
	}
}
