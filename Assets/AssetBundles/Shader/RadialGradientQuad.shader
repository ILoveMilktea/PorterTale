﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//RadialGradientQuad.shader
Shader "Custom/RadialGradientQuad" {
	Properties{
		_ColorA("Color A", Color) = (1, 1, 1, 1)
		_ColorB("Color B", Color) = (0, 0, 0, 0)
		_Slide("Slide", Range(0, 1)) = 0.5
	}

		SubShader{
			Blend SrcAlpha OneMinusSrcAlpha
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane"}
			LOD 100

			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
				};

				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = v.texcoord;
					return o;
				}

				fixed4 _ColorA, _ColorB;
				float _Slide;

				fixed4 frag(v2f i) : SV_Target
				{
					float t = length(i.texcoord - float2(0.5, 0.5)) * 1.41421356237; // 1.141... = sqrt(2)
					float ratio = t + _Slide;

					return ratio < 0.7 ? lerp(_ColorA, _ColorB, 0) : lerp(_ColorA, _ColorB, (ratio - 0.7)*2);
				}
				ENDCG
			}
	}
}