Shader "Bend/Wave (UV Distorted)" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color (RGBA)", Vector) = (1,1,1,1)
		_AmplitudeX ("Amplitude X", Float) = 0.5
		_AmplitudeZ ("Amplitude Z", Float) = 0.1
		_FrequenceyX ("Frequencey X", Float) = 2
		_FrequenceyZ ("Frequencey Z", Float) = 2
		_SpeedX ("Speed X", Float) = 2
		_SpeedZ ("Speed Z", Float) = 2
		_ScrollSpeed ("Scroll speed (xy)", Vector) = (1,1,0,0)
		_xDistortionAplitude ("X Scroll Distortion Aplitude", Float) = 0.01
		_xDistortionFrequency ("X Scroll Distortion Frequency", Float) = 0.01
		_xDistortionSpeed ("X Scroll Distortion Speed", Float) = 0.01
		_xWaveYDistortionAplitude ("X Wave Y Distortion Aplitude", Float) = 0.01
		_xWaveYDistortionFrequency ("X Wave Y Distortion Frequency", Float) = 20
		_xWaveYDistortionSpeed ("X Wave Y Distortion Speed", Float) = 2
		_xWaveYDistortionStart ("X Wave Y Distortion Start", Float) = 0.5
		_xWaveYDistortionPosFactor ("X Wave Y Distortion Pos Factor", Float) = 0.4
		_yDistortionAplitude ("Y Scroll Distortion Aplitude", Float) = 0.4
		_yDistortionFrequency ("Y Scroll Distortion Frequency", Float) = 0.01
		_yDistortionSpeed ("Y Scroll Distortion Speed", Float) = 0.01
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
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	mediump vec4 _FogColor;
uniform 	mediump float _FogEndDistance;
uniform 	mediump float _FogTransitionLengthReciprocal;
uniform 	mediump float _BackgroundStartDistance;
uniform 	mediump float _BackgroundToFogTransitionLengthReciprocal;
uniform 	vec2 _Distort;
uniform 	float _AmplitudeX;
uniform 	float _AmplitudeZ;
uniform 	float _FrequenceyX;
uniform 	float _FrequenceyZ;
uniform 	float _SpeedX;
uniform 	float _SpeedZ;
uniform 	vec4 _ScrollSpeed;
in highp vec4 in_POSITION0;
in mediump vec4 in_TEXCOORD0;
out mediump vec2 vs_TEXCOORD0;
mediump  vec4 phase0_Output0_1;
out mediump float vs_TEXCOORD2;
out mediump vec4 vs_TEXCOORD1;
out mediump vec2 vs_TEXCOORD3;
vec4 u_xlat0;
vec4 u_xlat1;
float u_xlat2;
float u_xlat3;
void main()
{
    u_xlat0.xy = _Time.yy * vec2(_SpeedX, _SpeedZ);
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat1;
    u_xlat0.xy = vec2(_FrequenceyX, _FrequenceyZ) * u_xlat1.xz + u_xlat0.xy;
    u_xlat0.xy = sin(u_xlat0.xy);
    u_xlat0.x = _AmplitudeX * u_xlat0.x + u_xlat1.y;
    u_xlat0.x = _AmplitudeZ * u_xlat0.y + u_xlat0.x;
    u_xlat0 = u_xlat0.xxxx * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
    u_xlat3 = u_xlat0.w * u_xlat0.w;
    gl_Position.xy = vec2(u_xlat3) * _Distort.xy + u_xlat0.xy;
    gl_Position.zw = u_xlat0.zw;
    u_xlat0.x = u_xlat1.z + (-_WorldSpaceCameraPos.z);
    vs_TEXCOORD3.xy = u_xlat1.xz;
    u_xlat2 = (-u_xlat0.x) + _BackgroundStartDistance;
    u_xlat0.x = (-u_xlat0.x) + _FogEndDistance;
    u_xlat0.x = u_xlat0.x * _FogTransitionLengthReciprocal;
#ifdef UNITY_ADRENO_ES3
    u_xlat0.x = min(max(u_xlat0.x, 0.0), 1.0);
#else
    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
#endif
    vs_TEXCOORD1.w = u_xlat0.x;
    u_xlat0.x = u_xlat2 * _BackgroundToFogTransitionLengthReciprocal;
    u_xlat0.x = u_xlat0.x * 12.0 + -6.0;
    u_xlat0.x = u_xlat0.x * 1.44269502;
    u_xlat0.x = exp2(u_xlat0.x);
    u_xlat0.x = u_xlat0.x + 1.0;
    u_xlat0.x = float(1.0) / u_xlat0.x;
    u_xlat0.z = (-u_xlat0.x) + 1.0;
    u_xlat0.xy = _ScrollSpeed.xy * _Time.yy + in_TEXCOORD0.xy;
    phase0_Output0_1.xyz = u_xlat0.xyz;
    vs_TEXCOORD1.xyz = _FogColor.xyz;
vs_TEXCOORD0 = phase0_Output0_1.xy;
vs_TEXCOORD2 = phase0_Output0_1.z;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp int;
uniform 	vec4 _Time;
uniform 	mediump vec4 _Color;
uniform 	float _xDistortionAplitude;
uniform 	float _xDistortionFrequency;
uniform 	float _xDistortionSpeed;
uniform 	float _xWaveYDistortionAplitude;
uniform 	float _xWaveYDistortionFrequency;
uniform 	float _xWaveYDistortionSpeed;
uniform 	float _xWaveYDistortionStart;
uniform 	float _xWaveYDistortionPosFactor;
uniform 	float _yDistortionAplitude;
uniform 	float _yDistortionFrequency;
uniform 	float _yDistortionSpeed;
uniform lowp sampler2D _MainTex;
in mediump vec2 vs_TEXCOORD0;
in mediump float vs_TEXCOORD2;
in mediump vec4 vs_TEXCOORD1;
in mediump vec2 vs_TEXCOORD3;
layout(location = 0) out mediump vec4 SV_Target0;
vec2 u_xlat0;
lowp vec3 u_xlat10_0;
mediump vec3 u_xlat16_1;
float u_xlat2;
float u_xlat4;
float u_xlat6;
void main()
{
    u_xlat0.x = (-_xWaveYDistortionStart) + 1.0;
    u_xlat2 = vs_TEXCOORD0.y + (-_xWaveYDistortionStart);
    u_xlat0.x = u_xlat2 / u_xlat0.x;
#ifdef UNITY_ADRENO_ES3
    u_xlat0.x = min(max(u_xlat0.x, 0.0), 1.0);
#else
    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
#endif
    u_xlat0.x = u_xlat0.x * u_xlat0.x;
    u_xlat0.x = u_xlat0.x * _xWaveYDistortionFrequency;
    u_xlat0.x = _Time.y * _xWaveYDistortionSpeed + u_xlat0.x;
    u_xlat0.x = _xWaveYDistortionPosFactor * vs_TEXCOORD3.y + u_xlat0.x;
    u_xlat2 = vs_TEXCOORD3.y * _xDistortionFrequency;
    u_xlat0.y = _Time.y * _xDistortionSpeed + u_xlat2;
    u_xlat0.xy = sin(u_xlat0.xy);
    u_xlat2 = _xDistortionAplitude * u_xlat0.y + vs_TEXCOORD0.x;
    u_xlat0.x = _xWaveYDistortionAplitude * u_xlat0.x + u_xlat2;
    u_xlat4 = vs_TEXCOORD3.y * _yDistortionFrequency;
    u_xlat4 = _Time.y * _yDistortionSpeed + u_xlat4;
    u_xlat4 = sin(u_xlat4);
    u_xlat16_1.x = (-vs_TEXCOORD0.y) + 1.0;
    u_xlat6 = u_xlat16_1.x * _yDistortionAplitude;
    u_xlat0.y = (-u_xlat6) * u_xlat4 + vs_TEXCOORD0.y;
#ifdef UNITY_ADRENO_ES3
    u_xlat0.y = min(max(u_xlat0.y, 0.0), 1.0);
#else
    u_xlat0.y = clamp(u_xlat0.y, 0.0, 1.0);
#endif
    u_xlat10_0.xyz = texture(_MainTex, u_xlat0.xy).xyz;
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
