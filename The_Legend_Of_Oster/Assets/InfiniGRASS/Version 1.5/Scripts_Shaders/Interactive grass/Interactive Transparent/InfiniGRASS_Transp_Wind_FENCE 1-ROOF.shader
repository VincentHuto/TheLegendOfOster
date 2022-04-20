// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_LightmapInd', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D
// Upgrade NOTE: replaced tex2D unity_LightmapInd with UNITY_SAMPLE_TEX2D_SAMPLER

Shader "InfiniGRASS/InfiniGrass Directional Wind ROOF" 
{
	Properties
	{
		// Specular vs Metallic workflow
		[HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0

		[MainColor] _BaseColor("Color", Color) = (0.5,0.5,0.5,1)
		[MainTexture] _BaseMap("Albedo", 2D) = "white" {}

	_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
		_SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

	_SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
		_SpecGlossMap("Specular", 2D) = "white" {}

	[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

	_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

	_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

	// Blending state
	[HideInInspector] _Surface("__surface", Float) = 0.0
		[HideInInspector] _Blend("__blend", Float) = 0.0
		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		_Cull("__cull", Float) = 2.0

		_ReceiveShadows("Receive Shadows", Float) = 1.0
		// Editmode props
		[HideInInspector] _QueueOffset("Queue offset", Float) = 0.0

		// ObsoleteProperties
		[HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
	[HideInInspector] _Color("Base Color", Color) = (0.5, 0.5, 0.5, 1)
		[HideInInspector] _GlossMapScale("Smoothness", Float) = 0.0
		[HideInInspector] _Glossiness("Smoothness", Float) = 0.0
		[HideInInspector] _GlossyReflections("EnvironmentReflections", Float) = 0.0


		//INFINIGRASS 1
		_BulgeScale("Bulge Scale", Float) = 0.2
		_BulgeShape("Bulge Shape", Float) = 5
		_BulgeScale_copy("Bulge Scale_copy", Float) = 1.2 // control turbulence

		_WaveControl1("Waves", Vector) = (1, 0.01, 0.001, 0.41) // _WaveControl1.w controls interaction power
		_TimeControl1("Time", Vector) = (1, 1, 1, 100)
		_OceanCenter("Ocean Center", Vector) = (0, 0, 0, 0)

		_RandYScale("Vary Height Ammount", Float) = 1
		_RippleScale("Vary Height", Float) = 0

		//INFINIGRASS - hero position for fading and lowering dynamics to reduce jitter while interacting  

		_InteractPos("Interact Position", Vector) = (0, 0, 0) //for lowering motion when interaction item is near

		_InteractSpeed("Interact Speed", Vector) = (0, 0, 0) //v1.5
		_FadeThreshold("Fade out Threshold", Float) = 100
		_StopMotionThreshold("Stop motion Threshold", Float) = 10

		_Color("Grass tint", Color) = (0.5,0.8,0.5,0) //0.5,0.8,0.5
		_ColorGlobal("Global tint", Color) = (0.5,0.5,0.5,0) //0.5,0.8,0.5
		_TintPower("tint power", Float) = 0
		_TintFrequency("tint frequency", Float) = 0.1
		_SpecularPower("Specular", Float) = 1

		_SmoothMotionFactor("Smooth wave motion", Float) = 105
		_WaveXFactor("Wave Control x axis", Float) = 1
		_WaveYFactor("Wave Control y axis", Float) = 1

		//SNOW
		_SnowTexture("Snow texture", 2D) = "white" {}

	//ROOF version of FENCE shader - control local vs global wind
	//_TimeControl1.y for global, _TimeControl1.z for local
	_BaseLight("Grass Base light control", Float) = 0
		_BaseColorYShift("Shift Base color Y axis", Float) = 1

		//v2.0.6
		_InteractMaxYoffset("Interaction max offset in Y axis", Float) = 1.5

		_SnowOffset("_SnowOffset", Float) = 0
		//END INFINIGRASS 1

		//INFINIGRASS 2
		//v1.7 - Local interact radial - amplitude - frequency
		_InteractPos1("Interact Position 1", Vector) = (0, 0, 0)
		_InteractAmpFreqRad("_InteractAmpFreqRadial", Vector) = (1, 1, 1)
		_StopMotionThresholdHELI("Stop motion Threshold HELICOPTER", Float) = 10
		//END INFINIGRASS 2		
	}

		SubShader
	{
		// Lightweight Pipeline tag is required. If Lightweight render pipeline is not set in the graphics settings
		// this Subshader will fail. One can add a subshader below or fallback to Standard built-in to make this
		// material work with both Lightweight Render Pipeline and Builtin Unity Pipeline
		//Tags{ "RenderType" = "Transparent" "RenderPipeline" = "LightweightPipeline" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		Tags{ "RenderType" = "AlphaTest" "RenderPipeline" = "LightweightPipeline" "IgnoreProjector" = "True" "Queue" = "AlphaTest" }
		LOD 300

		// ------------------------------------------------------------------
		//  Forward pass. Shades all light in a single pass. GI + emission + Fog
		Pass
	{
		// Lightmode matches the ShaderPassName set in LightweightRenderPipeline.cs. SRPDefaultUnlit and passes with
		// no LightMode tag are also rendered by Lightweight Render Pipeline
		Name "ForwardLit"
		//Tags{ "LightMode" = "LightweightForward" }
		Tags
		{
			"LightMode" = "UniversalForward"
		}

		Blend[_SrcBlend][_DstBlend]
		ZWrite[_ZWrite]
		Cull[_Cull]

		HLSLPROGRAM
		// Required to compile gles 2.0 with standard SRP library
		// All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
//#pragma prefer_hlslcc gles
//#pragma exclude_renderers d3d11_9x
//#pragma target 2.0

		// -------------------------------------
		// Material Keywords
#pragma shader_feature _NORMALMAP
#pragma shader_feature _ALPHATEST_ON
#pragma shader_feature _ALPHAPREMULTIPLY_ON
#pragma shader_feature _EMISSION
#pragma shader_feature _METALLICSPECGLOSSMAP
#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature _OCCLUSIONMAP

#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature _ENVIRONMENTREFLECTIONS_OFF
#pragma shader_feature _SPECULAR_SETUP
#pragma shader_feature _RECEIVE_SHADOWS_ON

		// -------------------------------------
		// Lightweight Pipeline keywords
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile _ _SHADOWS_SOFT
#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

		// -------------------------------------
		// Unity defined keywords
#pragma multi_compile _ DIRLIGHTMAP_COMBINED
#pragma multi_compile _ LIGHTMAP_ON
#pragma multi_compile_fog

		//--------------------------------------
		// GPU Instancing
#pragma multi_compile_instancing

#pragma vertex LitPassVertex
#pragma fragment LitPassFragment

#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
		//#include "Packages/com.unity.render-pipelines.lightweight/Shaders/LitForwardPass.hlsl"


#ifndef LIGHTWEIGHT_FORWARD_LIT_PASS_INCLUDED
#define LIGHTWEIGHT_FORWARD_LIT_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


		//INFINIGRASS 1
		uniform float4 _TimeEditor;
	uniform float _BulgeScale;
	uniform float _BulgeShape;
	uniform float _BulgeScale_copy;
	float4 _WaveControl1;
	float4 _TimeControl1;
	float3 _OceanCenter;
	//uniform float _Cutoff;
	uniform float _RandYScale;
	uniform float _RippleScale;

	float3 _InteractPos;
	float _FadeThreshold;
	float _StopMotionThreshold;

	float3 _Color;
	float3 _ColorGlobal;
	float _TintPower;
	float _SpecularPower;
	float _SmoothMotionFactor;
	float _WaveXFactor;
	float _WaveYFactor;

	sampler2D _SnowTexture;
	float _SnowCoverage;
	float _TintFrequency;
	float3 _InteractSpeed;
	float _InteractMaxYoffset;
	float _SnowOffset;
	//END INFINIGRASS 1

	//INFINIGRASS 2
	float3 _InteractPos1;
	float3 _InteractAmpFreqRad;
	float _StopMotionThresholdHELI;
	//END INFINIGRASS 2


	struct Attributes
	{
		float4 positionOS   : POSITION;
		float3 normalOS     : NORMAL;
		float4 tangentOS    : TANGENT;
		float2 texcoord     : TEXCOORD0;
		float2 lightmapUV   : TEXCOORD1;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		//float facing : VFACE; 

		float2 uv                       : TEXCOORD0;
		DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

#ifdef _ADDITIONAL_LIGHTS
		float3 positionWS               : TEXCOORD2;
#endif

#ifdef _NORMALMAP
		half4 normalWS                  : TEXCOORD3;    // xyz: normal, w: viewDir.x
		half4 tangentWS                 : TEXCOORD4;    // xyz: tangent, w: viewDir.y
		half4 bitangentWS                : TEXCOORD5;    // xyz: bitangent, w: viewDir.z
#else
		half3 normalWS                  : TEXCOORD3;
		half3 viewDirWS                 : TEXCOORD4;
#endif

		half4 fogFactorAndVertexLight   : TEXCOORD6; // x: fogFactor, yzw: vertex light

#ifdef _MAIN_LIGHT_SHADOWS
		float4 shadowCoord              : TEXCOORD7;
#endif

		float4 positionCS               : SV_POSITION;
		//float facing : VFACE;
		//https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics?redirectedfrom=MSDN#Semantics_All1
		//uint facing               :SV_IsFrontFace;
		//float facing : VFACE;

		UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
	};

	void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
	{
		inputData = (InputData)0;

#ifdef _ADDITIONAL_LIGHTS
		inputData.positionWS = input.positionWS;
#endif

#ifdef _NORMALMAP
		half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
		inputData.normalWS = TransformTangentToWorld(normalTS,
			half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
#else
		half3 viewDirWS = input.viewDirWS;
		inputData.normalWS = input.normalWS;
#endif

		inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
		viewDirWS = SafeNormalize(viewDirWS);

		inputData.viewDirectionWS = viewDirWS;
#if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
		inputData.shadowCoord = input.shadowCoord;
#else
		inputData.shadowCoord = float4(0, 0, 0, 0);
#endif
		inputData.fogCoord = input.fogFactorAndVertexLight.x;
		inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
		inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
	}

	///////////////////////////////////////////////////////////////////////////////
	//                  Vertex and Fragment functions                            //
	///////////////////////////////////////////////////////////////////////////////

	// Used in Standard (Physically Based) shader
	Varyings LitPassVertex(Attributes input)
	{
		Varyings output = (Varyings)0;

		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_TRANSFER_INSTANCE_ID(input, output);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

		/////////////// INFINIGRASS 1
		VertexPositionInputs vertexInputPre = GetVertexPositionInputs(input.positionOS.xyz);
		float3 positionWS = vertexInputPre.positionWS;

		////// MODIFY WIND				
		float3 SpeedFac = float3(0, 0, 0);  //	SpeedFac =  _InteractSpeed;  
		float distA = distance(_InteractPos, positionWS) / (_StopMotionThreshold * 1);
		if (distance(_InteractPos, positionWS) < _StopMotionThreshold * 1) {
			SpeedFac = _InteractSpeed * _WaveControl1.w;

			if (input.texcoord.y > 0.19) {
				_WaveXFactor = _WaveXFactor - (1 - distA)*(1 - distA)*SpeedFac.z * 1;
				_WaveYFactor = _WaveYFactor - (1 - distA)*(1 - distA)*SpeedFac.x * 1;
			}
			if (input.texcoord.y > 0.5) {
				positionWS.y = positionWS.y - (1 - distA)*_InteractMaxYoffset*(input.texcoord.y - 0.5)*sin(positionWS.z + _Time.y);//+_BulgeScale*0.5*cos(positionWS.x*_WaveControl1.x+_Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z+_Time.y) ;
			}
		}

		//v1.5
		_BulgeScale = _BulgeScale * _BulgeScale_copy;
		float dist = 90 * (cos(_BulgeShape + _Time.y / 15)) - _SmoothMotionFactor;

		if (input.texcoord.y > 0.1) {
			positionWS.x += _BulgeScale * 1 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y)
				+ _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 5)
				+ _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 5);
			positionWS.z += _BulgeScale * 1 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y)
				+ _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 5)
				+ _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 6);
		}
		if (input.texcoord.y > 0.2) {
			positionWS.x += _BulgeScale * 2 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y)
				+ _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 3)
				+ _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 3);
			positionWS.z += _BulgeScale * 2 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y)
				+ _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 3)
				+ _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 3);
		}
		if (input.texcoord.y > 0.3) {
			positionWS.x += _BulgeScale * 3 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y)
				+ _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 3)
				+ _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 4);
			positionWS.z += _BulgeScale * 3 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y)
				+ _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 3)
				+ _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 3);
		}
		if (input.texcoord.y > 0.4) {
			positionWS.x += _BulgeScale * 4 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y)
				+ _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 2)
				+ _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 2);
			positionWS.z += _BulgeScale * 4 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y)
				+ _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 2)
				+ _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 2);
		}
		if (input.texcoord.y > 0.96) {
			positionWS.x += _BulgeScale * 5 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y)
				+ _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 0.9)
				+ _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 1);
			positionWS.z += _BulgeScale * 5 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y)
				+ _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 0.9)
				+ _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 1);
		}
		//// END MODIFY WIND			
		//input.positionOS = float4(TransformWorldToObject(positionWS), 1); //modify object space from world space
		/////////// END INFINIGRASS 1

		////////// INFINIGRASS 2
		/////////////////////////////////////////////////////////////////////////////////////////// LOCAL INTERACTOR 2 /////////////////////////////////////////////
		SpeedFac = float3(0, 0, 0);
		distA = distance(_InteractPos1, positionWS) / (_StopMotionThresholdHELI);
		distA = distA + 0.08;
		if (distance(_InteractPos1, positionWS) < _StopMotionThresholdHELI) {
			SpeedFac = 3 * (positionWS - _InteractPos1) *_WaveControl1.w
				+ _InteractAmpFreqRad.z*(1.1*cross(float3(0, 1, 0.5), positionWS - _InteractPos1) - 2.71*cross(float3(0, 1, 0), _InteractPos1 - positionWS))
				+ _InteractAmpFreqRad.x*(positionWS - _InteractPos1) *_WaveControl1.w *sin(positionWS.z + _InteractAmpFreqRad.y*_Time.y);
			//_BulgeScale = _BulgeScale + _BulgeScale * (1 / (distA + 0.01));
			_BulgeScale = 0.1*_BulgeScale * (1 / (distA + 0.01));

			if (input.texcoord.y < 0.3) {
			}
			if (input.texcoord.y > 0.49) {
				_WaveXFactor = _WaveXFactor - (1 - distA)*(1 - distA)*SpeedFac.z;
				_WaveYFactor = _WaveYFactor - (1 - distA)*(1 - distA)*SpeedFac.x;

				/////
				positionWS.x += _BulgeScale * (_WaveXFactor * ((2 + sin(positionWS.x / distA)) * 1 / 0.5)
					+ _WaveYFactor * ((3 + cos(3 * positionWS.z / distA)) * 1 / 1));
				positionWS.z += _BulgeScale * (_WaveXFactor * ((2 + sin(positionWS.z / distA)) * 1 / 0.9)
					+ _WaveYFactor * ((3 + cos(3 * positionWS.x / distA)) * 1 / 1));
			}
			if (input.texcoord.y > 0.5) {
				positionWS.y = positionWS.y - (1 - distA) * 3 * (input.texcoord.y - 0.5)*sin(positionWS.z + _Time.y);
			}
		}
		/////////////////////////////////////////////////////////////////////////////////////////// END LOCAL INTERACTOR 2 /////////////////////////////////////////////
		////////// END INFINIGRASS 2		

		//APPLY INFINIGRASS CHANGES FINAL
		input.positionOS = float4(TransformWorldToObject(positionWS), 1); //modify object space from world space


		VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

		VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
		half3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
		half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
		half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

		output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);

