using System.Collections.Generic;
using UnityEngine;

public class SunRenderTextureManager : MonoBehaviour
{
    [Header("Shader")]
    [SerializeField] private Shader sunShader;

    [Header("Render Textures")]
    [SerializeField] private RenderTexture sunRT_A;
    [SerializeField] private RenderTexture sunRT_B;

    [Header("Blend")]
    [Range(0f, 1f)]
    [SerializeField] private float blend = 0f;

    private readonly List<Renderer> sunRenderers = new();
    private MaterialPropertyBlock block;

    void Awake()
    {
        block = new MaterialPropertyBlock();

        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        foreach (Renderer r in allRenderers)
        {
            if (r.sharedMaterial != null && r.sharedMaterial.shader == sunShader)
            {
                sunRenderers.Add(r);
            }
        }
    }

    void LateUpdate()
    {
        foreach (Renderer r in sunRenderers)
        {
            r.GetPropertyBlock(block);
            block.SetTexture("_SunRT_A", sunRT_A);
            block.SetTexture("_SunRT_B", sunRT_B);
            block.SetFloat("_Blend", blend);
            r.SetPropertyBlock(block);
        }
    }

    // Public API
    public void SetBlend(float value)
    {
        blend = Mathf.Clamp01(value);
    }

    public void SwapSecondRT(RenderTexture newRT)
    {
        sunRT_B = newRT;
    }
}

