using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Plant : MonoBehaviour
{
    public ProgressBar progressBar; // progress bar to update
    private XRGrabInteractable grabInteractable; // lets user grab plant
    bool bloom = false; // check if plant has bloomed

    // Start is called before the first frame update
    void Start()
    {
        if(progressBar == null) {
            progressBar = FindObjectOfType<ProgressBar>();
        }

        grabInteractable = GetComponent<XRGrabInteractable>(); // get the XRGrabInteractable

        // register events
        grabInteractable.selectEntered.AddListener(OnGrabbed);

        // add plant
        progressBar.addPlant();
    }

    private void OnGrabbed(SelectEnterEventArgs args) {
        // if its already registered as bloomed, then return
        if(bloom) {
            return;
        }

        progressBar.bloomPlant();
        bloom = true;
    }


    void OnDestroy() {
        // if its registered as bloomed, then decrease numBloomed count as well
        if(bloom) {
            progressBar.removeBloomPlant();
            return;
        }

        progressBar.removePlant();
    }
}
