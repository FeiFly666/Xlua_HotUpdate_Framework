using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void EventHandler(object args);

    Dictionary<int,EventHandler> _Events = new Dictionary<int,EventHandler>();

    public void Subscribe(int id, EventHandler Event)
    {
        if(_Events.ContainsKey(id))
        {
            _Events[id] += Event;
        }
        else
        {
            _Events.Add(id, Event);
        }
    }

    public void Unsubscribe(int id, EventHandler Event)
    {
        if(_Events.ContainsKey(id))
        {
            if (_Events[id] != null)
            {
                _Events[id] -= Event;
            }

            if (_Events[id] == null)
            {
                _Events.Remove(id);
            }
        }
    }

    public void Execute(int id, object args = null)
    {
        EventHandler handler;
        if(_Events.TryGetValue(id, out handler))
        {
            handler(args);
        }
    }
}
