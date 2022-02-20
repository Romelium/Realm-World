using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class Heightmap
{
    public float scale = 1;
    public int octaves = 10;
    public float persistance = 0.5f;
    public float lacunarity = 2f;

    public int additionalOctaves = 1;
    public float octavesOffset = 1000f;

    public float[,] Generate(int heightmapResolution, Vector3 position, Vector3 size)
    {
        var heightmap = new float[heightmapResolution, heightmapResolution];
        var coord = new float2(position.x / size.x, position.z / size.z).yx;
        var offset = coord * heightmapResolution - coord;
        for (int x = 0; x < heightmapResolution; x++)
        {
            for (int y = 0; y < heightmapResolution; y++)
            {
                heightmap[x, y] = Noise.SimplexNoise_EX(new float2(x, y) + offset, scale, octaves, persistance, lacunarity, additionalOctaves, octavesOffset);
            }
        }

        return heightmap;
    }
}