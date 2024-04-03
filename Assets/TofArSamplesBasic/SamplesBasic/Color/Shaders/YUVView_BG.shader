/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "TofAr/Color/YUVShader_BG"
{
	Properties
	{
		_YTex ("Y Texture", 2D) = "Black" {}
		_UVTex ("UV Texture", 2D) = "Black" {}
		//_paddingCutOff ("Texture padding cutoff", float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Geometry-2"}
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

			sampler2D _YTex;
			float4 _YTex_ST;
			sampler2D _UVTex;
			float _YUVShader_paddingCutOff;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				v.uv.y *= -1.0f;
				v.uv.x *= _YUVShader_paddingCutOff;

				o.uv = TRANSFORM_TEX(v.uv, _YTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
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

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
