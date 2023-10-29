Shader "KFF/Unlit Transparent Textured Vertex Color" 
{
	Properties {
	    _MainTex ("Texture (RGBA)", 2D) = "white" {}
	}
	 
	SubShader {
    LOD 200
	    Tags { "Queue"="Transparent" "RenderType" = "Transparent" }

	    // Render normally
	    Pass {
	        ZWrite Off
          Cull Off
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