using System;
using UnityEngine;

public static class TextureGenerator
{

    private static Func<float, Color>[] getColorFunctions = {
        GetNoise,
        GetColor,
    };

    public static Color[] GenerateColor(float[,] noiseMap, DrawMode drawMode) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Color[] textureColors = new Color[width * height];

        // Fill values from row to row, not column to column
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                textureColors[y * width + x] = getColorFunctions[Mathf.Clamp((int) drawMode, 0, 1)](noiseMap[x, y]);
            }
        }

        return textureColors;
    }

    public static Texture2D GenerateTextureFromColor(Color[] colors, float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D mapTexture = new Texture2D(width, height) {
            wrapMode = MapDisplay.Ins.wrapMode,
            filterMode = MapDisplay.Ins.filterMode,
        };

        mapTexture.SetPixels(colors);
        mapTexture.Apply();

        return mapTexture;
    }

    public static Texture2D GenerateTexture(float[,] noiseMap, DrawMode drawMode) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D mapTexture = new Texture2D(width, height) {
            wrapMode = MapDisplay.Ins.wrapMode,
            filterMode = MapDisplay.Ins.filterMode,
        };

        Color[] textureColors = new Color[width * height];

        // Fill values from row to row, not column to column
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                textureColors[y * width + x] = getColorFunctions[Mathf.Clamp((int) drawMode, 0, 1)](noiseMap[x, y]);
            }
        }

        mapTexture.SetPixels(textureColors);
		mapTexture.Apply();

        return mapTexture;
    }

    private static Color GetNoise(float value) {
        return Color.Lerp(Color.black, Color.white, value);
    }

    private static Color GetColor(float value) {
        TerrainType[] regions = MapGenerator.Ins.regions;
        for (int i = 0; i < regions.Length; i++) {
            if (value <= regions[i].height) {
                return regions[i].color;
            }
        }
        return Color.white;
    }
}
