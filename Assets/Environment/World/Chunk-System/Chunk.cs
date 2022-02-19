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
    /// Make gameObject to be at chunk world position
    /// </summary>
    public static Chunk Wrap(GameObject gameObject, Transform parent, Vector2Int coord, Vector3 size, bool keepName = false)
    {
        gameObject.transform.parent = parent;
        gameObject.transform.localPosition = new Vector3(coord.x * size.x, 0, coord.y * size.y);

        var chunk = gameObject.AddComponent<Chunk>();
        chunk.coord = coord;
        gameObject.name = coord + (keepName ? ":" + gameObject.name : "");
        return chunk;
    }
    /// <summary>
    /// instantiate gameObject gameObject to be at chunk world position 
    /// </summary>
    public static Chunk InstantiateWrap(GameObject gameObject, Transform parent, Vector2Int coord, Vector3 size, bool keepName = false)
    {
        return Wrap(Instantiate(gameObject), parent, coord, size, keepName);
    }

    private void setVisible(bool value)
    {
        visible = value;
        gameObject.SetActive(value);
    }
}