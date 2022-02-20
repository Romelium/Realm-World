using System;
using Unity.Mathematics;
using UnityEngine;
using ZS.Tools;

public class TerrainChunk : MonoBehaviour
{
    public float scale = 0.002f;
    public int octaves = 5;
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    public SplatSetting[] splatSettings;
    private TerrainData terrainData;

    // Start is called before the first frame update
    void Start()
    {
        var terrain = gameObject.GetComponent<Terrain>();
        terrainData = TerrainDataCloner.Clone(terrain.terrainData);

        GenerateChunk();

        terrain.terrainData = terrainData;
        gameObject.GetComponent<TerrainCollider>().terrainData = terrainData;
    }

    private void GenerateChunk()
    {
        GenerateHeightmap((heightmap) =>
        {
            terrainData.SetHeights(0, 0, heightmap);
            GenerateSplatmap(heightmap, (splatmap) =>
            {
                terrainData.SetAlphamaps(0, 0, splatmap);
            });
        });
    }

    private void GenerateHeightmap(Action<float[,]> Callback)
    {
        var position = transform.position;
        var size = terrainData.size;
        var heightmapResolution = terrainData.heightmapResolution;
        ThreadedDataRequester.RequestData(
            () =>
            {
                // Warning everything here will be run on different thread
                // SO THERE WILL BE NO ERRORS GOING TO SHOW UP IF SOMETHING GO WRONG

                return MakeHeightmap(position, size, heightmapResolution);
            },
            (heightmap) => Callback((float[,])heightmap)
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

    private void GenerateSplatmap(float[,] heightmap, Action<float[,,]> Callback)
    {
        var position = transform.position;
        var size = terrainData.size;
        var heightmapResolution = terrainData.heightmapResolution;
        var splatmapResolution = terrainData.alphamapResolution;
        var terrainLayersLength = terrainData.terrainLayers.Length;
        var heightScale = terrainData.size.y * heightmapResolution / ((terrainData.size.x + terrainData.size.z) / 2);
        ThreadedDataRequester.RequestData(
            () =>
            {
                // Warning everything here will be run on different thread
                // SO THERE WILL BE NO ERRORS GOING TO SHOW UP IF SOMETHING GO WRONG

                return Splat.GenerateSplatMap(splatSettings, heightmap, heightScale, heightmapResolution, splatmapResolution, terrainLayersLength); ;
            },
            (heightmap) => Callback((float[,,])heightmap)
        );
    }
    // Update is called once per frame
    void Update()
    {

    }
    void OnValidate()
    {
        if (terrainData != null)
        {
            GenerateChunk();
        }
    }
}
