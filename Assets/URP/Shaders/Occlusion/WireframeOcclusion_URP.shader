﻿/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ToF_ARFoundation/WireframeOcclusion_URP"
{
    Properties
    {
        _MainColor("Color", Color) = (1, 1, 1)
        _LineWidth("Line Width", float) = 0.01

    }
    // Universal Render Pipeline subshader. If URP is installed this will be used.
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}

        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            ZWrite On
            ColorMask 0
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers metal

            
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

        ZWrite On
        Pass
        {
            Name "Unlit"
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers metal
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            float _ClippingDistance;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float clip : DEPTH;
            };

            CBUFFER_START(UnityPerMaterial)
                float _LineWidth;
                half4 _MainColor;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vertexInput.positionCS;
                if (v.vertex.z < _ClippingDistance) {
                    o.clip = 0;
                }
                else {
                    o.clip = -1;
                }
                o.uv = v.uv;
                return o;
            }

            float mindist(float2 p0, float2 p1, float2 p) {
                float2 vw = p1 - p0;
                float l2 = dot(vw, vw);
                float t = max(0, min(1, dot(p - p0, vw) / l2));
                float2 projection = p0 + t * (vw);
                return distance(p, projection);
            }

            half4 frag(v2f i) : SV_Target
            {
    
                float depth = (i.vertex.z);
                float lineWidth = _LineWidth * 15000 * depth;
                float2 tangent = float2(ddx(i.uv.x),ddy(i.uv.x));
                float2 binormal = float2(ddx(i.uv.y),ddy(i.uv.y));
                float uvDistMax = lineWidth * length(tangent + binormal);

                float tangentDistMax = lineWidth * length(tangent);
                float binormalDistMax = lineWidth * length(binormal);
                float minUDist = min(i.uv.x,1.0 - i.uv.x);
                float minVDist = min(i.uv.y,1.0 - i.uv.y);
                float distNormU = minUDist / tangentDistMax;
                float distNormV = minVDist / binormalDistMax;

                float2 top = float2(0.0,1.0);
                float2 right = float2(1.0,0.0);
                float uvDist = mindist(top,right,i.uv);
                float uvDistNorm = uvDist / uvDistMax;

                float distNorm = min(min(distNormU,distNormV),uvDistNorm);

                float alpha = 1.0 - smoothstep(1.0,1.0 + (1.f / lineWidth),distNorm);

                //float alpha = 1.0 - 1.0 / (1.0 + exp(-2000 * (distNorm - lineWidth)));

                alpha *= _MainColor.a;
                half4 col = _MainColor;
                clip(i.clip);

                return half4(col.rgb,alpha);
            }

            ENDHLSL
        }
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            ZWrite On
            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma exclude_renderers metal
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2g
            {
                float4 vertex : SV_POSITION;
            };


            struct g2f
            {
                float3 dist : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _MainColor;
            float _LineWidth;

            v2g vert(appdata v)
            {
                v2g o;
                o.vertex = v.vertex;
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> stream)
            {
                float3 v0 = input[0].vertex;
                float3 v1 = input[1].vertex;
                float3 v2 = input[2].vertex;

                float3 e0 = v2 - v1;
                float3 e1 = v0 - v2;
                float3 e2 = v1 - v0;

                float dist0 = length(cross(e0, e2)) / length(e0);
                float dist1 = length(cross(e1, e0)) / length(e1);
                float dist2 = length(cross(e2, e1)) / length(e2);

                g2f o;
                o.vertex = TransformObjectToHClip(input[0].vertex.xyz);
                o.dist = float3(0, 0, dist0);
                stream.Append(o);

                o.vertex = TransformObjectToHClip(input[1].vertex.xyz);
                o.dist = float3(0, dist1, 0);
                stream.Append(o);

                o.vertex = TransformObjectToHClip(input[2].vertex.xyz);
                o.dist = float3(dist2, 0, 0);
                stream.Append(o);
            }

            half4 frag(g2f i) : SV_Target
            {
                float3 dist = i.dist;
                float minDist = min(dist[0], min(dist[1], dist[2]));

                float a = 1 - 1 / (1 + exp(-2000 * (minDist - _LineWidth)));

                half4 col = _MainColor;
                col.a = col.a * a;
                return col;
            }
            ENDHLSL
        }
    }
}