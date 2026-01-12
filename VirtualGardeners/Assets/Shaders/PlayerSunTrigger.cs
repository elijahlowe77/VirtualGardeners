using UnityEngine;

public class PlayerSunTrigger : MonoBehaviour
{
    public RTManager sunManager;
    public float blendOnEnter = 1f;
    public float blendOnExit = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sunManager.SetBlend(blendOnEnter);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sunManager.SetBlend(blendOnExit);
        }
    }
}

