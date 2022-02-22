using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ChunkLoader))]
public class TerrainOverview : MonoBehaviour
{
    private ChunkLoader chunkLoader;
    private void Awake()
    {
        chunkLoader = GetComponent<ChunkLoader>();
        if (chunkLoader.chunkObject.GetComponent<Terrain>() == null)
            Debug.LogError("chunkLoader.chunkObject must have a terrain Terrain component");
    }
    public float SampleHeight(Vector3 pos)
    {
        return chunkLoader.chunks[chunkLoader.chunkSettings.worldPosToCoord(pos)].GetComponent<Terrain>().SampleHeight(pos);
    }
    public float SampleSteepness(Vector3 pos)
    {
        var terrainData = chunkLoader.chunks[chunkLoader.chunkSettings.worldPosToCoord(pos)].GetComponent<Terrain>().terrainData;
        return terrainData.GetSteepness((pos.x % terrainData.size.x) * terrainData.heightmapResolution / terrainData.size.x, (pos.z % terrainData.size.z) * terrainData.heightmapResolution / terrainData.size.z);
    }
}
