using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class SocketStarter : MonoBehaviour
{
    [Tooltip("The tool to start in this socket")]
    public XRGrabInteractable startingTool;

    private XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    void Start()
    {
        if (startingTool != null)
        {
            // Move the tool to the socket's attach transform
            Transform attach = socket.attachTransform ? socket.attachTransform : socket.transform;
            startingTool.transform.position = attach.position;
            startingTool.transform.rotation = attach.rotation;

            // Force selection so the socket considers it "grabbed"
            socket.StartManualInteraction(startingTool as IXRSelectInteractable);
        }
    }
}
