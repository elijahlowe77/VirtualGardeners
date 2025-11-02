using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class SeedSpawner : MonoBehaviour
{
    public GameObject seedPrefab;

    private XRSimpleInteractable interactable;
    private XRInteractionManager interactionManager;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnGrabAttempt);

        // Get the manager (usually on XR Rig)
        interactionManager = FindObjectOfType<XRInteractionManager>();
    }

    void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnGrabAttempt);
    }

    private void OnGrabAttempt(SelectEnterEventArgs args)
    {
        Transform handTransform = args.interactorObject.transform;

        // Spawn seed
        GameObject seed = Instantiate(seedPrefab, handTransform.position, handTransform.rotation);

        // Force grab using the interaction manager
        XRGrabInteractable grabInteractable = seed.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null && interactionManager != null)
        {
            // Select the new seed with the same interactor
            interactionManager.SelectEnter(args.interactorObject, grabInteractable);
            
            // Release the spawner
            interactable.interactionManager.SelectExit(args.interactorObject, interactable);
        }
    }
}
