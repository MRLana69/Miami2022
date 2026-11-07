Shader "Custom/Distorted/Unlit with overlay and shine" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color Tint", Vector) = (0,0,0,0)
		_ShineTex ("Shine (RGB), Alpha (A)", 2D) = "black" {}
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

uniform 	vec4 _Time;
uniform 	vec4 _ProjectionParams;
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	vec2 _Distort;
uniform 	mediump vec4 _MainTex_ST;
in highp vec4 in_POSITION0;
in mediump vec2 in_TEXCOORD0;
out mediump vec4 vs_COLOR0;
out mediump vec2 vs_TEXCOORD0;
out highp vec2 vs_TEXCOORD1;
vec4 u_xlat0;
vec4 u_xlat1;
vec2 u_xlat2;
bool u_xlatb3;
float u_xlat4;
vec2 u_xlat6;
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
    u_xlat4 = u_xlat1.x * _Distort.y;
    u_xlat2.x = u_xlat1.x * _Distort.x + u_xlat0.x;
    u_xlat2.y = u_xlat4 * _ProjectionParams.x + u_xlat0.y;
    gl_Position.xy = u_xlat2.xy;
    u_xlat1.xy = u_xlat2.xy / u_xlat0.ww;
    gl_Position.zw = u_xlat0.zw;
    vs_COLOR0 = vec4(0.0, 0.0, 0.0, 0.0);
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    u_xlat0.x = _Time.y * 0.13333334;
#ifdef UNITY_ADRENO_ES3
    u_xlatb3 = !!(u_xlat0.x>=(-u_xlat0.x));
#else
    u_xlatb3 = u_xlat0.x>=(-u_xlat0.x);
#endif
    u_xlat0.x = fract(abs(u_xlat0.x));
    u_xlat0.x = (u_xlatb3) ? u_xlat0.x : (-u_xlat0.x);
    u_xlat0.x = u_xlat0.x * 15.0 + -7.0;
    u_xlat1.z = u_xlat0.x + u_xlat1.y;
    u_xlat0.xy = u_xlat1.xz + vec2(-0.5, -0.5);
    u_xlat6.x = dot(vec2(0.866025627, 0.499999613), u_xlat0.xy);
    u_xlat6.y = dot(vec2(-0.499999613, 0.866025627), u_xlat0.xy);
    vs_TEXCOORD1.xy = u_xlat6.xy + vec2(0.5, 0.5);
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp int;
uniform 	mediump vec4 _Color;
uniform lowp sampler2D _MainTex;
uniform lowp sampler2D _ShineTex;
in mediump vec2 vs_TEXCOORD0;
in highp vec2 vs_TEXCOORD1;
layout(location = 0) out mediump vec4 SV_Target0;
mediump vec4 u_xlat16_0;
lowp vec4 u_xlat10_0;
mediump float u_xlat16_1;
lowp vec4 u_xlat10_1;
void main()
{
    u_xlat10_0 = texture(_ShineTex, vs_TEXCOORD1.xy);
    u_xlat16_1 = u_xlat10_0.z + u_xlat10_0.x;
    u_xlat16_1 = u_xlat10_0.y + u_xlat16_1;
    u_xlat16_1 = u_xlat16_1 * 0.330000013;
    u_xlat16_0 = u_xlat10_0 * vec4(u_xlat16_1);
    u_xlat10_1 = texture(_MainTex, vs_TEXCOORD0.xy);
    SV_Target0 = u_xlat10_1 * _Color + u_xlat16_0;
    return;
}

#endif
			ENDGLSL
		}
	}
}
