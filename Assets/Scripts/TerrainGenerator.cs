using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : Singleton<TerrainGenerator>
{
    public const float maxViewDistance = 600;
    public Transform viewer;
    public Material terrainMaterial;

    public static Vector2 viewerPosition;
    
    private int chunkSize;
    private int chunksVisible;
    private Dictionary<Vector2, TerrainChunk> chunkSpawned;
    private List<TerrainChunk> chunksShowned;

    private void Start() {
        chunkSize = MapGenerator.chunkSize;
        chunksVisible = Mathf.FloorToInt(maxViewDistance / chunkSize);
        chunkSpawned = new Dictionary<Vector2, TerrainChunk>();
        chunksShowned = new List<TerrainChunk>();
    }

    private void Update() {
        viewerPosition = new Vector3(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    private void UpdateVisibleChunks() {
        int currentChunkCoordX = Mathf.FloorToInt(viewerPosition.x / chunkSize); 
        int currentChunkCoordY = Mathf.FloorToInt(viewerPosition.y / chunkSize);

        foreach (TerrainChunk chunk in chunksShowned)
        {
            // UpdateVisible should work here to turn off only chunk out of sight
            // dont know why not work
            chunk.SetVisible(false);
        }

        chunksShowned.Clear();

        for (int x = -chunksVisible; x <= chunksVisible; x++)
        {
            for (int y = -chunksVisible; y <= chunksVisible; y++)
            {
                Vector2 chunkCoord = new Vector2(currentChunkCoordX + x, currentChunkCoordY + y);

                if (!chunkSpawned.ContainsKey(chunkCoord)) {
                    chunkSpawned[chunkCoord] = new TerrainChunk(chunkCoord, chunkSize, terrainMaterial);
                }

                chunkSpawned[chunkCoord].UpdateVisible();
                
                if (chunkSpawned[chunkCoord].Visible) {
                    chunksShowned.Add(chunkSpawned[chunkCoord]);
                }
            }
        }
    }

    public class TerrainChunk {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public TerrainChunk(Vector2 coord, int size, Material material) {
            position = coord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            bounds = new Bounds(position, Vector2.one * size);
            
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.parent = Ins.transform;
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size / 10f; // Base size of plane is 10 so divide by 10 to get size 1
            SetVisible(false);

            Generate(OnMapDataReceived);
        }

        private void Generate(Action<MapData> callback) {
            MapGenerator.Ins.RequestMapData(position, callback);
        }

        // Go to Generate -> RequestMapData -> MapDataCallback -> MapThreadInfo -> Executed in Update
        private void OnMapDataReceived(MapData mapData) {
            meshRenderer.material.mainTexture = TextureGenerator.GenerateTextureFromColor(mapData.colors, mapData.heightMap);
            RequestMeshData(mapData, OnMeshDataReceived);
        }

        private void RequestMeshData(MapData mapData, Action<MeshData> callback) {
            MapGenerator.Ins.RequestMeshData(mapData.heightMap, callback);
        }

        private void OnMeshDataReceived(MeshData meshData) {
            Mesh mesh = meshData.GenerateMesh();
            meshFilter.mesh = mesh;
        }

        public void UpdateVisible() {
            float distanceToViewer = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = distanceToViewer <= maxViewDistance;
            if (visible != Visible) SetVisible(visible);
            meshObject.transform.localScale = Vector3.one;
        }

        public void SetVisible(bool visible) {
            meshObject.SetActive(visible);
        }

        public bool Visible => meshObject.activeSelf;
    }

}
