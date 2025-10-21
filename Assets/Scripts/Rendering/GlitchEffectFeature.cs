using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlitchEffectFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class GlitchSettings
    {
        public Material glitchMaterial = null;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public GlitchSettings settings = new GlitchSettings();

    private GlitchEffectPass glitchPass;

    public override void Create()
    {
        if (settings.glitchMaterial != null)
        {
            glitchPass = new GlitchEffectPass(settings.glitchMaterial)
            {
                renderPassEvent = settings.renderPassEvent
            };
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.glitchMaterial == null) return;

        // no Setup needed, pass resolves the camera target itself
        renderer.EnqueuePass(glitchPass);
    }
}
