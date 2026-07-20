using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Zone reveal trigger: crossfades ZonShader to a target RT, plays VFX, then enables reveal content.
/// </summary>
public class ShaderTrigger : MonoBehaviour
{
    [FormerlySerializedAs("rtManager")]
    public RTManager zoneRtManager;

    [FormerlySerializedAs("transitionRT")]
    public RenderTexture targetZoneRT;

    [FormerlySerializedAs("particleSystem")]
    public ParticleSystem revealParticles;

    [FormerlySerializedAs("FirstChild")]
    public GameObject revealContent;

    [SerializeField] private float contentRevealDelay = 1f;
    [SerializeField] private string playerObjectName = "PlayerModel";

    void Start()
    {
        if (revealContent == null && transform.childCount > 0)
            revealContent = transform.GetChild(0).gameObject;

        if (revealContent != null)
            revealContent.SetActive(false);

        if (zoneRtManager != null && targetZoneRT != null)
            zoneRtManager.StartTransition(targetZoneRT);

        StartCoroutine(RevealContentAfterDelay());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name != playerObjectName)
            return;

        if (zoneRtManager != null && targetZoneRT != null)
            zoneRtManager.StartTransition(targetZoneRT);

        if (revealParticles != null)
            revealParticles.Play();

        StartCoroutine(RevealContentAfterDelay());
    }

    IEnumerator RevealContentAfterDelay()
    {
        yield return new WaitForSeconds(contentRevealDelay);
        if (revealContent != null)
            revealContent.SetActive(true);
    }
}
