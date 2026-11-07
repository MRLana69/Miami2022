Shader "Custom/Distorted/Alpha Blended (with Reflection)" {
	Properties {
		_TintColor ("Tint Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_ReflectColor ("Reflection Color", Vector) = (1,1,1,0.5)
		_Cube ("Reflection Cubemap", Cube) = "" {}
	}
	SubShader {
		Tags { "IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent" }
		LOD 0
		Pass {
			Tags { "IGNOREPROJECTOR"="true" "QUEUE"="Transparent" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			ZTest LEqual
			ZWrite Off
			Cull Off
			GLSLPROGRAM
#ifdef VERTEX
#version 300 es

uniform 	vec3 _WorldSpaceCameraPos;
uniform 	vec4 _ProjectionParams;
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_WorldToObject[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	mediump vec4 _FogColor;
uniform 	mediump float _FogEndDistance;
uniform 	mediump float _FogTransitionLengthReciprocal;
uniform 	mediump float _BackgroundStartDistance;
uniform 	mediump float _BackgroundToFogTransitionLengthReciprocal;
uniform 	vec2 _Distort;
uniform 	vec4 _MainTex_ST;
in highp vec4 in_POSITION0;
in highp vec2 in_TEXCOORD0;
in mediump vec3 in_NORMAL0;
out mediump vec2 vs_TEXCOORD0;
mediump  vec4 phase0_Output0_1;
out mediump float vs_TEXCOORD2;
out mediump vec4 vs_TEXCOORD1;
out mediump vec3 vs_TEXCOORD3;
out mediump vec3 vs_TEXCOORD4;
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
    gl_Position.x = u_xlat1.x * _Distort.x + u_xlat0.x;
    gl_Position.y = u_xlat3 * _ProjectionParams.x + u_xlat0.y;
    gl_Position.zw = u_xlat0.zw;
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1].zxyz;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0].zxyz * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2].zxyz * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[3].zxyz * in_POSITION0.wwww + u_xlat0;
    u_xlat0 = u_xlat0 + (-_WorldSpaceCameraPos.zxyz);
    u_xlat1.x = (-u_xlat0.x) + _BackgroundStartDistance;
    u_xlat1.x = u_xlat1.x * _BackgroundToFogTransitionLengthReciprocal;
    u_xlat1.x = u_xlat1.x * 12.0 + -6.0;
    u_xlat1.x = u_xlat1.x * 1.44269502;
    u_xlat1.x = exp2(u_xlat1.x);
    u_xlat1.x = u_xlat1.x + 1.0;
    u_xlat1.x = float(1.0) / u_xlat1.x;
    u_xlat1.z = (-u_xlat1.x) + 1.0;
    u_xlat1.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    phase0_Output0_1.xyz = u_xlat1.xyz;
    u_xlat0.x = (-u_xlat0.x) + _FogEndDistance;
    vs_TEXCOORD4.xyz = u_xlat0.yzw;
    u_xlat0.x = u_xlat0.x * _FogTransitionLengthReciprocal;
#ifdef UNITY_ADRENO_ES3
    u_xlat0.x = min(max(u_xlat0.x, 0.0), 1.0);
#else
    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
#endif
    vs_TEXCOORD1.w = u_xlat0.x;
    vs_TEXCOORD1.xyz = _FogColor.xyz;
    u_xlat0.x = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[0].xyz);
    u_xlat0.y = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[1].xyz);
    u_xlat0.z = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[2].xyz);
    vs_TEXCOORD3.xyz = u_xlat0.xyz;
vs_TEXCOORD0 = phase0_Output0_1.xy;
vs_TEXCOORD2 = phase0_Output0_1.z;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp int;
uniform 	mediump vec4 _TintColor;
uniform 	mediump vec4 _ReflectColor;
uniform lowp sampler2D _MainTex;
uniform lowp samplerCube _Cube;
in mediump vec2 vs_TEXCOORD0;
in mediump float vs_TEXCOORD2;
in mediump vec4 vs_TEXCOORD1;
in mediump vec3 vs_TEXCOORD3;
in mediump vec3 vs_TEXCOORD4;
layout(location = 0) out mediump vec4 SV_Target0;
mediump vec4 u_xlat16_0;
lowp vec4 u_xlat10_0;
mediump vec3 u_xlat16_1;
lowp vec4 u_xlat10_1;
mediump vec3 u_xlat16_2;
mediump float u_xlat16_9;
void main()
{
    u_xlat16_0.x = dot(vs_TEXCOORD4.xyz, vs_TEXCOORD4.xyz);
    u_xlat16_0.x = inversesqrt(u_xlat16_0.x);
    u_xlat16_0.xyz = u_xlat16_0.xxx * vs_TEXCOORD4.xyz;
    u_xlat16_9 = dot(vs_TEXCOORD3.xyz, vs_TEXCOORD3.xyz);
    u_xlat16_9 = inversesqrt(u_xlat16_9);
    u_xlat16_1.xyz = vec3(u_xlat16_9) * vs_TEXCOORD3.xyz;
    u_xlat16_9 = dot(u_xlat16_0.xyz, u_xlat16_1.xyz);
    u_xlat16_9 = u_xlat16_9 + u_xlat16_9;
    u_xlat16_0.xyz = u_xlat16_1.xyz * (-vec3(u_xlat16_9)) + u_xlat16_0.xyz;
    u_xlat10_0 = texture(_Cube, u_xlat16_0.xyz);
    u_xlat16_0 = u_xlat10_0 * _ReflectColor;
    u_xlat10_1 = texture(_MainTex, vs_TEXCOORD0.xy);
    u_xlat16_2.x = u_xlat10_1.z + u_xlat10_1.x;
    u_xlat16_2.x = u_xlat10_1.y + u_xlat16_2.x;
    u_xlat16_2.x = u_xlat16_2.x * 0.330000013;
    u_xlat16_0 = u_xlat16_0 * u_xlat16_2.xxxx;
    u_xlat16_0 = _TintColor * u_xlat10_1 + u_xlat16_0;
    u_xlat16_2.xyz = u_xlat16_0.xyz + (-vs_TEXCOORD1.xyz);
    SV_Target0.w = u_xlat16_0.w * vs_TEXCOORD2;
    SV_Target0.xyz = vs_TEXCOORD1.www * u_xlat16_2.xyz + vs_TEXCOORD1.xyz;
    return;
}

#endif
			ENDGLSL
		}
	}
}
