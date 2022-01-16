using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Node : MonoBehaviour
{
    [SerializeField]
    private int _id;
    private int _x;
    private int _y;
    private HashSet<Node> _neighbours;

    [SerializeField]
    private SpriteRenderer _nodeType;
    [SerializeField]
    private SpriteRenderer _nodeBackground;

    [SerializeField]
    private Animation _anim;

    private static DataManager _dataManager;

    #region Properties
    public int Id
    {
        get
        {
            return _id;
        }
    }

    public int X
    {
        get
        {
            return _x;
        }
    }

    public int Y
    {
        get
        {
            return _y;
        }
    }

    public Vector2 Coordinates
    {
        get
        {
            return new Vector2(_x, _y);
        }
    }

    public HashSet<Node> Neighbours
    {
        get
        {
            return _neighbours;
        }
    }
    #endregion

    #region Setters
    public void SetId(int id)
    {
        _id = id;

        if (_id < 0)
        {
            Hide();
        } else
        {
            _nodeType.sprite = _dataManager.GetLevelSprite(id);
        }
    }

    public void SetCoordinates(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void AddNeighbour(Node neighbour)
    {
        _neighbours.Add(neighbour);
    }
    #endregion

    private void Awake()
    {
        _neighbours = new HashSet<Node>();

        if (!_dataManager)
        {
            _dataManager = GameObject.FindObjectOfType<DataManager>();
        }
    }

    private void OnMouseDown()
    {
        EventManager.DispatchEvent(EventId.NODE_CLICK, this);
    }

    #region Selection
    public void Select()
    {
        _nodeBackground.color = _dataManager.GetLevelSelectedColor();
    }

    public void Unselect()
    {
        _nodeBackground.color = _dataManager.GetLevelUnselectedColor();
    }

    public void CancelSelection()
    {
        Unselect();

        _anim.Play("Anim_Shake");
    }

    public void Match()
    {
        _anim.Stop();

        Hide();
    }

    public void Hint()
    {
        Debug.Log( _anim.GetClipCount());
        _anim.Play("Anim_Highlight");
    }
    #endregion

    private void Hide()
    {
        _id = -1;

        gameObject.SetActive(false);
    }
}
