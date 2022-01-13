using System;
using System.Collections.Generic;
using UnityEngine;

// IN PROCESS! HAS TEMP THINGS THAT WILL BE CHANGED!
[RequireComponent(typeof(SpriteRenderer))]
public class NodeView : MonoBehaviour
{
    private int _id;
    private bool _isEmpty;
    private Vector2 _coordinates;
    [SerializeField]
    private SpriteRenderer _typeSprite;

    private Action<NodeView> _callback;

    [SerializeField]
    private List<NodeView> _neighbours;

    #region Properties
    public List<NodeView> Neighbours
    {
        get
        {
            return _neighbours;
        }
    }

    public bool IsEmpty
    {
        get
        {
            return _isEmpty;
        }
    }

    public Vector2 Coordinates
    {
        get
        {
            return _coordinates;
        }
    }
    #endregion

    private void Awake()
    {
        _neighbours = new List<NodeView>();
    }

    private void Start()
    {
        // UNIFY THE LEVEL INFO LATER ON!!
        var level = GameObject.FindObjectOfType<ScreenManager>().Level;
        var tiles = GameObject.FindObjectOfType<DataManager>()[level].tilesData.sprites;

        _id = UnityEngine.Random.Range(0, tiles.Length - 1);
        _typeSprite.sprite = tiles[_id];
    }

    private void OnMouseDown()
    {
        if (!_isEmpty)
        {
            _callback(this);
        }
    }

    #region Setters
    internal void SetIsEmpty(bool isEmpty)
    {
        _isEmpty = isEmpty;
    }

    internal void SetCoordinates(Vector2 coordinates)
    {
        _coordinates = coordinates;
    }

    internal void SetCallback(Action<NodeView> callback)
    {
        _callback = callback;
    }

    internal void AddNeighbour(NodeView neighbour)
    {
        _neighbours.Add(neighbour);
    }
    #endregion
}
