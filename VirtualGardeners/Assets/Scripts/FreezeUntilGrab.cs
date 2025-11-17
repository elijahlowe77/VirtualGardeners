using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FreezeUntilGrab : MonoBehaviour
{
    Rigidbody rb;
    XRGrabInteractable grab;
    bool hasBeenGrabbed = false;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        // Freeze everything
        rb.constraints = RigidbodyConstraints.FreezeAll;

        grab.selectEntered.AddListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args) {
        if (hasBeenGrabbed) return;
        hasBeenGrabbed = true;

        // Unfreeze all
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
    }
}
