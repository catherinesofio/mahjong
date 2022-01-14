using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private int _score;

    // Count of cells by axis
    private int _countX;
    private int _countY;

    [SerializeField]
    private int _nodeWidth;
    [SerializeField]
    private int _nodeHeight;
    [SerializeField]
    private GameObject _nodePrefab;

    private Node _selected;
    private Node[] _graph;

    private Coroutine _coroutine;

    private void Start()
    {
        _score = 0;

        EventManager.AddEventListener(EventId.NODE_SELECT, OnSelectNode);

        LoadLevel();
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(EventId.NODE_SELECT, OnSelectNode);
    }

    #region Load Level
    private void LoadLevel()
    {
        var layout = GetGameLayout();

        CreateGraph(layout);

        CenterGraph();
    }

    private char[] GetGameLayout()
    {
        var dataManager = GameObject.FindObjectOfType<DataManager>();

        var path = $"{dataManager.GetLevelPath()}";
        var stringLayout = Utils.ReadTextFile(path);

        var nodes = stringLayout.ToCharArray().Where(x => x == 'X' || x == '0').ToArray();
        var reader = new StringReader(stringLayout);
        
        _countX = reader.ReadLine().Length;
        _countY = nodes.Length / _countX;

        return nodes;

    }

    // Could be improved so as to avoid creating a Board without a solution!!
    private void CreateGraph(char[] layout)
    {
        _graph = new Node[_countX * _countY];

        for (var i = 0; i < _countX * _countY; i++)
        {
            GetOrCreateNode(i, layout);
        }
    }

    private Node GetOrCreateNode(int id, char[] layout)
    {
        if (_graph[id] != null)
        {
            return _graph[id];
        }

        var x = id % _countX;
        var y = id / _countX;

        var go = GameObject.Instantiate(_nodePrefab, new Vector3(_nodeWidth * x, -_nodeHeight * y, 0), Quaternion.identity, transform);
        var node = go.GetComponent<Node>();
        node.SetCoordinates(new Vector2(x, y));

        if (layout[id].Equals('0'))
        {
            node.SetId(-1);
        } else
        {
            var dataManager = GameObject.FindObjectOfType<DataManager>();
            node.SetId(UnityEngine.Random.Range(0, dataManager.GetLevelTilesCount() - 1));
        }

        _graph[id] = node;

        SetNeighbours(id, layout);

        return node;
    }

    private void SetNeighbours(int id, char[] layout)
    {
        var x = id % _countX;
        var y = id / _countX;

        #region Horizontal
        if (x > 0)
        {
            _graph[id].AddNeighbour(GetOrCreateNode(id - 1, layout));
        }

        if (x < _countX - 1)
        {
            _graph[id].AddNeighbour(GetOrCreateNode(id + 1, layout));
        }
        #endregion

        #region Vertical
        if (y > 0)
        {
            _graph[id].AddNeighbour(GetOrCreateNode(id - _countX, layout));
        }

        if (y < _countY - 1)
        {
            _graph[id].AddNeighbour(GetOrCreateNode(id + _countX, layout));
        }
        #endregion
    }

    private void CenterGraph()
    {
        var posX = _countX * _nodeWidth / 2f;
        var posY = _countY * _nodeHeight / 2f;
        transform.position = new Vector2(-posX, posY);

        Camera.main.orthographicSize = Math.Max(_countX, _countY);
    }
    #endregion

    private void OnSelectNode(object obj)
    {
        var node = (Node)obj;

        var isSurrounded = node.Neighbours.Count == node.Neighbours.Where(n => n.Id > 0).Count();

        if (_selected == null && !isSurrounded)
        {
            _selected = node;
        }
        else if (_selected == node)
        {
            _selected = null;
        }
        else if (!isSurrounded && _selected.Id == node.Id)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            Debug.Log($"{_selected.Id} => {node.Id}");

            _coroutine = StartCoroutine(CalculateTurns(_selected, node));
        }
    }

    // Basically Dijkstra
    private IEnumerator CalculateTurns(Node from, Node to)
    {
        var turns = 0;

        var nextNodeToGoal = new Dictionary<Node, Node>();
        var costToReachNode = new Dictionary<Node, int>();

        var queue = new PriorityQueue<Node>();

        queue.Enqueue(to, 0);
        costToReachNode[to] = 0;

        while (!queue.IsEmpty)
        {
            var curr = queue.Dequeue();
            var currNode = curr.Item1;
            var currDir = curr.Item2;

            if (currNode == from)
            {
                turns = costToReachNode[currNode];
                break;
            }

            // Filter out NOT empty nodes
            var neighbours = currNode.Neighbours.Where(n => n.Id < 0);
            foreach (var n in neighbours)
            {
                var nDir = (currNode.Coordinates.x == n.Coordinates.x) ? 1 : 2;
                var cost = costToReachNode[currNode] + ((currDir == 0 || currDir == nDir) ? 0 : 1);

                if (!costToReachNode.ContainsKey(n) || cost < costToReachNode[n])
                {
                    costToReachNode[n] = cost;
                    queue.Enqueue(n, cost, nDir);
                    nextNodeToGoal[n] = currNode;
                }
            }

            yield return null;
        }

        Debug.Log(turns);
    }
}