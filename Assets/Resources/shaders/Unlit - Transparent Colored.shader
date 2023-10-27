Shader "Unlit/Transparent Colored" {
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
  Blend SrcAlpha OneMinusSrcAlpha
  Offset -1, -1
  GpuProgramID 45292
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
varying mediump vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;
void main ()
{
  mediump vec2 tmpvar_1;
  tmpvar_1 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_COLOR = _glesColor;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying mediump vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_COLOR;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
  gl_FragData[0] = tmpvar_1;
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