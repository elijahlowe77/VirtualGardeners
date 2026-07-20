using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Instantly sets ZonShader transition blend when the player enters/exits a trigger.
/// </summary>
public class PlayerSunTrigger : MonoBehaviour
{
    [FormerlySerializedAs("sunManager")]
    public RTManager zoneRtManager;

    public float blendOnEnter = 1f;
    public float blendOnExit = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && zoneRtManager != null)
            zoneRtManager.SetBlend(blendOnEnter);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && zoneRtManager != null)
            zoneRtManager.SetBlend(blendOnExit);
    }
}
