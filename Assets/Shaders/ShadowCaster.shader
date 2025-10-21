Shader "CustomRenderTexture/ShadowCaster"
{
    SubShader{
        Tags { "RenderType" = "Opaque" }
        LOD 100

        ColorMask 0
        ZWrite On

        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
        }
    }
}
