using System.Collections.Generic;

public class Node<T>
{
	private T _element;
	private int _weight;

	public T Element { 
		get
		{
			return _element;
		}
	}
	public int Weight { 
		get {
			return _weight;
		}
	}

	public Node(T element, int weight)
	{
		_element = element;
		_weight = weight;
	}
}