#ifdef _NORMALMAP
		output.normalWS = half4(normalInput.normalWS, viewDirWS.x);
		output.tangentWS = half4(normalInput.tangentWS, viewDirWS.y);
		output.bitangentWS = half4(normalInput.bitangentWS, viewDirWS.z);
#else
		output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
		output.viewDirWS = viewDirWS;
#endif

		OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
		OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

		output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

#ifdef _ADDITIONAL_LIGHTS
		output.positionWS = vertexInput.positionWS;
#endif

#if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
		output.shadowCoord = GetShadowCoord(vertexInput);
#endif

		output.positionCS = vertexInput.positionCS;

		return output;
	}


	float3 _LightDirection;


	// Used in Standard (Physically Based) shader
	half4 LitPassFragment(Varyings input, half facing : VFACE) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(input);
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	SurfaceData surfaceData;
	InitializeStandardLitSurfaceData(input.uv, surfaceData);

	InputData inputData;

	//INFINIGRASS 1
#ifdef _NORMALMAP
	float3 viewDir = float3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
	//float3 viewDir = float3(inputData.viewDirectionWS.x, inputData.viewDirectionWS.y, inputData.viewDirectionWS.z);
	//InitializeInputData(input, dot(surfaceData.normalTS.xyz, viewDir.xyz), inputData);
	//inputData.normalWS = dot(inputData.normalWS.xyz, viewDir.xyz);
	//inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
#else
	//InitializeInputData(input, surfaceData.normalTS, inputData);
	float3 viewDir = float3(input.viewDirWS.x, input.viewDirWS.y, input.viewDirWS.z); //viewDirectionWS
																					  //float3 viewDir = float3(inputData.viewDirectionWS.x, inputData.viewDirectionWS.y, inputData.viewDirectionWS.z);
																					  //InitializeInputData(input, dot(surfaceData.normalTS.xyz, viewDir.xyz), inputData);
																					  //inputData.normalWS = dot(inputData.normalWS.xyz, viewDir.xyz);
																					  //inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
#endif
	surfaceData.normalTS = dot(-viewDir,surfaceData.normalTS) * facing;
	//surfaceData.normalTS = surfaceData.normalTS * facing;
	//surfaceData.normalTS = dot(surfaceData.normalTS, -_LightDirection);
	//surfaceData.normalTS = dot(-surfaceData.normalTS* facing, _LightDirection); //EMULATE TRANSLUCENCY
	//surfaceData.normalTS = dot(surfaceData.normalTS, -_LightDirection.xyz* 1);
	//#ifndef _ADDITIONAL_LIGHTS
	//if (normalize(_LightDirection).z > 0.9) {
	//if (dot(_LightDirection.z, float3(0,0,1)) == 1  ) {
	//if (_LightDirection.x == _MainLightPosition.x && _LightDirection.y == _MainLightPosition.y && _LightDirection.z == _MainLightPosition.z) {
	//if (0 == _MainLightPosition.x && 0 == _MainLightPosition.y && 0 == _MainLightPosition.z) {
	if (_LightDirection.x == 0 && _LightDirection.y == 0 && _LightDirection.z == 0) {
		//float normalZ = surfaceData.normalTS.y;
		//surfaceData.normalTS = dot(surfaceData.normalTS, -_LightDirection.xyz);
		//surfaceData.normalTS.y = normalZ;
	}
	else {
		float normalY = surfaceData.normalTS.y;
		surfaceData.normalTS = dot(surfaceData.normalTS, -_LightDirection.xyz);
		//surfaceData.normalTS.y = normalZ;
		surfaceData.normalTS.y = normalY * facing;
	}
	//#endif
	//surfaceData.normalTS = -surfaceData.normalTS;			
	//END INFINIGRASS 1

	InitializeInputData(input, surfaceData.normalTS, inputData);
	//#ifdef _NORMALMAP
	//			//float3 viewDir = float3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
	//			float3 viewDir = float3(inputData.viewDirectionWS.x, inputData.viewDirectionWS.y, inputData.viewDirectionWS.z);
	//			//InitializeInputData(input, dot(surfaceData.normalTS.xyz, viewDir.xyz), inputData);
	//			//inputData.normalWS = dot(inputData.normalWS.xyz, viewDir.xyz);
	//			//inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
	//#else
	//			//InitializeInputData(input, surfaceData.normalTS, inputData);
	//			//float3 viewDir = float3(input.viewDirWS.x, input.viewDirWS.y, input.viewDirWS.z); //viewDirectionWS
	//			float3 viewDir = float3(inputData.viewDirectionWS.x, inputData.viewDirectionWS.y, inputData.viewDirectionWS.z);
	//			//InitializeInputData(input, dot(surfaceData.normalTS.xyz, viewDir.xyz), inputData);
	//			//inputData.normalWS = dot(inputData.normalWS.xyz, viewDir.xyz);
	//			//inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
	//#endif
	//InitializeInputData(input, mul(surfaceData.normalTS.xyz, input.viewDirWS.xyz), inputData);
	//InitializeInputData(input, surfaceData.normalTS, inputData);

	half4 color = LightweightFragmentPBR(inputData, surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha);

	color.rgb = MixFog(color.rgb, inputData.fogCoord);

	//INFINIGRASS 1
	color.rgb = lerp(color.rgb, color.rgb*_Color, _TintPower*input.uv.y*(0.9 + 0.6*cos(inputData.positionWS.x * 2 * _TintFrequency)
		+ 0.6*sin(inputData.positionWS.z * 3 * _TintFrequency) + 0.6*sin(inputData.positionWS.z * 1 * _TintFrequency + 0.1)));
	float4 SnowTexColor = tex2D(_SnowTexture, input.uv);
	//if (input.uv.y >= 1 - (4 * (_SnowCoverage + _SnowOffset + _TimeControl1.y - 1)) *clamp(color.r, 0.85*(color.r + 0.2), 1)* clamp(color.r, 0.35, 1) * 1 + 0.01) //v2.0.7
	if (input.uv.y >= 1 - (4 * (_SnowCoverage + _SnowOffset + _TimeControl1.y - 1))
		* clamp(SnowTexColor.r, 0.85*(SnowTexColor.r + 0.2), 1)
		* clamp(SnowTexColor.r, 0.35, 1) * 1 + 0.01)
	{
		if (input.uv.y < 0.99) {
			color = 0.92*(float4(input.uv.y, input.uv.y, input.uv.y, 1) * 4 * +_TimeControl1.z)*(1 + color)*clamp(color.r, 0.85*(color.r + 0.2), 1);   //v2.0.7
		}
	}
	//END INDINIGRASS 1

	//return color;
	return float4(color.r, color.g, color.b, 0);
	}
