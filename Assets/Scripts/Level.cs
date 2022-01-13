using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    private int _tileWidth;
    [SerializeField]
    private int _tileHeight;

    [SerializeField]
    private string _folderName;
    [SerializeField]
    private string _layoutFile;

    [SerializeField]
    private GameObject _tilePrefab;

    private Tile _from;
    private Tile _to;

    private int _countX;
    private int _countY;
    [SerializeField]
    private List<Tile> _graph;

    private Coroutine _coroutine;

    private void Start()
    {
        LoadLevel();
    }

    private string GetLevelData()
    {
        var path = $"{ Application.dataPath}/{_folderName}/{_layoutFile}";
        StreamReader reader = new StreamReader(path);

        return reader.ReadToEnd();
    }

    private void LoadLevel()
    {
        _graph = new List<Tile>();

        var data = GetLevelData();

        // Create Graph
        var x = 0;
        var y = 0;
        using (StringReader reader = new StringReader(data))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                foreach (char c in line)
                {
                    var go = GameObject.Instantiate(_tilePrefab, new Vector3(_tileWidth * x, _tileHeight * -y, 0), Quaternion.identity, transform);
                    var tile = go.GetComponent<Tile>();
                    if (!c.Equals('0'))
                    {
                        tile.SetIsEmpty(false);
                        tile.SetCallback(OnSelectTile);
                        tile.SetId((TileId)UnityEngine.Random.Range(0, (int)TileId.COUNT - 1));
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

        // Set Graph's Neighbours
        SetNeighbours();
    }

    private void SetNeighbours()
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

    private void OnSelectTile(Tile tile)
    {
        if (_from == null)
        {
            _from = tile;
            _to = null;
        }
        else if (_from == tile)
        {
            _from = null;
            _to = null;
        }
        else if (_to == null)
        {
            _to = tile;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(CalculateTurns());
        }
    }

    private IEnumerator CalculateTurns()
    {
        var direction = 0;
        var turns = 0;

        var visited = new HashSet<Vector2>();

        var queue = new Queue<Node<Tile>>();
        queue.Enqueue(new Node<Tile>(_from, 0));

        while (queue.Count > 0)
        {
            var curr = queue.Dequeue();

            if (curr.Equals(_to))
            {
                break;
            }

            if (visited.Contains(curr.Element.Coordinates))
            {
                continue;
            }
            visited.Add(curr.Element.Coordinates);

            var nData = new List<Tuple<Tile, int, int>>();
            var neighbours = curr.Element.Neighbours.Where(n => n.IsEmpty && !visited.Contains(n.Coordinates));
            foreach (var n in neighbours)
            {
                // 1 = vertical movement
                // 2 = horizontal movement
                var dir = (n.Coordinates.x == curr.Element.Coordinates.x) ? 1 : 2;

                nData.Add(new Tuple<Tile, int, int>(n, (dir == direction || direction == 0) ? 0 : 1, dir));
            }

            var next = nData.OrderBy(n => n.Item2).FirstOrDefault();
            if (next != null)
            {
                turns += (next.Item3 == direction || direction == 0) ? 0 : 1;
                direction = next.Item3;

                queue.Enqueue(new Node<Tile>(next.Item1, next.Item2));
            }

            yield return null;
        }

        Debug.Log(turns);
        // CALL EVENT TO
        // -> DESTROY TILES (IF APPLICABLE)
        //    ADD POINTS 
        // -> REMOVE POINTS

        _coroutine = null;
        _from = null;
        _to = null;
    }
}