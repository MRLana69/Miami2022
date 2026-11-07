Shader "Custom/Distorted/Projector Multiply" {
	Properties {
		_ShadowTex ("Cookie", 2D) = "gray" {}
		_Falloff ("Falloff distance", Float) = 0.01
	}
	SubShader {
		Tags { "RenderType"="Transparent-1" }
		LOD 0
		Pass {
			Tags { "RenderType"="Transparent-1" }
			Blend DstColor Zero
			ColorMask RGB
			ZTest LEqual
			ZWrite Off
			Cull Back
			Offset -1, -1
			GLSLPROGRAM
#ifdef VERTEX
#version 300 es

uniform 	vec4 _ProjectionParams;
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	vec2 _Distort;
uniform 	mediump float _Falloff;
uniform 	mediump vec4 hlslcc_mtx4x4unity_Projector[4];
in highp vec4 in_POSITION0;
out mediump vec4 vs_TEXCOORD0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
mediump vec4 u_xlat16_0;
vec4 u_xlat1;
mediump vec4 u_xlat16_1;
float u_xlat2;
bool u_xlatb2;
mediump float u_xlat16_3;
float u_xlat5;
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
    u_xlat5 = u_xlat1.x * _Distort.y;
    gl_Position.x = u_xlat1.x * _Distort.x + u_xlat0.x;
    gl_Position.y = u_xlat5 * _ProjectionParams.x + u_xlat0.y;
    gl_Position.zw = u_xlat0.zw;
    u_xlat16_0.x = hlslcc_mtx4x4unity_Projector[0].x;
    u_xlat16_0.y = hlslcc_mtx4x4unity_Projector[1].x;
    u_xlat16_0.z = hlslcc_mtx4x4unity_Projector[2].x;
    u_xlat16_0.w = hlslcc_mtx4x4unity_Projector[3].x;
    u_xlat0.x = dot(u_xlat16_0, in_POSITION0);
    u_xlat16_1.x = hlslcc_mtx4x4unity_Projector[0].y;
    u_xlat16_1.y = hlslcc_mtx4x4unity_Projector[1].y;
    u_xlat16_1.z = hlslcc_mtx4x4unity_Projector[2].y;
    u_xlat16_1.w = hlslcc_mtx4x4unity_Projector[3].y;
    u_xlat0.y = dot(u_xlat16_1, in_POSITION0);
    u_xlat16_1.x = hlslcc_mtx4x4unity_Projector[0].w;
    u_xlat16_1.y = hlslcc_mtx4x4unity_Projector[1].w;
    u_xlat16_1.z = hlslcc_mtx4x4unity_Projector[2].w;
    u_xlat16_1.w = hlslcc_mtx4x4unity_Projector[3].w;
    u_xlat0.w = dot(u_xlat16_1, in_POSITION0);
    u_xlat16_1.x = hlslcc_mtx4x4unity_Projector[0].z;
    u_xlat16_1.y = hlslcc_mtx4x4unity_Projector[1].z;
    u_xlat16_1.z = hlslcc_mtx4x4unity_Projector[2].z;
    u_xlat16_1.w = hlslcc_mtx4x4unity_Projector[3].z;
    u_xlat0.z = dot(u_xlat16_1, in_POSITION0);
    vs_TEXCOORD0 = u_xlat0;
#ifdef UNITY_ADRENO_ES3
    u_xlatb2 = !!(u_xlat0.z<0.0);
#else
    u_xlatb2 = u_xlat0.z<0.0;
#endif
    u_xlat16_3 = u_xlat0.z * _Falloff;
#ifdef UNITY_ADRENO_ES3
    u_xlat16_3 = min(max(u_xlat16_3, 0.0), 1.0);
#else
    u_xlat16_3 = clamp(u_xlat16_3, 0.0, 1.0);
#endif
    u_xlat2 = u_xlatb2 ? 1.0 : float(0.0);
    u_xlat0 = vec4(u_xlat16_3) + vec4(u_xlat2);
    vs_COLOR0 = u_xlat0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp int;
uniform lowp sampler2D _ShadowTex;
in mediump vec4 vs_TEXCOORD0;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
mediump vec2 u_xlat16_0;
lowp vec4 u_xlat10_0;
mediump vec4 u_xlat16_1;
void main()
{
    u_xlat16_0.xy = vs_TEXCOORD0.xy / vs_TEXCOORD0.ww;
    u_xlat10_0 = texture(_ShadowTex, u_xlat16_0.xy);
    u_xlat16_1 = (-u_xlat10_0) + vec4(1.0, 1.0, 1.0, 1.0);
    SV_Target0 = vs_COLOR0.xxxx * u_xlat16_1 + u_xlat10_0;
    return;
}

#endif
			ENDGLSL
		}
	}
}
