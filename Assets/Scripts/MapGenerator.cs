using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DrawMode
{
    NoiseMap = 0,
    ColorMap = 1,
    Mesh = 2,
}

public class MapGenerator : Singleton<MapGenerator>
{

    public DrawMode drawMode;
    public const int chunkSize = 241;
    public float zoom;
    [Tooltip("Number of layers")]
    public int octaves;
    [Tooltip("Later octave influence")]
    public float persistance; // Reduce later octave influence
    [Tooltip("More detail")]
    public float lacrunarity; // Add more detail
    public int seed;
    public Vector2 offset;
    public float heightScale;
    public AnimationCurve heightCurve;
    [Range(0, 6)]
    [Tooltip("Increase this reduce level of detail")]
    public int levelOfDetail; // Increase this reduce level of detail
    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, zoom, seed, offset, octaves, persistance, lacrunarity);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if ((int) drawMode < 2) {
            display.DrawTexture(TextureGenerator.GenerateTexture(noiseMap, drawMode));
        } else if (drawMode == DrawMode.Mesh) {
            display.DrawMesh(noiseMap, drawMode);
        }
    }

    // Call when script's values change
    private void OnValidate()
    {
        if (octaves < 1)
        {
            octaves = 1;
        }
        if (zoom < 1) {
            zoom = 1;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}