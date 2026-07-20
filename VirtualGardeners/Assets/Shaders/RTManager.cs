using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Drives ZonShader zone reveal: pushes transition RT + blend via MaterialPropertyBlock.
/// </summary>
public class RTManager : MonoBehaviour
{
    private static readonly int TransitionTextureId = Shader.PropertyToID("_TransitionTexture");
    private static readonly int TransitionBlendId = Shader.PropertyToID("_TransitionBlend");

    [Header("Shader")]
    [FormerlySerializedAs("sunShader")]
    [SerializeField] private Shader zonShader;

    [Header("Render Textures")]
    [FormerlySerializedAs("sunRT_B")]
    [SerializeField] private RenderTexture transitionRT;

    [Header("Blend")]
    [Range(0f, 1f)]
    [FormerlySerializedAs("blend")]
    [SerializeField] private float transitionBlend = 0f;

    [SerializeField] private float transitionDuration = 3f;

    private readonly List<Renderer> zonRenderers = new();
    private MaterialPropertyBlock block;
    private Coroutine transitionCoroutine;

    void Awake()
    {
        block = new MaterialPropertyBlock();

        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        foreach (Renderer r in allRenderers)
        {
            if (r.sharedMaterial != null && r.sharedMaterial.shader == zonShader)
            {
                zonRenderers.Add(r);
            }
        }
    }

    void LateUpdate()
    {
        foreach (Renderer r in zonRenderers)
        {
            r.GetPropertyBlock(block);
            if (transitionRT != null)
                block.SetTexture(TransitionTextureId, transitionRT);
            block.SetFloat(TransitionBlendId, transitionBlend);
            r.SetPropertyBlock(block);
        }
    }

    public void SetBlend(float value)
    {
        transitionBlend = Mathf.Clamp01(value);
    }

    public void SwapTransitionRT(RenderTexture newRT)
    {
        transitionRT = newRT;
    }

    public void StartTransition(RenderTexture newRT)
    {
        SwapTransitionRT(newRT);

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(TransitionBlendRoutine(transitionDuration));
    }

    private IEnumerator TransitionBlendRoutine(float duration)
    {
        float startBlend = 0f;
        float endBlend = 1f;
        float elapsed = 0f;

        transitionBlend = startBlend;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transitionBlend = Mathf.Lerp(startBlend, endBlend, elapsed / duration);
            yield return null;
        }

        transitionBlend = endBlend;
        transitionCoroutine = null;
    }
}
