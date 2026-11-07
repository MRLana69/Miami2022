Shader "Bend/Flag" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color (RGBA)", Vector) = (1,1,1,1)
		_AmplitudeX ("Amplitude X", Float) = 0.05
		_AmplitudeY ("Amplitude Y", Float) = 0.05
		_FrequenceyX ("Frequencey X", Float) = 9.5
		_FrequenceyY ("Frequencey Y", Float) = 6.5
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
uniform 	float _AmplitudeX;
uniform 	float _AmplitudeY;
uniform 	float _FrequenceyX;
uniform 	float _FrequenceyY;
in highp vec4 in_POSITION0;
in mediump vec4 in_TEXCOORD0;
in mediump vec4 in_TEXCOORD1;
out mediump vec2 vs_TEXCOORD0;
out mediump float vs_TEXCOORD2;
out mediump vec4 vs_TEXCOORD1;
vec4 u_xlat0;
vec4 u_xlat1;
vec4 u_xlat2;
mediump vec2 u_xlat16_2;
float u_xlat3;
float u_xlat4;
float u_xlat5;
vec2 u_xlat7;
float u_xlat9;
float u_xlat11;
void main()
{
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[0].xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[0].zzzz + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[0].wwww + u_xlat0;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[1].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[1].xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[1].zzzz + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[1].wwww + u_xlat1;
    u_xlat16_2.xy = in_TEXCOORD1.yy * vec2(0.699999988, 0.100000001);
    u_xlat16_2.xy = in_POSITION0.zz * vec2(0.00999999978, 0.00999999978) + u_xlat16_2.xy;
    u_xlat3 = _Time.y * _FrequenceyY + u_xlat16_2.y;
    u_xlat3 = sin(u_xlat3);
    u_xlat7.xy = in_TEXCOORD1.yy * vec2(_AmplitudeX, _AmplitudeY);
    u_xlat3 = u_xlat3 * u_xlat7.y + in_POSITION0.y;
    u_xlat1 = u_xlat1 * vec4(u_xlat3);
    u_xlat3 = u_xlat3 * hlslcc_mtx4x4unity_ObjectToWorld[1].z;
    u_xlat11 = in_TEXCOORD1.x * 9.99999975e-005 + _FrequenceyX;
    u_xlat11 = (-_Time.y) * u_xlat11 + u_xlat16_2.x;
    u_xlat11 = sin(u_xlat11);
    u_xlat7.x = u_xlat11 * u_xlat7.x + in_POSITION0.x;
    u_xlat0 = u_xlat0 * u_xlat7.xxxx + u_xlat1;
    u_xlat1.x = hlslcc_mtx4x4unity_ObjectToWorld[0].z * u_xlat7.x + u_xlat3;
    u_xlat2 = hlslcc_mtx4x4unity_ObjectToWorld[2].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[2].xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[2].zzzz + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[2].wwww + u_xlat2;
    u_xlat5 = in_POSITION0.z;
    u_xlat0 = u_xlat2 * vec4(u_xlat5) + u_xlat0;
    u_xlat1.x = hlslcc_mtx4x4unity_ObjectToWorld[2].z * u_xlat5 + u_xlat1.x;
    u_xlat1.x = hlslcc_mtx4x4unity_ObjectToWorld[3].z * in_POSITION0.w + u_xlat1.x;
    u_xlat1.x = u_xlat1.x + (-_WorldSpaceCameraPos.z);
    u_xlat2 = hlslcc_mtx4x4unity_ObjectToWorld[3].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[3].xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[3].zzzz + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[3].wwww + u_xlat2;
    u_xlat0 = u_xlat0 + u_xlat2;
    u_xlat5 = u_xlat0.w * u_xlat0.w;
    u_xlat9 = u_xlat5 * _Distort.y;
    gl_Position.x = u_xlat5 * _Distort.x + u_xlat0.x;
    gl_Position.y = u_xlat9 * _ProjectionParams.x + u_xlat0.y;
    gl_Position.zw = u_xlat0.zw;
    u_xlat0.x = (-u_xlat1.x) + _BackgroundStartDistance;
    u_xlat4 = (-u_xlat1.x) + _FogEndDistance;
    u_xlat4 = u_xlat4 * _FogTransitionLengthReciprocal;
#ifdef UNITY_ADRENO_ES3
    u_xlat4 = min(max(u_xlat4, 0.0), 1.0);
#else
    u_xlat4 = clamp(u_xlat4, 0.0, 1.0);
#endif
    vs_TEXCOORD1.w = u_xlat4;
    u_xlat0.x = u_xlat0.x * _BackgroundToFogTransitionLengthReciprocal;
    u_xlat0.x = u_xlat0.x * 12.0 + -6.0;
    u_xlat0.x = u_xlat0.x * 1.44269502;
    u_xlat0.x = exp2(u_xlat0.x);
    u_xlat0.x = u_xlat0.x + 1.0;
    u_xlat0.x = float(1.0) / u_xlat0.x;
    u_xlat0.x = (-u_xlat0.x) + 1.0;
    vs_TEXCOORD2 = u_xlat0.x;
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    vs_TEXCOORD1.xyz = _FogColor.xyz;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp int;
uniform 	mediump vec4 _Color;
uniform lowp sampler2D _MainTex;
in mediump vec2 vs_TEXCOORD0;
in mediump float vs_TEXCOORD2;
in mediump vec4 vs_TEXCOORD1;
layout(location = 0) out mediump vec4 SV_Target0;
lowp vec3 u_xlat10_0;
mediump vec3 u_xlat16_1;
void main()
{
    u_xlat10_0.xyz = texture(_MainTex, vs_TEXCOORD0.xy).xyz;
    u_xlat16_1.xyz = u_xlat10_0.xyz * _Color.xyz + (-vs_TEXCOORD1.xyz);
    SV_Target0.xyz = vs_TEXCOORD1.www * u_xlat16_1.xyz + vs_TEXCOORD1.xyz;
    SV_Target0.w = vs_TEXCOORD2;
    return;
}

#endif
			ENDGLSL
		}
	}
}
