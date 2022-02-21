using System;
using StringExtensions;
using UnityEngine;
using ZS.Tools;

public class TerrainChunk : MonoBehaviour
{
    public string seed = "hello world";
    public Heightmap heightmapSetting;
    public TreePlanter treePlanter;
    public SplatSetting[] splatSettings;
    private TerrainData terrainData;
    private uint chunkSeed;
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
        // Seeding
        uint seedHash = unchecked((uint)seed.GetStableHashCode());
        var random = new Unity.Mathematics.Random(seedHash == 0 ? 1 : seedHash); // zero is not allowed!
        Noise.PermuteTable(random.NextInt);
        chunkSeed = Bitcast.FloatToUInt(transform.position.x) ^ Bitcast.FloatToUInt(transform.position.z) ^ seedHash;

        GenerateHeightmap((heightmap) =>
        {
            terrainData.SetHeights(0, 0, heightmap);
            GenerateSplatmap(heightmap, (splatmap) =>
            {
                terrainData.SetAlphamaps(0, 0, splatmap);
            });
            GenerateTrees(heightmap, (treeInstances) =>
            {
                terrainData.SetTreeInstances(treeInstances, false);
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
                // SO THERE WILL BE A HIGH CHANCE NO ERRORS GOING TO SHOW UP IF SOMETHING GO WRONG

                return heightmapSetting.Generate(heightmapResolution, position, size);
            },
            (heightmap) => Callback((float[,])heightmap)
        );
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
                // SO THERE WILL BE A HIGH CHANCE NO ERRORS GOING TO SHOW UP IF SOMETHING GO WRONG

                return Splat.GenerateSplatMap(splatSettings, heightmap, heightScale, heightmapResolution, splatmapResolution, terrainLayersLength); ;
            },
            (splatmap) => Callback((float[,,])splatmap)
        );
    }
    private void GenerateTrees(float[,] heightmap, Action<TreeInstance[]> Callback)
    {
        var size = terrainData.size;
        var heightmapResolution = terrainData.heightmapResolution;
        var treePrototypesLength = terrainData.treePrototypes.Length;
        var random = new Unity.Mathematics.Random(chunkSeed == 0 ? 1 : chunkSeed); // zero is not allowed!
        Func<float> rng_01 = () => random.NextFloat(1);
        ThreadedDataRequester.RequestData(
            () =>
            {
                // Warning everything here will be run on different thread
                // SO THERE WILL BE A HIGH CHANCE NO ERRORS GOING TO SHOW UP IF SOMETHING GO WRONG

                return treePlanter.GenerateInstances(heightmap, heightmapResolution, treePrototypesLength, size, rng_01); ;
            },
            (treeInstances) => Callback((TreeInstance[])treeInstances)
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
