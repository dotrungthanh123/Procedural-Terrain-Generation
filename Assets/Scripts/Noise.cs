using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int width, int height, float zoom, int seed, Vector2 offset, int octaves, float persistance, float lacrunarity)
    {
        float[,] noiseMap = new float[width, height];

        System.Random rand = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            // Random numbers created every refresh 
            // Same seed gives the same number every refresh
            octaveOffsets[i] = new Vector2(rand.Next(-10000, 10000) + offset.x, rand.Next(-10000, 10000) + offset.y);
        }

        if (zoom <= 0)
        {
            zoom = 0.0001f;
        }

        float minHeight = float.MaxValue, maxHeight = float.MinValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float cummulatedHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    // Higher frequency gives more difference between points
                    float sampleX = (x - halfWidth) / zoom * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / zoom * frequency + octaveOffsets[i].y;

                    float noiseHeight = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // Value range from -1 to 1

                    cummulatedHeight += noiseHeight * amplitude;

                    // amplitude = persistance ^ octave
                    // frequency = lacrunarity ^ octave
                    amplitude *= persistance;
                    frequency *= lacrunarity;
                }

                noiseMap[x, y] = cummulatedHeight;

                if (cummulatedHeight > maxHeight)
                {
                    maxHeight = cummulatedHeight;
                }
                if (cummulatedHeight < minHeight)
                {
                    minHeight = cummulatedHeight;
                }
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[x, y]); // Convert value to be between 0 and 1
            }
        }

        return noiseMap;
    }
}
