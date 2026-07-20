using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Dual-RT driver for ZonShader (base + transition). Prefer RTManager for transition-only workflows.
/// </summary>
public class SunRenderTextureManager : MonoBehaviour
{
    private static readonly int BaseTextureId = Shader.PropertyToID("_BaseTexture");
    private static readonly int TransitionTextureId = Shader.PropertyToID("_TransitionTexture");
    private static readonly int TransitionBlendId = Shader.PropertyToID("_TransitionBlend");

    [Header("Shader")]
    [FormerlySerializedAs("sunShader")]
    [SerializeField] private Shader zonShader;

    [Header("Render Textures")]
    [FormerlySerializedAs("sunRT_A")]
    [SerializeField] private RenderTexture baseRT;

    [FormerlySerializedAs("sunRT_B")]
    [SerializeField] private RenderTexture transitionRT;

    [Header("Blend")]
    [Range(0f, 1f)]
    [FormerlySerializedAs("blend")]
    [SerializeField] private float transitionBlend = 0f;

    private readonly List<Renderer> zonRenderers = new();
    private MaterialPropertyBlock block;

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
            if (baseRT != null)
                block.SetTexture(BaseTextureId, baseRT);
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
}
