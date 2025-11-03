using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WateringCan : MonoBehaviour
{
    public Transform spoutTip;           // tip of the can where water comes out
    public float tiltThreshold = 30f;    // degrees from upright to start pouring
    public ParticleSystem waterParticles;
    public AudioSource waterSound;

    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    void Update()
    {
        if (!grab.isSelected) return;

        if (IsPouring())
        {
            // Pour water (sound and particles)
            if (waterParticles != null && !waterParticles.isPlaying)
                waterParticles.Play();
            if (waterSound != null && !waterSound.isPlaying)
                waterSound.Play();

            // Raycast down to check for dirt below the spout
            LayerMask mask = LayerMask.GetMask("Dirt");
            Ray ray = new Ray(spoutTip.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, mask, QueryTriggerInteraction.Ignore))
            {
                // Check if hit dirt
                if (hit.collider.CompareTag("Dirt"))
                {
                    // Water dirt
                    Dirt dirt = hit.collider.GetComponentInParent<Dirt>();
                    if (dirt != null)
                        dirt.Water();
                }
            }
        }
        else
        {
            // Stop pouring
            if (waterParticles != null && waterParticles.isPlaying)
                waterParticles.Stop();
            if (waterSound != null && waterSound.isPlaying)
                waterSound.Stop();
        }
    }


    bool IsPouring()
    {
        // Get the forward/down direction of the spout
        Vector3 spoutDirection = spoutTip.forward; // or transform.forward if no spoutTip
        // Pour if pointing downward enough
        float angle = Vector3.Angle(spoutDirection, Vector3.down);
        return angle < tiltThreshold;
    }

}
