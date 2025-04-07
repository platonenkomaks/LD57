using System;
using System.Collections.Generic;

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
        foreach (var listener in eventListener)
        {
          listener(eventInstance);
        }
      }
    }
  }
}