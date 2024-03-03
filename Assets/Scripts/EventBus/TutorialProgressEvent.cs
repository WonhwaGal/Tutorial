
public struct TutorialProgressEvent : IEvent
{
    public readonly int FinishedStep;
    public TutorialProgressEvent(int finishedStep) => FinishedStep = finishedStep;
}