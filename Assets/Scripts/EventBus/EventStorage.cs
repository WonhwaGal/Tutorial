using System;
using System.Collections.Generic;


public class EventStorage
{
    private Dictionary<Type, List<IEventBaseSubscriber>> _subscribersByEventType = new();

    public void Add<T>(IEventSubscriber<T> subcriber) where T : struct, IEvent
    {
        if (!_subscribersByEventType.ContainsKey(typeof(T)))
            _subscribersByEventType.Add(typeof(T), new List<IEventBaseSubscriber>());

        _subscribersByEventType[typeof(T)].Add(subcriber);
    }

    public void Remove<T>(IEventSubscriber<T> unsubcriber) where T : struct, IEvent
    {
        if(_subscribersByEventType.TryGetValue(typeof(T), out List<IEventBaseSubscriber> subscriberList))
        {
            subscriberList.Remove(unsubcriber);
        }
    }

    public void InvokeEvent<T>(T eventName) where T : struct, IEvent
    {
        if (_subscribersByEventType.ContainsKey(typeof(T)))
        {
            for (int i = 0; i < _subscribersByEventType[typeof(T)].Count; i++)
            {
                ((IEventSubscriber<T>)_subscribersByEventType[typeof(T)][i]).OnEvent(eventName);
            }
        }
    }
}
