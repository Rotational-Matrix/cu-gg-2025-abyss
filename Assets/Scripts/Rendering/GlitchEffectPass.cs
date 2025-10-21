using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlitchEffectPass : ScriptableRenderPass
{
    private Material material;
    private RenderTargetHandle tempTexture;

    public GlitchEffectPass(Material mat)
    {
        this.material = mat;
        tempTexture.Init("_TempGlitchTexture");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (material == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("GlitchEffect"); 
        RenderTargetIdentifier cameraTarget = renderingData.cameraData.renderer.cameraColorTarget;

        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        opaqueDesc.depthBufferBits = 0;

        cmd.GetTemporaryRT(tempTexture.id, opaqueDesc, FilterMode.Bilinear);
        cmd.Blit(cameraTarget, tempTexture.Identifier(), material);
        cmd.Blit(tempTexture.Identifier(), cameraTarget);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        if (cmd == null) return;
        cmd.ReleaseTemporaryRT(tempTexture.id);
    }
}
