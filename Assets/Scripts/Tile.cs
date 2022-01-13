using System;
using System.Collections.Generic;
using UnityEngine;

// ADD SELECTION EVENT!!
[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    private bool _isEmpty;
    private Vector2 _coordinates;
    private TileId _id;
    [SerializeField]
    private SpriteRenderer _typeSprite;

    private Action<Tile> _callback;

    [SerializeField]
    private List<Tile> _neighbours;

    public List<Tile> Neighbours
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

    private void Awake()
    {
        _neighbours = new List<Tile>();
    }

    internal void SetIsEmpty(bool isEmpty)
    {
        _isEmpty = isEmpty;
    }

    internal void SetCoordinates(Vector2 coordinates)
    {
        _coordinates = coordinates;
    }

    internal void SetId(TileId id)
    {
        _id = id;

        var flyweight = GameObject.FindObjectOfType<TileFlyweight>();
        _typeSprite.sprite = flyweight[id];
    }

    internal void SetCallback(Action<Tile> callback)
    {
        _callback = callback;
    }

    internal void AddNeighbour(Tile neighbour)
    {
        _neighbours.Add(neighbour);
    }

    private void OnMouseDown()
    {
        if (!_isEmpty)
        {
            _callback(this);
        }
    }
}
