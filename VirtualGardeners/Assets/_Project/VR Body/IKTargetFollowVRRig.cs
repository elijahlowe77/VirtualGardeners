using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget; 
    public Transform headTarget; 
    public Transform ikTarget;
    public Vector3 trackingPositionOffset;   
     
    private Vector3 PreviousTrackingPositionOffset; 
    private Vector3 PreviousTrackingRotationOffset;
    public Vector3 trackingRotationOffset; 
    public Vector3 headTargetOffset; 
    public Vector3 GrabOffsetPosition;  
    public Vector3 GrabOffsetRotation; 
    public void Map()
    {
        
         if(headTarget != null){  
             ikTarget.position = headTarget.TransformPoint(trackingPositionOffset);    
             ikTarget.position = new Vector3(ikTarget.position.x, headTarget.position.y + headTargetOffset.y, ikTarget.position.z); 
             ikTarget.rotation = headTarget.rotation * Quaternion.Euler(trackingRotationOffset);
       } 
       else{
            ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);  
            ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }
    }  
    
    public void Grab()
    {
        PreviousTrackingPositionOffset = trackingPositionOffset; 
        PreviousTrackingRotationOffset = trackingRotationOffset;
        trackingPositionOffset = GrabOffsetPosition;
        trackingRotationOffset = GrabOffsetRotation;
    } 
    public void Release()
    {
        trackingPositionOffset = PreviousTrackingPositionOffset;
        trackingRotationOffset = PreviousTrackingRotationOffset;
    }
 
} 
 


public class IKTargetFollowVRRig : MonoBehaviour
{
    [Range(0,1)]
    public float turnSmoothness = 0.1f;
    public VRMap head; 
    public VRMap leftHand;
    public VRMap rightHand; 
    private Animator animator;
    [Tooltip("Input action property that represents the grab button for either controller.")]
    public InputActionProperty grabInputLeft;
    public InputActionProperty grabInputRight; 
    public InputActionProperty moveInput;

   
    public Vector3 headBodyPositionOffset;
    public float headBodyYawOffset;

    void OnEnable()
    {
        if (grabInputLeft.action != null)
            grabInputLeft.action.Enable();
        if (grabInputRight.action != null)
            grabInputRight.action.Enable(); 
        if (moveInput.action != null)
            moveInput.action.Enable();
        }

    void OnDisable()
    {
        if (grabInputLeft.action != null)
            grabInputLeft.action.Disable();
        if (grabInputRight.action != null)
            grabInputRight.action.Disable();
        if (moveInput.action != null)
            moveInput.action.Disable();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = head.ikTarget.position + headBodyPositionOffset;
        float yaw = head.vrTarget.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z),turnSmoothness);
        
        head.Map();
        
        leftHand.Map();
        rightHand.Map();

        HandleGrab();
        HandleMove();
    } 

    public void HandleMove()
    { 
       
        var moveInputValue = moveInput.action.ReadValue<Vector2>();
        if(moveInputValue!=Vector2.zero)
        {
            animator.SetBool("Walking", true);
        }else{
            animator.SetBool("Walking", false);
        }
    }

    public void HandleGrab()
    {
        var actionLeft = grabInputLeft.action;
        var actionRight = grabInputRight.action;
        if (actionLeft == null || actionRight == null)
        {
            Debug.LogWarning("Grab action not assigned. Please set the grab input action in the inspector.", this);
            return;
        } 

        if (actionLeft.WasPressedThisFrame())
        {
            Debug.Log("Left hand grab input pressed", this); 
            leftHand.Grab();
        }else if (actionLeft.WasReleasedThisFrame())
        {
            Debug.Log("Left hand grab input released", this); 
            leftHand.Release();
        }
        if (actionRight.WasPressedThisFrame())
        {
            Debug.Log("Right hand grab input pressed", this);
            rightHand.Grab();
        }else if (actionRight.WasReleasedThisFrame())
        {
            Debug.Log("Right hand grab input released", this);
            rightHand.Release();
        }
    }
}

