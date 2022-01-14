using System.Collections.Generic;
using UnityEngine;

public delegate void EventCallback(object obj);

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;

    private static Dictionary<EventId, EventCallback> _events;

    private void Awake()
    {
        if (_instance)
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }

        _events = new Dictionary<EventId, EventCallback>();
    }

    internal static void AddEventListener(EventId eventId, EventCallback callback)
    {
        if (_events.ContainsKey(eventId))
        {
            _events[eventId] += callback;
        } else
        {
            _events.Add(eventId, callback);
        }
    }

    internal static void RemoveEventListener(EventId eventId, EventCallback callback)
    {
        if (_events.ContainsKey(eventId))
        {
            _events[eventId] -= callback;
        }
    }

    internal static void DispatchEvent(EventId eventId, object obj = null)
    {
        if (_events.ContainsKey(eventId))
        {
            _events[eventId](obj);
        }
    }
}
