using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [NonSerialized]
    public Renderer meshRenderer;
    [NonSerialized]
    public MeshFilter meshFilter;
    public FilterMode filterMode;
    public TextureWrapMode wrapMode;

    public void DrawTexture(Texture texture) {
        meshRenderer = GetComponent<Renderer>();

        meshRenderer.sharedMaterial.mainTexture = texture;
        meshRenderer.sharedMaterial.mainTexture.wrapMode = wrapMode;
        meshRenderer.sharedMaterial.mainTexture.filterMode = filterMode;
        
        transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(float[,] noiseMap, Texture texture) {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<Renderer>();

        Mesh mesh = MeshGenerator.GenerateTerrainMesh(noiseMap).GenerateMesh();
        meshFilter.sharedMesh = mesh;     
        meshRenderer.sharedMaterial.mainTexture = texture;   
    }
}
