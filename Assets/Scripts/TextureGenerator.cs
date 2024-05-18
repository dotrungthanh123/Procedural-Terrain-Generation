using System;
using UnityEngine;

public static class TextureGenerator
{

    private static Func<float, Color>[] getColorFunctions = {
        GetNoise,
        GetColor,
    };

    public static Texture GenerateTexture(float[,] noiseMap, DrawMode drawMode) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D mapTexture = new Texture2D(width, height);

        Color[] textureColors = new Color[width * height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                textureColors[x * height + y] = getColorFunctions[Mathf.Clamp((int) drawMode, 0, 1)](noiseMap[x, y]);
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
