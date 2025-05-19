/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "TofAr/ColoredPointCloudMesh_RGB"
{
  Properties
  {
    _MainTex("Texture", 2D) = "White" {}
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

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float _PointSize;
        int _isLinearColorSpace;

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
            o.uv =  TRANSFORM_TEX(v.uv, _MainTex);
            o.psize = _PointSize * 1000;
            return o;
        }

        fixed4 frag(v2f i) : COLOR
        {
            clip(i.clip);
            return tex2D(_MainTex, i.uv);
        }
        ENDCG
    }


  }
}
