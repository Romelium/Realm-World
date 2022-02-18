using Unity.Mathematics;
using UnityEngine;
using ZS.Tools;

public class TerrainChunk : MonoBehaviour
{
    public float scale = 0.002f;
    public int octaves = 5;
    public float persistance = 0.5f;
    public float lacunarity = 2f;

    // Start is called before the first frame update
    void Start()
    {
        var terrain = gameObject.GetComponent<Terrain>();
        var terrainData = TerrainDataCloner.Clone(terrain.terrainData);

        var heightmap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        var coord = new float2(transform.position.x / terrainData.size.x, transform.position.z / terrainData.size.z).yx;
        var offset = coord * terrainData.heightmapResolution - coord;

        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                heightmap[x, y] = Noise.SimplexNoise_EX(new float2(x, y) + offset, scale, octaves, persistance, lacunarity);
            }
        }

        terrainData.SetHeights(0, 0, heightmap);

        terrain.terrainData = terrainData;
        gameObject.GetComponent<TerrainCollider>().terrainData = terrainData;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
