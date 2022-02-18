using UnityEngine;

[System.Serializable]
public class Chunk : MonoBehaviour
{
    public Vector2Int Coord { get => coord; }
    [SerializeField]
    private Vector2Int coord;
    public bool Visible { get => visible; set => setVisible(value); }
    [SerializeField]
    private bool visible;
    /// <summary>
    /// Create a new chunk
    /// </summary>
    public static Chunk Create(Transform parent, Vector2Int coord, Vector3 size)
    {
        var chunkObject = new GameObject(coord.ToString());
        chunkObject.transform.parent = parent;
        chunkObject.transform.localPosition = new Vector3(coord.x * size.x, 0, coord.y * size.y);

        var chunk = chunkObject.AddComponent<Chunk>();
        chunk.coord = coord;
        return chunk;
    }
    /// <summary>
    /// Set parent of gameObject as chunk and add position to gameObject
    /// </summary>
    public Chunk Wrap(GameObject gameObject)
    {
        gameObject.transform.parent = transform;
        gameObject.transform.position += transform.position;
        return this;
    }
    /// <summary>
    /// Create and Wrap
    /// </summary>
    public static Chunk CreateWrap(GameObject gameObject, Transform parent, Vector2Int coord, Vector3 size)
    {
        return Create(parent, coord, size).Wrap(gameObject);
    }
    /// <summary>
    /// Create and Wrap the instantiate gameObject
    /// </summary>
    public static Chunk CreateWrapInstantiate(GameObject gameObject, Transform parent, Vector2Int coord, Vector3 size)
    {
        return CreateWrap(Instantiate(gameObject), parent, coord, size);
    }

    private void setVisible(bool value)
    {
        visible = value;
        gameObject.SetActive(value);
    }
}