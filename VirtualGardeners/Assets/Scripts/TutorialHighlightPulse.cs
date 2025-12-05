using UnityEngine;

/// <summary>
/// Simple helper that gently scales a visual indicator up/down to draw the player's eye.
/// Attach this to any highlight prefab that you toggle on/off during the tutorial.
/// </summary>
public class TutorialHighlightPulse : MonoBehaviour
{
    [SerializeField] float pulseSpeed = 2f;
    [SerializeField] float pulseAmount = 0.08f;

    Vector3 initialScale;

    void OnEnable()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = initialScale * scale;
    }
}
