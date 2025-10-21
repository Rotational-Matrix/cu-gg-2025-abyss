using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlitchEffect : ScriptableRendererFeature
{
    class GlitchPass : ScriptableRenderPass
    {
        public Material material;
        private RenderTargetIdentifier source;
        private RenderTargetHandle temporaryColorTexture;

        public GlitchPass(Material mat)
        {
            this.material = mat;
            temporaryColorTexture.Init("_TemporaryColorTexture");
        }

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("GlitchEffect");

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(temporaryColorTexture.id, opaqueDesc);
            cmd.Blit(source, temporaryColorTexture.Identifier(), material);
            cmd.Blit(temporaryColorTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public Material glitchMaterial;
    GlitchPass glitchPass;

    public override void Create()
    {
        glitchPass = new GlitchPass(glitchMaterial);
        glitchPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (glitchMaterial != null)
            glitchPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(glitchPass);
    }
}
