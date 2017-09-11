using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOWManager : MonoBehaviour
{
    /// <summary>
    /// Texture size in both X and Y.
    /// Should be power of 2.
    /// </summary>
    [SerializeField]
    int textureSize = 1024;
    [SerializeField]
    Color color;
    [SerializeField]
    LayerMask layer;

    Texture2D texture;
    Color[] pixels;
    List<Revealer> revealers;
    int pixelsPerUnit;
    Vector2 centerPixel;

    Renderer renderer;
    Material material;

    static FOWManager instance;

    public static FOWManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;

        renderer = GetComponent<Renderer>();

        if(renderer != null)
        {
            material = renderer.material;
        }
        if(material == null)
        {
            Debug.LogError("No FOW Material!");
            return;
        }

        texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;

        pixels = texture.GetPixels();
        ClearPixels();
        texture.SetPixels(pixels);

        material.mainTexture = texture;

        revealers = new List<Revealer>();

        pixelsPerUnit = Mathf.RoundToInt(textureSize / transform.lossyScale.x);

        centerPixel = new Vector2(textureSize * 0.5f, textureSize * 0.5f);
    }

    public void RevealTile(HexCoordinates coords)
    {
        CreateCircle(coords.Z*pixelsPerUnit, coords.X*pixelsPerUnit, (int)HexMetrics.outerRad);
        texture.SetPixels(pixels);
        material.mainTexture = texture;
    }

    public void SetQuadSize(float x, float z)
    {
        transform.localScale = new Vector3(x * HexMetrics.innerRad*2, (z + 1) * HexMetrics.outerRad * 1.5f, 0);
        transform.position = new Vector3((x - 1) * HexMetrics.innerRad, 10, (z - 1.5f) * HexMetrics.outerRad * 0.75f);
    }

    public void RegisterRevealer(Revealer revealer)
    {
        revealers.Add(revealer);
    }

    void ClearPixels()
    {
        for (var i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
    }

    void CreateCircle(int centreX, int centreY, int radius)
    {
        for (int y = -radius * pixelsPerUnit; y <= radius * pixelsPerUnit; y++)
        {
            for (int x = -radius * pixelsPerUnit; x <= radius * pixelsPerUnit; x++)
            {
                if (x * x + y * y <= (radius * pixelsPerUnit) * (radius * pixelsPerUnit))
                {
                    pixels[(centreY + y) * textureSize + centreX + x] = new Color(0, 0, 0, 0);
                }
            }
        }
    }

    void CreateHex(int centreX, int centreZ)
    {
        int innerRad = Mathf.RoundToInt(HexMetrics.innerRad * pixelsPerUnit);
        int outerRad = Mathf.RoundToInt(HexMetrics.outerRad * pixelsPerUnit);

        for (int x = -innerRad; x <= innerRad; x++)
        {
            for (int z = -outerRad; z <= outerRad; z++)
            {
                if (x * x < 3.0f/4.0f * z * z )
                {
                    pixels[(centreZ + z) * textureSize + centreX + x] = new Color(0, 0, 0, 0);
                }
            }
        }
    }
}