#endif			
		ENDHLSL
	}




		Pass
	{
		Name "ShadowCaster"
		Tags{ "LightMode" = "ShadowCaster" }

		ZWrite On
		ZTest LEqual
		//	ZTest Less
		//	ZTest NotEqual
		//	ZTest GEqual
		//	ZTest Greater
		//	ZTest Always
		Cull[_Cull]

		HLSLPROGRAM
		// Required to compile gles 2.0 with standard srp library
#pragma prefer_hlslcc gles
#pragma exclude_renderers d3d11_9x
#pragma target 2.0

		// -------------------------------------
		// Material Keywords
#pragma shader_feature _ALPHATEST_ON

		//--------------------------------------
		// GPU Instancing
#pragma multi_compile_instancing
#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

#pragma vertex ShadowPassVertex
#pragma fragment ShadowPassFragment

#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
		//#include "Packages/com.unity.render-pipelines.lightweight/Shaders/ShadowCasterPass.hlsl"


#ifndef LIGHTWEIGHT_SHADOW_CASTER_PASS_INCLUDED
#define LIGHTWEIGHT_SHADOW_CASTER_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

		float3 _LightDirection;

	//INFINIGRASS 1
	uniform float4 _TimeEditor;
	uniform float _BulgeScale;
	uniform float _BulgeShape;
	uniform float _BulgeScale_copy;
	float4 _WaveControl1;
	float4 _TimeControl1;
	float3 _OceanCenter;
	//uniform float _Cutoff;
	uniform float _RandYScale;
	uniform float _RippleScale;

	float3 _InteractPos;
	float _FadeThreshold;
	float _StopMotionThreshold;
	float _SmoothMotionFactor;
	float _WaveXFactor;
	float _WaveYFactor;

	float3 _InteractSpeed;
	float _InteractMaxYoffset;
	//END INFINIGRASS 1

	//INFINIGRASS 2
	float3 _InteractPos1;
	float3 _InteractAmpFreqRad;
	float _StopMotionThresholdHELI;
	//END INFINIGRASS 2

	struct Attributes
	{
		float4 positionOS   : POSITION;
		float3 normalOS     : NORMAL;
		float2 texcoord     : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float2 uv           : TEXCOORD0;
		float4 positionCS   : SV_POSITION;
	};

	float4 GetShadowPositionHClip(Attributes input)
	{
		float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);

		//positionWS.z = positionWS.z + 0.03;

		float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

		float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

#if UNITY_REVERSED_Z
		positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#else
		positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#endif


		positionCS.z = positionCS.z - 0.002; // INFINIGRASS, fix - remove self shadowing of grass

		return positionCS;
	}

	Varyings ShadowPassVertex(Attributes input)
	{
		Varyings output;
		UNITY_SETUP_INSTANCE_ID(input);

		/////////////// INFINIGRASS 1
		VertexPositionInputs vertexInputPre = GetVertexPositionInputs(input.positionOS.xyz);
		float3 positionWS = vertexInputPre.positionWS;

		////// MODIFY WIND				
		float3 SpeedFac = float3(0, 0, 0);  //	SpeedFac =  _InteractSpeed;  
		float distA = distance(_InteractPos, positionWS) / (_StopMotionThreshold * 1);
		if (distance(_InteractPos, positionWS) < _StopMotionThreshold * 1) {
			SpeedFac = _InteractSpeed * _WaveControl1.w;

			if (input.texcoord.y > 0.19) {
				_WaveXFactor = _WaveXFactor - (1 - distA)*(1 - distA)*SpeedFac.z * 1;
				_WaveYFactor = _WaveYFactor - (1 - distA)*(1 - distA)*SpeedFac.x * 1;
			}
			if (input.texcoord.y > 0.5) {
				positionWS.y = positionWS.y - (1 - distA)*_InteractMaxYoffset*(input.texcoord.y - 0.5)*sin(positionWS.z + _Time.y);//+_BulgeScale*0.5*cos(positionWS.x*_WaveControl1.x+_Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z+_Time.y) ;
			}
		}

		//v1.5
		_BulgeScale = _BulgeScale * _BulgeScale_copy;
		float dist = 90 * (cos(_BulgeShape + _Time.y / 15)) - _SmoothMotionFactor;

		if (input.texcoord.y > 0.1) {
			positionWS.x += _BulgeScale * 1 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y) + _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 5) + _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 5);
			positionWS.z += _BulgeScale * 1 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y) + _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 5) + _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 6);
		}
		if (input.texcoord.y > 0.2) {
			positionWS.x += _BulgeScale * 2 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y) + _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 3) + _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 3);
			positionWS.z += _BulgeScale * 2 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y) + _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 3) + _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 3);
		}
		if (input.texcoord.y > 0.3) {

			positionWS.x += _BulgeScale * 3 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y) + _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 3) + _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 4);
			positionWS.z += _BulgeScale * 3 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y) + _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 3) + _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 3);
		}
		if (input.texcoord.y > 0.4) {
			positionWS.x += _BulgeScale * 4 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y) + _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 2) + _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 2);
			positionWS.z += _BulgeScale * 4 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y) + _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 2) + _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 2);
		}
		if (input.texcoord.y > 0.96) {

			positionWS.x += _BulgeScale * 5 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y) + _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 0.9) + _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 1);
			positionWS.z += _BulgeScale * 5 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y) + _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 0.9) + _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 1);
		}
		//// END MODIFY WIND			
		//input.positionOS = float4(TransformWorldToObject(positionWS), 1); //modify object space from world space
		/////////// END INFINIGRASS 1

		////////// INFINIGRASS 2
		/////////////////////////////////////////////////////////////////////////////////////////// LOCAL INTERACTOR 2 /////////////////////////////////////////////
		SpeedFac = float3(0, 0, 0);
		distA = distance(_InteractPos1, positionWS) / (_StopMotionThresholdHELI);
		distA = distA + 0.08;
		if (distance(_InteractPos1, positionWS) < _StopMotionThresholdHELI) {
			SpeedFac = 3 * (positionWS - _InteractPos1) *_WaveControl1.w
				+ _InteractAmpFreqRad.z*(1.1*cross(float3(0, 1, 0.5), positionWS - _InteractPos1) - 2.71*cross(float3(0, 1, 0), _InteractPos1 - positionWS))
				+ _InteractAmpFreqRad.x*(positionWS - _InteractPos1) *_WaveControl1.w *sin(positionWS.z + _InteractAmpFreqRad.y*_Time.y);
			//_BulgeScale = _BulgeScale + _BulgeScale * (1 / (distA + 0.01));
			_BulgeScale = 0.1*_BulgeScale * (1 / (distA + 0.01));

			if (input.texcoord.y < 0.3) {
			}
			if (input.texcoord.y > 0.49) {
				_WaveXFactor = _WaveXFactor - (1 - distA)*(1 - distA)*SpeedFac.z;
				_WaveYFactor = _WaveYFactor - (1 - distA)*(1 - distA)*SpeedFac.x;

				/////
				positionWS.x += _BulgeScale * (_WaveXFactor * ((2 + sin(positionWS.x / distA)) * 1 / 0.5)
					+ _WaveYFactor * ((3 + cos(3 * positionWS.z / distA)) * 1 / 1));
				positionWS.z += _BulgeScale * (_WaveXFactor * ((2 + sin(positionWS.z / distA)) * 1 / 0.9)
					+ _WaveYFactor * ((3 + cos(3 * positionWS.x / distA)) * 1 / 1));
			}
			if (input.texcoord.y > 0.5) {
				positionWS.y = positionWS.y - (1 - distA) * 3 * (input.texcoord.y - 0.5)*sin(positionWS.z + _Time.y);
			}
		}
		/////////////////////////////////////////////////////////////////////////////////////////// END LOCAL INTERACTOR 2 /////////////////////////////////////////////
		////////// END INFINIGRASS 2		

		//APPLY INFINIGRASS CHANGES FINAL
		input.positionOS = float4(TransformWorldToObject(positionWS), 1); //modify object space from world space

		output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
		output.positionCS = GetShadowPositionHClip(input);

		return output;
	}

	half4 ShadowPassFragment(Varyings input, half facing : VFACE) : SV_TARGET
	{

		//positionWS.z = positionWS.z - 0.03;

		//Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
		if (facing == 1) {
			//input.uv.x = 1 - input.uv.x;
			Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
		}
	if (facing == -1) {
		//input.positionCS.z = input.positionCS.z - 0.58;
		//input.uv.x = 1 - input.uv.x; //positionCS.z = positionCS.z - 0.003;
		//if (input.positionCS.z < 0) {
		//	Alpha(0, _BaseColor, _Cutoff);
		//}
		//else {					
		Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
		//}
	}
	return 0;
	}

