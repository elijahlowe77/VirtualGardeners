using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    // Bush assets for different states
    public GameObject bushNormal;
    public GameObject bushTrimmed;
    public GameObject bushPruned;

    // Particle and noise to play when pruning
    public ParticleSystem pruneParticles;
    public AudioClip pruneSound;

    [HideInInspector]
    public float state = 0; // 0 = untrimmed, 1 = partially trimmed, 2 = fully trimmed

    private void Start()
    {
        bushTrimmed.SetActive(false);
        bushPruned.SetActive(false);
    }

    public void Prune() {
        if (state >= 2) return;
        state++;

        if (state == 1) {
            bushNormal.SetActive(false);
            bushTrimmed.SetActive(true);
        }
        else if (state == 2) {
            bushTrimmed.SetActive(false);
            bushPruned.SetActive(true);
        }

        if (pruneParticles != null)
        {
            ParticleSystem ps = Instantiate(pruneParticles, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        if (pruneSound != null) AudioSource.PlayClipAtPoint(pruneSound, transform.position, 0.5f);
    }
}
