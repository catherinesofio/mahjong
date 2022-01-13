using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IN PROCESS! HAS TEMP THINGS THAT WILL BE CHANGED!
public class GameLogic : MonoBehaviour
{
    // Count of cells by axis
    private int _countX;
    private int _countY;

    [SerializeField]
    private int _tileWidth;
    [SerializeField]
    private int _tileHeight;
    [SerializeField]
    private GameObject _tilePrefab;

    private NodeView _selected;

    // MAY CHANGE
    private List<NodeView> _graph;
    private Coroutine _coroutine;

    private void Start()
    {
        LoadLevel();
    }

    #region LoadLevel
    private void LoadLevel()
    {
        var layout = GetGameLayout();

        CreateGraph(layout);

        SetGraphNeighbours();

        CenterGraph();
    }

    private string GetGameLayout()
    {
        var dataManager = GameObject.FindObjectOfType<DataManager>();

        var level = GameObject.FindObjectOfType<ScreenManager>().Level;
        var path = $"{dataManager.LevelsFolder}/{dataManager[level].layoutFiles}";

        return Utils.ReadTextFile(path);
    }

    private void CreateGraph(string layout)
    {
        _graph = new List<NodeView>();

        var x = 0;
        var y = 0;
        using (StringReader reader = new StringReader(layout))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                foreach (char c in line)
                {
                    var go = GameObject.Instantiate(_tilePrefab, new Vector3(_tileWidth * x, _tileHeight * -y, 0), Quaternion.identity, transform);
                    var tile = go.GetComponent<NodeView>();
                    if (!c.Equals('0'))
                    {
                        tile.SetIsEmpty(false);
                        tile.SetCallback(OnSelectTile);
                    }
                    else
                    {
                        tile.SetIsEmpty(true);
                        tile.gameObject.SetActive(false);
                    }
                    tile.SetCoordinates(new Vector2(x, y));

                    _graph.Add(tile);

                    x++;
                }

                y++;

                _countX = x;
                _countY = y;

                x = 0;
            }
        }
    }

    private void SetGraphNeighbours()
    {
        for (var id = 0; id < _graph.Count - 1; id++)
        {
            var x = id % _countX;
            var y = id / _countX;

            #region Horizontal
            if (x > 0)
            {
                _graph[id].AddNeighbour(_graph[id - 1]);
            }

            if (x < _countX - 1)
            {
                _graph[id].AddNeighbour(_graph[id + 1]);
            }
            #endregion

            #region Vertical
            if (y > 0)
            {
                _graph[id].AddNeighbour(_graph[id - _countX]);
            }

            if (y < _countY - 1)
            {
                _graph[id].AddNeighbour(_graph[id + _countX]);
            }
            #endregion
        }
    }

    private void CenterGraph()
    {
        var posX = _countX * _tileWidth / 2f;
        var posY = _countY * _tileHeight / 2f;
        transform.position = new Vector2(-posX, posY);

        Camera.main.orthographicSize = Math.Max(_countX, _countY);
    }
    #endregion

    private void OnSelectTile(NodeView tile)
    {
        if (_selected == null)
        {
            _selected = tile;
            // call select tile
        }
        else if (_selected == tile)
        {
            _selected = null;
            // call unselect tile
        }
        else
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(CalculateTurns(_selected, tile));
        }
    }

    private IEnumerator CalculateTurns(NodeView from, NodeView to)
    {
        var queue = new Queue();

        while (queue.Count > 0)
        {
            yield return null;
        }
    }
}