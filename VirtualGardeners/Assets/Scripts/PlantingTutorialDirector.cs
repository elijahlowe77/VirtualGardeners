using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Guides the player through the complete planting loop by sequencing messages,
/// optional highlight objects, and waiting for the referenced Dirt plot to reach
/// each milestone (dig hole, seed, rake, water, grow).
/// Drop this on an empty GameObject in the scene and wire the serialized fields
/// in the inspector.
/// </summary>
public class PlantingTutorialDirector : MonoBehaviour
{
    [Serializable]
    public class StepSettings
    {
        [TextArea]
        public string message = "Explain what to do.";
        [Tooltip("Optional extra feedback triggers (vfx, audio) for when the step begins.")]
        public UnityEvent onStepStart;
        [Tooltip("Optional helper visuals (arrows, glow rings, etc). They will be toggled on/off for this step.")]
        public GameObject[] highlightTargets;
        [Tooltip("Wait this long after the success condition is met before advancing.")]
        public float holdAfterSuccess = 0.35f;
        [Tooltip("Override for auto-dismiss delay while this step is active. Keep at 0 to require manual dismiss.")]
        public float overlayDismissOverride = 0f;
    }

    [Header("Core References")]
    [SerializeField] TutorialOverlay tutorialOverlay;
    [SerializeField] Dirt tutorialPlot;

    [Header("Step Configuration")]
    [SerializeField] StepSettings digHoleStep;
    [SerializeField] StepSettings seedStep;
    [SerializeField] StepSettings rakeStep;
    [SerializeField] StepSettings waterStep;
    [SerializeField] StepSettings waitForGrowthStep;

    [Header("Flow")]
    [SerializeField] float startDelay = 1.5f;
    [SerializeField, TextArea] string completionMessage = "Nice! You just grew a plant. Explore the garden and repeat the steps on other plots.";
    [SerializeField] float completionMessageDuration = 6f;
    [SerializeField] UnityEvent onTutorialComplete;

    Coroutine sequenceRoutine;
    bool hasCompleted;

    void OnEnable()
    {
        if (tutorialPlot == null)
        {
            Debug.LogWarning($"{nameof(PlantingTutorialDirector)} is missing a Dirt reference.", this);
            enabled = false;
            return;
        }

        sequenceRoutine = StartCoroutine(RunSequence());
    }

    void OnDisable()
    {
        if (sequenceRoutine != null)
        {
            StopCoroutine(sequenceRoutine);
            sequenceRoutine = null;
        }

        HideAllHighlights();
    }

    IEnumerator RunSequence()
    {
        yield return new WaitForSeconds(startDelay);

        yield return RunStep(digHoleStep, () => tutorialPlot.state == "Hole");
        yield return RunStep(seedStep, () => tutorialPlot.state == "Seeded");
        yield return RunStep(rakeStep, () => tutorialPlot.state == "Raked");
        yield return RunStep(waterStep, () => tutorialPlot.watered);
        yield return RunStep(waitForGrowthStep, () => tutorialPlot.plant != null && tutorialPlot.plant.activeSelf);

        if (tutorialOverlay != null)
        {
            tutorialOverlay.ShowTutorial(completionMessage, completionMessageDuration);
        }

        hasCompleted = true;
        onTutorialComplete?.Invoke();
    }

    IEnumerator RunStep(StepSettings step, Func<bool> successCondition)
    {
        if (successCondition == null || step == null)
            yield break;

        if (successCondition())
            yield break; // already satisfied (helps when restarting tutorial late in the flow)

        ShowStep(step);

        while (!successCondition())
            yield return null;

        if (step.holdAfterSuccess > 0f)
            yield return new WaitForSeconds(step.holdAfterSuccess);

        HideStep(step);
    }

    void ShowStep(StepSettings step)
    {
        if (step == null) return;

        if (tutorialOverlay != null && !string.IsNullOrWhiteSpace(step.message))
        {
            tutorialOverlay.ShowTutorial(step.message, step.overlayDismissOverride);
        }

        ToggleHighlights(step.highlightTargets, true);
        step.onStepStart?.Invoke();
    }

    void HideStep(StepSettings step)
    {
        if (step == null) return;

        ToggleHighlights(step.highlightTargets, false);

        if (tutorialOverlay != null && !hasCompleted)
        {
            tutorialOverlay.HideTutorial();
        }
    }

    void ToggleHighlights(GameObject[] targets, bool value)
    {
        if (targets == null) return;
        foreach (var go in targets)
        {
            if (go != null)
            {
                go.SetActive(value);
            }
        }
    }

    void HideAllHighlights()
    {
        HideStep(digHoleStep);
        HideStep(seedStep);
        HideStep(rakeStep);
        HideStep(waterStep);
        HideStep(waitForGrowthStep);
    }
}
