Shader "Unlit/Depth" {
SubShader { 
 LOD 100
 Tags { "QUEUE"="Geometry+1" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Geometry+1" "RenderType"="Opaque" }
  ColorMask 0
  GpuProgramID 27799
Program "vp" {
SubProgram "gles " {
"!!GLES
#version 100

#ifdef VERTEX
attribute vec4 _glesVertex;
attribute vec4 _glesColor;
uniform highp mat4 glstate_matrix_mvp;
varying lowp vec4 xlv_COLOR0;
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
  gl_Position = (glstate_matrix_mvp * tmpvar_3);
}


#endif
#ifdef FRAGMENT
varying lowp vec4 xlv_COLOR0;
void main ()
{
  gl_FragData[0] = xlv_COLOR0;
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