#endif


		ENDHLSL
	}

		Pass
	{
		Name "DepthOnly"
		Tags{ "LightMode" = "DepthOnly" }

		ZWrite On
		ColorMask 0
		Cull[_Cull]

		HLSLPROGRAM
		// Required to compile gles 2.0 with standard srp library
#pragma prefer_hlslcc gles
#pragma exclude_renderers d3d11_9x
#pragma target 2.0

#pragma vertex DepthOnlyVertex
#pragma fragment DepthOnlyFragment

		// -------------------------------------
		// Material Keywords
#pragma shader_feature _ALPHATEST_ON
#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

		//--------------------------------------
		// GPU Instancing
#pragma multi_compile_instancing

#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
		//#include "Packages/com.unity.render-pipelines.lightweight/Shaders/DepthOnlyPass.hlsl"

#ifndef LIGHTWEIGHT_DEPTH_ONLY_PASS_INCLUDED
#define LIGHTWEIGHT_DEPTH_ONLY_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		//INFINIGRASS 1
		uniform float4 _TimeEditor;
	uniform float _BulgeScale;
	uniform float _BulgeShape;
	uniform float _BulgeScale_copy;
	float4 _WaveControl1;
	float4 _TimeControl1;
	float3 _OceanCenter;
	//uniform float _Cutoff;
	uniform float _RandYScale;
	uniform float _RippleScale;

	float3 _InteractPos;
	float _FadeThreshold;
	float _StopMotionThreshold;
	float _SmoothMotionFactor;
	float _WaveXFactor;
	float _WaveYFactor;

	float3 _InteractSpeed;
	float _InteractMaxYoffset;
	//END INFINIGRASS 1

	//INFINIGRASS 2
	float3 _InteractPos1;
	float3 _InteractAmpFreqRad;
	float _StopMotionThresholdHELI;
	//END INFINIGRASS 2

	struct Attributes
	{
		float4 position     : POSITION;
		float2 texcoord     : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float2 uv           : TEXCOORD0;
		float4 positionCS   : SV_POSITION;
		UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings DepthOnlyVertex(Attributes input)
	{
		Varyings output = (Varyings)0;
		UNITY_SETUP_INSTANCE_ID(input);

		/////////////// INFINIGRASS 1
		VertexPositionInputs vertexInputPre = GetVertexPositionInputs(input.position.xyz);
		float3 positionWS = vertexInputPre.positionWS;

		////// MODIFY WIND				
		float3 SpeedFac = float3(0, 0, 0);  //	SpeedFac =  _InteractSpeed;  
		float distA = distance(_InteractPos, positionWS) / (_StopMotionThreshold * 1);
		if (distance(_InteractPos, positionWS) < _StopMotionThreshold * 1) {
			SpeedFac = _InteractSpeed * _WaveControl1.w;

			if (input.texcoord.y > 0.19) {
				_WaveXFactor = _WaveXFactor - (1 - distA)*(1 - distA)*SpeedFac.z * 1;
				_WaveYFactor = _WaveYFactor - (1 - distA)*(1 - distA)*SpeedFac.x * 1;
			}
			if (input.texcoord.y > 0.5) {
				positionWS.y = positionWS.y - (1 - distA)*_InteractMaxYoffset*(input.texcoord.y - 0.5)*sin(positionWS.z + _Time.y);//+_BulgeScale*0.5*cos(positionWS.x*_WaveControl1.x+_Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z+_Time.y) ;
			}
		}

		//v1.5
		_BulgeScale = _BulgeScale * _BulgeScale_copy;
		float dist = 90 * (cos(_BulgeShape + _Time.y / 15)) - _SmoothMotionFactor;

		if (input.texcoord.y > 0.1) {
			positionWS.x += _BulgeScale * 1 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y) + _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 5) + _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 5);
			positionWS.z += _BulgeScale * 1 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y) + _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 5) + _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 6);
		}
		if (input.texcoord.y > 0.2) {
			positionWS.x += _BulgeScale * 2 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y) + _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 3) + _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 3);
			positionWS.z += _BulgeScale * 2 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y) + _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 3) + _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 3);
		}
		if (input.texcoord.y > 0.3) {

			positionWS.x += _BulgeScale * 3 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y) + _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 3) + _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 4);
			positionWS.z += _BulgeScale * 3 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y) + _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 3) + _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 3);
		}
		if (input.texcoord.y > 0.4) {
			positionWS.x += _BulgeScale * 4 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y) + _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 2) + _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 2);
			positionWS.z += _BulgeScale * 4 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y) + _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 2) + _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 2);
		}
		if (input.texcoord.y > 0.96) {

			positionWS.x += _BulgeScale * 5 * cos(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*sin(positionWS.z + _Time.y) + _WaveXFactor * ((2 + cos(positionWS.x / dist))*_OceanCenter.x / 0.9) + _WaveYFactor * ((3 + sin(2 * positionWS.z / dist))*_OceanCenter.z / 1);
			positionWS.z += _BulgeScale * 5 * sin(positionWS.x*_WaveControl1.x + _Time.y*_TimeControl1.x + positionWS.z*_WaveControl1.z)*0.1*cos(positionWS.z + _Time.y) + _WaveXFactor * ((2 + sin(positionWS.z / dist))*_OceanCenter.z / 0.9) + _WaveYFactor * ((3 + cos(3 * positionWS.x / dist))*_OceanCenter.x / 1);
		}
		//// END MODIFY WIND			
		//input.position = float4(TransformWorldToObject(positionWS), 1); //modify object space from world space
		/////////// END INFINIGRASS 1

		////////// INFINIGRASS 2
		/////////////////////////////////////////////////////////////////////////////////////////// LOCAL INTERACTOR 2 /////////////////////////////////////////////
		SpeedFac = float3(0, 0, 0);
		distA = distance(_InteractPos1, positionWS) / (_StopMotionThresholdHELI);
		distA = distA + 0.08;
		if (distance(_InteractPos1, positionWS) < _StopMotionThresholdHELI) {
			SpeedFac = 3 * (positionWS - _InteractPos1) *_WaveControl1.w
				+ _InteractAmpFreqRad.z*(1.1*cross(float3(0, 1, 0.5), positionWS - _InteractPos1) - 2.71*cross(float3(0, 1, 0), _InteractPos1 - positionWS))
				+ _InteractAmpFreqRad.x*(positionWS - _InteractPos1) *_WaveControl1.w *sin(positionWS.z + _InteractAmpFreqRad.y*_Time.y);
			//_BulgeScale = _BulgeScale + _BulgeScale * (1 / (distA + 0.01));
			_BulgeScale = 0.1*_BulgeScale * (1 / (distA + 0.01));

			if (input.texcoord.y < 0.3) {
			}
			if (input.texcoord.y > 0.49) {
				_WaveXFactor = _WaveXFactor - (1 - distA)*(1 - distA)*SpeedFac.z;
				_WaveYFactor = _WaveYFactor - (1 - distA)*(1 - distA)*SpeedFac.x;

				/////
				positionWS.x += _BulgeScale * (_WaveXFactor * ((2 + sin(positionWS.x / distA)) * 1 / 0.5)
					+ _WaveYFactor * ((3 + cos(3 * positionWS.z / distA)) * 1 / 1));
				positionWS.z += _BulgeScale * (_WaveXFactor * ((2 + sin(positionWS.z / distA)) * 1 / 0.9)
					+ _WaveYFactor * ((3 + cos(3 * positionWS.x / distA)) * 1 / 1));
			}
			if (input.texcoord.y > 0.5) {
				positionWS.y = positionWS.y - (1 - distA) * 3 * (input.texcoord.y - 0.5)*sin(positionWS.z + _Time.y);
			}
		}
		/////////////////////////////////////////////////////////////////////////////////////////// END LOCAL INTERACTOR 2 /////////////////////////////////////////////
		////////// END INFINIGRASS 2		

		//APPLY INFINIGRASS CHANGES FINAL
		input.position = float4(TransformWorldToObject(positionWS), 1); //modify object space from world space


		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

		output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
		output.positionCS = TransformObjectToHClip(input.position.xyz);
		return output;
	}

	half4 DepthOnlyFragment(Varyings input, half facing : VFACE) : SV_TARGET
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	if (facing == 1) {
		Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
	}
	if (facing == -1) {
		Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
	}
	return 0;
	}
#endif

		ENDHLSL
	}

		// This pass it not used during regular rendering, only for lightmap baking.
		Pass
	{
		Name "Meta"
		Tags{ "LightMode" = "Meta" }

		Cull Off

		HLSLPROGRAM
		// Required to compile gles 2.0 with standard srp library
#pragma prefer_hlslcc gles
#pragma exclude_renderers d3d11_9x

#pragma vertex LightweightVertexMeta
#pragma fragment LightweightFragmentMeta

#pragma shader_feature _SPECULAR_SETUP
#pragma shader_feature _EMISSION
#pragma shader_feature _METALLICSPECGLOSSMAP
#pragma shader_feature _ALPHATEST_ON
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

#pragma shader_feature _SPECGLOSSMAP

#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"

		ENDHLSL
	}

	}
		FallBack "Hidden/InternalErrorShader"
		//CustomEditor "UnityEditor.Rendering.LWRP.ShaderGUI.LitShader"
}


