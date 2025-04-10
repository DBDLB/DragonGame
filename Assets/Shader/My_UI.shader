Shader "My/UI/Default"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [hdr]_Color ("Tint", Color) = (1,1,1,1)
        _Progress ("Progress", Range(0, 1)) = 0.5
        [hdr]_WaterLineColor ("Water Line Color", Color) = (1,1,1,0.5)
        _waterWaveWeight ("Water Wave Weight", Range(0, 1)) = 0.01

        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255

        [HideInInspector]_ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest ("ZTest", Int) = 4
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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite on
        ZTest [_ZTest]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

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
                float4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);  SAMPLER(sampler_LinearClamp);
            CBUFFER_START (UnityPerMaterial)
            float4 _Color;
            float4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _Progress;
            float4 _WaterLineColor;
            float _waterWaveWeight;
            CBUFFER_END

            // 计算颜色灰度
            float Luminance(float3 color)
            {
                // 使用标准亮度系数计算灰度值
                return dot(color, float3(0.2126, 0.7152, 0.0722));
            }
            
            // 将彩色转为灰度色
            half4 GrayscaleColor(half4 color)
            {
                // 计算灰度值
                float gray = Luminance(color.rgb);
                // 返回灰度颜色，保留原alpha值
                return half4(gray, gray, gray, color.a);
            }

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = TransformObjectToHClip(v.vertex);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            float4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                // 垂直波纹效果
                uv.y += sin(uv.x*30 + _Time.y*3) * 0.007;
                uv.y += sin(uv.x*20 + _Time.y*4) * 0.01;
                
                // half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                half4 color = (SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, IN.texcoord) + _TextureSampleAdd)* IN.color;

                // 基于IN.texcoord.y和进度值计算裁剪效果
                // 若IN.texcoord.y大于_Progress，则该像素为空（透明）
                // 转换为灰度
                half4 cardHeadGray = GrayscaleColor(color);
                color = lerp(cardHeadGray, color,step(uv.y, _Progress));

                // 添加波动参数
                float _WaterWaveStrength = 0.01; // 在Properties中声明
                float _WaterWaveFreq = 30.0;

                float waterLineA = (smoothstep(0,_waterWaveWeight,(uv.y-_Progress))).xxx;
                float waterLineB = (smoothstep(0,_waterWaveWeight,(_Progress-uv.y))).xxx;

                float waterLine = 1-saturate(max(waterLineA,waterLineB));
                
                // 使用sin函数创建波浪边缘
                // float waterLine = _Progress + sin(uv.x*30 + _Time.y*3) * 0.07 + sin(uv.x*20 + _Time.y*4) * 0.01;
                // float waterEdge = smoothstep(uv.y, uv.y + 0.1, _Progress);
                // color = lerp(cardHeadGray, color, waterEdge);
                
                                // 添加高光参数
                // float4 _WaterLineColor = float4(1,1,1,0.5); // 在Properties中声明
                
                // 计算水线高光区域
                // float highlight = saturate(1.0 - abs(uv.y-waterEdge) / 0.1);
                // color = lerp(cardHeadGray, color, waterEdge);
                color = lerp(color, _WaterLineColor*color, waterLine);
                // color.rgb += _WaterLineColor.rgb * highlight * _WaterLineColor.a;

                
                // 应用颜色乘法
                // color *= IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                // color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return float4(color);
            }
        ENDHLSL
        }
    }
}