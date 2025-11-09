using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget; 
    public Transform headTarget; 
    
    public Transform ikTarget;
    public Vector3 trackingPositionOffset; 
    public Vector3 trackingRotationOffset; 
    public Vector3 headTargetOffset;
    public void Map()
    {
        
        if(headTarget != null){  
            ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);    
            ikTarget.position = new Vector3(ikTarget.position.x, headTarget.position.y + headTargetOffset.y, ikTarget.position.z); 
            ikTarget.rotation = headTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        } 
        else{ 
            ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);  
            ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }
    } 
    
 
} 


public class IKTargetFollowVRRig : MonoBehaviour
{
    [Range(0,1)]
    public float turnSmoothness = 0.1f;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Vector3 headBodyPositionOffset;
    public float headBodyYawOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = head.ikTarget.position + headBodyPositionOffset;
        float yaw = head.vrTarget.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z),turnSmoothness);
        
        head.Map();
        
        leftHand.Map();
        rightHand.Map();
    } 
    
}
