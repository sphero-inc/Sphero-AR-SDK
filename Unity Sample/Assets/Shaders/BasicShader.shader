Shader "Custom/BasicShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Base Color", Color) = (1,1,1,1)
	}
	SubShader {
		Pass {
			Color[_Color]
			SetTexture [_MainTex] {
				constantColor [_Color]
				Combine texture * constant
			}
		}
	}
}
