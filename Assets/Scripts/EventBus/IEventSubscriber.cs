public interface IEventSubscriber<T> : IEventBaseSubscriber where T : struct, IEvent
{
    public void OnEvent(T eventName);
}