//    Properties {
//        _Diffuse ("Diffuse", 2D) = "white" {}
//        _Normal ("Normal", 2D) = "bump" {}
//        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
//        
//        _BulgeScale ("Bulge Scale", Float ) = 0.2
//        _BulgeShape ("Bulge Shape", Float ) = 5
//        _BulgeScale_copy ("Bulge Scale_copy", Float ) = 1.2 // control turbulence
//                               
//        _WaveControl1("Waves", Vector) = (1, 0.01, 0.001, 0.41) // _WaveControl1.w controls interaction power
//        _TimeControl1("Time", Vector) = (1, 1, 1, 100)
//        _OceanCenter("Ocean Center", Vector) = (0, 0, 0, 0)
//        
//         _RandYScale ("Vary Height Ammount", Float ) = 1
//         _RippleScale ("Vary Height", Float ) = 0     
//        
//           //INFINIGRASS - hero position for fading and lowering dynamics to reduce jitter while interacting  
//           
//           _InteractPos("Interact Position", Vector) = (0, 0, 0) //for lowering motion when interaction item is near
//            _InteractSpeed("Interact Speed", Vector) = (0, 0, 0) //v1.5
//           _FadeThreshold ("Fade out Threshold", Float ) = 100
//           _StopMotionThreshold ("Stop motion Threshold", Float ) = 10
//           
//           _Color ("Grass tint", Color) = (0.5,0.8,0.5,0) //0.5,0.8,0.5
//           _ColorGlobal ("Global tint", Color) = (0.5,0.5,0.5,0) //0.5,0.8,0.5
//           _TintPower("tint power", Float) = 0
//            _TintFrequency("tint frequency", Float) = 0.1
//           _SpecularPower("Specular", Float) = 1
//           
//           _SmoothMotionFactor("Smooth wave motion", Float) = 105
//           _WaveXFactor("Wave Control x axis", Float) = 1
//           _WaveYFactor("Wave Control y axis", Float) = 1
//           
//           //SNOW
//           _SnowTexture ("Snow texture", 2D) = "white" {}
//
//           //ROOF version of FENCE shader - control local vs global wind
//           //_TimeControl1.y for global, _TimeControl1.z for local
//           _BaseLight("Grass Base light control", Float) = 0
//           _BaseColorYShift("Shift Base color Y axis", Float) = 1
//
//           //v2.0.6
//           _InteractMaxYoffset("Interaction max offset in Y axis", Float) = 1.5
//    }
//    SubShader {
//        Tags {
//            "Queue"="AlphaTest"
//            "RenderType"="TransparentCutout"
//        }
//        Pass {
//            Name "ForwardBase"
//            Tags {
//                "LightMode"="ForwardBase"
//            }
//            Cull Off
//            
//            
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            //#define UNITY_PASS_FORWARDBASE
//            #include "UnityCG.cginc"
//            #include "AutoLight.cginc"
//            #include "Lighting.cginc"
//            #pragma multi_compile_instancing //v1.7.8
//            #pragma multi_compile_fwdbase_fullshadows
//			#pragma multi_compile_fwdbase nolightmap //v4.1
//
//           // #pragma exclude_renderers gles xbox360 ps3 flash 
//            #pragma target 3.0
//            uniform float4 _TimeEditor;
//            #ifndef LIGHTMAP_OFF
//                // float4 unity_LightmapST;
//                // sampler2D unity_Lightmap;
//                #ifndef DIRLIGHTMAP_OFF
//                    // sampler2D unity_LightmapInd;
//                #endif
//            #endif
//            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
//            uniform sampler2D _Normal; uniform float4 _Normal_ST;
//            
//            uniform float _BulgeScale; 
//            uniform float _BulgeShape;
//            uniform float _BulgeScale_copy;
//            float4 _WaveControl1;
//   			float4 _TimeControl1;
//    		float3 _OceanCenter;
//            uniform fixed _Cutoff;
//             uniform float _RandYScale;
//            uniform float _RippleScale;
//            //float3 _CameraPos;
//            float3 _InteractPos;
//            float _FadeThreshold;
//            float _StopMotionThreshold;
//            
//            float3 _Color;
//            float3 _ColorGlobal;
//           	float _TintPower; 
//           	float _SpecularPower;
//           	float _SmoothMotionFactor;
//           	float _WaveXFactor;
//           	float _WaveYFactor;
//           	
//           	sampler2D _SnowTexture;
//           	float _SnowCoverage; 	
//           	float _TintFrequency;
//
//           	 float3 _InteractSpeed;
//           	 float _InteractMaxYoffset;
//            
//            struct VertexInput {
//                float4 vertex : POSITION;
//                float3 normal : NORMAL;
//                float4 tangent : TANGENT;
//                float2 texcoord0 : TEXCOORD0;
//                float2 texcoord1 : TEXCOORD1;
//                float4 vertexColor : COLOR;
//                UNITY_VERTEX_INPUT_INSTANCE_ID //v1.7.8
//            };
//            struct VertexOutput {
//                float4 pos : SV_POSITION;
//                float2 uv0 : TEXCOORD0;
//                float4 posWorld : TEXCOORD1;
//                float3 normalDir : TEXCOORD2;
//                float3 tangentDir : TEXCOORD3;
//                float3 binormalDir : TEXCOORD4;
//                float4 vertexColor : COLOR;
//                LIGHTING_COORDS(5,6)
//                #ifndef LIGHTMAP_OFF
//                    float2 uvLM : TEXCOORD7;
//                #endif
//            };
//            VertexOutput vert (VertexInput v) {
//                VertexOutput o;
//                UNITY_SETUP_INSTANCE_ID(v); //v1.7.8
//                o.uv0 = v.texcoord0;
//                o.vertexColor = v.vertexColor;
//                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
//                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
//                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
//                float4 node_389 = o.vertexColor;
//                float4 node_392 = _Time + _TimeEditor;
//           //     v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
//                
//                
//                float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
//                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
//                
//                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
//                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
//                            
//                       
//                       //INIFNIGRASS
//                       float4 modelY = float4(0.0,1.0,0.0,0.0);
//                               float4 ModelYWorld =mul(unity_ObjectToWorld,modelY);
//                               float scaleY = length(ModelYWorld);
//                                
//                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
//              // o.posWorld =  v.vertex;
//
//
//
//                                                                                                  
////                if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold){                 
////                	_BulgeScale = 0;
////                	_BulgeScale_copy = 0;
////                }
////
//  float3 SpeedFac = float3(0,0,0);  //	SpeedFac =  _InteractSpeed;  
//                float distA =  distance(_InteractPos,o.posWorld)/ (_StopMotionThreshold*1);      
//                  if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold*1){ 
//               // if( distance(_InteractPos.x,o.posWorld.x) < _StopMotionThreshold/2 ){    
//               //      if( distance(_InteractPos.z,o.posWorld.z) < _StopMotionThreshold/2){                  
//                	//_BulgeScale = 0;
//                	//_BulgeScale_copy = 0;                	
//                	SpeedFac =  _InteractSpeed *_WaveControl1.w;
////                		if( o.uv0.y > 0.2){
////							o.posWorld.x += (_InteractSpeed.x*22+0.1)*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
////							o.posWorld.z += (_InteractSpeed.z+0.1)*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
////						}
//                	if( o.uv0.y < 0.3){
//                	//_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
//                	//_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
//                	}
//                	if( o.uv0.y > 0.19){
//						_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z*1;
//                		_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x*1;
//                	}
//                	if( o.uv0.y > 0.5){
//						//o.posWorld.y = o.posWorld.y - (1-distA)*3*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ;//+_BulgeScale*0.5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) ;
//						o.posWorld.y = o.posWorld.y - (1-distA)*_InteractMaxYoffset*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ; //v2.0.6
//					}             
//               }
//
//
//
//
//	                if( o.uv0.y > 0.1){
//	       //         	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) /scaleY;// unity_Scale.w;
//					}
//	                if( o.uv0.y >= 0.01){
//	       //         	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
//	                }
//
//	                //v1.5
//	           _BulgeScale= _BulgeScale* _BulgeScale_copy;
//	            //  _OceanCenter.x = 0.0;
//	          // _OceanCenter.z = 0.0;
//
//	                     dist = 90* (cos(_BulgeShape+_Time.y/15))-_SmoothMotionFactor;
//				///////////////////////// 
//				if( o.uv0.y > 0.1){
//					o.posWorld.x += _BulgeScale*1*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
//					o.posWorld.z += _BulgeScale*1*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
//				}
//				if( o.uv0.y > 0.2){					
//					o.posWorld.x += _BulgeScale*2*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/3);
//					o.posWorld.z += _BulgeScale*2*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);	
//				}
//				if( o.uv0.y > 0.3){
//					
//					o.posWorld.x += _BulgeScale*3*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/4);
//					o.posWorld.z += _BulgeScale*3*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);
//				}
//				if( o.uv0.y > 0.4){
//					
//					o.posWorld.x += _BulgeScale*4*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/2) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/2);
//					o.posWorld.z += _BulgeScale*4*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/2) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/2);	
//				}		
//				if( o.uv0.y > 0.96){
//					
//					o.posWorld.x += _BulgeScale*5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/0.9)	+ _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/1);
//					o.posWorld.z += _BulgeScale*5*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/0.9) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/1);
//				}	
//				//ADD GLOBAL ROTATION - WIND						
//				v.vertex = mul(unity_WorldToObject, o.posWorld);	            
//	           // v.vertex =  o.posWorld;
//                                                                                
//
//                
//                
//                o.pos = UnityObjectToClipPos(v.vertex);
//                #ifndef LIGHTMAP_OFF
//                    o.uvLM = v.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
//                #endif
//                TRANSFER_VERTEX_TO_FRAGMENT(o)
//                return o;
//            }
//            
//            
//            fixed4 frag(VertexOutput i) : COLOR {
//                i.normalDir = normalize(i.normalDir);
//                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
//                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
///////// Normals:
//                float2 node_582 = i.uv0;
//                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_582.rg, _Normal))).rgb;
//                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
//                
//                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
//                i.normalDir *= nSign;
//                normalDirection *= nSign;
//                
//                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_582.rg, _Diffuse));
//                
//                
//                
//                clip(node_1.a - _Cutoff);
//                
//                //DEFINE FADE BASED ON CAMERA - INFINIGRASS
//				float Aplha = 1;
//				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
//					 clip(-1);
//				}
//                
//                #ifndef LIGHTMAP_OFF
//                    float4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uvLM);
//                    #ifndef DIRLIGHTMAP_OFF
//                        float3 lightmap = DecodeLightmap(lmtex);
//                        float3 scalePerBasisVector = DecodeLightmap(UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap,i.uvLM));
//                        UNITY_DIRBASIS
//                        half3 normalInRnmBasis = saturate (mul (unity_DirBasis, normalLocal));
//                        lightmap *= dot (normalInRnmBasis, scalePerBasisVector);
//                    #else
//                        float3 lightmap = DecodeLightmap(lmtex);
//                    #endif
//                #endif
//                #ifndef LIGHTMAP_OFF
//                    #ifdef DIRLIGHTMAP_OFF
//                        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
//                    #else
//                        float3 lightDirection = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
//                        lightDirection = mul(lightDirection,tangentTransform); // Tangent to world
//                    #endif
//                #else
//                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
//                #endif
//                
//                
//                lightDirection =  normalize(reflect(_WorldSpaceLightPos0.xyz,normalDirection));
//                
//                
//                float3 halfDirection = normalize(viewDirection+lightDirection);
//////// Lighting:
//                //float attenuation = LIGHT_ATTENUATION(i); 
//                UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWorld.xyz);
//                
//                   //INFINIGRASS
//                 float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);
//                 float node_133 = pow((abs((frac((i.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
//                   float dist = distance(_OceanCenter, float3(_WaveControl1.x*i.posWorld.y,_WaveControl1.y*i.posWorld.x,_WaveControl1.z*i.posWorld.z) );
//               
//               float3 attenColor = attenuation * (_LightColor0.xyz);
//               
//               
//               
///////// Diffuse:
//                float NdotL = dot( normalDirection, lightDirection );
//                float3 w = float3(0.9,0.9,0.8)*0.5; // Light wrapping
//                float3 NdotLWrap = NdotL * ( 1.0 - w );
//                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
//                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) ;//* float3(0.9,1,0.5); //v1.4
//                #ifndef LIGHTMAP_OFF
//                    float3 diffuse = lightmap.rgb;
//                #else
//                    float3 diffuse = (forwardLight+backLight) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
//                #endif
/////////// Gloss:
//                float gloss = 0.4;
//                float specPow = exp2( gloss * 10.0+1.0);
//////// Specular:
//                NdotL = max(0.0, NdotL);
//                float node_3 = 0.2;
//                float3 specularColor = float3(node_3,node_3,node_3);
//                float3 specular = 3 * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
////                #ifndef LIGHTMAP_OFF
////                    #ifndef DIRLIGHTMAP_OFF
////                        specular *= lightmap;
////                    #else
////                        specular *= (floor(attenuation) * _LightColor0.xyz);
////                    #endif
////                #else
////                    specular *= (floor(attenuation) * _LightColor0.xyz);
////                #endif
//                specular *= ((attenuation) * _LightColor0.xyz);
//                float3 finalColor = 0;
//                float3 diffuseLight = diffuse;
//                float node_331 = 1.0;
//             //   finalColor += diffuseLight * (lerp(float3(node_331,node_331,node_331),float3(0.9632353,0.8224623,0.03541304),i.vertexColor.b)*node_1.rgb);
//                finalColor += diffuseLight * (node_1.rgb)*_ColorGlobal; //v1.4
//                
//                specular = specular * (i.uv0.y*2-0.5) ;
//                finalColor = lerp(finalColor, finalColor*_Color,_TintPower*i.uv0.y*(0.9+0.6*cos(i.posWorld.x*2*_TintFrequency)+0.6*sin(i.posWorld.z*3*_TintFrequency)+0.6*sin(i.posWorld.z*1*_TintFrequency+0.1)));
//                
//                finalColor += specular * _SpecularPower;
///// Final Color:
//
//				//SNOW
//                float3 col = finalColor;
//                
//                float4 SnowTexColor = tex2D(_SnowTexture,  i.uv0);
//				
//				//if(i.uv0.y >= 1-(3 * (_SnowCoverage+_TimeControl1.y-1)) * col.r* col.r)
//				//if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) * col.r* col.r* col.r+0.01) //v1.7.6
//				if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) *clamp(col.r,0.85*(col.r+0.2),1)* clamp(col.r,0.35,1)* 1+0.01) //v2.0.7
//	            {     	  
//	            	if(i.uv0.y < 0.99 ){    //v1.7.6
//		                //col =  lerp (  col , SnowTexColor*0.9,1-(0.5 * _SnowCoverage)) ;   
//		                //    col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;
//		                //o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*1);   
//		                //col.rgb = float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z; 
//		                //v1.7.6
//		               // col.rgb = (float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z)*(1+finalColor)*1.6;  
//		               col.rgb = (float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z)*(1+finalColor)*clamp(col.r,0.85*(col.r+0.2),1);   //v2.0.7
//	                }                      
//	            }
//	            else
//	            {
//					//col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;						
//				//	col.rgb *= input.color.rgb;
//				//	clip(col.a);
//				//	col=col* _UnityTerrainTreeTintColorSM;
//				}
//                
//                //END SNOW
//
//                return fixed4(col,1);
//            }
//            ENDCG
//        }
//        Pass {
//            Name "ForwardAdd"
//            Tags {
//                "LightMode"="ForwardAdd"
//            }
//            Blend One One
//            Cull Off
//            
//            
//            Fog { Color (0,0,0,0) }
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            //#define UNITY_PASS_FORWARDADD
//            #include "UnityCG.cginc"
//            #include "AutoLight.cginc"
//            #include "Lighting.cginc"
//            #pragma multi_compile_fwdadd_fullshadows
//			#pragma multi_compile_fwdbase nolightmap //v4.1
//           // #pragma exclude_renderers gles xbox360 ps3 flash 
//           #pragma multi_compile_instancing //v1.7.8
//            #pragma target 3.0
//            uniform float4 _TimeEditor;
//            #ifndef LIGHTMAP_OFF
//                // float4 unity_LightmapST;
//                // sampler2D unity_Lightmap;
//                #ifndef DIRLIGHTMAP_OFF
//                    // sampler2D unity_LightmapInd;
//                #endif
//            #endif
//            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
//            uniform sampler2D _Normal; uniform float4 _Normal_ST;
//            
//               uniform float _BulgeScale; 
//            uniform float _BulgeShape;
//            uniform float _BulgeScale_copy;
//            float4 _WaveControl1;
//   			float4 _TimeControl1;
//    		float3 _OceanCenter;
//            uniform fixed _Cutoff;
//             uniform float _RandYScale;
//            uniform float _RippleScale;
//            
//            float3 _InteractPos;
//            float _FadeThreshold;
//            float _StopMotionThreshold;
//            float _SmoothMotionFactor;
//            float _WaveXFactor;
//           	float _WaveYFactor;
//
//
//           	//v2.0.9
//           	 float3 _Color;
//            float3 _ColorGlobal;
//           	float _TintPower; 
//           	float _SpecularPower;
//           
//           	
//           	sampler2D _SnowTexture;
//           	float _SnowCoverage; 	
//           	float _TintFrequency;
//
//           	 float3 _InteractSpeed;
//           	 float _InteractMaxYoffset;
//
//
//           	
//            
//            struct VertexInput {
//                float4 vertex : POSITION;
//                float3 normal : NORMAL;
//                float4 tangent : TANGENT;
//                float2 texcoord0 : TEXCOORD0;
//                float2 texcoord1 : TEXCOORD1;
//                float4 vertexColor : COLOR;
//                UNITY_VERTEX_INPUT_INSTANCE_ID //v1.7.8
//            };
//            struct VertexOutput {
//                float4 pos : SV_POSITION;
//                float2 uv0 : TEXCOORD0;
//                float4 posWorld : TEXCOORD1;
//                float3 normalDir : TEXCOORD2;
//                float3 tangentDir : TEXCOORD3;
//                float3 binormalDir : TEXCOORD4;
//                float4 vertexColor : COLOR;
//                LIGHTING_COORDS(5,6)
//               // UNITY_SHADOW_COORDS(7)
//            };
//            VertexOutput vert (VertexInput v) {
//                VertexOutput o;
//                UNITY_SETUP_INSTANCE_ID(v); //v1.7.8
//                o.uv0 = v.texcoord0;
//                o.vertexColor = v.vertexColor;
//                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
//                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
//                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
//                float4 node_389 = o.vertexColor;
//                float4 node_392 = _Time + _TimeEditor;
//            //    v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
//                
//                float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
//                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
//                
//                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
//                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
//                               
//                                 //INIFNIGRASS
//                       float4 modelY = float4(0.0,1.0,0.0,0.0);
//                               float4 ModelYWorld =mul(unity_ObjectToWorld,modelY);
//                               float scaleY = length(ModelYWorld);     
//                               
//                  o.posWorld = mul(unity_ObjectToWorld, v.vertex);
//             //  o.posWorld =  v.vertex;
//                                      
////                if( distance(_InteractPos,o.posWorld) <  _StopMotionThreshold){                 
////                	_BulgeScale = 0;
////                	_BulgeScale_copy = 0;
////                }
//
//float3 SpeedFac = float3(0,0,0);  //	SpeedFac =  _InteractSpeed;  
//                float distA =  distance(_InteractPos,o.posWorld)/ (_StopMotionThreshold*1);      
//                  if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold*1){ 
//               // if( distance(_InteractPos.x,o.posWorld.x) < _StopMotionThreshold/2 ){    
//               //      if( distance(_InteractPos.z,o.posWorld.z) < _StopMotionThreshold/2){                  
//                	//_BulgeScale = 0;
//                	//_BulgeScale_copy = 0;                	
//                	SpeedFac =  _InteractSpeed * _WaveControl1.w;
////                		if( o.uv0.y > 0.2){
////							o.posWorld.x += (_InteractSpeed.x*22+0.1)*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
////							o.posWorld.z += (_InteractSpeed.z+0.1)*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
////						}
//                	if( o.uv0.y < 0.3){
//                	//_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
//                	//_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
//                	}
//                	if( o.uv0.y > 0.19){
//						_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z*1;
//                		_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x*1;
//                	}
//                	if( o.uv0.y > 0.5){
//						o.posWorld.y = o.posWorld.y - (1-distA)*_InteractMaxYoffset*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ;//+_BulgeScale*0.5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) ;
//					}             
//               }
//                               
//                if( o.uv0.y > 0.1){
//            //    	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) / scaleY;
//				}
//                if( o.uv0.y >= 0.01){
//           //     	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
//                }
//                
//                  //v1.5
//	           _BulgeScale= _BulgeScale* _BulgeScale_copy;
//	           //   _OceanCenter.x = 0.0;
//	          // _OceanCenter.z = 0.0;
//
//                  dist = 90* (cos(_BulgeShape+_Time.y/15))-_SmoothMotionFactor;
//				///////////////////////// 
//				if( o.uv0.y > 0.1){
//					o.posWorld.x += _BulgeScale*1*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
//					o.posWorld.z += _BulgeScale*1*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
//				}
//				if( o.uv0.y > 0.2){					
//					o.posWorld.x += _BulgeScale*2*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/3);
//					o.posWorld.z += _BulgeScale*2*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);	
//				}
//				if( o.uv0.y > 0.3){
//					
//					o.posWorld.x += _BulgeScale*3*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/4);
//					o.posWorld.z += _BulgeScale*3*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);
//				}
//				if( o.uv0.y > 0.4){
//					
//					o.posWorld.x += _BulgeScale*4*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/2) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/2);
//					o.posWorld.z += _BulgeScale*4*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/2) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/2);	
//				}		
//				if( o.uv0.y > 0.96){
//					
//					o.posWorld.x += _BulgeScale*5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/0.9)	+ _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/1);
//					o.posWorld.z += _BulgeScale*5*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/0.9) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/1);
//				}				
//				//ADD GLOBAL ROTATION - WIND						
//			v.vertex = mul(unity_WorldToObject, o.posWorld);	            
//	          //  v.vertex =  o.posWorld;
//                
//                
//                //o.posWorld = mul(_Object2World, v.vertex);
//                o.pos = UnityObjectToClipPos(v.vertex);
//                //UNITY_TRANSFER_SHADOW(o,v.uv0);
//                TRANSFER_VERTEX_TO_FRAGMENT(o)
//                return o;
//            }
//            
//            fixed4 frag(VertexOutput i) : COLOR {
//            
////                i.normalDir = normalize(i.normalDir);
////                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
////                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////////// Normals:
////                float2 node_583 = i.uv0;
////                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_583.rg, _Normal))).rgb;
////                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
////                
////                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
////                i.normalDir *= nSign;
////                normalDirection *= nSign;
////                
////                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_583.rg, _Diffuse));
////                clip(node_1.a - _Cutoff);
////                
////                 //DEFINE FADE BASED ON CAMERA - INFINIGRASS
////				float Aplha = 1;
////				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
////					 clip(-1);
////				}
////                
////                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////                float3 halfDirection = normalize(viewDirection+lightDirection);
////////// Lighting:
////                float attenuation = LIGHT_ATTENUATION(i);
////                
////                //INFINIGRASS
////                 float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);
////                  float node_133 = pow((abs((frac((i.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
////                
////      //          float3 attenColor = attenuation * (_LightColor0.xyz*(node_133+1)  + _LightColor0.xyz*dot(viewDirection,lightDirection)/1);
////                
////                float3 attenColor = attenuation * _LightColor0.xyz ;
/////////// Diffuse:
////                float NdotL = dot( normalDirection, lightDirection );
////                float3 w = float3(0.9,0.9,0.8)*0.5; // Light wrapping
////                float3 NdotLWrap = NdotL * ( 1.0 - w );
////                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
////                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) ;//* float3(0.9,1,0.5); //v2.0.9
////                float3 diffuse = (forwardLight+backLight) * attenColor;
///////////// Gloss:
////                float gloss = 0.4;
////                float specPow = exp2( gloss * 10.0+1.0);
////////// Specular:
////                NdotL = max(0.0, NdotL);
////                float node_3 = 0.2;
////                float3 specularColor = float3(node_3,node_3,node_3)*node_1;//v2.0.9
////                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
////                float3 finalColor = 0;
////                float3 diffuseLight = diffuse;
////                float node_331 = 1.0;
////                finalColor += float3(0,0,0);//diffuseLight * (lerp(float3(node_331,node_331,node_331),float3(0.9632353,0.8224623,0.03541304),i.vertexColor.b)*node_1.rgb);
////                
////                
////                //INFINIGRASS
////                //if( i.uv0.y > 0.6){
//////                if(dot(viewDirection,lightDirection) > 0.9){
//////                	finalColor += (i.uv0.y)/6 ;
//////                }
////                //}
////                
////                
////                finalColor += specular ;
/////// Final Color:
////
////				
////
////                return fixed4(finalColor * 1,0);
//
//
//i.normalDir = normalize(i.normalDir);
//                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
//                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
///////// Normals:
//                float2 node_582 = i.uv0;
//                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_582.rg, _Normal))).rgb;
//                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
//                
//                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
//                i.normalDir *= nSign;
//                normalDirection *= nSign;
//                
//                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_582.rg, _Diffuse));
//                
//                
//                
//                clip(node_1.a - _Cutoff);
//                
//                //DEFINE FADE BASED ON CAMERA - INFINIGRASS
//				float Aplha = 1;
//				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
//					 clip(-1);
//				}
//                
////                #ifndef LIGHTMAP_OFF
////                    float4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uvLM);
////                    #ifndef DIRLIGHTMAP_OFF
////                        float3 lightmap = DecodeLightmap(lmtex);
////                        float3 scalePerBasisVector = DecodeLightmap(UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap,i.uvLM));
////                        UNITY_DIRBASIS
////                        half3 normalInRnmBasis = saturate (mul (unity_DirBasis, normalLocal));
////                        lightmap *= dot (normalInRnmBasis, scalePerBasisVector);
////                    #else
////                        float3 lightmap = DecodeLightmap(lmtex);
////                    #endif
////                #endif
////                #ifndef LIGHTMAP_OFF
////                    #ifdef DIRLIGHTMAP_OFF
////                        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////                    #else
////                        float3 lightDirection = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
////                        lightDirection = mul(lightDirection,tangentTransform); // Tangent to world
////                    #endif
////                #else
//                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
//               // #endif
//                
//                
//                lightDirection =  normalize(reflect(_WorldSpaceLightPos0.xyz,normalDirection));
//                
//                
//                float3 halfDirection = normalize(viewDirection+lightDirection);
//////// Lighting:
//                //float attenuation = 0;// LIGHT_ATTENUATION(i);
//                UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWorld.xyz);
//                
//                
//                   //INFINIGRASS
//                 float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);
//                 float node_133 = pow((abs((frac((i.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
//                   float dist = distance(_OceanCenter, float3(_WaveControl1.x*i.posWorld.y,_WaveControl1.y*i.posWorld.x,_WaveControl1.z*i.posWorld.z) );
//               
//               float3 attenColor = attenuation * (_LightColor0.xyz);
//               
//               
//               
///////// Diffuse:
//                float NdotL = dot( normalDirection, lightDirection );
//                float3 w = float3(0.9,0.9,0.8)*0.5; // Light wrapping
//                float3 NdotLWrap = NdotL * ( 1.0 - w );
//                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
//                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) ;//* float3(0.9,1,0.5); //v1.4
//
//
//                //v2.0.9
////                #ifndef LIGHTMAP_OFF
////                    float3 diffuse = lightmap.rgb;
////                #else
////                    float3 diffuse = (forwardLight+backLight) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
////                #endif
//
//
//                //v2.0.9
//                float3 diffuse = (forwardLight+backLight) * attenColor  + UNITY_LIGHTMODEL_AMBIENT.rgb;
//
/////////// Gloss:
//                float gloss = 0.4;
//                float specPow = exp2( gloss * 10.0+1.0);
//////// Specular:
//                NdotL = max(0.0, NdotL);
//                float node_3 = 0.2;
//                float3 specularColor = float3(node_3,node_3,node_3)*node_1;
//                float3 specular = 3 * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
////                #ifndef LIGHTMAP_OFF
////                    #ifndef DIRLIGHTMAP_OFF
////                        specular *= lightmap;
////                    #else
////                        specular *= (floor(attenuation) * _LightColor0.xyz);
////                    #endif
////                #else
////                    specular *= (floor(attenuation) * _LightColor0.xyz);
////                #endif
//                specular *= ((attenuation) * _LightColor0.xyz);
//                float3 finalColor = 0;
//                float3 diffuseLight = diffuse;
//                float node_331 = 1.0;
//             //   finalColor += diffuseLight * (lerp(float3(node_331,node_331,node_331),float3(0.9632353,0.8224623,0.03541304),i.vertexColor.b)*node_1.rgb);
//				finalColor += diffuseLight * (node_1.rgb)*_ColorGlobal * 5 * attenuation; //v1.9.6 finalColor += diffuseLight * (node_1.rgb)*_ColorGlobal; //v1.4
//                
//                specular = specular * (i.uv0.y*2-0.5) ;
//                finalColor = lerp(finalColor, finalColor*_Color,_TintPower*i.uv0.y*(0.9+0.6*cos(i.posWorld.x*2*_TintFrequency)+0.6*sin(i.posWorld.z*3*_TintFrequency)+0.6*sin(i.posWorld.z*1*_TintFrequency+0.1)));
//                
//                finalColor += specular * _SpecularPower;
///// Final Color:
//
//				//SNOW
//                float3 col = finalColor;
//                
//                float4 SnowTexColor = tex2D(_SnowTexture,  i.uv0);
//				
//				//if(i.uv0.y >= 1-(3 * (_SnowCoverage+_TimeControl1.y-1)) * col.r* col.r)
//				//if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) * col.r* col.r* col.r+0.01) //v1.7.6
//				if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) *clamp(col.r,0.85*(col.r+0.2),1)* clamp(col.r,0.35,1)* 1+0.01 ) //v2.0.7 
//				//if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) *clamp(col.r,0.85*(col.r+0.2),1)* clamp(col.r,0.35,1)* 1+0.01 +  i.uv0.y*2+0.6  ) //v2.0.8
//				//if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)+  i.uv0.y*0.3 + 0.03 ) *clamp(col.r,0.85*(col.r+0.2),1)* clamp(col.r,0.35,1)* 1+0.01 +  i.uv0.y*0.3 + 0.03  ) //v2.0.8
//	            {     	  
//	            	if(i.uv0.y < 0.99 ){    //v1.7.6
//	            		//if(i.uv0.y >=  1-(4 * (_SnowCoverage+_TimeControl1.y-1))*clamp(col.r,0.85*(col.r+0.2),1)* clamp(col.r,0.35,1)* 1+0.01 ){
//			                //col =  lerp (  col , SnowTexColor*0.9,1-(0.5 * _SnowCoverage)) ;   
//			                //    col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;
//			                //o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*1);   
//			                //col.rgb = float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z; 
//			                //v1.7.6
//			               // col.rgb = (float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z)*(1+finalColor)*1.6;  
//			               col.rgb = (float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z)*(1+finalColor)*clamp(col.r,0.85*(col.r+0.2),1);   //v2.0.7
//			              // col.rgb = float3(1,1,1)*2*(_TimeControl1.z+i.uv0.y*1)*clamp(col.r,0.35*(col.r+0.2),1);   //v2.0.8
//		               //}
//	                }                      
//	            }
//	            else
//	            {
//					//col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;						
//				//	col.rgb *= input.color.rgb;
//				//	clip(col.a);
//				//	col=col* _UnityTerrainTreeTintColorSM;
//				}
//                
//                //END SNOW
//
//                return fixed4(col,1);
//
//
//            }
//            ENDCG
//        }
////        Pass {
////            Name "ShadowCollector"
////            Tags {
////                "LightMode"="ShadowCollector"
////            }
////            Cull Off
////            
////            Fog {Mode Off}
////            CGPROGRAM
////            #pragma vertex vert
////            #pragma fragment frag
////            #define UNITY_PASS_SHADOWCOLLECTOR
////            #define SHADOW_COLLECTOR_PASS
////            #include "UnityCG.cginc"
////            #include "Lighting.cginc"
////            #pragma fragmentoption ARB_precision_hint_fastest
////            #pragma multi_compile_shadowcollector
////            #pragma exclude_renderers gles xbox360 ps3 flash 
////            #pragma target 3.0
////            uniform float4 _TimeEditor;
////            #ifndef LIGHTMAP_OFF
////                // float4 unity_LightmapST;
////                // sampler2D unity_Lightmap;
////                #ifndef DIRLIGHTMAP_OFF
////                    // sampler2D unity_LightmapInd;
////                #endif
////            #endif
////            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
////            
////            uniform float _BulgeScale; 
////            uniform float _BulgeShape;
////            uniform float _BulgeScale_copy;
////            float4 _WaveControl1;
////   			float4 _TimeControl1;
////    		float3 _OceanCenter;
////    		uniform fixed _Cutoff;
////    		 uniform float _RandYScale;
////            uniform float _RippleScale;
////            
////            float3 _InteractPos;
////            float _FadeThreshold;
////            
////            struct VertexInput {
////                float4 vertex : POSITION;
////                float3 normal : NORMAL;
////                float2 texcoord0 : TEXCOORD0;
////                float4 vertexColor : COLOR;
////            };
////            struct VertexOutput {
////                V2F_SHADOW_COLLECTOR;
////                float2 uv0 : TEXCOORD5;
////                float3 normalDir : TEXCOORD6;
////                float4 vertexColor : COLOR;
////            };
////            VertexOutput vert (VertexInput v) {
////                VertexOutput o;
////                o.uv0 = v.texcoord0;
////                o.vertexColor = v.vertexColor;
////                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
////                float4 node_389 = o.vertexColor;
////                float4 node_392 = _Time + _TimeEditor;
////            //    v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
////                
////                  float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(_Object2World, v.vertex).y,_WaveControl1.y*mul(_Object2World, v.vertex).x,_WaveControl1.z*mul(_Object2World, v.vertex).z) );
////                float dist2 = distance(_OceanCenter, float3(mul(_Object2World, v.vertex).y,mul(_Object2World, v.vertex).x*0.10,0.1*mul(_Object2World, v.vertex).z) );
////                
////                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
////                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
////                              
////                               //INIFNIGRASS
////                       float4 modelY = float4(0.0,1.0,0.0,0.0);
////                               float4 ModelYWorld =mul(_Object2World,modelY);
////                               float scaleY = length(ModelYWorld);
////                                 
////                if( o.uv0.y > 0.1){
////                	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) /scaleY;
////				}
////				if( o.uv0.y >= 0.01){
////                	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
////                }
////                
////                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
////                TRANSFER_SHADOW_COLLECTOR(o)
////                return o;
////            }
////            fixed4 frag(VertexOutput i) : COLOR {
////                i.normalDir = normalize(i.normalDir);
////                float2 node_584 = i.uv0;
////                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_584.rg, _Diffuse));
////                clip(node_1.a - _Cutoff);
////                
////                 //DEFINE FADE BASED ON CAMERA - INFINIGRASS
////				float Aplha = 1;
////				//if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
////				//	 clip(-1);
////				//}
////                
////                
////                SHADOW_COLLECTOR_FRAGMENT(i)
////            }
////            ENDCG
////        }
//        Pass {
//            Name "ShadowCaster"
//            Tags {
//                "LightMode"="ShadowCaster"
//            }
//            Cull Off
//            Offset 1, 1
//            
//            Fog {Mode Off}
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            //#define UNITY_PASS_SHADOWCASTER
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #pragma multi_compile_shadowcaster
//        	//    #pragma exclude_renderers gles xbox360 ps3 flash 
//			#pragma multi_compile_instancing //v1.7.8
//			#pragma multi_compile_fwdbase nolightmap //v4.1
//            #pragma target 3.0
//            uniform float4 _TimeEditor;
//            #ifndef LIGHTMAP_OFF
//                // float4 unity_LightmapST;
//                // sampler2D unity_Lightmap;
//                #ifndef DIRLIGHTMAP_OFF
//                    // sampler2D unity_LightmapInd;
//                #endif
//            #endif
//            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
//            
//            uniform float _BulgeScale; 
//            uniform float _BulgeShape;
//            uniform float _BulgeScale_copy;
//            float4 _WaveControl1;
//   			float4 _TimeControl1;
//    		float3 _OceanCenter;
//    		uniform fixed _Cutoff;
//    		 uniform float _RandYScale;
//            uniform float _RippleScale;
//            
//            float3 _InteractPos;
//            float _FadeThreshold;
//            float _StopMotionThreshold;
//            float _SmoothMotionFactor;
//            float _WaveXFactor;
//           	float _WaveYFactor;
//
//           	 float3 _InteractSpeed;
//           	 float _InteractMaxYoffset;
//            
//            struct VertexInput {
//                float4 vertex : POSITION;
//                float3 normal : NORMAL;
//                float2 texcoord0 : TEXCOORD0;
//                float4 vertexColor : COLOR;
//                UNITY_VERTEX_INPUT_INSTANCE_ID //v1.7.8
//            };
//            struct VertexOutput {
//                V2F_SHADOW_CASTER;
//                float2 uv0 : TEXCOORD1;
//                float3 normalDir : TEXCOORD2;
//                float4 vertexColor : COLOR;
//                float4 posWorld : TEXCOORD3;
//            };
//            VertexOutput vert (VertexInput v) {
//                VertexOutput o;
//                UNITY_SETUP_INSTANCE_ID(v); //v1.7.8
//                o.uv0 = v.texcoord0;
//                o.vertexColor = v.vertexColor;
//                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
//                float4 node_389 = o.vertexColor;
//                float4 node_392 = _Time + _TimeEditor;
//              //  v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
//                
//                  float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
//                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
//                
//                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
//                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
//                            
//                                //INIFNIGRASS
//                       float4 modelY = float4(0.0,1.0,0.0,0.0);
//                               float4 ModelYWorld =mul(unity_ObjectToWorld,modelY);
//                               float scaleY = length(ModelYWorld);
//                                  
//                    o.posWorld = mul(unity_ObjectToWorld, v.vertex);
//              // o.posWorld =  v.vertex;
//                                      
////                if( distance(_InteractPos,o.posWorld) <  _StopMotionThreshold){                 
////                	_BulgeScale = 0;
////                	_BulgeScale_copy = 0;
////                }   
//
//float3 SpeedFac = float3(0,0,0);  //	SpeedFac =  _InteractSpeed;  
//                float distA =  distance(_InteractPos,o.posWorld)/ (_StopMotionThreshold*1);      
//                  if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold*1){ 
//               // if( distance(_InteractPos.x,o.posWorld.x) < _StopMotionThreshold/2 ){    
//               //      if( distance(_InteractPos.z,o.posWorld.z) < _StopMotionThreshold/2){                  
//                	//_BulgeScale = 0;
//                	//_BulgeScale_copy = 0;                	
//                	SpeedFac =  _InteractSpeed * _WaveControl1.w;
////                		if( o.uv0.y > 0.2){
////							o.posWorld.x += (_InteractSpeed.x*22+0.1)*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
////							o.posWorld.z += (_InteractSpeed.z+0.1)*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
////						}
//                	if( o.uv0.y < 0.3){
//                	//_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
//                	//_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
//                	}
//                	if( o.uv0.y > 0.19){
//						_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z*1;
//                		_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x*1;
//                	}
//                	if( o.uv0.y > 0.5){
//						o.posWorld.y = o.posWorld.y - (1-distA)*_InteractMaxYoffset*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ;//+_BulgeScale*0.5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) ;
//					}             
//               }
//                                                                                         
//                if( o.uv0.y > 0.1){
//          //      	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) / scaleY;
//				}
//				if( o.uv0.y >= 0.01){
//          //      	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
//                }
//
//                  //v1.5
//	           _BulgeScale= _BulgeScale* _BulgeScale_copy;
//	         //  _OceanCenter.x = 0.0;
//	          // _OceanCenter.z = 0.0;
//                
//               dist = 90* (cos(_BulgeShape+_Time.y/15))-_SmoothMotionFactor;
//				///////////////////////// 
//				if( o.uv0.y > 0.1){
//					o.posWorld.x += _BulgeScale*1*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
//					o.posWorld.z += _BulgeScale*1*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
//				}
//				if( o.uv0.y > 0.2){					
//					o.posWorld.x += _BulgeScale*2*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/3);
//					o.posWorld.z += _BulgeScale*2*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);	
//				}
//				if( o.uv0.y > 0.3){
//					
//					o.posWorld.x += _BulgeScale*3*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/4);
//					o.posWorld.z += _BulgeScale*3*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);
//				}
//				if( o.uv0.y > 0.4){
//					
//					o.posWorld.x += _BulgeScale*4*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/2) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/2);
//					o.posWorld.z += _BulgeScale*4*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/2) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/2);	
//				}		
//				if( o.uv0.y > 0.96){
//					
//					o.posWorld.x += _BulgeScale*5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/0.9)	+ _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/1);
//					o.posWorld.z += _BulgeScale*5*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/0.9) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/1);
//				}					
//				//ADD GLOBAL ROTATION - WIND						
//				v.vertex = mul(unity_WorldToObject, o.posWorld);	            
//	            //v.vertex =  o.posWorld;
//                
//                
//                
//                
//                o.pos = UnityObjectToClipPos(v.vertex);
//               // o.posWorld = mul(_Object2World, v.vertex);
//                TRANSFER_SHADOW_CASTER(o)
//                return o;
//            }
//            fixed4 frag(VertexOutput i) : COLOR {
//                i.normalDir = normalize(i.normalDir);
//                float2 node_585 = i.uv0;
//                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_585.rg, _Diffuse));
//                clip(node_1.a - _Cutoff);
//                
//                 //DEFINE FADE BASED ON CAMERA - INFINIGRASS
//				float Aplha = 1;
//				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
//					 clip(-1);
//				}
//                
//                SHADOW_CASTER_FRAGMENT(i)
//            }
//            ENDCG
//        }
//    }
//    FallBack "Transparent/Cutout/Diffuse"
//   
//}