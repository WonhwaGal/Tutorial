using System;

public static class EventBus
{
    private static EventStorage _storage = new();

    public static void RegisterTo<T>(IEventSubscriber<T> subcriber) 
        where T : struct, IEvent
    {
        _storage.Add(subcriber);
    }

    public static void UnregisterFrom<T>(IEventSubscriber<T> unsubcriber) 
        where T : struct, IEvent
    {
        _storage.Remove(unsubcriber);
    }

    public static void RaiseEvent<T>(T eventName) 
        where T : struct, IEvent
    {
        _storage.InvokeEvent(eventName);
    }
}
