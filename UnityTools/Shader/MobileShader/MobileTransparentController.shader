Shader "UnityTools/Mobile/Transparent Controller" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="Ture" "RenderType"="Transparent" }
		LOD 200
		Cull Off Lighting Off ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}

		SubShader {
			Pass {
				SetTexture[_MainTex] {
					ConstantColor [_Color]
					Combine Texture * constant
				}
			}
		}
	}
}
