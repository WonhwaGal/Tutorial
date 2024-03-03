using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TutorialSO), menuName = "Configs/TutorialSO")]
public class TutorialSO : ScriptableObject
{
    [field: SerializeField, ReadOnly] public bool IsFinished { get; set; }
    [SerializeField] private List<TutorialStep> _tutorials;

    public List<TutorialStep> TutorialSteps => _tutorials;

    public bool TryFindNextStep(out TutorialStep step)
    {
        step = null;
        for(int i =0;  i < TutorialSteps.Count; i++)
        {
            if (!TutorialSteps[i].IsFinished)
            {
                step = TutorialSteps[i];
                return true;
            }
        }
        return false;
    }

    [Serializable]
    public class TutorialStep
    {
        public string Name;
        public int Order;
        [Space]
        public bool IsResumeStep;
        public int WhenResumedGoStepNumber;
        [Space]
        public TutorialTargetType TargetType; // to know if it is a standard button with onClick...
        public bool TargetIsUi; // to know how calculate the position
        public bool OnlyPointer;
        public bool ShouldFreeze;
        [Range(0.0f, 450.0f)] public float ClosenessToTarget;
        [Range(0.0f, 7.0f)] public float UnmaskScale;
        public bool AutoGoToNextStep;
        [ReadOnly]public bool IsFinished;

        public Transform TargetT { get; set; }
    }
}

public enum TutorialTargetType
{
    None = 0,
    Button = 1,
    Event = 2
}