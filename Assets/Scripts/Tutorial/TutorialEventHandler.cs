using UnityEngine;

public sealed class TutorialEventHandler
{
    // for handling unique circumstances

    // Example:
    public static void CheckStartBattleEvent()
    {
        var lastStep = PlayerPrefs.GetInt("LastTutorialStep", -1);
        switch (lastStep)
        {
            case 0:
                EventBus.RaiseEvent(new TutorialStepEvent(lastStep + 1, beginStep: true));
                break;
            case 9:
                EventBus.RaiseEvent(new TutorialStepEvent(lastStep + 1, beginStep: true));
                break;
            case 13:
                EventBus.RaiseEvent(new TutorialStepEvent(lastStep + 1, beginStep: true));
                break;
        }
    }
}