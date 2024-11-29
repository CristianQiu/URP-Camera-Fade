Shader "Hidden/FullScreenBlur"
{
    SubShader
    {
        Tags
        { 
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "FullScreenBlurHorizontal"
            
            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "./GaussianBlur.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            int _BlurKernelRadius;
            float _BlurStandardDeviation;

            SAMPLER(sampler_BlitTexture);

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 originalColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord);
                float3 blurred = GaussianBlur(input.texcoord, float2(1.0, 0.0), _BlurKernelRadius, _BlurStandardDeviation, _BlitTexture, sampler_PointClamp, _BlitTexture_TexelSize.xy);

                return float4(blurred, originalColor.a);
            }

            ENDHLSL
        }

        Pass
        {
            Name "FullScreenBlurVertical"
            
            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "./GaussianBlur.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            int _BlurKernelRadius;
            float _BlurStandardDeviation;

            SAMPLER(sampler_BlitTexture);

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 originalColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord);
                float3 blurred = GaussianBlur(input.texcoord, float2(0.0, 1.0), _BlurKernelRadius, _BlurStandardDeviation, _BlitTexture, sampler_PointClamp, _BlitTexture_TexelSize.xy);

                return float4(blurred, originalColor.a);
            }

            ENDHLSL
        }
    }

    Fallback Off
}