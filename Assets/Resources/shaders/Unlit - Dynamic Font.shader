// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Dynamic Font"
{
	Properties
	{
		_MainTex ("Alpha (A)", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		AlphaTest Greater .01
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = i.color;
				col.a *= tex2D(_MainTex, i.texcoord).a;
				return col;
			}
			ENDCG 
		}
	}
	
	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		Lighting Off
		Cull Off
		ZTest Always
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
		BindChannels
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		Pass
		{
			SetTexture [_MainTex]
			{
				constantColor [_Color] combine constant * primary, constant * texture
			}
		}
	}
}