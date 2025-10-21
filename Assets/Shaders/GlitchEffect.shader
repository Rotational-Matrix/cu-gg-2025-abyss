Shader "Custom/GlitchEffectURP"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _ChromAberrX("Chromatic X", Float) = 0.01
        _ChromAberrY("Chromatic Y", Float) = 0.01
        _Displace("Displacement", Vector) = (0,0,0,0)
        _WavyFreq("Wavy Freq", Float) = 10
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
            Pass
            {
                Name "GlitchPass"
                ZWrite Off Cull Off
                HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);

                float _ChromAberrX;
                float _ChromAberrY;
                float4 _Displace;
                float _WavyFreq;

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct Varyings
                {
                    float4 positionHCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                Varyings Vert(Attributes IN)
                {
                    Varyings OUT;
                    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                    OUT.uv = IN.uv;
                    return OUT;
                }

                float rand(float2 co)
                {
                    return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
                }

                float4 Frag(Varyings IN) : SV_Target
                {
                    float2 uv = IN.uv;

                    // stripes
                    float stripesR = step(0.7, rand(floor(uv.y * 10)));
                    float stripesL = step(0.7, rand(floor(uv.y * 10)));

                    float2 disp = (_Displace.xy * stripesR) - (_Displace.xy * stripesL);
                    float wavy = (sin(uv.y * _WavyFreq) + 1) / 2;
                    disp += (_Displace.zw * wavy);

                    // chromatic
                    float r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + disp + float2(_ChromAberrX, _ChromAberrY)).r;
                    float g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + disp).g;
                    float b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + disp - float2(_ChromAberrX, _ChromAberrY)).b;

                    return float4(r, g, b, 1);
                }
                ENDHLSL
            }
        }
}
