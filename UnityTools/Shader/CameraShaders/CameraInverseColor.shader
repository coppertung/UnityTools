Shader "UnityTools/Camera/Inverse Color" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Strength ("Strength", Range(0, 1.0)) = 0
	}
	SubShader {
		Pass {
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Strength;

			float4 frag(v2f_img i) : COLOR
			{
				float4 c = tex2D(_MainTex, i.uv);
				float3 bw = float3(1 - c.r, 1 - c.g, 1 - c.b);
				float4 result = c;
				result.rgb = lerp(c.rgb, bw, _Strength);
				return result;
			}

			ENDCG
		}
	}
}
