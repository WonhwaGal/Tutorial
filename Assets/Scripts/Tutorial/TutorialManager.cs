using UnityEngine;
using UnityEngine.UI;
using static TutorialSO;

public class TutorialManager : MonoBehaviour, IEventSubscriber<TutorialStepEvent>
{
    [Header("Tutorial tools")]
    [SerializeField] private GameObject _maskScreen;
    [SerializeField] private GameObject _unmaskCircle;
    [SerializeField] private Transform _pointer;
    [SerializeField] private TutorialSO _tutorialSO;

    [Header("Screen Buttons")]
    [SerializeField] private Button[] _surroundingButtons;

    [Header("Tutorial targets")]
    [SerializeField] private Transform[] _taskTargets;

    [Header("GameObjects")]
    [SerializeField] private Canvas _canvas;

    private Vector3 _pointerInitialPos;
    private TutorialStep _currentStep;
    private bool _stepInProgress;

    private void Awake()
    {
        _pointerInitialPos = _pointer.transform.position;
        SetUpTutorialTools(false);
        if (IsTutorialDone())
            return;

        EventBus.RegisterTo(this);
        AssignTargets();
        if (TutorialHandler.CheckAutoActivation(_currentStep.Order))
            ActivateStep();
    }

    private bool IsTutorialDone()
    {
        var lastFinishedStep = PlayerPrefs.GetInt("LastTutorialStep", -1);
        var lasTutorialStepNumber = _tutorialSO.TutorialSteps.Count - 1;
        // update SO data
        for (int i = 0; i < _tutorialSO.TutorialSteps.Count; i++)
            _tutorialSO.TutorialSteps[i].IsFinished =
                _tutorialSO.TutorialSteps[i].Order <= lastFinishedStep;
        // set up current state
        if (lastFinishedStep == -1)
            _currentStep = _tutorialSO.TutorialSteps[0];
        else
            GoBackToResumeStep();
        _tutorialSO.IsFinished = lastFinishedStep == lasTutorialStepNumber;
        return _tutorialSO.IsFinished;
    }

    private void GoBackToResumeStep()
    {
        if (_tutorialSO.TryFindNextStep(out TutorialStep step))
        {
            if (!AssignCurrentStep(step))
            {
                var resumeStep = step.WhenResumedGoStepNumber;
                AssignCurrentStep(_tutorialSO.TutorialSteps[resumeStep]);
            }
        }
    }

    private bool AssignCurrentStep(TutorialStep step)
    {
        if (step.IsResumeStep)
        {
            _currentStep = step;
            for (int i = 0; i < step.Order; i++)
            {
                _tutorialSO.TutorialSteps[i].IsFinished = true;
                RegisterStep(i);
            }
            return true;
        }
        return false;
    }

    private void AssignTargets()
    {
        for (int i = 0; i < _tutorialSO.TutorialSteps.Count; i++)
            _tutorialSO.TutorialSteps[i].TargetT = _taskTargets[i];
    }

    private void ActivateStep()
    {
        if (_currentStep == null || _stepInProgress)
            return;

        _stepInProgress = true;
        if (_currentStep.ShouldFreeze)
            Time.timeScale = 0;
        KeepSurroundingsActive(false);
        SetUpTutorialTools(true);
        ActivateTaskTarget(true);
    }

    private void FinishStep()
    {
        if (!_stepInProgress || _currentStep == null)
            return;

        _stepInProgress = false;
        RegisterStep();
        if (Time.timeScale <= 1)
            Time.timeScale = 1;
        _currentStep.IsFinished = true;
        ActivateTaskTarget(false);
        KeepSurroundingsActive(true);
        SetUpTutorialTools(false);
        CheckForAutoContinue();
    }

    private void SetUpTutorialTools(bool activate)
    {
        if (activate)
        {
            Vector3 finalPos = TutorialHandler.GetTargetPosition(_currentStep);
            _maskScreen.SetActive(!_currentStep.OnlyPointer);
            _unmaskCircle.transform.localScale = Vector3.one * _currentStep.UnmaskScale;
            _unmaskCircle.transform.position = finalPos;
            _pointer.rotation = TutorialHandler.GetAngle(finalPos, _pointerInitialPos);
            _pointer.position = TutorialHandler.GetPointerPosition(
                finalPos, _pointerInitialPos, _currentStep.ClosenessToTarget, _canvas.scaleFactor);
        }

        if (!activate)
        {
            _pointer.position = _pointerInitialPos;
            _maskScreen.SetActive(activate);
        }
        _pointer.gameObject.SetActive(activate);
    }

    private void ActivateTaskTarget(bool toActivate)
    {
        if (_currentStep.TargetType != TutorialTargetType.Button)
            return;

        var button = _currentStep.TargetT.GetComponent<Button>();
        if (!toActivate)
        {
            button.onClick.RemoveListener(FinishStep);
            return;
        }
        button.interactable = true;
        button.onClick.AddListener(FinishStep);
    }

    private void KeepSurroundingsActive(bool activate)
    {
        for (int i = 0; i < _surroundingButtons.Length; i++)
            _surroundingButtons[i].interactable = activate;
    }

    private void CheckForAutoContinue()
    {
        var finishedStep = _currentStep;
        if (_tutorialSO.TryFindNextStep(out TutorialStep step))
            _currentStep = step;
        if (finishedStep.AutoGoToNextStep)
            ActivateStep();
    }

    public void OnEvent(TutorialStepEvent eventName)
    {
        if (_currentStep != null && eventName.StepNumber != _currentStep.Order)
            return;
        if (eventName.BeginStep)
            ActivateStep();
        else
            FinishStep();
    }

    private void RegisterStep(int step = -1)
    {
        var stepNumber = step == -1 ? _currentStep.Order : step;
        if (stepNumber >= PlayerPrefs.GetInt("LastTutorialStep"))
            PlayerPrefs.SetInt("LastTutorialStep", stepNumber);
        EventBus.RaiseEvent(new TutorialProgressEvent(stepNumber));
    }

    private void OnDestroy() => EventBus.UnregisterFrom(this);
}