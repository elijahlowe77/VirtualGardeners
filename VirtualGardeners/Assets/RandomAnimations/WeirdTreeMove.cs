using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeirdTreeMove : MonoBehaviour
{ 
    Animator[] TreeChildrenAnimators; 
    int currentAnimatorIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        TreeChildrenAnimators = GetComponentsInChildren<Animator>(); 
        StartCoroutine(RandomAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }  
    IEnumerator RandomAnimation()
    {
        while (true)
        {
            // Wait 1 second before activating the next animator
           
            
            // Activate the current animator 
            
            TreeChildrenAnimators[currentAnimatorIndex].enabled = true; 
                yield return new WaitForSeconds(0.1f);
            
            
            // Disable the current animator and move to next
            
            currentAnimatorIndex++;
            
            // Reset index if we've gone through all animators
            if (currentAnimatorIndex >= TreeChildrenAnimators.Length)
            {
                currentAnimatorIndex = 0; 
            }
        }
    }

}
