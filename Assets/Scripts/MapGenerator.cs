using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

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

    // Function to be run after getting data
    Queue<MapThreadInfo<MapData>> mapDataThreadInfos = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfos = new Queue<MapThreadInfo<MeshData>>();

    // Create and start new thread for height map and texture
    public void RequestMapData(Vector2 centre, Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            MapDataCallback(centre, callback);
        };

        new Thread(threadStart).Start();
    }

    // Actual function to get height map and texture
    private void MapDataCallback(Vector2 centre, Action<MapData> callback) {
        MapData data = GenerateMapData(centre);
        lock (mapDataThreadInfos) {
            mapDataThreadInfos.Enqueue(new MapThreadInfo<MapData>(callback, data));
        }
    }

    public void RequestMeshData(float[,] heightMap, Action<MeshData> callback) {
        ThreadStart threadStart = delegate {
            MeshCallback(heightMap, callback);
        };

        new Thread(threadStart).Start();
    }

    private void MeshCallback(float[,] heightMap, Action<MeshData> callback) {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap);
        lock (meshDataThreadInfos) {
            meshDataThreadInfos.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        lock (mapDataThreadInfos)
        {
            while (mapDataThreadInfos.Count > 0)
            {
                MapThreadInfo<MapData> info = mapDataThreadInfos.Dequeue();
                info.callback(info.parameter);
            }
        }

        lock (meshDataThreadInfos)
        {
            while (meshDataThreadInfos.Count > 0)
            {
                MapThreadInfo<MeshData> info = meshDataThreadInfos.Dequeue();
                info.callback(info.parameter);
            }
        }

    }

    public void GenerateMap()
    {
        MapData mapData = GenerateMapData();
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if ((int) drawMode < 2) {
            display.DrawTexture(TextureGenerator.GenerateTextureFromColor(mapData.colors, mapData.heightMap));
        } else if (drawMode == DrawMode.Mesh) {
            display.DrawMesh(mapData.heightMap, TextureGenerator.GenerateTextureFromColor(mapData.colors, mapData.heightMap));
        }
    }

    public MapData GenerateMapData() {
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, zoom, seed, offset, octaves, persistance, lacrunarity);
        Color[] colors = TextureGenerator.GenerateColor(noiseMap, drawMode);

        return new MapData(noiseMap, colors);
    }

    public MapData GenerateMapData(Vector2 centre) {
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, zoom, seed, centre + offset, octaves, persistance, lacrunarity);
        Color[] colors = TextureGenerator.GenerateColor(noiseMap, drawMode);

        return new MapData(noiseMap, colors);
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

public struct MapData {
    public float[,] heightMap;
    public Color[] colors;

    public MapData(float[,] heightMap, Color[] colors)
    {
        this.heightMap = heightMap;
        this.colors = colors;
    }
}

// Store value gotten from thread and function that use it
public struct MapThreadInfo<T> {
    public Action<T> callback;
    public T parameter;

    public MapThreadInfo(Action<T> callback, T value)
    {
        this.callback = callback;
        this.parameter = value;
    }
}