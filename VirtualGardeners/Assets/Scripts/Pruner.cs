using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class Pruner : MonoBehaviour
{
    private bool isUsing;
    public float cooldown;
    public AudioSource useSound;

    public void Activate()
    {
        if (!isUsing) StartCoroutine(ToggleRoutine());
    }

    private IEnumerator ToggleRoutine()
    {
        isUsing = true;
        if(useSound != null) useSound.Play();
        // ADD ANIMATION HERE
        yield return new WaitForSeconds(cooldown);
        isUsing = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (isUsing && other.CompareTag("Bush"))
        {
            other.GetComponentInParent<Bush>()?.Prune();
        }
    }
}
