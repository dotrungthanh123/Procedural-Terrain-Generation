using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    // Return mesh data instead of mesh because this is called in thread and new mesh must be called outside of thread
    public static MeshData GenerateTerrainMesh(float[,] heightMap)
    {
        int chunkSize = MapGenerator.chunkSize;
        int levelOfDetail = MapGenerator.Ins.levelOfDetail;
        int meshSimplicationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int verticesPerLine = (chunkSize - 1) / meshSimplicationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

        // Center the mesh
        float topLeftX = (chunkSize - 1) / -2f;
        float topLeftY = (chunkSize - 1) / 2f;

        int verticeIndex = 0;

        for (int x = 0; x < chunkSize; x += meshSimplicationIncrement)
        {
            for (int y = 0; y < chunkSize; y += meshSimplicationIncrement)
            {
                meshData.vertices[verticeIndex] = new Vector3(topLeftX + x, MapGenerator.Ins.heightCurve.Evaluate(heightMap[x, y]) * MapGenerator.Ins.heightScale, topLeftY - y);
                meshData.uvs[verticeIndex] = new Vector2(x / (float)chunkSize, y / (float)chunkSize);

                if (x < chunkSize - 1 && y < chunkSize - 1)
                {
                    meshData.AddTriangle(verticeIndex, verticeIndex + verticesPerLine, verticeIndex + verticesPerLine + 1);
                    meshData.AddTriangle(verticeIndex, verticeIndex + verticesPerLine + 1, verticeIndex + 1);
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
