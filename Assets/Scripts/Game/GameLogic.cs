using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private int _score;

    [SerializeField]
    private int _posPoints;
    [SerializeField]
    private int _negPoints;

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

    private Dictionary<int, List<Node>> _activeNodes;

    private void Start()
    {
        _score = 0;
        LoadLevel();

        EventManager.AddEventListener(EventId.NODE_CLICK, SelectNode);
        EventManager.AddEventListener(EventId.GET_HINT, ShowHint);
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(EventId.NODE_CLICK, SelectNode);
        EventManager.RemoveEventListener(EventId.NODE_CLICK, ShowHint);
    }

    #region Load Level
    private void LoadLevel()
    {
        CreateGraph(GetGameLayout());
        CenterGraph();
    }

    private char[] GetGameLayout()
    {
        var dataManager = GameObject.FindObjectOfType<DataManager>();

        var path = $"{dataManager.GetLevelPath()}";
        var stringLayout = Utils.ReadTextFile(path);

        var reader = new StringReader(stringLayout);
        _countX = reader.ReadLine().Length + 2;

        // I am surrounding the graph with empty nodes to be able to match nodes in the border
        // Line (beginning and end)
        stringLayout = "0" + stringLayout.Replace(System.Environment.NewLine, "00") + "0";

        // Bottom & Top
        var borderLine = new string('0', _countX);
        stringLayout = borderLine + stringLayout + borderLine;

        var nodes = stringLayout.ToCharArray().Where(x => x == 'X' || x == '0').ToArray();
        
        _countY = nodes.Length / _countX;

        return nodes;

    }

    private void CreateGraph(char[] layout)
    {
        _graph = new Node[_countX * _countY];
        _activeNodes = new Dictionary<int, List<Node>>();

        var unmatchedTile = -1;
        for (var i = 0; i < _countX * _countY; i++)
        {
            GetOrCreateNode(i, layout, unmatchedTile);
        }
    }

    private Node GetOrCreateNode(int id, char[] layout, int unmatchedTile)
    {
        if (_graph[id] != null)
        {
            return _graph[id];
        }

        var x = id % _countX;
        var y = id / _countX;

        var go = GameObject.Instantiate(_nodePrefab, new Vector3(_nodeWidth * x, -_nodeHeight * y, 0), Quaternion.identity, transform);
        go.name = $"{id}_({x}:{y})";

        var node = go.GetComponent<Node>();
        node.SetCoordinates(x, y);

        if (layout[id].Equals('0'))
        {
            node.SetId(-1);
        } else
        {
            var nodeId = unmatchedTile;

            if (unmatchedTile > 0)
            {
                node.SetId(unmatchedTile);
                unmatchedTile = -1;
            } else
            {
                var dataManager = GameObject.FindObjectOfType<DataManager>();
                nodeId = UnityEngine.Random.Range(0, dataManager.GetLevelTilesCount() - 1);

                node.SetId(nodeId);
                unmatchedTile = nodeId;
            }

            if (_activeNodes.ContainsKey(nodeId))
            {
                _activeNodes[nodeId].Add(node);
            } else
            {
                _activeNodes.Add(nodeId, new List<Node> { node });
            }
        }

        _graph[id] = node;

        SetNeighbours(id, layout, unmatchedTile);

        return node;
    }

    private void SetNeighbours(int id, char[] layout, int unmatchedTile)
    {
        var x = id % _countX;
        var y = id / _countX;

        if (x > 0)
        {
            _graph[id].AddNeighbour(GetOrCreateNode(id - 1, layout, unmatchedTile));
        }

        if (x < _countX - 1)
        {
            _graph[id].AddNeighbour(GetOrCreateNode(id + 1, layout, unmatchedTile));
        }

        if (y > 0)
        {
            _graph[id].AddNeighbour(GetOrCreateNode(id - _countX, layout, unmatchedTile));
        }

        if (y < _countY - 1)
        {
            _graph[id].AddNeighbour(GetOrCreateNode(id + _countX, layout, unmatchedTile));
        }
    }

    private void CenterGraph()
    {
        var posX = (_countX - 2) * _nodeWidth / 2f;
        var posY = (_countY - 2) * _nodeHeight / 2f;
        transform.position = new Vector2(-posX, posY);

        Camera.main.orthographicSize = Math.Max(_countX, _countY);
    }
    #endregion

    #region Node Selection
    private void SelectNode(object obj)
    {
        var node = (Node)obj;
        if (_selected == node)
        {
            node.Unselect(); 

            _selected = null;

            return;
        }

        var isSurrounded = node.Neighbours.Count == node.Neighbours.Where(n => n.Id >= 0).Count();
        if (isSurrounded)
        {
            node.CancelSelection();
        }
        else if (_selected == null)
        {
            node.Select();

            _selected = node;
        }
        else if (_selected.Id != node.Id)
        {
            AddScore(_negPoints);
            node.CancelSelection();
        }
        else
        {
            node.Select();

            var turns = GetTurns(_selected, node);
            if (turns < 0 || turns > 2)
            {
                node.CancelSelection();
                _selected.CancelSelection();

                AddScore(_negPoints);
            }
            else
            {
                _activeNodes[node.Id].Remove(node);
                _activeNodes[node.Id].Remove(_selected);

                node.Match();
                _selected.Match();

                AddScore(_posPoints);

                CheckGameStatus();
            }

            _selected = null;
        }
    }

    // Basically Dijkstra
    private int GetTurns(Node from, Node to)
    {
        var nextNodeToGoal = new Dictionary<Node, Node>();
        var costToReachNode = new Dictionary<Node, float>();

        var queue = new PriorityQueue<Node, float>();

        queue.Enqueue(to, 0, 0);
        costToReachNode[to] = 0;

        while (!queue.IsEmpty)
        {
            var curr = queue.Dequeue();
            var currNode = curr.Item1;
            var currDir = curr.Item2;

            if (currNode == from)
            {
                break;
            }

            // Filter out the NOT empty nodes
            var neighbours = currNode.Neighbours.Where(n => (n.Id < 0 || n == from));
            foreach (var n in neighbours)
            {
                var nDir = (currNode.X == n.X) ? 1 : 2;
                var cost = costToReachNode[currNode] + ((currDir == 0 || currDir == nDir) ? 0 : 1);

                if (!costToReachNode.ContainsKey(n) || cost < costToReachNode[n])
                {
                    costToReachNode[n] = cost;
                    queue.Enqueue(n, cost, nDir);
                    nextNodeToGoal[n] = currNode;
                }
            }
        }

        return (!nextNodeToGoal.ContainsKey(from)) ? -1 : (int)costToReachNode[from];
    }

    private Tuple<Node, Node> GetMatch()
    {
        foreach (var x in _activeNodes)
        {
            var nodes = x.Value;
            var unmatchable = new Dictionary<Node, Node>();

            if (nodes.Count > 1)
            {
                var curr = nodes[0];
                for (var i = 1; i < nodes.Count; i++)
                {
                    var next = nodes[i];

                    if (unmatchable.ContainsKey(next) && unmatchable[next] == curr)
                    {
                        continue;
                    }

                    var turns = GetTurns(curr, nodes[i]);

                    if (turns >= 0 && turns <= 2)
                    {
                        return new Tuple<Node, Node>(curr, next);
                    }
                }
            }
        }

        return null;
    }

    private bool IsUnsolvable()
    {
        return GetMatch() == null;
    }
    #endregion

    private void AddScore(int points)
    {
        _score += points;
        _score = Mathf.Max(_score, 0);

        EventManager.DispatchEvent(EventId.SCORE_UPDATE, _score);
    }

    private void CheckGameStatus()
    {
        var activeCount = _activeNodes.Select(x => x.Value.Count).Aggregate((a, b) => a + b);

        if (activeCount <= 1)
        {
            EventManager.DispatchEvent(EventId.GAME_WON, _score);
        }
        else if (IsUnsolvable())
        {
            EventManager.DispatchEvent(EventId.GAME_LOST);
        }
    }

    private void ShowHint(object obj = null)
    {
        var match = GetMatch();

        match.Item1.Hint();
        match.Item2.Hint();
    }

    /* OLD LOGIC (using coroutines)
    private void SelectNode(object obj)
    {
        if (_coroutine != null)
        {
            return;
        }

        var node = (Node)obj;
        if (_selected == node)
        {
            node.Unselect(); 

            _selected = null;

            return;
        }

        var isSurrounded = node.Neighbours.Count == node.Neighbours.Where(n => n.Id >= 0).Count();
        if (isSurrounded)
        {
            node.CancelSelection();
        }
        else if (_selected == null)
        {
            node.Select();
            
            _selected = node;
        }
        else if (_selected.Id != node.Id)
        {
            AddScore(_negPoints);
            node.CancelSelection();
        }
        else
        {
            node.Select();

            _coroutine = StartCoroutine(CalculateTurns(_selected, node));
        }
    }

    private IEnumerator CalculateTurns(Node from, Node to)
    {
        var nextNodeToGoal = new Dictionary<Node, Node>();
        var costToReachNode = new Dictionary<Node, float>();

        var queue = new PriorityQueue<Node, float>();

        queue.Enqueue(to, 0, 0);
        costToReachNode[to] = 0;

        while (!queue.IsEmpty)
        {
            var curr = queue.Dequeue();
            var currNode = curr.Item1;
            var currDir = curr.Item2;
            
            if (currNode == from)
            {
                break;
            }

            // Filter out the NOT empty nodes
            var neighbours = currNode.Neighbours.Where(n => (n.Id < 0 || n == from));
            foreach (var n in neighbours)
            {
                var nDir = (currNode.X == n.X) ? 1 : 2;
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

        if (!nextNodeToGoal.ContainsKey(from))
        {
            from.CancelSelection();
            to.CancelSelection();
        } else
        {
            // JUST FOR TESTING!
            Debug.Log($"({from.X}:{from.Y}) => ({to.X}:{to.Y})");
            var pathNode = from;
            var path = $"({from.X}:{from.Y})";
            while (pathNode != to)
            {
                pathNode = nextNodeToGoal[pathNode];
                path += $" => ({pathNode.X}:{pathNode.Y})";
            }
            Debug.Log(path);
            // CAN ABSOLUTELY BE REMOVED :)

            if (costToReachNode[from] > 2)
            {
                AddScore(_negPoints);

                from.CancelSelection();
                to.CancelSelection();
            } else
            {
                _activeNodes[from.Id].Remove(from);
                _activeNodes[to.Id].Remove(to);

                from.Match();
                to.Match();

                AddScore(_posPoints);
            }
        }

        _selected = null;
        _coroutine = null;
    }
    */
}