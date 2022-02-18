using UnityEngine;

/// <summary>
/// Settings that don't change chunk to chunk
/// </summary>
[System.Serializable]
public class ChunkSettings
{
    public Vector2 size = new Vector2(100, 100);

    public Vector2Int worldPosToCoord(Vector3 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x / size.x), Mathf.FloorToInt(pos.z / size.y));
    }
    public Vector3 coordToWorldPos(Vector2Int pos)
    {
        return new Vector3(pos.x * size.x, pos.y * size.y);
    }
    public Vector3 worldPosToWorldPosCenter(Vector3 pos)
    {
        return pos + new Vector3(size.x, 0, size.y) / 2;
    }
    public Vector3 coordToWorldPosCenter(Vector2Int pos)
    {
        return worldPosToWorldPosCenter(coordToWorldPos(pos));
    }
}