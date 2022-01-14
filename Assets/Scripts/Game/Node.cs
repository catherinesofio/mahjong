using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Node : MonoBehaviour
{
	private int _id;
    private Vector2 _coordinates;
    
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private List<Node> _neighbours;

    internal int Id
    {
        get
        {
            return _id;
        }
    }

    internal Vector2 Coordinates
    {
        get
        {
            return _coordinates;
        }
    }

    internal List<Node> Neighbours
    {
        get
        {
            return _neighbours;
        }
    }

    private void Awake()
    {
        _neighbours = new List<Node>();
    }

    private void Start()
    {
        EventManager.AddEventListener(EventId.NODE_MATCH, Hide);
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(EventId.NODE_MATCH, Hide);
    }

    internal void SetId(int id)
    {
        if (id < 0)
        {
            Hide();
        } else
        {
            _id = id;
            var dataManager = GameObject.FindObjectOfType<DataManager>();
            _spriteRenderer.sprite = dataManager.GetSprite(_id);
        }
    }

    internal void SetCoordinates(Vector2 coordinates)
    {
        _coordinates = coordinates;
    }

    internal void AddNeighbour(Node neighbour)
    {
        if (!_neighbours.Contains(neighbour))
        {
            _neighbours.Add(neighbour);
        }
    }

    private void Hide(object obj = null)
    {
        _id = -1;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnMouseDown()
    {
        EventManager.DispatchEvent(EventId.NODE_SELECT, (object)this);
    }
}