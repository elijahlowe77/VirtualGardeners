using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class TutorialOverlay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] CanvasGroup tutorialCanvas;
    [SerializeField] Text tutorialText;

    [Header("Behavior")]
    [SerializeField] bool showOnStart = true;
    [TextArea]
    [SerializeField] string initialMessage = "Welcome to Virtual Gardeners! Grab your tools and start planting.";
    [SerializeField] float autoDismissDelay = 6f;
    [SerializeField] float fadeDuration = 0.25f;

    [Header("Manual Dismiss")]
    [SerializeField] bool allowManualDismiss = true;
    [SerializeField] List<XRController> controllers = new List<XRController>();
    [SerializeField] InputHelpers.Button dismissButton = InputHelpers.Button.PrimaryButton;
    [SerializeField, Range(0.01f, 1f)] float inputThreshold = 0.1f;
    [SerializeField] KeyCode fallbackKeyboardKey = KeyCode.Space;

    Coroutine fadeRoutine;
    Coroutine dismissRoutine;
    bool tutorialVisible;

    void Awake()
    {
        if (tutorialCanvas == null) 
            tutorialCanvas = GetComponentInChildren<CanvasGroup>(true);

        if (tutorialText == null && tutorialCanvas != null) 
            tutorialText = tutorialCanvas.GetComponentInChildren<Text>(true);

        HideImmediate();
    }

    void Start()
    {
        if (showOnStart && !string.IsNullOrEmpty(initialMessage))
        {
            ShowTutorial(initialMessage, autoDismissDelay);
        }
    }

    void Update()
    {
        if (!tutorialVisible || !allowManualDismiss) return;

        if (CheckControllerInput() || Input.GetKeyDown(fallbackKeyboardKey))
        {
            HideTutorial();
        }
    }

    public void ShowTutorial(string message, float? overrideDismissDelay = null)
    {
        if (tutorialCanvas == null) return;

        if (tutorialText != null) 
            tutorialText.text = message;

        tutorialVisible = true;
        tutorialCanvas.interactable = true;
        tutorialCanvas.blocksRaycasts = true;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeCanvas(1f));

        if (dismissRoutine != null)
        {
            StopCoroutine(dismissRoutine);
            dismissRoutine = null;
        }

        float delay = overrideDismissDelay.HasValue ? overrideDismissDelay.Value : autoDismissDelay;
        if (delay > 0f)
        {
            dismissRoutine = StartCoroutine(AutoDismiss(delay));
        }
    }

    public void HideTutorial()
    {
        if (tutorialCanvas == null || !tutorialVisible) return;

        tutorialVisible = false;
        tutorialCanvas.interactable = false;
        tutorialCanvas.blocksRaycasts = false;

        if (dismissRoutine != null)
        {
            StopCoroutine(dismissRoutine);
            dismissRoutine = null;
        }

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeCanvas(0f));
    }

    public void HideImmediate()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        if (dismissRoutine != null)
        {
            StopCoroutine(dismissRoutine);
            dismissRoutine = null;
        }

        if (tutorialCanvas == null) return;

        tutorialVisible = false;
        tutorialCanvas.alpha = 0f;
        tutorialCanvas.interactable = false;
        tutorialCanvas.blocksRaycasts = false;
    }

    IEnumerator AutoDismiss(float delay)
    {
        yield return new WaitForSeconds(delay);
        dismissRoutine = null;
        HideTutorial();
    }

    IEnumerator FadeCanvas(float targetAlpha)
    {
        if (tutorialCanvas == null) yield break;

        float startAlpha = tutorialCanvas.alpha;
        float time = 0f;

        if (fadeDuration <= 0f)
        {
            tutorialCanvas.alpha = targetAlpha;
            yield break;
        }

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            tutorialCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        tutorialCanvas.alpha = targetAlpha;
    }

    bool CheckControllerInput()
    {
        if (controllers == null) return false;

        foreach (var controller in controllers)
        {
            if (controller == null) continue;

            bool pressed;
            // XRController exposes inputDevice, so this now compiles
            if (InputHelpers.IsPressed(controller.inputDevice, dismissButton, out pressed, inputThreshold) && pressed)
            {
                return true;
            }
        }

        return false;
    }
}