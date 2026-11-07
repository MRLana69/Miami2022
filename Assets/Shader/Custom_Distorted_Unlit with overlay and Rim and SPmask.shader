Shader "Custom/Distorted/Unlit with overlay and Rim and SPmask" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask ("Mask (RGB)", 2D) = "white" {}
		_Color ("Color Tint", Vector) = (0,0,0,0)
		_RimColor ("Reflection Color", Vector) = (1,1,1,0.5)
	}
	SubShader {
		
		LOD 0
		Pass {
			ZTest LEqual
			ZWrite On
			Cull Back
			GLSLPROGRAM
#ifdef VERTEX
#version 300 es

uniform 	vec3 _WorldSpaceCameraPos;
uniform 	vec4 _ProjectionParams;
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	mediump vec4 _FogColor;
uniform 	mediump float _FogEndDistance;
uniform 	mediump float _FogTransitionLengthReciprocal;
uniform 	mediump float _BackgroundStartDistance;
uniform 	mediump float _BackgroundToFogTransitionLengthReciprocal;
uniform 	vec2 _Distort;
uniform 	mediump vec4 _MainTex_ST;
in highp vec4 in_POSITION0;
in mediump vec2 in_TEXCOORD0;
out mediump vec4 vs_TEXCOORD0;
out mediump float vs_TEXCOORD1;
out mediump vec2 vs_TEXCOORD2;
out highp vec4 vs_TEXCOORD3;
vec4 u_xlat0;
vec4 u_xlat1;
float u_xlat3;
void main()
{
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[1].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[1].xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[1].zzzz + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[1].wwww + u_xlat0;
    u_xlat0 = u_xlat0 * in_POSITION0.yyyy;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[0].xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[0].zzzz + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[0].wwww + u_xlat1;
    u_xlat0 = u_xlat1 * in_POSITION0.xxxx + u_xlat0;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[2].xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[2].zzzz + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[2].wwww + u_xlat1;
    u_xlat0 = u_xlat1 * in_POSITION0.zzzz + u_xlat0;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[3].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[3].xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[3].zzzz + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[3].wwww + u_xlat1;
    u_xlat0 = u_xlat0 + u_xlat1;
    u_xlat1.x = u_xlat0.w * u_xlat0.w;
    u_xlat3 = u_xlat1.x * _Distort.y;
    u_xlat0.x = u_xlat1.x * _Distort.x + u_xlat0.x;
    u_xlat0.y = u_xlat3 * _ProjectionParams.x + u_xlat0.y;
    gl_Position = u_xlat0;
    u_xlat1.x = in_POSITION0.y * hlslcc_mtx4x4unity_ObjectToWorld[1].z;
    u_xlat1.x = hlslcc_mtx4x4unity_ObjectToWorld[0].z * in_POSITION0.x + u_xlat1.x;
    u_xlat1.x = hlslcc_mtx4x4unity_ObjectToWorld[2].z * in_POSITION0.z + u_xlat1.x;
    u_xlat1.x = hlslcc_mtx4x4unity_ObjectToWorld[3].z * in_POSITION0.w + u_xlat1.x;
    u_xlat1.x = u_xlat1.x + (-_WorldSpaceCameraPos.z);
    u_xlat3 = (-u_xlat1.x) + _FogEndDistance;
    u_xlat1.x = (-u_xlat1.x) + _BackgroundStartDistance;
    u_xlat1.x = u_xlat1.x * _BackgroundToFogTransitionLengthReciprocal;
    u_xlat1.x = u_xlat1.x * 12.0 + -6.0;
    u_xlat1.x = u_xlat1.x * 1.44269502;
    u_xlat1.x = exp2(u_xlat1.x);
    u_xlat1.x = u_xlat1.x + 1.0;
    u_xlat1.x = float(1.0) / u_xlat1.x;
    u_xlat1.x = (-u_xlat1.x) + 1.0;
    vs_TEXCOORD1 = u_xlat1.x;
    u_xlat1.x = u_xlat3 * _FogTransitionLengthReciprocal;
#ifdef UNITY_ADRENO_ES3
    u_xlat1.x = min(max(u_xlat1.x, 0.0), 1.0);
#else
    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
#endif
    vs_TEXCOORD0.w = u_xlat1.x;
    vs_TEXCOORD0.xyz = _FogColor.xyz;
    vs_TEXCOORD2.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    u_xlat1.x = float(0.5);
    u_xlat1.z = float(0.5);
    u_xlat1.y = _ProjectionParams.x;
    u_xlat1.xyz = u_xlat0.xyw * u_xlat1.xyz;
    vs_TEXCOORD3.zw = u_xlat0.zw;
    u_xlat1.w = u_xlat1.y * 0.5;
    vs_TEXCOORD3.xy = u_xlat1.zz + u_xlat1.xw;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp int;
uniform 	mediump vec4 _Color;
uniform lowp sampler2D _Mask;
uniform lowp sampler2D _MainTex;
in mediump vec4 vs_TEXCOORD0;
in mediump float vs_TEXCOORD1;
in mediump vec2 vs_TEXCOORD2;
in highp vec4 vs_TEXCOORD3;
layout(location = 0) out mediump vec4 SV_Target0;
vec2 u_xlat0;
lowp vec3 u_xlat10_0;
lowp vec3 u_xlat10_1;
mediump vec3 u_xlat16_2;
void main()
{
    u_xlat0.xy = vs_TEXCOORD3.xy / vs_TEXCOORD3.ww;
    u_xlat10_0.xyz = texture(_Mask, u_xlat0.xy).xyz;
    u_xlat10_1.xyz = texture(_MainTex, vs_TEXCOORD2.xy).xyz;
    u_xlat16_2.xyz = u_xlat10_1.xyz * _Color.xyz + u_xlat10_0.xyz;
    u_xlat16_2.xyz = u_xlat16_2.xyz + (-vs_TEXCOORD0.xyz);
    SV_Target0.xyz = vs_TEXCOORD0.www * u_xlat16_2.xyz + vs_TEXCOORD0.xyz;
    SV_Target0.w = vs_TEXCOORD1;
    return;
}

#endif
			ENDGLSL
		}
	}
}
