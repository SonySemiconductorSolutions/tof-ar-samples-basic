/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "TofAr/Tof/TransparentConfidenceViewShader_BG"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Alpha ("Alpha", float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Geometry-1" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Alpha;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = float2(i.uv.x, 1 - i.uv.y);
				fixed4 col = tex2D(_MainTex, uv);
				col *= 16;

				float u16 = ((uint)col.r * (uint)pow(2, 3 * 4)) + ((uint)col.g * (uint)pow(2, 2 * 4)) + ((uint)col.b * (uint)pow(2, 1 * 4)) + col.a;

				float grey = u16 / 7.0f;
				col = fixed4(grey, grey, grey, _Alpha);
				return col;
			}
			ENDCG
		}
	}
}
