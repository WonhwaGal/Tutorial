
public struct TutorialStepEvent : IEvent
{
	public readonly int StepNumber;
	public bool BeginStep;

    public TutorialStepEvent(int stepNumber, bool beginStep)
    {
        StepNumber = stepNumber;
        BeginStep = beginStep;
    }
}