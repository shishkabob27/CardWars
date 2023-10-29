Shader "Transparent/VertexLit with Z" 
{
	Properties {
	    _Color ("Main Color", Color) = (1,1,1,1)
	    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	 
	SubShader {
	    Tags { "Queue"="Transparent"}

	    // Render normally
	    Pass {
	        ZWrite on
	        Blend SrcAlpha OneMinusSrcAlpha
	        ColorMask RGB
	        Material {
	            Diffuse [_Color]
	            Ambient [_Color]
	        }
	        Lighting Off
	        SetTexture [_MainTex] {
	            Combine texture * primary
	        } 
	    }
	}
}