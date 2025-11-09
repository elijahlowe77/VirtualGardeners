using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Dirt : MonoBehaviour
{
    // Assets to be swapped in and out, should align
    [Header("Assets")]
    public GameObject dirtNormal;   // "Normal"
    public GameObject dirtHole;     // "Hole"
    public GameObject dirtSeeded;   // "Seeded"
    public GameObject dirtRaked;    // "Raked"
    public GameObject sprout;       // Initial plant sprout
    public GameObject plant;        // Final plant

    // Particles for different states
    [Header("Particles")]
    public ParticleSystem digParticles;
    public ParticleSystem seedParticles;
    public ParticleSystem rakeParticles;
    public ParticleSystem growthParticles;

    // Sound to play when entering that state
    [Header("Sounds")]
    public AudioSource digSound;
    public AudioSource seedSound;
    public AudioSource rakeSound;

    // Material to set dirt to when watered + Water needed to be watered
    [Header("Water")]
    public Material wetMaterial;
    public float waterNeeded = 50;
    private float waterAmount = 0;

    [Header("Growth")]
    public float sproutTime = 10f;
    public float growthTime = 10f;

    [HideInInspector] public string state = "Normal";
    [HideInInspector] public bool watered = false;

    private void Start()
    {
        dirtHole.SetActive(false);
        dirtSeeded.SetActive(false);
        dirtRaked.SetActive(false);
        sprout.SetActive(false);
        plant.SetActive(false);
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

            // Begin sprout timer
            StartCoroutine(Grow());
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
        if (waterAmount < waterNeeded)
        {
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
    
    IEnumerator Grow()
    {
        if(growthParticles != null) growthParticles.Play();
        yield return new WaitForSeconds(sproutTime);
        if(growthParticles != null) growthParticles.Stop();
        sprout.SetActive(true);

        // Halt growth until watered & raked
        yield return new WaitUntil(() => (watered && state == "Raked"));

        if(growthParticles != null) growthParticles.Play();
        yield return new WaitForSeconds(growthTime);
        if(growthParticles != null) growthParticles.Stop();
        sprout.SetActive(false);
        plant.SetActive(true);
    }
}
