using Unity.Mathematics;
using UnityEngine;
using ZS.Tools;

public class TerrainChunk : MonoBehaviour
{
    public float scale = 0.002f;
    public int octaves = 5;
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    private TerrainData terrainData;

    // Start is called before the first frame update
    void Start()
    {
        var terrain = gameObject.GetComponent<Terrain>();
        terrainData = TerrainDataCloner.Clone(terrain.terrainData);
        GenerateHeightmap();

        terrain.terrainData = terrainData;
        gameObject.GetComponent<TerrainCollider>().terrainData = terrainData;
    }

    private void GenerateHeightmap()
    {
        var position = transform.position;
        var size = terrainData.size;
        var heightmapResolution = terrainData.heightmapResolution;
        ThreadedDataRequester.RequestData(() =>
        {
            float[,] heightmap = MakeHeightmap(position, size, heightmapResolution);
            return heightmap;
        },
        (heightmap) => terrainData.SetHeights(0, 0, (float[,])heightmap)
        );
    }
    private float[,] MakeHeightmap(Vector3 position, Vector3 size, int heightmapResolution)
    {
        var heightmap = new float[heightmapResolution, heightmapResolution];
        var coord = new float2(position.x / size.x, position.z / size.z).yx;
        var offset = coord * heightmapResolution - coord;
        for (int x = 0; x < heightmapResolution; x++)
        {
            for (int y = 0; y < heightmapResolution; y++)
            {
                heightmap[x, y] = Noise.SimplexNoise_EX(new float2(x, y) + offset, scale, octaves, persistance, lacunarity);
            }
        }

        return heightmap;
    }
    // Update is called once per frame
    void Update()
    {

    }
    void OnValidate()
    {
        GenerateHeightmap();
    }
}
