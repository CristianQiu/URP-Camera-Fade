Shader "Hidden/CameraFade"
{
    SubShader
    {
        Tags
        { 
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "CameraFade"
            
            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float _Progress;
            float3 _Color;

            SAMPLER(sampler_BlitTexture);

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 cameraColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord);
                float4 targetColor = float4(_Color, cameraColor.a);

                return lerp(cameraColor, targetColor, _Progress);
            }

            ENDHLSL
        }
    }

    Fallback Off
}