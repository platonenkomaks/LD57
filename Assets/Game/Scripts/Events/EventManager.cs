using System;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
  public class EventManager
  {
    private readonly Dictionary<Type, List<Action<IEvent>>> eventListeners = new();

    public void Register<T>(Action<T> listener) where T : IEvent
    {
      var eventType = typeof(T);
      if (!eventListeners.ContainsKey(eventType))
      {
        eventListeners[eventType] = new List<Action<IEvent>>();
      }
      eventListeners[eventType].Add(e => listener((T)e));
    }

    public void Unregister<T>(Action<T> listener) where T : IEvent
    {
      var eventType = typeof(T);
      if (eventListeners.TryGetValue(eventType, out var eventListener))
      {
        eventListener.Remove(e => listener((T)e));
      }
    }

    public void Trigger<T>(T eventInstance) where T : IEvent
    {
      var eventType = typeof(T);
      if (eventListeners.TryGetValue(eventType, out var eventListener))
      {
        for (int i = eventListener.Count - 1; i >= 0; i--)
        {
          var listener = eventListener[i];
          if (listener.Target == null)
          {
            eventListener.RemoveAt(i);
            Debug.LogWarning($"EventManager: listener {listener} is null");
          }
          else
          {
            listener(eventInstance);
          }
        }
      }
    }
  }
}