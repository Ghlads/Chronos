Shader "Chronos/Sea"
{
    Properties
    { 
        [MainTex] _MainTex("Wave Mask", 2D) = "white" {}
        [MainColor] _MainColor("Water color", Color) = (0,0,1,1)
        _ShadowColor("Shadow Color", Color) = (0,0,1,1)
        _HighlightColor("HighLight", Color) = (0.8,0.8,1,1)
        _WaveSpeed("Wave Speed", Vector) = (1,.5,0,0)
        _PixelisationFactor("Pixel Unit", float) = 16
        _NoiseStrenght("Wave Strenght", float) = 1
        _CalmNoiseInfo("Calm Zone Size/Strength/Direction", Vector) = (1,1,0,0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _MainColor;
            float4 _ShadowColor;
            float4 _HighlightColor;
            float4 _WaveSpeed;
            float _PixelisationFactor;
            float _NoiseStrenght;
            float4 _CalmNoiseInfo;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS.xyz);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float3 pixelatedWS = floor(IN.positionWS * _PixelisationFactor) / _PixelisationFactor;
                float noise = SimplexNoise(pixelatedWS.xy);
                float calmZoneNoise = SimplexNoise(floor((IN.positionWS * _CalmNoiseInfo.x + (_Time.y * _CalmNoiseInfo.zw)) * _PixelisationFactor) / _PixelisationFactor) * _CalmNoiseInfo.y;
                calmZoneNoise = saturate(calmZoneNoise);
                float specularMask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, (IN.positionWS.xy * _MainTex_ST.xy) + (_Time.y * _WaveSpeed.yz) + (noise * _NoiseStrenght)).r;
                float waveMask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, (IN.positionWS.xy * _MainTex_ST.xy) + (_Time.y * _WaveSpeed.xy) + (noise * _NoiseStrenght)).g;
                float shadowMask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, (IN.positionWS.xy * _MainTex_ST.xy) + (_Time.y * _WaveSpeed.zw) + (noise * _NoiseStrenght)).b;
                float4 color = lerp(_MainColor, _ShadowColor, shadowMask);
                color = lerp(color, _HighlightColor, waveMask);
                color = lerp(color, (1,1,1,1), specularMask * waveMask);
                color = lerp(_MainColor, color, max(calmZoneNoise, 0.1));
                return color;
            }
            ENDHLSL
        }
    }
}