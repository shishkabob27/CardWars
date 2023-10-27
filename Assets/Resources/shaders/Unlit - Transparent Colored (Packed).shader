Shader "Unlit/Transparent Colored (Packed)" {
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
  GpuProgramID 57496
Program "vp" {
SubProgram "gles " {
"!!GLES
#version 100

#ifdef VERTEX
attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
varying mediump vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = _glesColor;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying mediump vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  mediump vec4 col_1;
  mediump vec4 mask_2;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0);
  mask_2 = tmpvar_3;
  mediump vec4 tmpvar_4;
  tmpvar_4 = clamp (ceil((xlv_COLOR - 0.5)), 0.0, 1.0);
  mediump vec4 tmpvar_5;
  tmpvar_5 = clamp (((
    (tmpvar_4 * 0.51)
   - xlv_COLOR) / -0.49), 0.0, 1.0);
  col_1.xyz = tmpvar_5.xyz;
  mask_2 = (mask_2 * tmpvar_4);
  col_1.w = (tmpvar_5.w * ((mask_2.x + mask_2.y) + (mask_2.z + mask_2.w)));
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