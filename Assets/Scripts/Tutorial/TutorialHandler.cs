using UnityEngine;
using static TutorialSO;

public sealed class TutorialHandler
{
    // for the first actions after loading a scene

    //Example:
    public static bool CheckAutoActivation(int currentStep)
    {
        if(currentStep is 0 or 4 or 9 or 21)
            return true;
        return false;
    }

    public static Vector3 GetTargetPosition(TutorialStep step)
    {
        Vector3 finalPos = step.TargetT.position;
        if (!step.TargetIsUi)
            finalPos = Camera.main.WorldToScreenPoint(step.TargetT.position);
        return finalPos;
    }

    public static Vector3 GetPointerPosition(Vector3 finalPos, Vector3 startPos, float closeRef, float canvasScaler)
    {
        var oneUnitVector = Vector3.Normalize(finalPos - startPos);
        return startPos + closeRef * canvasScaler * oneUnitVector;
    }

    public static Quaternion GetAngle(Vector3 targetPos, Vector3 originPos)
    {
        var multiplier = 1;
        if (targetPos.x < originPos.x)
            multiplier = -1;
        var dir = targetPos - originPos;
        var angle = Vector3.Angle(Vector3.up, dir) * multiplier;
        return Quaternion.AngleAxis(angle, -Vector3.forward);
    }
}