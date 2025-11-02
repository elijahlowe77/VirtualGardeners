using System.Collections.Generic;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    // Dirt assets for different states
    public GameObject dirtNormal;   // "Normal"
    public GameObject dirtHole;     // "Hole"
    public GameObject dirtSeeded;   // "Seeded"
    public GameObject dirtRaked;    // "Raked"

    // Particles for different states
    public ParticleSystem digParticles;
    public ParticleSystem seedParticles;
    public ParticleSystem rakeParticles;

    // Sound to play when entering that state
    public AudioSource digSound;
    public AudioSource seedSound;
    public AudioSource rakeSound;

    // Material to set dirt to when watered + Water needed to be watered
    public Material wetMaterial;
    public float waterNeeded = 50;
    private float waterAmount = 0;
    
    public string state = "Normal";
    private bool watered = false;

    private void Start()
    {
        dirtHole.SetActive(false);
        dirtSeeded.SetActive(false);
        dirtRaked.SetActive(false);
    }

    // Seed Detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Seed") && state == "Hole")
        {
            Destroy(other.gameObject); // delete seed
            ChangeState("Seeded");
        }
    }

    public void ChangeState(string newState)
    {
        if (state == "Normal" && newState == "Hole")
        {
            dirtNormal.SetActive(false);
            dirtHole.SetActive(true);

            if(digParticles != null) digParticles.Play();
            if(digSound != null) digSound.Play();

            state = newState;
        }
        else if (state == "Hole" && newState == "Seeded")
        {
            dirtHole.SetActive(false);
            dirtSeeded.SetActive(true);

            if(seedParticles != null) seedParticles.Play();
            if(seedSound != null) seedSound.Play();

            state = newState;
        }
        else if (state == "Seeded" && newState == "Raked")
        {
            dirtSeeded.SetActive(false);
            dirtRaked.SetActive(true);

            if(rakeParticles != null) rakeParticles.Play();
            if(rakeSound != null) rakeSound.Play();

            state = newState;
        }
    }

    public void Water()
    {
        // Increase water amount each time called until watered
        if (watered) return;
        if (waterAmount < waterNeeded) {
            waterAmount++;
            return;
        }

        List<GameObject> dirtObjects = new List<GameObject>()
        {dirtNormal, dirtHole, dirtSeeded, dirtRaked};

        // Set all materials to wet
        foreach (GameObject dirt in dirtObjects)
        {
            if (dirt != null)
            {
                Renderer rend = dirt.GetComponent<Renderer>();
                if (rend != null)
                    rend.material = wetMaterial;
            }
        }

        watered = true;
    }
}
