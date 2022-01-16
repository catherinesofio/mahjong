using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Node : MonoBehaviour
{
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
            _nodeType.sprite = DataManager.GetLevelSprite(id);
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
    }

    private void OnMouseDown()
    {
        if (!GameLogic.IsPaused)
        {
            EventManager.DispatchEvent(EventId.NODE_CLICK, this);
        }
    }

    #region Selection
    public void Select()
    {
        _nodeBackground.color = DataManager.GetLevelSelectedColor();

        EventManager.DispatchEvent(EventId.PLAY_SOUND, SoundId.NODE_SELECT);
    }

    public void Unselect()
    {
        _nodeBackground.color = DataManager.GetLevelUnselectedColor();

        EventManager.DispatchEvent(EventId.PLAY_SOUND, SoundId.NODE_UNSELECT);
    }

    public void CancelSelection()
    {
        Unselect();
        _anim.Play("Anim_Shake");

        EventManager.DispatchEvent(EventId.PLAY_SOUND, SoundId.NODE_CANCEL);
    }

    public void Match()
    {
        Hide();

        EventManager.DispatchEvent(EventId.PLAY_SOUND, SoundId.NODE_MATCH);
    }

    public void Hint()
    {
        _anim.Play("Anim_Highlight");

        EventManager.DispatchEvent(EventId.PLAY_SOUND, SoundId.NODE_HINT);
    }

    public void UnHint()
    {
        _anim.Play("Anim_Idle");
    }
    #endregion

    private void Hide()
    {
        _id = -1;
        _anim.Stop();
        gameObject.SetActive(false);
    }
}
