Shader "UnityTools/2D Blend Mode/Darken"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1, 1, 1, 1)
		_Strength("Strength", Range(0, 1.0)) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		// Get the image behind self
		GrabPass {}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
			};

			float Darken(float base, float top)
			{
				float output = min(base, top);
				return output;
			}

			fixed4 _Color;
			float _Strength;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = o.vertex;
				o.texcoord = v.texcoord;
				o.color = v.color * _Color * _Strength;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _GrabTexture;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 top = tex2D(_MainTex, i.texcoord) * i.color;

				// get the texcoord of the image behind
				float2 grabTexcoord = i.screenPos.xy / i.screenPos.w;
				grabTexcoord.x = (grabTexcoord.x + 1.0) / 2.0;
				grabTexcoord.y = (grabTexcoord.y + 1.0) / 2.0;
				#if UNITY_UV_STARTS_AT_TOP
					grabTexcoord.y = 1.0 - grabTexcoord.y;
				#endif
				fixed4 base = tex2D(_GrabTexture, grabTexcoord);

				fixed4 col = fixed4(0, 0, 0, 0);
				col.r = Darken(base.r, top.r);
				col.g = Darken(base.g, top.g);
				col.b = Darken(base.b, top.b);
				col.a = top.a;

				return col;
			}
			ENDCG
		}
	}
}
