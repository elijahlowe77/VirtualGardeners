using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTrigger : MonoBehaviour
{ 
    public RTManager rtManager;
    public RenderTexture transitionRT; 
    public ParticleSystem particleSystem; 
    public GameObject FirstChild;
    // Start is called before the first frame update
    void Start()
    {
        FirstChild = transform.GetChild(0).gameObject; 
        FirstChild.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    } 
    void OnTriggerEnter(Collider other)
    { 
        Debug.Log(other.name);
        if (other.name == "PlayerModel")
        {
            Debug.Log("Player entered the trigger");
            rtManager.StartTransition(transitionRT); 
            particleSystem.Play(); 
            StartCoroutine(WaitForTransition());
        } 
        
    }
    IEnumerator WaitForTransition()
    {
        yield return new WaitForSeconds(1f);
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
