/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "Unlit/Occlusion_URP"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            ColorMask 0
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _ClippingDistance;

            struct appdata
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv           : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
                float clip : DEPTH;
            };

            v2f vert(appdata v)
            {
                v2f o;
                if (v.positionOS.z < _ClippingDistance) {
                    o.clip = 0;
                }
                else {
                    o.clip = -1;
                }
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                clip(i.clip);
                return half4(0,0,0,0);
            }
            ENDHLSL
        }
    }
}
