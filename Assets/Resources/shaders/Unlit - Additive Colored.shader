Shader "Unlit/Additive Colored" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" { }
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend One One
  ColorMask RGB
  GpuProgramID 48749
Program "vp" {
SubProgram "gles " {
"!!GLES
#version 100

#ifdef VERTEX
attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _MainTex_ST;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 tmpvar_2;
  tmpvar_2 = clamp (_glesColor, 0.0, 1.0);
  tmpvar_1 = tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = _glesVertex.xyz;
  xlv_COLOR0 = tmpvar_1;
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  gl_Position = (glstate_matrix_mvp * tmpvar_3);
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying lowp vec4 xlv_COLOR0;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 col_1;
  col_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
  if ((col_1.w <= 0.01)) {
    discard;
  };
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
}