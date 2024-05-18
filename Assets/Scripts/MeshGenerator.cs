using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        MeshData meshData = new MeshData(width, height);

        int levelOfDetail = MapGenerator.Ins.levelOfDetail;
        int meshSimplicationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;

        // Center the mesh
        float topLeftX = (width - 1) / -2f;
        float topLeftY = (height - 1) / 2f;

        int verticeIndex = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                meshData.vertices[verticeIndex] = new Vector3(topLeftX + x, MapGenerator.Ins.heightCurve.Evaluate(heightMap[x, y]) * MapGenerator.Ins.heightScale, topLeftY - y);
                meshData.uvs[verticeIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(verticeIndex, verticeIndex + height, verticeIndex + height + 1);
                    meshData.AddTriangle(verticeIndex, verticeIndex + height + 1, verticeIndex + 1);
                }

                verticeIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    private int triangleIndex;

    public MeshData(int width, int height)
    {
        vertices = new Vector3[width * height];
        triangles = new int[(height - 1) * (width - 1) * 6];
        uvs = new Vector2[width * height];
        triangleIndex = 0;
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;

        triangleIndex += 3;
    }

    public Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs,
            name = "Terrain"
        };
        mesh.RecalculateNormals();

        return mesh;
    }
}
