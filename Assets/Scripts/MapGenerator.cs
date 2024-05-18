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
    public int width;
    public int height;
    public float zoom;
    public int octaves;
    public float persistance; // Reduce later octave influence
    public float lacrunarity; // Add more detail
    public int seed;
    public Vector2 offset;
    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, zoom, seed, offset, octaves, persistance, lacrunarity);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if ((int) drawMode < 2) {
            display.DrawTexture(TextureGenerator.GenerateTexture(noiseMap, drawMode));
        } else if (drawMode == DrawMode.Mesh) {
            display.DrawMesh(noiseMap, TextureGenerator.GenerateTexture(noiseMap, drawMode));
        }
    }

    // Call when script's values change
    private void OnValidate()
    {
        if (width < 1)
        {
            width = 1;
        }
        if (height < 1)
        {
            height = 1;
        }
        if (octaves < 1)
        {
            octaves = 1;
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