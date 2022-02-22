using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public int radius = 2;
    public ChunkSettings chunkSettings;
    [Tooltip("Instantiate at each chunk")]
    public GameObject chunkObject;
    [Tooltip("Fallback is self")]
    public Transform parent;
    [Tooltip("Fallback is main camera")]
    public Transform viewer;
    [HideInInspector]
    public Vector2Int viewerCoord;
    [HideInInspector]
    public Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private List<Chunk> visibleChunks = new List<Chunk>();

    [Tooltip("Editor Only")]
    public bool showChunkBorder = false;
    void Awake()
    {
        if (viewer == null && Camera.main != null)
            viewer = Camera.main.transform;
        if (parent == null)
            parent = transform;
    }
    void Start()
    {
        viewerCoord = chunkSettings.worldPosToCoord(viewer.position);
        visibleUpdate();
        radiusUpdate();
    }

    void Update()
    {
        var newViewerCoord = chunkSettings.worldPosToCoord(viewer.position);
        if (!newViewerCoord.Equals(viewerCoord))
        {
            viewerCoord = newViewerCoord;
            visibleUpdate();
            radiusUpdate();
        }
    }
    void radiusUpdate()
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                var coord = new Vector2Int(x, y) + viewerCoord;

                if (chunks.ContainsKey(coord))
                {
                    if (visibleCheckAndSet(chunks[coord]) && !visibleChunks.Contains(chunks[coord]))
                        visibleChunks.Add(chunks[coord]);
                }
                else if (visibleCheck(coord))
                {
                    Chunk chunk;
                    if (chunkObject == null)
                    {
                        Debug.LogWarning("ChunkObject is Empty");
                        chunk = Chunk.Wrap(new GameObject("chunkObject Empty"), parent, coord, chunkSettings.size, true);
                    }
                    else
                        chunk = Chunk.InstantiateWrap(chunkObject, parent, coord, chunkSettings.size);
                    chunks.Add(coord, chunk);
                    visibleChunks.Add(chunk);
                }
            }
        }
    }
    public bool visibleCheck(Vector2Int coord)
    {
        return (coord - viewerCoord).sqrMagnitude <= radius * radius;
    }
    public bool visibleCheckAndSet(Chunk chunk)
    {
        chunk.Visible = visibleCheck(chunk.Coord);
        return chunk.Visible;
    }
    void visibleUpdate()
    {
        visibleChunks.RemoveAll(chunk => !visibleCheckAndSet(chunk));
    }

    private void OnDrawGizmos()
    {
        if (showChunkBorder)
        {
            foreach (var chunk in visibleChunks)
            {
                Gizmos.DrawWireCube(chunkSettings.worldPosToWorldPosCenter(chunk.transform.position), new Vector3(chunkSettings.size.x, (chunkSettings.size.x + chunkSettings.size.y) / 2, chunkSettings.size.y));
            }
        }
    }
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            Awake();
            Start();
        }
    }
}
