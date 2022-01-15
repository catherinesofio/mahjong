using System;
using System.Collections.Generic;

public class PriorityQueue<T, D>
{
    private List<Tuple<T, float, D>> _elements;

    public bool IsEmpty
    {
        get
        {
            return _elements.Count == 0;
        }
    }

    public PriorityQueue()
    {
        _elements = new List<Tuple<T, float, D>>();
    }

    public void Enqueue(T item, float priority, D direction)
    {
        _elements.Add(new Tuple<T, float, D>(item, priority, direction));
    }

    public Tuple<T, D> Dequeue()
    {
        int priorityIndex = 0;

        for (var i = 0; i < _elements.Count - 1; i++)
        {
            priorityIndex = (_elements[i].Item2 < _elements[priorityIndex].Item2) ? i : priorityIndex;
        }

        var element = _elements[priorityIndex].Item1;
        var direction = _elements[priorityIndex].Item3;

        _elements.RemoveAt(priorityIndex);

        return new Tuple<T, D>(element, direction);
    }
}
