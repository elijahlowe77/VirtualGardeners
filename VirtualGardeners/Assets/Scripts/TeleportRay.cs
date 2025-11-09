using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportRayToggle : MonoBehaviour {
    public XRRayInteractor ray;
    public XRInteractorLineVisual lineVisual;
    public InputActionProperty teleportAction;
    public XRDirectInteractor gripHand; // the hand this script is attached to

    void Update() {
        bool holdingGrip = gripHand.firstInteractableSelected != null;  // true if hand is holding something
        bool held = teleportAction.action.IsPressed();

        ray.gameObject.SetActive(true);                   // interactor always active
        lineVisual.enabled = held && !holdingGrip;        // only show if teleport pressed AND not gripping
    }
}
