using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<Tuple<T, int, int>> _elements;

    public bool IsEmpty
    {
        get
        {
            return _elements.Count == 0;
        }
    }

    public PriorityQueue()
    {
        _elements = new List<Tuple<T, int, int>>();
    }

    public void Enqueue(T item, int priority, int direction = 0)
    {
        _elements.Add(new Tuple<T, int, int>(item, priority, direction));
    }

    public Tuple<T, int> Dequeue()
    {
        int priorityIndex = 0;

        for (var i = 0; i < _elements.Count - 1; i++)
        {
            priorityIndex = (_elements[i].Item2 > _elements[priorityIndex].Item2) ? i : priorityIndex;
        }

        var element = _elements[priorityIndex].Item1;
        var direction = _elements[priorityIndex].Item3;
        _elements.RemoveAt(priorityIndex);

        return new Tuple<T, int>(element, direction);
    }
}
