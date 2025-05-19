/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

Shader "TofAr/Tof/TransDepthViewShader_BG"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DistanceMultiplier("Distance Multiplier", float) = 1
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
			float _DistanceMultiplier;
			int _isLinearColorSpace;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float _Alpha;

			float4 applyColorMap(float d, int idx)
			{
				static float4 colormapM[6] = { float4(0,1,0,0),float4(0,0,-1,0),float4(1,0,0,0),float4(0,-1,0,0),float4(0,0,1,0),float4(0,1,0,0) };
				static float4 colormapA[6] = { float4(0,0,1,_Alpha),float4(0,1,2,_Alpha),float4(-2,1,0,_Alpha),float4(1,4,0,_Alpha),float4(1,0,-4,_Alpha),float4(1,-5,1,_Alpha) };
				return colormapM[idx] * d + colormapA[idx];
			}

			float4 frag (v2f i) : SV_Target
			{
				float2 uv = float2(i.uv.x, 1 - i.uv.y);
				float4 col = tex2D(_MainTex, uv);

				/*
				mapping 
				{
				0    < d < 5/6   ->     0, d*6/5, 1
				5/6  < d < 10/6  ->     0, 1, 1 - (d - 5/6)*6/5
				10/6 < d < 15/6  ->     (d-10/6)*6/5, 1, 0
				15/6 < d < 20/6  ->     1,1 - (d-15/6)*6/5, 0
				20/6 < d < 25/6  ->     1,0,(d-20/6)*6/5
				25/6 < d < 30/6  ->     1,1(d-25/6)*6/5, 1
				}
				which tidies up to 
				{
				0    < d < 5/6   ->     0, d*6/5, 1
				5/6  < d < 10/6  ->     0, 1, 2 - d*6/5
				10/6 < d < 15/6  ->     d*6/5 - 2 , 1, 0
				15/6 < d < 20/6  ->     1, 4-d*6/5, 0
				20/6 < d < 25/6  ->     1,0,d*6/5 - 4
				25/6 < d < 30/6  ->     1,d*6/5 - 5, 1
				}

				this can be re expressed as two arrays M and A where col = M*d*6/5 + A
				these arrays are indexed by d*6/5, which avoids the branching problem
				
				M = [[0,1,0], [0,0,-1], [1,0,0],  [0,-1,0], [0,0,1],  [0,1,0]]
				A = [[0,0,1], [0,1,2],  [-2,1,0], [1,4,0],  [1,0,-4], [1,-5,1]]

				*/
				//convert from 0-1 range to 0-255 range
				col *= pow(2, 4);
				//cast back to the unsigned 16 bit didgit with some bit shifts (our input was 4 bits per channel
				float u16 = ((uint)col.r * (uint)pow(2, 3 * 4)) + (col.g * (uint)pow(2, 2 * 4)) + (col.b * (uint)pow(2, 1 * 4)) + col.a;
				float colScaleD = u16*.0012*_DistanceMultiplier; // /1000 to adjust the scale (6/(5*1000))
				int idx = floor(colScaleD);
				if (idx < 0) return fixed4(0, 0, 1, _Alpha);
				if (idx > 5) return fixed4(1, 1, 1, _Alpha);

				col = applyColorMap(colScaleD, idx);

				col = toGamma(col, _isLinearColorSpace);

				return col;
			}
			ENDCG
		}
	}
}
