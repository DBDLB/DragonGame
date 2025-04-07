Shader "Athena/2.0/MA/UI/MA_UI_Simple"
{
    Properties
    {
        [HideInInspector]_MainTex("MainTex",2D) = "white"{}
        _Bg("Bg Texture",2D) = "black" {}
        _CardHead ("Card Texture", 2D) = "black" {}
        _BgCostT("Bg Cost Top", 2D) = "black" {}
        _CardProperty("Card Property", 2D) = "black" {}
        _Occupation("Occupation", 2D) = "black" {}
        _BgCostB("Bg Cost Botton", 2D) = "black" {}
        _CardRamp("Card Ramp", 2D) = "black" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        

        Cull Off
        Lighting Off
        ZWrite off
        ZTest Always

        Pass
        {
            Name "Default"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_CardHead);  SAMPLER(sampler_LinearClamp);
            TEXTURE2D(_Bg);
            TEXTURE2D(_BgCostT);
            TEXTURE2D(_CardProperty);
            TEXTURE2D(_Occupation);
            TEXTURE2D(_BgCostB);
            TEXTURE2D(_CardRamp);
            
            
            CBUFFER_START (UnityPerMaterial)
            half4 _Color;
            float4 _Bg_ST;
            float4 _CardHead_ST;
            float4 _BgCostT_ST;
            float4 _CardProperty_ST;
            float4 _Occupation_ST;
            float4 _BgCostB_ST;
            float4 _CardRamp_ST;
            float4 _MainTex_TexelSize;
            CBUFFER_END

            float ClampEdge(float2 uv)
            {
               float2 quad = uv - 0.5;
               float2 len = 0.485;
               quad = step(0.99,(1-length(max(quad ,-len) - min(quad,len))));
               return quad;
            }
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = TransformObjectToHClip(v.vertex);

                OUT.texcoord = v.texcoord;
                return OUT;
            }

            half4 frag(v2f IN) : SV_Target
            {

                
                float2 uv_Bg = TRANSFORM_TEX(IN.texcoord, _Bg);
                float2 uv_cardHead = TRANSFORM_TEX(IN.texcoord, _CardHead);
                float2 uv_BgCostT = TRANSFORM_TEX(IN.texcoord, _BgCostT);
                float2 uv_CardProperty = TRANSFORM_TEX(IN.texcoord, _CardProperty);
                float2 uv_Occupation = TRANSFORM_TEX(IN.texcoord, _Occupation);
                float2 uv_BgCostB = TRANSFORM_TEX(IN.texcoord, _BgCostB);
                float2 uv_CardRamp = TRANSFORM_TEX(IN.texcoord, _CardRamp);
                
                half4 cardHead = SAMPLE_TEXTURE2D(_CardHead, sampler_LinearClamp, uv_cardHead)*ClampEdge(uv_cardHead);
                half4 BG = SAMPLE_TEXTURE2D(_Bg, sampler_LinearClamp, uv_Bg)*ClampEdge(uv_Bg);
                half4 BgCost = SAMPLE_TEXTURE2D(_BgCostT, sampler_LinearClamp, uv_BgCostT)*ClampEdge(uv_BgCostT);
                half4 CardProperty = SAMPLE_TEXTURE2D(_CardProperty, sampler_LinearClamp, uv_CardProperty)*ClampEdge(uv_CardProperty);
                half4 Occupation = SAMPLE_TEXTURE2D(_Occupation, sampler_LinearClamp, uv_Occupation)*ClampEdge(uv_Occupation);
                half4 BgCostB = SAMPLE_TEXTURE2D(_BgCostB, sampler_LinearClamp, uv_BgCostB)*ClampEdge(uv_BgCostB);
                half4 CardRamp = SAMPLE_TEXTURE2D(_CardRamp, sampler_LinearClamp, uv_CardRamp)*ClampEdge(uv_CardRamp);
                clip(saturate(cardHead.a+BG.a+BgCost.a+CardProperty.a+Occupation.a+BgCostB.a+CardRamp.a)-1.0);
                
                half4 color = lerp(cardHead,BG*BG.a, BG.a);
                color = lerp(color, BgCost, BgCost.a);
                color = lerp(color, CardProperty, CardProperty.a);
                color = lerp(color, Occupation, Occupation.a);
                color.rgb = lerp(color, CardRamp, CardRamp.a);
                color = lerp(color, BgCostB, BgCostB.a);

                return color;
            }
        ENDHLSL
        }
    }
}