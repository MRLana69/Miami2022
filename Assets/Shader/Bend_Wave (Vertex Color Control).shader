Shader "Bend/Wave (Vertex Color Control)" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color (RGBA)", Vector) = (1,1,1,1)
		_Frequency ("Frequency", Float) = 1
		_Speed ("Speed", Float) = 22
		_SpeedScales ("Speed Scales (XYZ)", Vector) = (1,1,1,0)
		_OffsetScale ("Offset Scale", Float) = 1
		_WaveDirection ("Wave Direction (XYZ)", Vector) = (1,0,1,0)
		_WavePlaneNormal ("Wave Plane Normal (XYZ)", Vector) = (0,1,0,0)
		_WaveHeight ("Wave Height", Float) = 1
		_VertexColorWeight ("Vertex Color Weight", Float) = 1
	}
	SubShader {
		
		LOD 0
		Pass {
			Blend [_BackgroundBlendSrcMode] [_BackgroundBlendDstMode]
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
uniform 	vec4 _MainTex_ST;
uniform 	float _Speed;
uniform 	vec4 _SpeedScales;
uniform 	float _Frequency;
uniform 	float _OffsetScale;
uniform 	vec3 _WaveDirection;
uniform 	vec3 _WavePlaneNormal;
uniform 	float _WaveHeight;
uniform 	float _VertexColorWeight;
in highp vec4 in_POSITION0;
in highp vec2 in_TEXCOORD0;
in mediump vec4 in_COLOR0;
out highp vec2 vs_TEXCOORD0;
out mediump float vs_TEXCOORD3;
out mediump vec4 vs_TEXCOORD1;
out highp vec4 vs_TEXCOORD2;
vec4 u_xlat0;
vec4 u_xlat1;
vec4 u_xlat2;
float u_xlat3;
float u_xlat4;
float u_xlat7;
void main()
{
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[1].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[1].xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[1].zzzz + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[1].wwww + u_xlat0;
    u_xlat1.xyz = in_POSITION0.yyy * hlslcc_mtx4x4unity_ObjectToWorld[1].xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_ObjectToWorld[0].xyz * in_POSITION0.xxx + u_xlat1.xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_ObjectToWorld[2].xyz * in_POSITION0.zzz + u_xlat1.xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
    u_xlat1.x = dot(u_xlat1.xyz, (-_WaveDirection.xyz));
    u_xlat4 = u_xlat1.z + (-_WorldSpaceCameraPos.z);
    u_xlat7 = in_COLOR0.w * _OffsetScale;
    u_xlat1.x = u_xlat1.x * _Frequency + u_xlat7;
    u_xlat7 = _Time.x * _Speed;
    u_xlat7 = u_xlat7 * 10.0;
    u_xlat1.xzw = vec3(u_xlat7) * _SpeedScales.xyz + u_xlat1.xxx;
    u_xlat1.xzw = sin(u_xlat1.xzw);
    u_xlat2.xyz = vec3(vec3(_WaveHeight, _WaveHeight, _WaveHeight)) * _WavePlaneNormal.xyz;
    u_xlat1.xzw = u_xlat1.xzw * u_xlat2.xyz;
    u_xlat2.x = in_COLOR0.x * _VertexColorWeight;
    u_xlat1.xzw = u_xlat1.xzw * u_xlat2.xxx + in_POSITION0.xyz;
    u_xlat0 = u_xlat0 * u_xlat1.zzzz;
    u_xlat2 = hlslcc_mtx4x4unity_ObjectToWorld[0].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[0].xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[0].zzzz + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[0].wwww + u_xlat2;
    u_xlat0 = u_xlat2 * u_xlat1.xxxx + u_xlat0;
    u_xlat2 = hlslcc_mtx4x4unity_ObjectToWorld[2].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[2].xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[2].zzzz + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[2].wwww + u_xlat2;
    u_xlat0 = u_xlat2 * u_xlat1.wwww + u_xlat0;
    u_xlat2 = hlslcc_mtx4x4unity_ObjectToWorld[3].yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[0] * hlslcc_mtx4x4unity_ObjectToWorld[3].xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[2] * hlslcc_mtx4x4unity_ObjectToWorld[3].zzzz + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_MatrixVP[3] * hlslcc_mtx4x4unity_ObjectToWorld[3].wwww + u_xlat2;
    u_xlat0 = u_xlat0 + u_xlat2;
    u_xlat1.x = u_xlat0.w * u_xlat0.w;
    u_xlat7 = u_xlat1.x * _Distort.y;
    gl_Position.x = u_xlat1.x * _Distort.x + u_xlat0.x;
    gl_Position.y = u_xlat7 * _ProjectionParams.x + u_xlat0.y;
    gl_Position.zw = u_xlat0.zw;
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    u_xlat0.x = (-u_xlat4) + _FogEndDistance;
    u_xlat3 = (-u_xlat4) + _BackgroundStartDistance;
    u_xlat3 = u_xlat3 * _BackgroundToFogTransitionLengthReciprocal;
    u_xlat3 = u_xlat3 * 12.0 + -6.0;
    u_xlat3 = u_xlat3 * 1.44269502;
    u_xlat3 = exp2(u_xlat3);
    u_xlat3 = u_xlat3 + 1.0;
    u_xlat3 = float(1.0) / u_xlat3;
    u_xlat3 = (-u_xlat3) + 1.0;
    vs_TEXCOORD3 = u_xlat3;
    u_xlat0.x = u_xlat0.x * _FogTransitionLengthReciprocal;
#ifdef UNITY_ADRENO_ES3
    u_xlat0.x = min(max(u_xlat0.x, 0.0), 1.0);
#else
    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
#endif
    vs_TEXCOORD1.w = u_xlat0.x;
    vs_TEXCOORD1.xyz = _FogColor.xyz;
    vs_TEXCOORD2 = in_POSITION0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp int;
uniform 	mediump vec4 _Color;
uniform lowp sampler2D _MainTex;
in highp vec2 vs_TEXCOORD0;
in mediump float vs_TEXCOORD3;
in mediump vec4 vs_TEXCOORD1;
layout(location = 0) out mediump vec4 SV_Target0;
lowp vec3 u_xlat10_0;
mediump vec3 u_xlat16_1;
void main()
{
    u_xlat10_0.xyz = texture(_MainTex, vs_TEXCOORD0.xy).xyz;
    u_xlat16_1.xyz = u_xlat10_0.xyz * _Color.xyz + (-vs_TEXCOORD1.xyz);
    SV_Target0.xyz = vs_TEXCOORD1.www * u_xlat16_1.xyz + vs_TEXCOORD1.xyz;
    SV_Target0.w = vs_TEXCOORD3;
    return;
}

#endif
			ENDGLSL
		}
	}
}
