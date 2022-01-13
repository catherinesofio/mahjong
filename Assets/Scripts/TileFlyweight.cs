using UnityEngine;

public class TileFlyweight : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _data;

    public Sprite this[TileId id]
    {
        get
        {
            return _data[(int)id];
        }
    }
}
