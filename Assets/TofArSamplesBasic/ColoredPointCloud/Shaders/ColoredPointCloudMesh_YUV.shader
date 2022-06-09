/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "TofAr/ColoredPointCloudMesh_YUV"
{
  Properties
  {
    _YTex ("Y Texture", 2D) = "Black" {}
    _UVTex ("UV Texture", 2D) = "Black" {}
    _PointSize("Point Size", float) = 0.01
    _ClippingDistance("Clipping Distance", float) = 16
  }

  SubShader
  {
    Tags { "RenderType" = "Opaque" }
    LOD 100

    Pass
    {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"
        float _ClippingDistance;

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float psize : PSIZE;
            float clip : DEPTH;
        };
        
        sampler2D _YTex;
        float4 _YTex_ST;
        sampler2D _UVTex;
        float _YUVShader_paddingCutOff;
        float _PointSize;
        

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            if (v.vertex.z < _ClippingDistance) {
                o.clip = 0;
            }
            else {
                o.clip = -1;
            }
            o.uv =  TRANSFORM_TEX(v.uv, _YTex);
            o.psize = _PointSize * 1000;
            return o;
        }

        fixed4 frag(v2f i) : COLOR
        {
            float3x3 yuv2rgb = float3x3(
                1.164f,  1.596f,  0.0f,
                1.164f, -0.813f, -0.391f,
                1.164f,  0.0f,    2.018f);

            fixed4 Y = tex2D(_YTex, i.uv);
            fixed4 UV = tex2D(_UVTex, i.uv);
            float y = Y.a - (16.0f / 255.0f);
            clamp(y, 0.0, 1.0);
            float v = UV.r * (15.0f * 16.0f / 255.0f) + UV.g * (16.0f / 255.0f);
            float u = UV.b * (15.0f * 16.0f / 255.0f) + UV.a * (16.0f / 255.0f);

            float3 rgb = mul(yuv2rgb, float3(y, u - 0.5f, v - 0.5f));
            fixed4 col = fixed4(rgb.r, rgb.g, rgb.b, 1.0f);
            clip(i.clip);
            return col;
        }
        ENDCG
    }
  }
}
