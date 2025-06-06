﻿/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "TofAr/Tof/ConfidenceViewShader_URP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Strength("Strength", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off  ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
			#include_with_pragmas "Assets/TofAr/TofAr/V0/Shaders/TofArCommon.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Strength;
            int _isLinearColorSpace;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = float2(i.uv.x, 1 - i.uv.y);
                fixed4 col = tex2D(_MainTex, uv);
                col *= 16;

                float u16 = ((uint)col.r * (uint)pow(2, 3 * 4)) + ((uint)col.g * (uint)pow(2, 2 * 4)) + ((uint)col.b * (uint)pow(2, 1 * 4)) + col.a;

                float grey = u16 / 7.0f * _Strength;
                col = fixed4(grey, grey, grey, 1);

                col = toGamma(col, _isLinearColorSpace);

                return col;
            }
            ENDCG
        }
    }
}
