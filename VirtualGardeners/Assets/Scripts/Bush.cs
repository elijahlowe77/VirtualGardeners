using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    // Bush assets for different states
    public GameObject bushNormal;
    public GameObject bushPruned;

    // Particle and noise to play when pruning
    public ParticleSystem pruneParticles;
    public AudioSource pruneSound;

    private bool pruned = false;

    private void Start()
    {
        bushPruned.SetActive(false);
    }

    public void Prune() {
        if (pruned) return;

        bushNormal.SetActive(false);
        bushPruned.SetActive(true);

        if(pruneParticles != null) pruneParticles.Play();
        if(pruneSound != null) pruneSound.Play();

        pruned = true;
    }
}
