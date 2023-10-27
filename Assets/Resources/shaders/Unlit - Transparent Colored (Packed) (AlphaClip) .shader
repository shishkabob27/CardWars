Shader "Unlit/Transparent Colored (Packed) (AlphaClip)" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" { }
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  Offset -1, -1
  GpuProgramID 4834
Program "vp" {
SubProgram "gles " {
"!!GLES
#version 100

#ifdef VERTEX
attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform mediump vec4 _MainTex_ST;
varying mediump vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = _glesColor;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = ((_glesVertex.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying mediump vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
void main ()
{
  mediump vec4 col_1;
  mediump vec4 mask_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0);
  mask_2 = tmpvar_3;
  mediump vec4 tmpvar_4;
  tmpvar_4 = clamp (ceil((xlv_COLOR - 0.5)), 0.0, 1.0);
  col_1 = clamp (((
    (tmpvar_4 * 0.51)
   - xlv_COLOR) / -0.49), 0.0, 1.0);
  highp vec2 tmpvar_5;
  tmpvar_5 = abs(xlv_TEXCOORD1);
  highp float tmpvar_6;
  tmpvar_6 = (1.0 - max (tmpvar_5.x, tmpvar_5.y));
  if ((tmpvar_6 < 0.0)) {
    col_1.w = 0.0;
  };
  mask_2 = (mask_2 * tmpvar_4);
  col_1.w = (col_1.w * ((mask_2.x + mask_2.y) + (mask_2.z + mask_2.w)));
  gl_FragData[0] = col_1;
}


#endif
"
}
}
Program "fp" {
SubProgram "gles " {
"!!GLES"
}
}
 }
}
Fallback Off
}