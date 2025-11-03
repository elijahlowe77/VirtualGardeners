using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GardenTool : MonoBehaviour
{
    public string stateToSet;
    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Only work when the player is holding it
        if (!grab.isSelected) return;

        if (other.CompareTag("Dirt"))
        {
            Dirt dirt = other.GetComponentInParent<Dirt>();
            if (dirt != null) dirt.ChangeState(stateToSet);
        }
    }
}
