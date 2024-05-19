using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapDisplay : Singleton<MapDisplay>
{
    [Header("Texture Render")]
    public Renderer textureRenderer;
    [Header("Mesh Render")]
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public FilterMode filterMode;
    public TextureWrapMode wrapMode;

    public void DrawTexture(Texture2D texture) {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(float[,] noiseMap, Texture texture) {
        Mesh mesh = MeshGenerator.GenerateTerrainMesh(noiseMap).GenerateMesh();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
