using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFollow : MonoBehaviour
{ 
    public Vector3 rotationOffset = new Vector3(0,0,-90);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform parent = transform.parent; 
        if(parent != null){ 
            foreach(Transform child in parent){
                string nameSuffix = child.name.Length > 22 ? child.name.Substring(22) : string.Empty;
                if(nameSuffix == "Attach"){
                    transform.position = child.position;
                    transform.rotation = child.rotation * Quaternion.Euler(rotationOffset);
                    return;
                }
            }
        }
    }
}
