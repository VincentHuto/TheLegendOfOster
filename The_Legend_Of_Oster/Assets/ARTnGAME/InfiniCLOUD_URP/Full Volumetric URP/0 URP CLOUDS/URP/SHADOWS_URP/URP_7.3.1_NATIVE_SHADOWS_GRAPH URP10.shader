Shader "URP 7.3.1 NATIVE SHADOWS GRAPH URP10"
{
	Properties
	{
		cutoff("cutoff", Float) = 0.5
		[NoScaleOffset]mask("mask", 2D) = "white" {}
		TilingA("TilingA", Vector) = (1, 1, 0, 0)
		OffsetA("OffsetA", Vector) = (0, 0, 0, 0)
		[HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		//v0.1 NASOS URP CLOUD SHADOWS
		_CloudMap("_CloudMap", 2D) = "white" {}
		_CloudMap1("_CloudMap1", 2D) = "white" {}
		_PaintMap("_PaintMap", 2D) = "white" {}
		_Density("_Density", Float) = 0.1
		_Coverage("_Coverage", Float) = 0.1
		_CutHeight("_CutHeight", Float) = 0.1
		_CoverageOffset("_CoverageOffset", Float) = 0.1
		_Transparency("_Transparency", Float) = 0.1
		_TranspOffset("_TranspOffset", Float) = 0.1
		Thickness("Thickness", Float) = 0.1
		_Cutoff("_Cutoff", Float) = 0.1
		_Velocity1("_Velocity1", Vector) = (1, 1 ,0, 0)
		_Velocity2("_Velocity2", Vector) = (1, 1,0, 0)
		_EdgeFactors("_EdgeFactors", Vector) = (1, 1,0, 0)

			//CHOOSE MODE, 0 = 2D noise shader clouds, 1 = image effect 3D noise clouds
			_shadowMode("Shadow Mode (0,1)", Float) = 0

			//FULL VOLUME SHADOWS
			//v3.5 clouds
			_SampleCount0("Sample Count (min)", Float) = 30
			_SampleCount1("Sample Count (max)", Float) = 90
			_SampleCountL("Sample Count (light)", Int) = 16
			[Space]
			_NoiseTex1("Noise Volume", 3D) = ""{}
			_NoiseTex2("Noise Volume", 3D) = ""{}
			_NoiseFreq1("Frequency 1", Float) = 3.1
			_NoiseFreq2("Frequency 2", Float) = 35.1
			_NoiseAmp1("Amplitude 1", Float) = 5
			_NoiseAmp2("Amplitude 2", Float) = 1
			_NoiseBias("Bias", Float) = -0.2
			[Space]
			_Scroll1("Scroll Speed 1", Vector) = (0.01, 0.08, 0.06, 0)
			_Scroll2("Scroll Speed 2", Vector) = (0.01, 0.05, 0.03, 0)
			timeDelay("timeDelay", Float) = 0
			[Space]
			_Altitude0("Altitude (bottom)", Float) = 1500
			_Altitude1("Altitude (top)", Float) = 3500
			_FarDist("Far Distance", Float) = 30000
				//v3.5 clouds
				_BackShade("Back shade of cloud top", Float) = 1
				_UndersideCurveFactor("Underside Curve Factor", Float) = 0
				//v3.5.3
				_InteractTexture("_Interact Texture", 2D) = "white" {}
				_InteractTexturePos("Interact Texture Pos", Vector) = (1 ,1, 0, 0)
				_InteractTextureAtr("Interact Texture Attributes - 2multi 2offset", Vector) = (1 ,1, 0, 0)
				_InteractTextureOffset("Interact Texture offsets", Vector) = (0 ,0, 0, 0) //v4.0
				//v3.5.1
				_NearZCutoff("Away from camera Cutoff", Float) = -2
				_HorizonYAdjust("Adjust horizon Height", Float) = 0
				_FadeThreshold("Fade Near", Float) = 0
					//v5.0c
					_LocalLightPos("Local Light Pos & Intensity", Vector) = (0 ,0, 0, 0) //local light position (x,y,z) and intensity (w)

					//FULL VOLUME CLOUDS
					_WeatherMap("_WeatherMap", 2D) = "white" {}
					_WeatherScale("_WeatherScale", Float) = 0
					_CoverageA("_CoverageA", Float) = 0
					_WindSpeed("_WindSpeed", Float) = 0
					_WindDirection("_WindDirection", Vector) = (0 ,0, 0, 0)
					_WindOffset("_WindOffset", Vector) = (0 ,0, 0, 0)
					_CoverageWindOffset("_CoverageWindOffset", Vector) = (0 ,0, 0, 0)
						_WeatherScale2("_WeatherScale2", Float) = 1
	}
		SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Transparent"
			"UniversalMaterialType" = "Lit"
			"Queue" = "Transparent"
		}
		Pass
		{
			Name "Universal Forward"
			Tags
			{
				"LightMode" = "UniversalForward"
			}

		// Render State
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
		ZTest LEqual
		ZWrite Off

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 4.5
		#pragma exclude_renderers gles gles3 glcore
		#pragma multi_compile_instancing
		#pragma multi_compile_fog
		#pragma multi_compile _ DOTS_INSTANCING_ON
		#pragma vertex vert
		#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
		#pragma multi_compile _ LIGHTMAP_ON
		#pragma multi_compile _ DIRLIGHTMAP_COMBINED
		#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
		#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
		#pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
		#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
		#pragma multi_compile _ _SHADOWS_SOFT
		#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
		#pragma multi_compile _ SHADOWS_SHADOWMASK
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define ATTRIBUTES_NEED_TEXCOORD1
		#define VARYINGS_NEED_POSITION_WS
		#define VARYINGS_NEED_NORMAL_WS
		#define VARYINGS_NEED_TANGENT_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define VARYINGS_NEED_VIEWDIRECTION_WS
		#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_FORWARD
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
		{
			float3 positionOS : POSITION;
			float3 normalOS : NORMAL;
			float4 tangentOS : TANGENT;
			float4 uv0 : TEXCOORD0;
			float4 uv1 : TEXCOORD1;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : INSTANCEID_SEMANTIC;
			#endif
		};
		struct Varyings
		{
			float4 positionCS : SV_POSITION;
			float3 positionWS;
			float3 normalWS;
			float4 tangentWS;
			float4 texCoord0;
			float3 viewDirectionWS;
			#if defined(LIGHTMAP_ON)
			float2 lightmapUV;
			#endif
			#if !defined(LIGHTMAP_ON)
			float3 sh;
			#endif
			float4 fogFactorAndVertexLight;
			float4 shadowCoord;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : CUSTOM_INSTANCE_ID;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
			#endif
		};
		struct SurfaceDescriptionInputs
		{
			float3 TangentSpaceNormal;
			float3 WorldSpacePosition;
			float4 uv0;
		};
		struct VertexDescriptionInputs
		{
			float3 ObjectSpaceNormal;
			float3 ObjectSpaceTangent;
			float3 ObjectSpacePosition;
		};
		struct PackedVaryings
		{
			float4 positionCS : SV_POSITION;
			float3 interp0 : TEXCOORD0;
			float3 interp1 : TEXCOORD1;
			float4 interp2 : TEXCOORD2;
			float4 interp3 : TEXCOORD3;
			float3 interp4 : TEXCOORD4;
			#if defined(LIGHTMAP_ON)
			float2 interp5 : TEXCOORD5;
			#endif
			#if !defined(LIGHTMAP_ON)
			float3 interp6 : TEXCOORD6;
			#endif
			float4 interp7 : TEXCOORD7;
			float4 interp8 : TEXCOORD8;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : CUSTOM_INSTANCE_ID;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
			#endif
		};

		PackedVaryings PackVaryings(Varyings input)
		{
			PackedVaryings output;
			output.positionCS = input.positionCS;
			output.interp0.xyz = input.positionWS;
			output.interp1.xyz = input.normalWS;
			output.interp2.xyzw = input.tangentWS;
			output.interp3.xyzw = input.texCoord0;
			output.interp4.xyz = input.viewDirectionWS;
			#if defined(LIGHTMAP_ON)
			output.interp5.xy = input.lightmapUV;
			#endif
			#if !defined(LIGHTMAP_ON)
			output.interp6.xyz = input.sh;
			#endif
			output.interp7.xyzw = input.fogFactorAndVertexLight;
			output.interp8.xyzw = input.shadowCoord;
			#if UNITY_ANY_INSTANCING_ENABLED
			output.instanceID = input.instanceID;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			output.cullFace = input.cullFace;
			#endif
			return output;
		}
		Varyings UnpackVaryings(PackedVaryings input)
		{
			Varyings output;
			output.positionCS = input.positionCS;
			output.positionWS = input.interp0.xyz;
			output.normalWS = input.interp1.xyz;
			output.tangentWS = input.interp2.xyzw;
			output.texCoord0 = input.interp3.xyzw;
			output.viewDirectionWS = input.interp4.xyz;
			#if defined(LIGHTMAP_ON)
			output.lightmapUV = input.interp5.xy;
			#endif
			#if !defined(LIGHTMAP_ON)
			output.sh = input.interp6.xyz;
			#endif
			output.fogFactorAndVertexLight = input.interp7.xyzw;
			output.shadowCoord = input.interp8.xyzw;
			#if UNITY_ANY_INSTANCING_ENABLED
			output.instanceID = input.instanceID;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			output.cullFace = input.cullFace;
			#endif
			return output;
		}

		// --------------------------------------------------
		// Graph

		// Graph Properties
		CBUFFER_START(UnityPerMaterial)
		float cutoff;
		float2 TilingA;
		float2 OffsetA;

		//v0.1 NASOS
		float _Density = 0.1;
		float _Coverage = 0.1;
		float _CutHeight = 0.1;
		float _CoverageOffset = 0.1;
		float _Transparency = 0.1;
		float _TranspOffset = 0.1;
		float Thickness = 0.1;
		float _Cutoff = 0.2;
		float2 _Velocity1 = float2(1.0, 1.0);
		float2 _Velocity2 = float2(1.0, 1.0);
		float2 _EdgeFactors = float2(1.0, 1.0);
		float _shadowMode;

		//////////// IMAGE EFFECT CLOUDS v0.2
		float _SampleCount0 = 2;
		float _SampleCount1 = 3;
		int _SampleCountL = 4;
		float _NoiseFreq1 = 3.1;
		float _NoiseFreq2 = 35.1;
		float _NoiseAmp1 = 5;
		float _NoiseAmp2 = 1;
		float _NoiseBias = -0.2;
		float3 _Scroll1 = float3 (0.01, 0.08, 0.06);
		float3 _Scroll2 = float3 (0.01, 0.05, 0.03);
		float timeDelay;
		float _Altitude0 = 1500;
		float _Altitude1 = 3500;
		float _FarDist = 30000;
		//v3.5.3
		//sampler2D _InteractTexture;
		float4 _InteractTexturePos;
		float4 _InteractTextureAtr;
		float4 _InteractTextureOffset; //v4.0
									   //v3.5.1
		float _NearZCutoff;
		float _HorizonYAdjust;
		float _FadeThreshold;
		float _BackShade;
		float _UndersideCurveFactor;
		//v5.0c
		float4 _LocalLightPos;
		//////////// END IMAGE EFFECT CLOUDS v0.2

		//v0.4
		float globalTIME = 0;

		CBUFFER_END



		TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
		SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);






















		// Graph Functions

		void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
		{
			Out = UV * Tiling + Offset;
		}

		void Unity_Add_float3(float3 A, float3 B, out float3 Out)
		{
			Out = A + B;
		}

		// Graph Vertex
		struct VertexDescription
		{
			float3 Position;
			float3 Normal;
			float3 Tangent;
		};

		VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
		{
			VertexDescription description = (VertexDescription)0;
			description.Position = IN.ObjectSpacePosition;
			description.Normal = IN.ObjectSpaceNormal;
			description.Tangent = IN.ObjectSpaceTangent;
			return description;
		}

		// Graph Pixel
		struct SurfaceDescription
		{
			float3 BaseColor;
			float3 NormalTS;
			float3 Emission;
			float Metallic;
			float Smoothness;
			float Occlusion;
			float Alpha;
			float AlphaClipThreshold;
		};








		SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
		{
			SurfaceDescription surface = (SurfaceDescription)0;
			float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
			float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
			float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
			Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
			float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
			float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
			float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
			float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
			float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
			float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
			float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
			Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
			surface.BaseColor = (_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.xyz);
			surface.NormalTS = IN.TangentSpaceNormal;
			surface.Emission = IN.WorldSpacePosition;
			surface.Metallic = 0;
			surface.Smoothness = 0.5;
			surface.Occlusion = 1;
			surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
			surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
			return surface;
		}

		// --------------------------------------------------
		// Build Graph Inputs

		VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
		{
			VertexDescriptionInputs output;
			ZERO_INITIALIZE(VertexDescriptionInputs, output);

			output.ObjectSpaceNormal = input.normalOS;
			output.ObjectSpaceTangent = input.tangentOS;
			output.ObjectSpacePosition = input.positionOS;

			return output;
		}

		SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
		{
			SurfaceDescriptionInputs output;
			ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



			output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


			output.WorldSpacePosition = input.positionWS;
			output.uv0 = input.texCoord0;
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
		#else
		#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
		#endif
		#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

			return output;
		}


		// --------------------------------------------------
		// Main

		#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

		ENDHLSL
	}
	Pass
	{
		Name "GBuffer"
		Tags
		{
			"LightMode" = "UniversalGBuffer"
		}

			// Render State
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off

			// Debug
			// <None>

			// --------------------------------------------------
			// Pass

			HLSLPROGRAM

			// Pragmas
			#pragma target 4.5
			#pragma exclude_renderers gles gles3 glcore
			#pragma multi_compile_instancing
			#pragma multi_compile_fog
			#pragma multi_compile _ DOTS_INSTANCING_ON
			#pragma vertex vert
			#pragma fragment frag

			// DotsInstancingOptions: <None>
			// HybridV1InjectedBuiltinProperties: <None>

			// Keywords
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ _GBUFFER_NORMALS_OCT
			// GraphKeywords: <None>

			// Defines
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define _AlphaClip 1
			#define _NORMALMAP 1
			#define _NORMAL_DROPOFF_TS 1
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define ATTRIBUTES_NEED_TEXCOORD0
			#define ATTRIBUTES_NEED_TEXCOORD1
			#define VARYINGS_NEED_POSITION_WS
			#define VARYINGS_NEED_NORMAL_WS
			#define VARYINGS_NEED_TANGENT_WS
			#define VARYINGS_NEED_TEXCOORD0
			#define VARYINGS_NEED_VIEWDIRECTION_WS
			#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
			#define FEATURES_GRAPH_VERTEX
			/* WARNING: $splice Could not find named fragment 'PassInstancing' */
			#define SHADERPASS SHADERPASS_GBUFFER
			/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

			// Includes
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			// --------------------------------------------------
			// Structs and Packing

			struct Attributes
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};
			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS;
				float3 normalWS;
				float4 tangentWS;
				float4 texCoord0;
				float3 viewDirectionWS;
				#if defined(LIGHTMAP_ON)
				float2 lightmapUV;
				#endif
				#if !defined(LIGHTMAP_ON)
				float3 sh;
				#endif
				float4 fogFactorAndVertexLight;
				float4 shadowCoord;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};
			struct SurfaceDescriptionInputs
			{
				float3 TangentSpaceNormal;
				float3 WorldSpacePosition;
				float4 uv0;
			};
			struct VertexDescriptionInputs
			{
				float3 ObjectSpaceNormal;
				float3 ObjectSpaceTangent;
				float3 ObjectSpacePosition;
			};
			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float3 interp0 : TEXCOORD0;
				float3 interp1 : TEXCOORD1;
				float4 interp2 : TEXCOORD2;
				float4 interp3 : TEXCOORD3;
				float3 interp4 : TEXCOORD4;
				#if defined(LIGHTMAP_ON)
				float2 interp5 : TEXCOORD5;
				#endif
				#if !defined(LIGHTMAP_ON)
				float3 interp6 : TEXCOORD6;
				#endif
				float4 interp7 : TEXCOORD7;
				float4 interp8 : TEXCOORD8;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			PackedVaryings PackVaryings(Varyings input)
			{
				PackedVaryings output;
				output.positionCS = input.positionCS;
				output.interp0.xyz = input.positionWS;
				output.interp1.xyz = input.normalWS;
				output.interp2.xyzw = input.tangentWS;
				output.interp3.xyzw = input.texCoord0;
				output.interp4.xyz = input.viewDirectionWS;
				#if defined(LIGHTMAP_ON)
				output.interp5.xy = input.lightmapUV;
				#endif
				#if !defined(LIGHTMAP_ON)
				output.interp6.xyz = input.sh;
				#endif
				output.interp7.xyzw = input.fogFactorAndVertexLight;
				output.interp8.xyzw = input.shadowCoord;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				return output;
			}
			Varyings UnpackVaryings(PackedVaryings input)
			{
				Varyings output;
				output.positionCS = input.positionCS;
				output.positionWS = input.interp0.xyz;
				output.normalWS = input.interp1.xyz;
				output.tangentWS = input.interp2.xyzw;
				output.texCoord0 = input.interp3.xyzw;
				output.viewDirectionWS = input.interp4.xyz;
				#if defined(LIGHTMAP_ON)
				output.lightmapUV = input.interp5.xy;
				#endif
				#if !defined(LIGHTMAP_ON)
				output.sh = input.interp6.xyz;
				#endif
				output.fogFactorAndVertexLight = input.interp7.xyzw;
				output.shadowCoord = input.interp8.xyzw;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				return output;
			}

			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			float cutoff;
			float2 TilingA;
			float2 OffsetA;
			CBUFFER_END



			TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
			SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

			// Graph Functions

			void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
			{
				Out = UV * Tiling + Offset;
			}

			void Unity_Add_float3(float3 A, float3 B, out float3 Out)
			{
				Out = A + B;
			}

			// Graph Vertex
			struct VertexDescription
			{
				float3 Position;
				float3 Normal;
				float3 Tangent;
			};

			VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
			{
				VertexDescription description = (VertexDescription)0;
				description.Position = IN.ObjectSpacePosition;
				description.Normal = IN.ObjectSpaceNormal;
				description.Tangent = IN.ObjectSpaceTangent;
				return description;
			}

			// Graph Pixel
			struct SurfaceDescription
			{
				float3 BaseColor;
				float3 NormalTS;
				float3 Emission;
				float Metallic;
				float Smoothness;
				float Occlusion;
				float Alpha;
				float AlphaClipThreshold;
			};

			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
			{
				SurfaceDescription surface = (SurfaceDescription)0;
				float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
				float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
				float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
				Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
				float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
				float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
				float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
				float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
				float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
				float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
				float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
				Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
				surface.BaseColor = (_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.xyz);
				surface.NormalTS = IN.TangentSpaceNormal;
				surface.Emission = IN.WorldSpacePosition;
				surface.Metallic = 0;
				surface.Smoothness = 0.5;
				surface.Occlusion = 1;
				surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
				surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
				return surface;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);

				output.ObjectSpaceNormal = input.normalOS;
				output.ObjectSpaceTangent = input.tangentOS;
				output.ObjectSpacePosition = input.positionOS;

				return output;
			}

			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



				output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


				output.WorldSpacePosition = input.positionWS;
				output.uv0 = input.texCoord0;
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
			#else
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
			#endif
			#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

				return output;
			}


			// --------------------------------------------------
			// Main

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"

			ENDHLSL
		}
		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

				// Render State
				Cull Back
				Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
				ZTest LEqual
				ZWrite On
				ColorMask 0

				// Debug
				// <None>

				// --------------------------------------------------
				// Pass

				HLSLPROGRAM

				// Pragmas
				#pragma target 4.5
				#pragma exclude_renderers gles gles3 glcore
				#pragma multi_compile_instancing
				#pragma multi_compile _ DOTS_INSTANCING_ON
				#pragma vertex vert
				#pragma fragment frag

				// DotsInstancingOptions: <None>
				// HybridV1InjectedBuiltinProperties: <None>

				// Keywords
				// PassKeywords: <None>
				// GraphKeywords: <None>

				// Defines
				#define _SURFACE_TYPE_TRANSPARENT 1
				#define _AlphaClip 1
				#define _NORMALMAP 1
				#define _NORMAL_DROPOFF_TS 1
				#define ATTRIBUTES_NEED_NORMAL
				#define ATTRIBUTES_NEED_TANGENT
				#define ATTRIBUTES_NEED_TEXCOORD0
				#define VARYINGS_NEED_POSITION_WS
				#define VARYINGS_NEED_TEXCOORD0
				#define FEATURES_GRAPH_VERTEX
				/* WARNING: $splice Could not find named fragment 'PassInstancing' */
				#define SHADERPASS SHADERPASS_SHADOWCASTER
				/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

				// Includes
				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

				// --------------------------------------------------
				// Structs and Packing

				struct Attributes
				{
					float3 positionOS : POSITION;
					float3 normalOS : NORMAL;
					float4 tangentOS : TANGENT;
					float4 uv0 : TEXCOORD0;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : INSTANCEID_SEMANTIC;
					#endif
				};
				struct Varyings
				{
					float4 positionCS : SV_POSITION;
					float3 positionWS;
					float4 texCoord0;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				};
				struct SurfaceDescriptionInputs
				{
					float3 WorldSpacePosition;
					float4 uv0;
				};
				struct VertexDescriptionInputs
				{
					float3 ObjectSpaceNormal;
					float3 ObjectSpaceTangent;
					float3 ObjectSpacePosition;
				};
				struct PackedVaryings
				{
					float4 positionCS : SV_POSITION;
					float3 interp0 : TEXCOORD0;
					float4 interp1 : TEXCOORD1;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				};

				PackedVaryings PackVaryings(Varyings input)
				{
					PackedVaryings output;
					output.positionCS = input.positionCS;
					output.interp0.xyz = input.positionWS;
					output.interp1.xyzw = input.texCoord0;
					#if UNITY_ANY_INSTANCING_ENABLED
					output.instanceID = input.instanceID;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					output.cullFace = input.cullFace;
					#endif
					return output;
				}
				Varyings UnpackVaryings(PackedVaryings input)
				{
					Varyings output;
					output.positionCS = input.positionCS;
					output.positionWS = input.interp0.xyz;
					output.texCoord0 = input.interp1.xyzw;
					#if UNITY_ANY_INSTANCING_ENABLED
					output.instanceID = input.instanceID;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					output.cullFace = input.cullFace;
					#endif
					return output;
				}

				// --------------------------------------------------
				// Graph

				// Graph Properties
				CBUFFER_START(UnityPerMaterial)
				float cutoff;
				float2 TilingA;
				float2 OffsetA;

				//v0.1 NASOS
				float _Density = 0.1;
				float _Coverage = 0.1;
				float _CutHeight = 0.1;
				float _CoverageOffset = 0.1;
				float _Transparency = 0.1;
				float _TranspOffset = 0.1;
				float Thickness = 0.1;
				float _Cutoff = 0.2;
				float2 _Velocity1 = float2(1.0, 1.0);
				float2 _Velocity2 = float2(1.0, 1.0);
				float2 _EdgeFactors = float2(1.0, 1.0);
				float _shadowMode;

				//////////// IMAGE EFFECT CLOUDS v0.2
				float _SampleCount0 = 2;
				float _SampleCount1 = 3;
				int _SampleCountL = 4;
				float _NoiseFreq1 = 3.1;
				float _NoiseFreq2 = 35.1;
				float _NoiseAmp1 = 5;
				float _NoiseAmp2 = 1;
				float _NoiseBias = -0.2;
				float3 _Scroll1 = float3 (0.01, 0.08, 0.06);
				float3 _Scroll2 = float3 (0.01, 0.05, 0.03);
				float timeDelay;
				float _Altitude0 = 1500;
				float _Altitude1 = 3500;
				float _FarDist = 30000;
				//v3.5.3
				//sampler2D _InteractTexture;
				float4 _InteractTexturePos;
				float4 _InteractTextureAtr;
				float4 _InteractTextureOffset; //v4.0
											   //v3.5.1
				float _NearZCutoff;
				float _HorizonYAdjust;
				float _FadeThreshold;
				float _BackShade;
				float _UndersideCurveFactor;
				//v5.0c
				float4 _LocalLightPos;
				//////////// END IMAGE EFFECT CLOUDS v0.2

				//v0.4
				float globalTIME = 0;

				CBUFFER_END



				TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
				SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);





				//v0.1 NASOS
	//float _Density = 0.1;
	//float _Coverage = 0.1;
	//float _CutHeight = 0.1;
	//float _CoverageOffset = 0.1;
	//float _Transparency = 0.1;
	//float _TranspOffset = 0.1;
	//float Thickness = 0.1;
	//float _Cutoff = 0.2;
	//float2 _Velocity1 = float2(1.0, 1.0);
	//float2 _Velocity2 = float2(1.0, 1.0);
	//float2 _EdgeFactors = float2(1.0, 1.0);
				TEXTURE2D(_CloudMap); SAMPLER(sampler_CloudMap); float4 _CloudMap_TexelSize;
				TEXTURE2D(_CloudMap1); SAMPLER(sampler_CloudMap1); float4 _CloudMap1_TexelSize;
				TEXTURE2D(_PaintMap); SAMPLER(sampler_PaintMap); float4 _PaintMap_TexelSize;

				//FULL VOLUME CLOUDS
				TEXTURE2D(_WeatherMap); SAMPLER(sampler_WeatherMap); float4 _WeatherMap_TexelSize; //_WeatherMap

				//////////// IMAGE EFFECT CLOUDS v0.2
				//sampler3D _NoiseTex1;
				//sampler3D _NoiseTex2;
				//sampler2D _InteractTexture;
				TEXTURE3D(_NoiseTex1); SAMPLER(sampler_NoiseTex1); float4 _NoiseTex1_TexelSize;
				TEXTURE3D(_NoiseTex2); SAMPLER(sampler_NoiseTex2); float4 _NoiseTex2_TexelSize;
				TEXTURE2D(_InteractTexture); SAMPLER(sampler_InteractTexture); float4 _InteractTexture_TexelSize;
				//v3.5 clouds
				float UVRandom(float2 uv)
				{
					float f = dot(float2(12.9898, 78.233), uv);
					return frac(43758.5453 * sin(f));
				}
				float SampleNoise(float3 uvw, float _Altitude1, float _NoiseAmp1, float Alpha)//v3.5.3
				{
					//v0.4 - construct time
					float4 _TimeA = _Time;
					if (globalTIME > 0) {
						_TimeA = float4(globalTIME / 20, globalTIME, 2 * globalTIME, 3 * globalTIME);
					}

					float AlphaFactor = clamp(Alpha*_InteractTextureAtr.w, _InteractTextureAtr.x, 1);
					const float baseFreq = 1e-5;
					float4 uvw1 = float4(uvw * _NoiseFreq1 * baseFreq, 0);
					float4 uvw2 = float4(uvw * _NoiseFreq2 * baseFreq, 0);
					uvw1.xyz += _Scroll1.xyz * (_TimeA.x + timeDelay);
					uvw2.xyz += _Scroll2.xyz * (_TimeA.x + timeDelay);

					//float n1 = tex3Dlod(_NoiseTex1, uvw1).a;
					//float n2 = tex3Dlod(_NoiseTex2, uvw2).a;
					//v0.2
					float n1 = SAMPLE_TEXTURE3D(_NoiseTex1, sampler_NoiseTex1, uvw1).a;
					float n2 = SAMPLE_TEXTURE3D(_NoiseTex1, sampler_NoiseTex1, uvw2).a;

					float n = n1 * _NoiseAmp1*AlphaFactor + n2 * _NoiseAmp2;//v3.5.3
					n = saturate(n + _NoiseBias);
					float y = uvw.y - _Altitude0;
					float h = _Altitude1 * 1 - _Altitude0;//v3.5.3
					n *= smoothstep(0, h * (0.1 + _UndersideCurveFactor), y);
					n *= smoothstep(0, h * 0.4, h - y);
					return n;
				}
				float BeerPowder(float depth)
				{
					float _Extinct = 0.01;
					return exp(-_Extinct * depth) * (1 - exp(-_Extinct * 2 * depth));
				}
				float Beer(float depth)
				{
					float _Extinct = 0.01;
					return exp(-_Extinct * depth * _BackShade);  // return exp(-_Extinct * depth); //_BackShade v3.5
				}
				//////////// END IMAGE EFFECT CLOUDS v0.2






				// Graph Functions

				void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
				{
					Out = UV * Tiling + Offset;
				}

				void Unity_Add_float3(float3 A, float3 B, out float3 Out)
				{
					Out = A + B;
				}

				// Graph Vertex
				struct VertexDescription
				{
					float3 Position;
					float3 Normal;
					float3 Tangent;
				};

				VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
				{
					VertexDescription description = (VertexDescription)0;
					description.Position = IN.ObjectSpacePosition;
					description.Normal = IN.ObjectSpaceNormal;
					description.Tangent = IN.ObjectSpaceTangent;
					return description;
				}

				// Graph Pixel
				struct SurfaceDescription
				{
					float Alpha;
					float AlphaClipThreshold;
				};






				//FULLVOLUME CLOUDS
	//uniform float _Coverage;
				uniform float _WeatherScale;
				uniform float _WeatherScale2;
				uniform float _CoverageA;
				uniform float _WindSpeed;
				uniform float3 _WindDirection;
				uniform float3 _WindOffset;
				uniform float2 _CoverageWindOffset;
				// samples weather texture
				float3 sampleWeather(float3 pos) {

					//v0.4 - construct time
					float4 _TimeA = _Time;
					if (globalTIME > 0) {
						_TimeA = float4(globalTIME / 20, globalTIME, 2 * globalTIME, 3 * globalTIME);
					}

					//float3 weatherData = tex2Dlod(_WeatherMap, float4((pos.xz + _CoverageWindOffset) * _WeatherScale, 0, 0)).rgb;
					//float3 weatherData = tex2D(_WeatherMap, float2((pos.xz + _CoverageWindOffset) * _WeatherScale)).rgb;
					//float3 weatherData = SAMPLE_TEXTURE2D(_WeatherMap, sampler_WeatherMap, float2((pos.xz + _CoverageWindOffset) * _WeatherScale*0.1)).rgb;

					float3 weatherData = SAMPLE_TEXTURE2D(_WeatherMap, sampler_WeatherMap,
						float2((pos.xz + _CoverageWindOffset) * _WeatherScale*0.1) + float2(_WindDirection.x*_TimeA.y*_WindSpeed, _WindDirection.z*_TimeA.y*_WindSpeed)).rgb;
					weatherData.r = saturate(weatherData.r - _CoverageA);
					//	float3 weatherData = SAMPLE_TEXTURE2D(_WeatherMap, sampler_WeatherMap, float2((pos.xz + _CoverageWindOffset) * _WeatherScale* _WeatherScale2)).rgb;
					//	weatherData.r = saturate(weatherData.r - _CoverageA);

						//v0.2
						//float4 texInteract = tex2Dlod(_InteractTexture, 0.0003*float4(
						//	_InteractTexturePos.x*pos.x + _InteractTexturePos.z * _Time.x + _InteractTextureOffset.x,
						//	_InteractTexturePos.y*pos.z + _InteractTexturePos.w * _Time.x + _InteractTextureOffset.y,
						//	0, 0));

					float4 texInteract = SAMPLE_TEXTURE2D(_InteractTexture, sampler_InteractTexture, float2(
						_InteractTexturePos.x*pos.x + _InteractTexturePos.z * _TimeA.x + _InteractTextureOffset.x,
						_InteractTexturePos.y*pos.z + _InteractTexturePos.w * _TimeA.x + _InteractTextureOffset.y));
					float3 _LocalLightPos = float3(0, 0, 0);
					float diffPos = length(_LocalLightPos.xyz - pos);
					texInteract.a = texInteract.a + clamp(_InteractTextureAtr.z * 0.1*(1 - 0.00024*diffPos), -1.5, 0);
					weatherData = weatherData * clamp(texInteract.a*_InteractTextureAtr.w, _InteractTextureAtr.y, 1);

					//	float4 texInteract = SAMPLE_TEXTURE2D(_InteractTexture, sampler_InteractTexture, 0.0003*float2(
					//		_InteractTexturePos.x*pos.x + _InteractTexturePos.z * _TimeA.x + _InteractTextureOffset.x,
					//		_InteractTexturePos.y*pos.z + _InteractTexturePos.w * _TimeA.x + _InteractTextureOffset.y
					//		));
					//	float diffPos = length(_LocalLightPos.xyz - pos);
					//	texInteract.a = texInteract.a + clamp(_InteractTextureAtr.z * 0.1*(1 - 0.00024*diffPos), -1.5, 0);
					//	weatherData = weatherData * clamp(texInteract.a*_InteractTextureAtr.w, _InteractTextureAtr.y, 1);

					return weatherData;
				}









				SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
				{
					SurfaceDescription surface = (SurfaceDescription)0;
					float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
					float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
					float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
					Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
					float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
					float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
					float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
					float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
					float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
					float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
					float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
					Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
					
					
					
					//v0.4 - construct time
					float4 _TimeA = _Time;
					if (globalTIME > 0) {
						_TimeA = float4(globalTIME / 20, globalTIME, 2 * globalTIME, 3 * globalTIME);
					}
					////// NASOS URP SHADOWS
					if (_shadowMode == 0) {
						float3 woeldPos = IN.WorldSpacePosition;// +_WorldSpaceCameraPos; // MAKE CAMERA RELATIVE !!! for HDRP
						float change_h = _CutHeight;
						float PosDiff = 0.0006*(woeldPos.y - change_h);
						float2 UVs = _Density * float2(woeldPos.x, woeldPos.z);
						float4 TimingF = 0.0012;
						float2 UVs1 = _Velocity1 * TimingF*_TimeA.y + UVs;
						float4 cloudTexture = SAMPLE_TEXTURE2D(_CloudMap, sampler_CloudMap, UVs1);
						float4 cloudTexture1 = SAMPLE_TEXTURE2D(_CloudMap1, sampler_CloudMap1, UVs1);
						float4 paintTexture1 = SAMPLE_TEXTURE2D(_PaintMap, sampler_PaintMap, UVs1);

						float2 UVs2 = (_Velocity2*TimingF*_TimeA.y / 1.4 + float2(_EdgeFactors.x, _EdgeFactors.y) + UVs);
						float4 Texture1 = SAMPLE_TEXTURE2D(_CloudMap, sampler_CloudMap, UVs2);
						float4 Texture2 = SAMPLE_TEXTURE2D(_CloudMap1, sampler_CloudMap1, UVs2);

						float DER = woeldPos.y*0.001;
						float3 normalA = (((DER*((_Coverage + _CoverageOffset) + ((cloudTexture.rgb * 2) - 1))) - (1 - (Texture1.rgb * 2))));
						float3 normalN = normalize(normalA);

						float DER1 = -(woeldPos.y - 0) * 1;
						float PosTDiff = woeldPos.y * 1;
						if (woeldPos.y > change_h) {
							DER1 = (1 - cloudTexture1.a) *  PosTDiff;
						}
						float shaper = ((_Transparency + _TranspOffset) - 0.48 + 0.0)*((DER1*(((_Coverage + _CoverageOffset) + cloudTexture1.a)))*Texture2.a);
						change_h = 10;
						PosDiff = Thickness * 0.0006*(woeldPos.y - change_h);
						PosTDiff = woeldPos.y*PosDiff;
						DER1 = -(woeldPos.y + 0)*PosDiff;
						if (woeldPos.y > change_h) {
							DER1 = (1 - cloudTexture1.a) *  PosTDiff;
						}
						if (woeldPos.y > 150) { 																																///////////////////////////// 650
							DER1 = (1 - cloudTexture1.a) *  PosTDiff*0.07;
							shaper = shaper * shaper;
						}
						surface.AlphaClipThreshold = 1 - (((shaper)*paintTexture1.r) - _Cutoff + 0.4);
						surface.Alpha = 0.5;
					}
					///// END NASOS URP SHADOWS


					if (_shadowMode == 1) {
						//FULL VOLUME CLOUD SHADOWS
						float3 pos = IN.WorldSpacePosition;// i.worldPos;
						int samples = 2;
						float depth = 0;

						float3 PixelWorld = pos + _WorldSpaceCameraPos + float3(0, _HorizonYAdjust, 0);
						float3 ray = PixelWorld;

						float dist0 = _Altitude0 / ray.y;
						float dist1 = _Altitude1 / ray.y;
						float stride = (dist1 - dist0) / samples;

						float2 uv = IN.uv0 + _TimeA.x;
						float offs = UVRandom(uv) * (dist1 - dist0) / samples;

						float3 acc = 0;

						//v4.0			
						//float4 texInteract = tex2Dlod(_InteractTexture, 0.0003*float4(
						//	_InteractTexturePos.x*pos.x + _InteractTexturePos.z*-_Scroll1.x * (_Time.x + timeDelay) + _InteractTextureOffset.x,
						//	_InteractTexturePos.y*pos.z + _InteractTexturePos.w*-_Scroll1.z * (_Time.x + timeDelay) + _InteractTextureOffset.y,
						//	0, 0));
						//SAMPLE_TEXTURE3D(_NoiseTex1, sampler_NoiseTex1, uvw1)
						float4 texInteract = SAMPLE_TEXTURE2D(_InteractTexture, sampler_InteractTexture, 0.0003*float4(
							_InteractTexturePos.x*pos.x + _InteractTexturePos.z*-_Scroll1.x * (_TimeA.x + timeDelay) + _InteractTextureOffset.x,
							_InteractTexturePos.y*pos.z + _InteractTexturePos.w*-_Scroll1.z * (_TimeA.x + timeDelay) + _InteractTextureOffset.y,
							0, 0));

						timeDelay = 21110;//offset time 

						UNITY_LOOP for (int s = 0; s < samples; s++)
						{
							//v5.0c - add local light
							float diffPos = length(_LocalLightPos.xyz - pos);
							texInteract.a = texInteract.a + clamp(_InteractTextureAtr.z * 0.1*(1 - 0.00024*diffPos), -1112.5, 10);
							_NoiseAmp2 = _NoiseAmp2 * clamp(texInteract.a*_InteractTextureAtr.w, _InteractTextureAtr.y, 1);

							float n = SampleNoise(pos, _Altitude1, _NoiseAmp1, texInteract.a);
							if (n > 0)
							{
								float density = n * stride;
								float rand = UVRandom(uv + s + 1);
								float scatter = 0.1;
								//acc += _LightColor0 * scatter* BeerPowder(depth) + BeerPowder(depth) * scatter;//v2.1.19
								acc += 1 * scatter* BeerPowder(depth) + BeerPowder(depth) * scatter;//v2.1.19
								depth += density;
							}
							pos += ray * stride;
						}
						acc += Beer(depth) * 1 + 1 * 1 * acc;
						acc = lerp(acc, 1 * 0.96, saturate(((dist0) / (_FarDist*0.5))) + 0.03);
						float4 finalColor = float4(acc, 1);

						//v4.0
						pos = IN.WorldSpacePosition;
						float n2 = SampleNoise(pos, _Altitude1, _NoiseAmp1, (texInteract.a)*0.0001*(1 - _InteractTextureAtr.x + 0.6));
						float n22 = SampleNoise(pos + float3(0, 2000, 0), _Altitude1 + 1050, _NoiseAmp1, 1);
						float n33 = SampleNoise(pos + float3(0, 0, 0), _Altitude1 + 0, _NoiseAmp1, 1);
						//clip(-((1111 + ((1 - texInteract.a) * 1) * 31111 * (1 - _InteractTextureAtr.x)) - (n22 + n22) * 18100) - _Cutoff + 0.4);//v5.0c

						surface.AlphaClipThreshold = 1 - (-((1111 + ((1 - texInteract.a) * 1) * 31111 * (1 - _InteractTextureAtr.x)) - (n22 + n22) * 18100) - _Cutoff + 0.4);
						surface.Alpha = 0.5;
						//SHADOW_CASTER_FRAGMENT(i)
					}

					//FULL VOLUME SHADOWS
					if (_shadowMode == 2) {
						float3 woeldPos = IN.WorldSpacePosition;// +_WorldSpaceCameraPos; // MAKE CAMERA RELATIVE !!! for HDRP
						float change_h = _CutHeight;
						float PosDiff = 0.0006*(woeldPos.y - change_h); //float PosDiff = 0.0006*(i.worldPos.y - change_h);
						float2 UVs = _Density * float2(woeldPos.x, woeldPos.z);
						float4 TimingF = 0.0012;
						float2 UVs1 = _Velocity1 * TimingF*_TimeA.y + UVs;

						float4 cloudTexture = SAMPLE_TEXTURE2D(_WeatherMap, sampler_WeatherMap, UVs1);
						float4 cloudTexture1 = SAMPLE_TEXTURE2D(_WeatherMap, sampler_WeatherMap, UVs1);
						float4 paintTexture1 = SAMPLE_TEXTURE2D(_PaintMap, sampler_PaintMap, UVs1);




						float2 UVs2 = (_Velocity2*TimingF*_TimeA.y / 1.4 + float2(_EdgeFactors.x, _EdgeFactors.y) + UVs);

						float4 Texture1 = SAMPLE_TEXTURE2D(_WeatherMap, sampler_WeatherMap, UVs2);
						float4 Texture2 = SAMPLE_TEXTURE2D(_WeatherMap, sampler_WeatherMap, UVs2);

						//FULL VOLUME
						cloudTexture.rgb = sampleWeather(1 * float3(woeldPos.x, 0, woeldPos.z));
						cloudTexture1.rgb = sampleWeather(1 * float3(woeldPos.x, 0, woeldPos.z));
						Texture1.rgb = sampleWeather(1 * float3(woeldPos.x, 0, woeldPos.z));
						Texture2.rgb = sampleWeather(1 * float3(woeldPos.x, 0, woeldPos.z));
						Texture1.a = Texture1.r + Texture1.g * 1 + Texture1.b * 0;
						Texture2.a = Texture2.r + Texture2.g * 1 + Texture2.b * 0;
						cloudTexture.a = cloudTexture.r + cloudTexture.g * 1 + cloudTexture.b * 0;
						cloudTexture1.a = cloudTexture1.r + cloudTexture1.g * 1 + cloudTexture1.b * 0;

						float DER = woeldPos.y*0.001;
						float3 normalA = (((DER*((_Coverage + _CoverageOffset) + ((cloudTexture.rgb * 2) - 1))) - (1 - (Texture1.rgb * 2))));
						float3 normalN = normalize(normalA);

						float DER1 = -(woeldPos.y - 0) * 1;
						float PosTDiff = woeldPos.y * 1;
						if (woeldPos.y > change_h) {
							DER1 = (1 - cloudTexture1.a) *  PosTDiff;
						}
						float shaper = ((_Transparency + _TranspOffset) - 0.48 + 0.0)*((DER1*(((_Coverage + _CoverageOffset) + cloudTexture1.a)))*Texture2.a);
						change_h = 10;
						PosDiff = Thickness * 0.0006*(woeldPos.y - change_h);
						PosTDiff = woeldPos.y*PosDiff;
						DER1 = -(woeldPos.y + 0)*PosDiff;
						if (woeldPos.y > change_h) {
							DER1 = (1 - cloudTexture1.a) *  PosTDiff;
						}
						if (woeldPos.y > 150) { 																																///////////////////////////// 650
							DER1 = (1 - cloudTexture1.a) *  PosTDiff*0.07;
							shaper = shaper * shaper;
						}


						//NEW
						//shaper = 1.6 * Texture2.r;// *(1 - Texture2.g);
						shaper = 1.5 *(Texture2.r) - 1.5*Texture2.b - 1.5*Texture2.g;

						//NEW
						//float3 weather = sampleWeather(float3(woeldPos.x, 0, woeldPos.z));
						//shaper = weather.r * shaper;

						//surface.AlphaClipThreshold = 1 - (((shaper)*paintTexture1) - _Cutoff + 0.4);
						surface.AlphaClipThreshold = 1 - (((shaper) * 1) - 0.45 + 0.4);
						surface.Alpha = 0;// _Cutoff + 0.4;// -_Cutoff + 0.4;
										  ///// END NASOS HDRP SHADOWS
					}


					
					
					//surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
					//surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
					return surface;
				}

				// --------------------------------------------------
				// Build Graph Inputs

				VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
				{
					VertexDescriptionInputs output;
					ZERO_INITIALIZE(VertexDescriptionInputs, output);

					output.ObjectSpaceNormal = input.normalOS;
					output.ObjectSpaceTangent = input.tangentOS;
					output.ObjectSpacePosition = input.positionOS;

					return output;
				}

				SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
				{
					SurfaceDescriptionInputs output;
					ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





					output.WorldSpacePosition = input.positionWS;
					output.uv0 = input.texCoord0;
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
				#else
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
				#endif
				#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

					return output;
				}


				// --------------------------------------------------
				// Main

				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

				ENDHLSL
			}
			Pass
			{
				Name "DepthOnly"
				Tags
				{
					"LightMode" = "DepthOnly"
				}

					// Render State
					Cull Back
					Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
					ZTest LEqual
					ZWrite On
					ColorMask 0

					// Debug
					// <None>

					// --------------------------------------------------
					// Pass

					HLSLPROGRAM

					// Pragmas
					#pragma target 4.5
					#pragma exclude_renderers gles gles3 glcore
					#pragma multi_compile_instancing
					#pragma multi_compile _ DOTS_INSTANCING_ON
					#pragma vertex vert
					#pragma fragment frag

					// DotsInstancingOptions: <None>
					// HybridV1InjectedBuiltinProperties: <None>

					// Keywords
					// PassKeywords: <None>
					// GraphKeywords: <None>

					// Defines
					#define _SURFACE_TYPE_TRANSPARENT 1
					#define _AlphaClip 1
					#define _NORMALMAP 1
					#define _NORMAL_DROPOFF_TS 1
					#define ATTRIBUTES_NEED_NORMAL
					#define ATTRIBUTES_NEED_TANGENT
					#define ATTRIBUTES_NEED_TEXCOORD0
					#define VARYINGS_NEED_POSITION_WS
					#define VARYINGS_NEED_TEXCOORD0
					#define FEATURES_GRAPH_VERTEX
					/* WARNING: $splice Could not find named fragment 'PassInstancing' */
					#define SHADERPASS SHADERPASS_DEPTHONLY
					/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

					// Includes
					#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
					#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

					// --------------------------------------------------
					// Structs and Packing

					struct Attributes
					{
						float3 positionOS : POSITION;
						float3 normalOS : NORMAL;
						float4 tangentOS : TANGENT;
						float4 uv0 : TEXCOORD0;
						#if UNITY_ANY_INSTANCING_ENABLED
						uint instanceID : INSTANCEID_SEMANTIC;
						#endif
					};
					struct Varyings
					{
						float4 positionCS : SV_POSITION;
						float3 positionWS;
						float4 texCoord0;
						#if UNITY_ANY_INSTANCING_ENABLED
						uint instanceID : CUSTOM_INSTANCE_ID;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
						#endif
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
						#endif
					};
					struct SurfaceDescriptionInputs
					{
						float3 WorldSpacePosition;
						float4 uv0;
					};
					struct VertexDescriptionInputs
					{
						float3 ObjectSpaceNormal;
						float3 ObjectSpaceTangent;
						float3 ObjectSpacePosition;
					};
					struct PackedVaryings
					{
						float4 positionCS : SV_POSITION;
						float3 interp0 : TEXCOORD0;
						float4 interp1 : TEXCOORD1;
						#if UNITY_ANY_INSTANCING_ENABLED
						uint instanceID : CUSTOM_INSTANCE_ID;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
						#endif
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
						#endif
					};

					PackedVaryings PackVaryings(Varyings input)
					{
						PackedVaryings output;
						output.positionCS = input.positionCS;
						output.interp0.xyz = input.positionWS;
						output.interp1.xyzw = input.texCoord0;
						#if UNITY_ANY_INSTANCING_ENABLED
						output.instanceID = input.instanceID;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
						#endif
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						output.cullFace = input.cullFace;
						#endif
						return output;
					}
					Varyings UnpackVaryings(PackedVaryings input)
					{
						Varyings output;
						output.positionCS = input.positionCS;
						output.positionWS = input.interp0.xyz;
						output.texCoord0 = input.interp1.xyzw;
						#if UNITY_ANY_INSTANCING_ENABLED
						output.instanceID = input.instanceID;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
						#endif
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						output.cullFace = input.cullFace;
						#endif
						return output;
					}

					// --------------------------------------------------
					// Graph

					// Graph Properties
					CBUFFER_START(UnityPerMaterial)
					float cutoff;
					float2 TilingA;
					float2 OffsetA;
					CBUFFER_END



					TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
					SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

					// Graph Functions

					void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
					{
						Out = UV * Tiling + Offset;
					}

					void Unity_Add_float3(float3 A, float3 B, out float3 Out)
					{
						Out = A + B;
					}

					// Graph Vertex
					struct VertexDescription
					{
						float3 Position;
						float3 Normal;
						float3 Tangent;
					};

					VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
					{
						VertexDescription description = (VertexDescription)0;
						description.Position = IN.ObjectSpacePosition;
						description.Normal = IN.ObjectSpaceNormal;
						description.Tangent = IN.ObjectSpaceTangent;
						return description;
					}

					// Graph Pixel
					struct SurfaceDescription
					{
						float Alpha;
						float AlphaClipThreshold;
					};

					SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
					{
						SurfaceDescription surface = (SurfaceDescription)0;
						float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
						float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
						float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
						Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
						float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
						float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
						float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
						float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
						float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
						float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
						float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
						Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
						surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
						surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
						return surface;
					}

					// --------------------------------------------------
					// Build Graph Inputs

					VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
					{
						VertexDescriptionInputs output;
						ZERO_INITIALIZE(VertexDescriptionInputs, output);

						output.ObjectSpaceNormal = input.normalOS;
						output.ObjectSpaceTangent = input.tangentOS;
						output.ObjectSpacePosition = input.positionOS;

						return output;
					}

					SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
					{
						SurfaceDescriptionInputs output;
						ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





						output.WorldSpacePosition = input.positionWS;
						output.uv0 = input.texCoord0;
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
					#else
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
					#endif
					#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

						return output;
					}


					// --------------------------------------------------
					// Main

					#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

					ENDHLSL
				}
				Pass
				{
					Name "DepthNormals"
					Tags
					{
						"LightMode" = "DepthNormals"
					}

						// Render State
						Cull Back
						Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
						ZTest LEqual
						ZWrite On

						// Debug
						// <None>

						// --------------------------------------------------
						// Pass

						HLSLPROGRAM

						// Pragmas
						#pragma target 4.5
						#pragma exclude_renderers gles gles3 glcore
						#pragma multi_compile_instancing
						#pragma multi_compile _ DOTS_INSTANCING_ON
						#pragma vertex vert
						#pragma fragment frag

						// DotsInstancingOptions: <None>
						// HybridV1InjectedBuiltinProperties: <None>

						// Keywords
						// PassKeywords: <None>
						// GraphKeywords: <None>

						// Defines
						#define _SURFACE_TYPE_TRANSPARENT 1
						#define _AlphaClip 1
						#define _NORMALMAP 1
						#define _NORMAL_DROPOFF_TS 1
						#define ATTRIBUTES_NEED_NORMAL
						#define ATTRIBUTES_NEED_TANGENT
						#define ATTRIBUTES_NEED_TEXCOORD0
						#define ATTRIBUTES_NEED_TEXCOORD1
						#define VARYINGS_NEED_POSITION_WS
						#define VARYINGS_NEED_NORMAL_WS
						#define VARYINGS_NEED_TANGENT_WS
						#define VARYINGS_NEED_TEXCOORD0
						#define FEATURES_GRAPH_VERTEX
						/* WARNING: $splice Could not find named fragment 'PassInstancing' */
						#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
						/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

						// Includes
						#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
						#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
						#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
						#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
						#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

						// --------------------------------------------------
						// Structs and Packing

						struct Attributes
						{
							float3 positionOS : POSITION;
							float3 normalOS : NORMAL;
							float4 tangentOS : TANGENT;
							float4 uv0 : TEXCOORD0;
							float4 uv1 : TEXCOORD1;
							#if UNITY_ANY_INSTANCING_ENABLED
							uint instanceID : INSTANCEID_SEMANTIC;
							#endif
						};
						struct Varyings
						{
							float4 positionCS : SV_POSITION;
							float3 positionWS;
							float3 normalWS;
							float4 tangentWS;
							float4 texCoord0;
							#if UNITY_ANY_INSTANCING_ENABLED
							uint instanceID : CUSTOM_INSTANCE_ID;
							#endif
							#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
							uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
							#endif
							#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
							uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
							#endif
							#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
							FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
							#endif
						};
						struct SurfaceDescriptionInputs
						{
							float3 TangentSpaceNormal;
							float3 WorldSpacePosition;
							float4 uv0;
						};
						struct VertexDescriptionInputs
						{
							float3 ObjectSpaceNormal;
							float3 ObjectSpaceTangent;
							float3 ObjectSpacePosition;
						};
						struct PackedVaryings
						{
							float4 positionCS : SV_POSITION;
							float3 interp0 : TEXCOORD0;
							float3 interp1 : TEXCOORD1;
							float4 interp2 : TEXCOORD2;
							float4 interp3 : TEXCOORD3;
							#if UNITY_ANY_INSTANCING_ENABLED
							uint instanceID : CUSTOM_INSTANCE_ID;
							#endif
							#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
							uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
							#endif
							#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
							uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
							#endif
							#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
							FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
							#endif
						};

						PackedVaryings PackVaryings(Varyings input)
						{
							PackedVaryings output;
							output.positionCS = input.positionCS;
							output.interp0.xyz = input.positionWS;
							output.interp1.xyz = input.normalWS;
							output.interp2.xyzw = input.tangentWS;
							output.interp3.xyzw = input.texCoord0;
							#if UNITY_ANY_INSTANCING_ENABLED
							output.instanceID = input.instanceID;
							#endif
							#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
							output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
							#endif
							#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
							output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
							#endif
							#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
							output.cullFace = input.cullFace;
							#endif
							return output;
						}
						Varyings UnpackVaryings(PackedVaryings input)
						{
							Varyings output;
							output.positionCS = input.positionCS;
							output.positionWS = input.interp0.xyz;
							output.normalWS = input.interp1.xyz;
							output.tangentWS = input.interp2.xyzw;
							output.texCoord0 = input.interp3.xyzw;
							#if UNITY_ANY_INSTANCING_ENABLED
							output.instanceID = input.instanceID;
							#endif
							#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
							output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
							#endif
							#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
							output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
							#endif
							#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
							output.cullFace = input.cullFace;
							#endif
							return output;
						}

						// --------------------------------------------------
						// Graph

						// Graph Properties
						CBUFFER_START(UnityPerMaterial)
						float cutoff;
						float2 TilingA;
						float2 OffsetA;
						CBUFFER_END



						TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
						SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

						// Graph Functions

						void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
						{
							Out = UV * Tiling + Offset;
						}

						void Unity_Add_float3(float3 A, float3 B, out float3 Out)
						{
							Out = A + B;
						}

						// Graph Vertex
						struct VertexDescription
						{
							float3 Position;
							float3 Normal;
							float3 Tangent;
						};

						VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
						{
							VertexDescription description = (VertexDescription)0;
							description.Position = IN.ObjectSpacePosition;
							description.Normal = IN.ObjectSpaceNormal;
							description.Tangent = IN.ObjectSpaceTangent;
							return description;
						}

						// Graph Pixel
						struct SurfaceDescription
						{
							float3 NormalTS;
							float Alpha;
							float AlphaClipThreshold;
						};

						SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
						{
							SurfaceDescription surface = (SurfaceDescription)0;
							float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
							float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
							float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
							Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
							float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
							float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
							float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
							float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
							float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
							float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
							float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
							Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
							surface.NormalTS = IN.TangentSpaceNormal;
							surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
							surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
							return surface;
						}

						// --------------------------------------------------
						// Build Graph Inputs

						VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
						{
							VertexDescriptionInputs output;
							ZERO_INITIALIZE(VertexDescriptionInputs, output);

							output.ObjectSpaceNormal = input.normalOS;
							output.ObjectSpaceTangent = input.tangentOS;
							output.ObjectSpacePosition = input.positionOS;

							return output;
						}

						SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
						{
							SurfaceDescriptionInputs output;
							ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



							output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


							output.WorldSpacePosition = input.positionWS;
							output.uv0 = input.texCoord0;
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
						#else
						#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
						#endif
						#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

							return output;
						}


						// --------------------------------------------------
						// Main

						#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
						#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
						#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

						ENDHLSL
					}
					Pass
					{
						Name "Meta"
						Tags
						{
							"LightMode" = "Meta"
						}

							// Render State
							Cull Off

							// Debug
							// <None>

							// --------------------------------------------------
							// Pass

							HLSLPROGRAM

							// Pragmas
							#pragma target 4.5
							#pragma exclude_renderers gles gles3 glcore
							#pragma vertex vert
							#pragma fragment frag

							// DotsInstancingOptions: <None>
							// HybridV1InjectedBuiltinProperties: <None>

							// Keywords
							#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
							// GraphKeywords: <None>

							// Defines
							#define _SURFACE_TYPE_TRANSPARENT 1
							#define _AlphaClip 1
							#define _NORMALMAP 1
							#define _NORMAL_DROPOFF_TS 1
							#define ATTRIBUTES_NEED_NORMAL
							#define ATTRIBUTES_NEED_TANGENT
							#define ATTRIBUTES_NEED_TEXCOORD0
							#define ATTRIBUTES_NEED_TEXCOORD1
							#define ATTRIBUTES_NEED_TEXCOORD2
							#define VARYINGS_NEED_POSITION_WS
							#define VARYINGS_NEED_TEXCOORD0
							#define FEATURES_GRAPH_VERTEX
							/* WARNING: $splice Could not find named fragment 'PassInstancing' */
							#define SHADERPASS SHADERPASS_META
							/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

							// Includes
							#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
							#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
							#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
							#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
							#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
							#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

							// --------------------------------------------------
							// Structs and Packing

							struct Attributes
							{
								float3 positionOS : POSITION;
								float3 normalOS : NORMAL;
								float4 tangentOS : TANGENT;
								float4 uv0 : TEXCOORD0;
								float4 uv1 : TEXCOORD1;
								float4 uv2 : TEXCOORD2;
								#if UNITY_ANY_INSTANCING_ENABLED
								uint instanceID : INSTANCEID_SEMANTIC;
								#endif
							};
							struct Varyings
							{
								float4 positionCS : SV_POSITION;
								float3 positionWS;
								float4 texCoord0;
								#if UNITY_ANY_INSTANCING_ENABLED
								uint instanceID : CUSTOM_INSTANCE_ID;
								#endif
								#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
								uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
								#endif
								#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
								uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
								#endif
								#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
								FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
								#endif
							};
							struct SurfaceDescriptionInputs
							{
								float3 WorldSpacePosition;
								float4 uv0;
							};
							struct VertexDescriptionInputs
							{
								float3 ObjectSpaceNormal;
								float3 ObjectSpaceTangent;
								float3 ObjectSpacePosition;
							};
							struct PackedVaryings
							{
								float4 positionCS : SV_POSITION;
								float3 interp0 : TEXCOORD0;
								float4 interp1 : TEXCOORD1;
								#if UNITY_ANY_INSTANCING_ENABLED
								uint instanceID : CUSTOM_INSTANCE_ID;
								#endif
								#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
								uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
								#endif
								#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
								uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
								#endif
								#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
								FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
								#endif
							};

							PackedVaryings PackVaryings(Varyings input)
							{
								PackedVaryings output;
								output.positionCS = input.positionCS;
								output.interp0.xyz = input.positionWS;
								output.interp1.xyzw = input.texCoord0;
								#if UNITY_ANY_INSTANCING_ENABLED
								output.instanceID = input.instanceID;
								#endif
								#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
								output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
								#endif
								#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
								output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
								#endif
								#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
								output.cullFace = input.cullFace;
								#endif
								return output;
							}
							Varyings UnpackVaryings(PackedVaryings input)
							{
								Varyings output;
								output.positionCS = input.positionCS;
								output.positionWS = input.interp0.xyz;
								output.texCoord0 = input.interp1.xyzw;
								#if UNITY_ANY_INSTANCING_ENABLED
								output.instanceID = input.instanceID;
								#endif
								#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
								output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
								#endif
								#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
								output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
								#endif
								#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
								output.cullFace = input.cullFace;
								#endif
								return output;
							}

							// --------------------------------------------------
							// Graph

							// Graph Properties
							CBUFFER_START(UnityPerMaterial)
							float cutoff;
							float2 TilingA;
							float2 OffsetA;
							CBUFFER_END



							TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
							SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

							// Graph Functions

							void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
							{
								Out = UV * Tiling + Offset;
							}

							void Unity_Add_float3(float3 A, float3 B, out float3 Out)
							{
								Out = A + B;
							}

							// Graph Vertex
							struct VertexDescription
							{
								float3 Position;
								float3 Normal;
								float3 Tangent;
							};

							VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
							{
								VertexDescription description = (VertexDescription)0;
								description.Position = IN.ObjectSpacePosition;
								description.Normal = IN.ObjectSpaceNormal;
								description.Tangent = IN.ObjectSpaceTangent;
								return description;
							}

							// Graph Pixel
							struct SurfaceDescription
							{
								float3 BaseColor;
								float3 Emission;
								float Alpha;
								float AlphaClipThreshold;
							};

							SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
							{
								SurfaceDescription surface = (SurfaceDescription)0;
								float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
								float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
								float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
								Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
								float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
								float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
								float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
								float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
								float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
								float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
								float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
								Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
								surface.BaseColor = (_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.xyz);
								surface.Emission = IN.WorldSpacePosition;
								surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
								surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
								return surface;
							}

							// --------------------------------------------------
							// Build Graph Inputs

							VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
							{
								VertexDescriptionInputs output;
								ZERO_INITIALIZE(VertexDescriptionInputs, output);

								output.ObjectSpaceNormal = input.normalOS;
								output.ObjectSpaceTangent = input.tangentOS;
								output.ObjectSpacePosition = input.positionOS;

								return output;
							}

							SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
							{
								SurfaceDescriptionInputs output;
								ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





								output.WorldSpacePosition = input.positionWS;
								output.uv0 = input.texCoord0;
							#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
							#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
							#else
							#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
							#endif
							#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

								return output;
							}


							// --------------------------------------------------
							// Main

							#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
							#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
							#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

							ENDHLSL
						}
						Pass
						{
								// Name: <None>
								Tags
								{
									"LightMode" = "Universal2D"
								}

								// Render State
								Cull Back
								Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
								ZTest LEqual
								ZWrite Off

								// Debug
								// <None>

								// --------------------------------------------------
								// Pass

								HLSLPROGRAM

								// Pragmas
								#pragma target 4.5
								#pragma exclude_renderers gles gles3 glcore
								#pragma vertex vert
								#pragma fragment frag

								// DotsInstancingOptions: <None>
								// HybridV1InjectedBuiltinProperties: <None>

								// Keywords
								// PassKeywords: <None>
								// GraphKeywords: <None>

								// Defines
								#define _SURFACE_TYPE_TRANSPARENT 1
								#define _AlphaClip 1
								#define _NORMALMAP 1
								#define _NORMAL_DROPOFF_TS 1
								#define ATTRIBUTES_NEED_NORMAL
								#define ATTRIBUTES_NEED_TANGENT
								#define ATTRIBUTES_NEED_TEXCOORD0
								#define VARYINGS_NEED_POSITION_WS
								#define VARYINGS_NEED_TEXCOORD0
								#define FEATURES_GRAPH_VERTEX
								/* WARNING: $splice Could not find named fragment 'PassInstancing' */
								#define SHADERPASS SHADERPASS_2D
								/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

								// Includes
								#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
								#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
								#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
								#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
								#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

								// --------------------------------------------------
								// Structs and Packing

								struct Attributes
								{
									float3 positionOS : POSITION;
									float3 normalOS : NORMAL;
									float4 tangentOS : TANGENT;
									float4 uv0 : TEXCOORD0;
									#if UNITY_ANY_INSTANCING_ENABLED
									uint instanceID : INSTANCEID_SEMANTIC;
									#endif
								};
								struct Varyings
								{
									float4 positionCS : SV_POSITION;
									float3 positionWS;
									float4 texCoord0;
									#if UNITY_ANY_INSTANCING_ENABLED
									uint instanceID : CUSTOM_INSTANCE_ID;
									#endif
									#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
									uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
									#endif
									#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
									uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
									#endif
									#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
									FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
									#endif
								};
								struct SurfaceDescriptionInputs
								{
									float3 WorldSpacePosition;
									float4 uv0;
								};
								struct VertexDescriptionInputs
								{
									float3 ObjectSpaceNormal;
									float3 ObjectSpaceTangent;
									float3 ObjectSpacePosition;
								};
								struct PackedVaryings
								{
									float4 positionCS : SV_POSITION;
									float3 interp0 : TEXCOORD0;
									float4 interp1 : TEXCOORD1;
									#if UNITY_ANY_INSTANCING_ENABLED
									uint instanceID : CUSTOM_INSTANCE_ID;
									#endif
									#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
									uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
									#endif
									#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
									uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
									#endif
									#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
									FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
									#endif
								};

								PackedVaryings PackVaryings(Varyings input)
								{
									PackedVaryings output;
									output.positionCS = input.positionCS;
									output.interp0.xyz = input.positionWS;
									output.interp1.xyzw = input.texCoord0;
									#if UNITY_ANY_INSTANCING_ENABLED
									output.instanceID = input.instanceID;
									#endif
									#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
									output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
									#endif
									#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
									output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
									#endif
									#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
									output.cullFace = input.cullFace;
									#endif
									return output;
								}
								Varyings UnpackVaryings(PackedVaryings input)
								{
									Varyings output;
									output.positionCS = input.positionCS;
									output.positionWS = input.interp0.xyz;
									output.texCoord0 = input.interp1.xyzw;
									#if UNITY_ANY_INSTANCING_ENABLED
									output.instanceID = input.instanceID;
									#endif
									#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
									output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
									#endif
									#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
									output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
									#endif
									#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
									output.cullFace = input.cullFace;
									#endif
									return output;
								}

								// --------------------------------------------------
								// Graph

								// Graph Properties
								CBUFFER_START(UnityPerMaterial)
								float cutoff;
								float2 TilingA;
								float2 OffsetA;
								CBUFFER_END



								TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
								SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

								// Graph Functions

								void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
								{
									Out = UV * Tiling + Offset;
								}

								void Unity_Add_float3(float3 A, float3 B, out float3 Out)
								{
									Out = A + B;
								}

								// Graph Vertex
								struct VertexDescription
								{
									float3 Position;
									float3 Normal;
									float3 Tangent;
								};

								VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
								{
									VertexDescription description = (VertexDescription)0;
									description.Position = IN.ObjectSpacePosition;
									description.Normal = IN.ObjectSpaceNormal;
									description.Tangent = IN.ObjectSpaceTangent;
									return description;
								}

								// Graph Pixel
								struct SurfaceDescription
								{
									float3 BaseColor;
									float Alpha;
									float AlphaClipThreshold;
								};

								SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
								{
									SurfaceDescription surface = (SurfaceDescription)0;
									float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
									float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
									float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
									Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
									float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
									float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
									float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
									float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
									float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
									float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
									float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
									Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
									surface.BaseColor = (_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.xyz);
									surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
									surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
									return surface;
								}

								// --------------------------------------------------
								// Build Graph Inputs

								VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
								{
									VertexDescriptionInputs output;
									ZERO_INITIALIZE(VertexDescriptionInputs, output);

									output.ObjectSpaceNormal = input.normalOS;
									output.ObjectSpaceTangent = input.tangentOS;
									output.ObjectSpacePosition = input.positionOS;

									return output;
								}

								SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
								{
									SurfaceDescriptionInputs output;
									ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





									output.WorldSpacePosition = input.positionWS;
									output.uv0 = input.texCoord0;
								#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
								#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
								#else
								#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
								#endif
								#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

									return output;
								}


								// --------------------------------------------------
								// Main

								#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
								#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
								#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

								ENDHLSL
							}
	}
		SubShader
								{
									Tags
									{
										"RenderPipeline" = "UniversalPipeline"
										"RenderType" = "Transparent"
										"UniversalMaterialType" = "Lit"
										"Queue" = "Transparent"
									}
									Pass
									{
										Name "Universal Forward"
										Tags
										{
											"LightMode" = "UniversalForward"
										}

									// Render State
									Cull Back
									Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
									ZTest LEqual
									ZWrite Off

									// Debug
									// <None>

									// --------------------------------------------------
									// Pass

									HLSLPROGRAM

									// Pragmas
									#pragma target 2.0
									#pragma only_renderers gles gles3 glcore
									#pragma multi_compile_instancing
									#pragma multi_compile_fog
									#pragma vertex vert
									#pragma fragment frag

									// DotsInstancingOptions: <None>
									// HybridV1InjectedBuiltinProperties: <None>

									// Keywords
									#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
									#pragma multi_compile _ LIGHTMAP_ON
									#pragma multi_compile _ DIRLIGHTMAP_COMBINED
									#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
									#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
									#pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
									#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
									#pragma multi_compile _ _SHADOWS_SOFT
									#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
									#pragma multi_compile _ SHADOWS_SHADOWMASK
									// GraphKeywords: <None>

									// Defines
									#define _SURFACE_TYPE_TRANSPARENT 1
									#define _AlphaClip 1
									#define _NORMALMAP 1
									#define _NORMAL_DROPOFF_TS 1
									#define ATTRIBUTES_NEED_NORMAL
									#define ATTRIBUTES_NEED_TANGENT
									#define ATTRIBUTES_NEED_TEXCOORD0
									#define ATTRIBUTES_NEED_TEXCOORD1
									#define VARYINGS_NEED_POSITION_WS
									#define VARYINGS_NEED_NORMAL_WS
									#define VARYINGS_NEED_TANGENT_WS
									#define VARYINGS_NEED_TEXCOORD0
									#define VARYINGS_NEED_VIEWDIRECTION_WS
									#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
									#define FEATURES_GRAPH_VERTEX
									/* WARNING: $splice Could not find named fragment 'PassInstancing' */
									#define SHADERPASS SHADERPASS_FORWARD
									/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

									// Includes
									#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
									#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
									#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
									#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
									#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
									#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

									// --------------------------------------------------
									// Structs and Packing

									struct Attributes
									{
										float3 positionOS : POSITION;
										float3 normalOS : NORMAL;
										float4 tangentOS : TANGENT;
										float4 uv0 : TEXCOORD0;
										float4 uv1 : TEXCOORD1;
										#if UNITY_ANY_INSTANCING_ENABLED
										uint instanceID : INSTANCEID_SEMANTIC;
										#endif
									};
									struct Varyings
									{
										float4 positionCS : SV_POSITION;
										float3 positionWS;
										float3 normalWS;
										float4 tangentWS;
										float4 texCoord0;
										float3 viewDirectionWS;
										#if defined(LIGHTMAP_ON)
										float2 lightmapUV;
										#endif
										#if !defined(LIGHTMAP_ON)
										float3 sh;
										#endif
										float4 fogFactorAndVertexLight;
										float4 shadowCoord;
										#if UNITY_ANY_INSTANCING_ENABLED
										uint instanceID : CUSTOM_INSTANCE_ID;
										#endif
										#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
										uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
										#endif
										#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
										uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
										#endif
										#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
										FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
										#endif
									};
									struct SurfaceDescriptionInputs
									{
										float3 TangentSpaceNormal;
										float3 WorldSpacePosition;
										float4 uv0;
									};
									struct VertexDescriptionInputs
									{
										float3 ObjectSpaceNormal;
										float3 ObjectSpaceTangent;
										float3 ObjectSpacePosition;
									};
									struct PackedVaryings
									{
										float4 positionCS : SV_POSITION;
										float3 interp0 : TEXCOORD0;
										float3 interp1 : TEXCOORD1;
										float4 interp2 : TEXCOORD2;
										float4 interp3 : TEXCOORD3;
										float3 interp4 : TEXCOORD4;
										#if defined(LIGHTMAP_ON)
										float2 interp5 : TEXCOORD5;
										#endif
										#if !defined(LIGHTMAP_ON)
										float3 interp6 : TEXCOORD6;
										#endif
										float4 interp7 : TEXCOORD7;
										float4 interp8 : TEXCOORD8;
										#if UNITY_ANY_INSTANCING_ENABLED
										uint instanceID : CUSTOM_INSTANCE_ID;
										#endif
										#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
										uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
										#endif
										#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
										uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
										#endif
										#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
										FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
										#endif
									};

									PackedVaryings PackVaryings(Varyings input)
									{
										PackedVaryings output;
										output.positionCS = input.positionCS;
										output.interp0.xyz = input.positionWS;
										output.interp1.xyz = input.normalWS;
										output.interp2.xyzw = input.tangentWS;
										output.interp3.xyzw = input.texCoord0;
										output.interp4.xyz = input.viewDirectionWS;
										#if defined(LIGHTMAP_ON)
										output.interp5.xy = input.lightmapUV;
										#endif
										#if !defined(LIGHTMAP_ON)
										output.interp6.xyz = input.sh;
										#endif
										output.interp7.xyzw = input.fogFactorAndVertexLight;
										output.interp8.xyzw = input.shadowCoord;
										#if UNITY_ANY_INSTANCING_ENABLED
										output.instanceID = input.instanceID;
										#endif
										#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
										output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
										#endif
										#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
										output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
										#endif
										#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
										output.cullFace = input.cullFace;
										#endif
										return output;
									}
									Varyings UnpackVaryings(PackedVaryings input)
									{
										Varyings output;
										output.positionCS = input.positionCS;
										output.positionWS = input.interp0.xyz;
										output.normalWS = input.interp1.xyz;
										output.tangentWS = input.interp2.xyzw;
										output.texCoord0 = input.interp3.xyzw;
										output.viewDirectionWS = input.interp4.xyz;
										#if defined(LIGHTMAP_ON)
										output.lightmapUV = input.interp5.xy;
										#endif
										#if !defined(LIGHTMAP_ON)
										output.sh = input.interp6.xyz;
										#endif
										output.fogFactorAndVertexLight = input.interp7.xyzw;
										output.shadowCoord = input.interp8.xyzw;
										#if UNITY_ANY_INSTANCING_ENABLED
										output.instanceID = input.instanceID;
										#endif
										#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
										output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
										#endif
										#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
										output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
										#endif
										#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
										output.cullFace = input.cullFace;
										#endif
										return output;
									}

									// --------------------------------------------------
									// Graph

									// Graph Properties
									CBUFFER_START(UnityPerMaterial)
									float cutoff;
									float2 TilingA;
									float2 OffsetA;
									CBUFFER_END



									TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
									SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

									// Graph Functions

									void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
									{
										Out = UV * Tiling + Offset;
									}

									void Unity_Add_float3(float3 A, float3 B, out float3 Out)
									{
										Out = A + B;
									}

									// Graph Vertex
									struct VertexDescription
									{
										float3 Position;
										float3 Normal;
										float3 Tangent;
									};

									VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
									{
										VertexDescription description = (VertexDescription)0;
										description.Position = IN.ObjectSpacePosition;
										description.Normal = IN.ObjectSpaceNormal;
										description.Tangent = IN.ObjectSpaceTangent;
										return description;
									}

									// Graph Pixel
									struct SurfaceDescription
									{
										float3 BaseColor;
										float3 NormalTS;
										float3 Emission;
										float Metallic;
										float Smoothness;
										float Occlusion;
										float Alpha;
										float AlphaClipThreshold;
									};

									SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
									{
										SurfaceDescription surface = (SurfaceDescription)0;
										float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
										float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
										float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
										Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
										float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
										float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
										float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
										float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
										float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
										float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
										float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
										Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
										surface.BaseColor = (_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.xyz);
										surface.NormalTS = IN.TangentSpaceNormal;
										surface.Emission = IN.WorldSpacePosition;
										surface.Metallic = 0;
										surface.Smoothness = 0.5;
										surface.Occlusion = 1;
										surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
										surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
										return surface;
									}

									// --------------------------------------------------
									// Build Graph Inputs

									VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
									{
										VertexDescriptionInputs output;
										ZERO_INITIALIZE(VertexDescriptionInputs, output);

										output.ObjectSpaceNormal = input.normalOS;
										output.ObjectSpaceTangent = input.tangentOS;
										output.ObjectSpacePosition = input.positionOS;

										return output;
									}

									SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
									{
										SurfaceDescriptionInputs output;
										ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



										output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


										output.WorldSpacePosition = input.positionWS;
										output.uv0 = input.texCoord0;
									#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
									#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
									#else
									#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
									#endif
									#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

										return output;
									}


									// --------------------------------------------------
									// Main

									#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
									#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
									#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

									ENDHLSL
								}
								Pass
								{
									Name "ShadowCaster"
									Tags
									{
										"LightMode" = "ShadowCaster"
									}

										// Render State
										Cull Back
										Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
										ZTest LEqual
										ZWrite On
										ColorMask 0

										// Debug
										// <None>

										// --------------------------------------------------
										// Pass

										HLSLPROGRAM

										// Pragmas
										#pragma target 2.0
										#pragma only_renderers gles gles3 glcore
										#pragma multi_compile_instancing
										#pragma vertex vert
										#pragma fragment frag

										// DotsInstancingOptions: <None>
										// HybridV1InjectedBuiltinProperties: <None>

										// Keywords
										// PassKeywords: <None>
										// GraphKeywords: <None>

										// Defines
										#define _SURFACE_TYPE_TRANSPARENT 1
										#define _AlphaClip 1
										#define _NORMALMAP 1
										#define _NORMAL_DROPOFF_TS 1
										#define ATTRIBUTES_NEED_NORMAL
										#define ATTRIBUTES_NEED_TANGENT
										#define ATTRIBUTES_NEED_TEXCOORD0
										#define VARYINGS_NEED_POSITION_WS
										#define VARYINGS_NEED_TEXCOORD0
										#define FEATURES_GRAPH_VERTEX
										/* WARNING: $splice Could not find named fragment 'PassInstancing' */
										#define SHADERPASS SHADERPASS_SHADOWCASTER
										/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

										// Includes
										#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
										#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
										#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
										#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
										#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

										// --------------------------------------------------
										// Structs and Packing

										struct Attributes
										{
											float3 positionOS : POSITION;
											float3 normalOS : NORMAL;
											float4 tangentOS : TANGENT;
											float4 uv0 : TEXCOORD0;
											#if UNITY_ANY_INSTANCING_ENABLED
											uint instanceID : INSTANCEID_SEMANTIC;
											#endif
										};
										struct Varyings
										{
											float4 positionCS : SV_POSITION;
											float3 positionWS;
											float4 texCoord0;
											#if UNITY_ANY_INSTANCING_ENABLED
											uint instanceID : CUSTOM_INSTANCE_ID;
											#endif
											#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
											uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
											#endif
											#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
											uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
											#endif
											#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
											FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
											#endif
										};
										struct SurfaceDescriptionInputs
										{
											float3 WorldSpacePosition;
											float4 uv0;
										};
										struct VertexDescriptionInputs
										{
											float3 ObjectSpaceNormal;
											float3 ObjectSpaceTangent;
											float3 ObjectSpacePosition;
										};
										struct PackedVaryings
										{
											float4 positionCS : SV_POSITION;
											float3 interp0 : TEXCOORD0;
											float4 interp1 : TEXCOORD1;
											#if UNITY_ANY_INSTANCING_ENABLED
											uint instanceID : CUSTOM_INSTANCE_ID;
											#endif
											#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
											uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
											#endif
											#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
											uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
											#endif
											#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
											FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
											#endif
										};

										PackedVaryings PackVaryings(Varyings input)
										{
											PackedVaryings output;
											output.positionCS = input.positionCS;
											output.interp0.xyz = input.positionWS;
											output.interp1.xyzw = input.texCoord0;
											#if UNITY_ANY_INSTANCING_ENABLED
											output.instanceID = input.instanceID;
											#endif
											#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
											output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
											#endif
											#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
											output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
											#endif
											#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
											output.cullFace = input.cullFace;
											#endif
											return output;
										}
										Varyings UnpackVaryings(PackedVaryings input)
										{
											Varyings output;
											output.positionCS = input.positionCS;
											output.positionWS = input.interp0.xyz;
											output.texCoord0 = input.interp1.xyzw;
											#if UNITY_ANY_INSTANCING_ENABLED
											output.instanceID = input.instanceID;
											#endif
											#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
											output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
											#endif
											#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
											output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
											#endif
											#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
											output.cullFace = input.cullFace;
											#endif
											return output;
										}

										// --------------------------------------------------
										// Graph

										// Graph Properties
										CBUFFER_START(UnityPerMaterial)
										float cutoff;
										float2 TilingA;
										float2 OffsetA;
										CBUFFER_END



										TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
										SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

										// Graph Functions

										void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
										{
											Out = UV * Tiling + Offset;
										}

										void Unity_Add_float3(float3 A, float3 B, out float3 Out)
										{
											Out = A + B;
										}

										// Graph Vertex
										struct VertexDescription
										{
											float3 Position;
											float3 Normal;
											float3 Tangent;
										};

										VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
										{
											VertexDescription description = (VertexDescription)0;
											description.Position = IN.ObjectSpacePosition;
											description.Normal = IN.ObjectSpaceNormal;
											description.Tangent = IN.ObjectSpaceTangent;
											return description;
										}

										// Graph Pixel
										struct SurfaceDescription
										{
											float Alpha;
											float AlphaClipThreshold;
										};

										SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
										{
											SurfaceDescription surface = (SurfaceDescription)0;
											float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
											float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
											float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
											Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
											float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
											float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
											float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
											float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
											float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
											float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
											float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
											Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
											surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
											surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
											return surface;
										}

										// --------------------------------------------------
										// Build Graph Inputs

										VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
										{
											VertexDescriptionInputs output;
											ZERO_INITIALIZE(VertexDescriptionInputs, output);

											output.ObjectSpaceNormal = input.normalOS;
											output.ObjectSpaceTangent = input.tangentOS;
											output.ObjectSpacePosition = input.positionOS;

											return output;
										}

										SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
										{
											SurfaceDescriptionInputs output;
											ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





											output.WorldSpacePosition = input.positionWS;
											output.uv0 = input.texCoord0;
										#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
										#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
										#else
										#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
										#endif
										#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

											return output;
										}


										// --------------------------------------------------
										// Main

										#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
										#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
										#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

										ENDHLSL
									}
									Pass
									{
										Name "DepthOnly"
										Tags
										{
											"LightMode" = "DepthOnly"
										}

											// Render State
											Cull Back
											Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
											ZTest LEqual
											ZWrite On
											ColorMask 0

											// Debug
											// <None>

											// --------------------------------------------------
											// Pass

											HLSLPROGRAM

											// Pragmas
											#pragma target 2.0
											#pragma only_renderers gles gles3 glcore
											#pragma multi_compile_instancing
											#pragma vertex vert
											#pragma fragment frag

											// DotsInstancingOptions: <None>
											// HybridV1InjectedBuiltinProperties: <None>

											// Keywords
											// PassKeywords: <None>
											// GraphKeywords: <None>

											// Defines
											#define _SURFACE_TYPE_TRANSPARENT 1
											#define _AlphaClip 1
											#define _NORMALMAP 1
											#define _NORMAL_DROPOFF_TS 1
											#define ATTRIBUTES_NEED_NORMAL
											#define ATTRIBUTES_NEED_TANGENT
											#define ATTRIBUTES_NEED_TEXCOORD0
											#define VARYINGS_NEED_POSITION_WS
											#define VARYINGS_NEED_TEXCOORD0
											#define FEATURES_GRAPH_VERTEX
											/* WARNING: $splice Could not find named fragment 'PassInstancing' */
											#define SHADERPASS SHADERPASS_DEPTHONLY
											/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

											// Includes
											#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
											#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
											#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
											#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
											#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

											// --------------------------------------------------
											// Structs and Packing

											struct Attributes
											{
												float3 positionOS : POSITION;
												float3 normalOS : NORMAL;
												float4 tangentOS : TANGENT;
												float4 uv0 : TEXCOORD0;
												#if UNITY_ANY_INSTANCING_ENABLED
												uint instanceID : INSTANCEID_SEMANTIC;
												#endif
											};
											struct Varyings
											{
												float4 positionCS : SV_POSITION;
												float3 positionWS;
												float4 texCoord0;
												#if UNITY_ANY_INSTANCING_ENABLED
												uint instanceID : CUSTOM_INSTANCE_ID;
												#endif
												#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
												uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
												#endif
												#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
												uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
												#endif
												#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
												FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
												#endif
											};
											struct SurfaceDescriptionInputs
											{
												float3 WorldSpacePosition;
												float4 uv0;
											};
											struct VertexDescriptionInputs
											{
												float3 ObjectSpaceNormal;
												float3 ObjectSpaceTangent;
												float3 ObjectSpacePosition;
											};
											struct PackedVaryings
											{
												float4 positionCS : SV_POSITION;
												float3 interp0 : TEXCOORD0;
												float4 interp1 : TEXCOORD1;
												#if UNITY_ANY_INSTANCING_ENABLED
												uint instanceID : CUSTOM_INSTANCE_ID;
												#endif
												#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
												uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
												#endif
												#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
												uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
												#endif
												#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
												FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
												#endif
											};

											PackedVaryings PackVaryings(Varyings input)
											{
												PackedVaryings output;
												output.positionCS = input.positionCS;
												output.interp0.xyz = input.positionWS;
												output.interp1.xyzw = input.texCoord0;
												#if UNITY_ANY_INSTANCING_ENABLED
												output.instanceID = input.instanceID;
												#endif
												#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
												output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
												#endif
												#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
												output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
												#endif
												#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
												output.cullFace = input.cullFace;
												#endif
												return output;
											}
											Varyings UnpackVaryings(PackedVaryings input)
											{
												Varyings output;
												output.positionCS = input.positionCS;
												output.positionWS = input.interp0.xyz;
												output.texCoord0 = input.interp1.xyzw;
												#if UNITY_ANY_INSTANCING_ENABLED
												output.instanceID = input.instanceID;
												#endif
												#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
												output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
												#endif
												#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
												output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
												#endif
												#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
												output.cullFace = input.cullFace;
												#endif
												return output;
											}

											// --------------------------------------------------
											// Graph

											// Graph Properties
											CBUFFER_START(UnityPerMaterial)
											float cutoff;
											float2 TilingA;
											float2 OffsetA;
											CBUFFER_END



											TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
											SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

											// Graph Functions

											void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
											{
												Out = UV * Tiling + Offset;
											}

											void Unity_Add_float3(float3 A, float3 B, out float3 Out)
											{
												Out = A + B;
											}

											// Graph Vertex
											struct VertexDescription
											{
												float3 Position;
												float3 Normal;
												float3 Tangent;
											};

											VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
											{
												VertexDescription description = (VertexDescription)0;
												description.Position = IN.ObjectSpacePosition;
												description.Normal = IN.ObjectSpaceNormal;
												description.Tangent = IN.ObjectSpaceTangent;
												return description;
											}

											// Graph Pixel
											struct SurfaceDescription
											{
												float Alpha;
												float AlphaClipThreshold;
											};

											SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
											{
												SurfaceDescription surface = (SurfaceDescription)0;
												float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
												float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
												float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
												Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
												float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
												float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
												float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
												float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
												float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
												float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
												float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
												Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
												surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
												surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
												return surface;
											}

											// --------------------------------------------------
											// Build Graph Inputs

											VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
											{
												VertexDescriptionInputs output;
												ZERO_INITIALIZE(VertexDescriptionInputs, output);

												output.ObjectSpaceNormal = input.normalOS;
												output.ObjectSpaceTangent = input.tangentOS;
												output.ObjectSpacePosition = input.positionOS;

												return output;
											}

											SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
											{
												SurfaceDescriptionInputs output;
												ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





												output.WorldSpacePosition = input.positionWS;
												output.uv0 = input.texCoord0;
											#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
											#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
											#else
											#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
											#endif
											#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

												return output;
											}


											// --------------------------------------------------
											// Main

											#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
											#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
											#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

											ENDHLSL
										}
										Pass
										{
											Name "DepthNormals"
											Tags
											{
												"LightMode" = "DepthNormals"
											}

												// Render State
												Cull Back
												Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
												ZTest LEqual
												ZWrite On

												// Debug
												// <None>

												// --------------------------------------------------
												// Pass

												HLSLPROGRAM

												// Pragmas
												#pragma target 2.0
												#pragma only_renderers gles gles3 glcore
												#pragma multi_compile_instancing
												#pragma vertex vert
												#pragma fragment frag

												// DotsInstancingOptions: <None>
												// HybridV1InjectedBuiltinProperties: <None>

												// Keywords
												// PassKeywords: <None>
												// GraphKeywords: <None>

												// Defines
												#define _SURFACE_TYPE_TRANSPARENT 1
												#define _AlphaClip 1
												#define _NORMALMAP 1
												#define _NORMAL_DROPOFF_TS 1
												#define ATTRIBUTES_NEED_NORMAL
												#define ATTRIBUTES_NEED_TANGENT
												#define ATTRIBUTES_NEED_TEXCOORD0
												#define ATTRIBUTES_NEED_TEXCOORD1
												#define VARYINGS_NEED_POSITION_WS
												#define VARYINGS_NEED_NORMAL_WS
												#define VARYINGS_NEED_TANGENT_WS
												#define VARYINGS_NEED_TEXCOORD0
												#define FEATURES_GRAPH_VERTEX
												/* WARNING: $splice Could not find named fragment 'PassInstancing' */
												#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
												/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

												// Includes
												#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
												#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
												#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
												#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
												#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

												// --------------------------------------------------
												// Structs and Packing

												struct Attributes
												{
													float3 positionOS : POSITION;
													float3 normalOS : NORMAL;
													float4 tangentOS : TANGENT;
													float4 uv0 : TEXCOORD0;
													float4 uv1 : TEXCOORD1;
													#if UNITY_ANY_INSTANCING_ENABLED
													uint instanceID : INSTANCEID_SEMANTIC;
													#endif
												};
												struct Varyings
												{
													float4 positionCS : SV_POSITION;
													float3 positionWS;
													float3 normalWS;
													float4 tangentWS;
													float4 texCoord0;
													#if UNITY_ANY_INSTANCING_ENABLED
													uint instanceID : CUSTOM_INSTANCE_ID;
													#endif
													#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
													uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
													#endif
													#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
													uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
													#endif
													#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
													FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
													#endif
												};
												struct SurfaceDescriptionInputs
												{
													float3 TangentSpaceNormal;
													float3 WorldSpacePosition;
													float4 uv0;
												};
												struct VertexDescriptionInputs
												{
													float3 ObjectSpaceNormal;
													float3 ObjectSpaceTangent;
													float3 ObjectSpacePosition;
												};
												struct PackedVaryings
												{
													float4 positionCS : SV_POSITION;
													float3 interp0 : TEXCOORD0;
													float3 interp1 : TEXCOORD1;
													float4 interp2 : TEXCOORD2;
													float4 interp3 : TEXCOORD3;
													#if UNITY_ANY_INSTANCING_ENABLED
													uint instanceID : CUSTOM_INSTANCE_ID;
													#endif
													#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
													uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
													#endif
													#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
													uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
													#endif
													#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
													FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
													#endif
												};

												PackedVaryings PackVaryings(Varyings input)
												{
													PackedVaryings output;
													output.positionCS = input.positionCS;
													output.interp0.xyz = input.positionWS;
													output.interp1.xyz = input.normalWS;
													output.interp2.xyzw = input.tangentWS;
													output.interp3.xyzw = input.texCoord0;
													#if UNITY_ANY_INSTANCING_ENABLED
													output.instanceID = input.instanceID;
													#endif
													#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
													output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
													#endif
													#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
													output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
													#endif
													#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
													output.cullFace = input.cullFace;
													#endif
													return output;
												}
												Varyings UnpackVaryings(PackedVaryings input)
												{
													Varyings output;
													output.positionCS = input.positionCS;
													output.positionWS = input.interp0.xyz;
													output.normalWS = input.interp1.xyz;
													output.tangentWS = input.interp2.xyzw;
													output.texCoord0 = input.interp3.xyzw;
													#if UNITY_ANY_INSTANCING_ENABLED
													output.instanceID = input.instanceID;
													#endif
													#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
													output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
													#endif
													#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
													output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
													#endif
													#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
													output.cullFace = input.cullFace;
													#endif
													return output;
												}

												// --------------------------------------------------
												// Graph

												// Graph Properties
												CBUFFER_START(UnityPerMaterial)
												float cutoff;
												float2 TilingA;
												float2 OffsetA;
												CBUFFER_END



												TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
												SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

												// Graph Functions

												void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
												{
													Out = UV * Tiling + Offset;
												}

												void Unity_Add_float3(float3 A, float3 B, out float3 Out)
												{
													Out = A + B;
												}

												// Graph Vertex
												struct VertexDescription
												{
													float3 Position;
													float3 Normal;
													float3 Tangent;
												};

												VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
												{
													VertexDescription description = (VertexDescription)0;
													description.Position = IN.ObjectSpacePosition;
													description.Normal = IN.ObjectSpaceNormal;
													description.Tangent = IN.ObjectSpaceTangent;
													return description;
												}

												// Graph Pixel
												struct SurfaceDescription
												{
													float3 NormalTS;
													float Alpha;
													float AlphaClipThreshold;
												};

												SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
												{
													SurfaceDescription surface = (SurfaceDescription)0;
													float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
													float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
													float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
													Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
													float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
													float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
													float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
													float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
													float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
													float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
													float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
													Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
													surface.NormalTS = IN.TangentSpaceNormal;
													surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
													surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
													return surface;
												}

												// --------------------------------------------------
												// Build Graph Inputs

												VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
												{
													VertexDescriptionInputs output;
													ZERO_INITIALIZE(VertexDescriptionInputs, output);

													output.ObjectSpaceNormal = input.normalOS;
													output.ObjectSpaceTangent = input.tangentOS;
													output.ObjectSpacePosition = input.positionOS;

													return output;
												}

												SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
												{
													SurfaceDescriptionInputs output;
													ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



													output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


													output.WorldSpacePosition = input.positionWS;
													output.uv0 = input.texCoord0;
												#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
												#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
												#else
												#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
												#endif
												#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

													return output;
												}


												// --------------------------------------------------
												// Main

												#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
												#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
												#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

												ENDHLSL
											}
											Pass
											{
												Name "Meta"
												Tags
												{
													"LightMode" = "Meta"
												}

													// Render State
													Cull Off

													// Debug
													// <None>

													// --------------------------------------------------
													// Pass

													HLSLPROGRAM

													// Pragmas
													#pragma target 2.0
													#pragma only_renderers gles gles3 glcore
													#pragma vertex vert
													#pragma fragment frag

													// DotsInstancingOptions: <None>
													// HybridV1InjectedBuiltinProperties: <None>

													// Keywords
													#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
													// GraphKeywords: <None>

													// Defines
													#define _SURFACE_TYPE_TRANSPARENT 1
													#define _AlphaClip 1
													#define _NORMALMAP 1
													#define _NORMAL_DROPOFF_TS 1
													#define ATTRIBUTES_NEED_NORMAL
													#define ATTRIBUTES_NEED_TANGENT
													#define ATTRIBUTES_NEED_TEXCOORD0
													#define ATTRIBUTES_NEED_TEXCOORD1
													#define ATTRIBUTES_NEED_TEXCOORD2
													#define VARYINGS_NEED_POSITION_WS
													#define VARYINGS_NEED_TEXCOORD0
													#define FEATURES_GRAPH_VERTEX
													/* WARNING: $splice Could not find named fragment 'PassInstancing' */
													#define SHADERPASS SHADERPASS_META
													/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

													// Includes
													#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
													#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
													#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
													#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
													#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
													#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

													// --------------------------------------------------
													// Structs and Packing

													struct Attributes
													{
														float3 positionOS : POSITION;
														float3 normalOS : NORMAL;
														float4 tangentOS : TANGENT;
														float4 uv0 : TEXCOORD0;
														float4 uv1 : TEXCOORD1;
														float4 uv2 : TEXCOORD2;
														#if UNITY_ANY_INSTANCING_ENABLED
														uint instanceID : INSTANCEID_SEMANTIC;
														#endif
													};
													struct Varyings
													{
														float4 positionCS : SV_POSITION;
														float3 positionWS;
														float4 texCoord0;
														#if UNITY_ANY_INSTANCING_ENABLED
														uint instanceID : CUSTOM_INSTANCE_ID;
														#endif
														#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
														uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
														#endif
														#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
														uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
														#endif
														#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
														FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
														#endif
													};
													struct SurfaceDescriptionInputs
													{
														float3 WorldSpacePosition;
														float4 uv0;
													};
													struct VertexDescriptionInputs
													{
														float3 ObjectSpaceNormal;
														float3 ObjectSpaceTangent;
														float3 ObjectSpacePosition;
													};
													struct PackedVaryings
													{
														float4 positionCS : SV_POSITION;
														float3 interp0 : TEXCOORD0;
														float4 interp1 : TEXCOORD1;
														#if UNITY_ANY_INSTANCING_ENABLED
														uint instanceID : CUSTOM_INSTANCE_ID;
														#endif
														#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
														uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
														#endif
														#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
														uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
														#endif
														#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
														FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
														#endif
													};

													PackedVaryings PackVaryings(Varyings input)
													{
														PackedVaryings output;
														output.positionCS = input.positionCS;
														output.interp0.xyz = input.positionWS;
														output.interp1.xyzw = input.texCoord0;
														#if UNITY_ANY_INSTANCING_ENABLED
														output.instanceID = input.instanceID;
														#endif
														#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
														output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
														#endif
														#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
														output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
														#endif
														#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
														output.cullFace = input.cullFace;
														#endif
														return output;
													}
													Varyings UnpackVaryings(PackedVaryings input)
													{
														Varyings output;
														output.positionCS = input.positionCS;
														output.positionWS = input.interp0.xyz;
														output.texCoord0 = input.interp1.xyzw;
														#if UNITY_ANY_INSTANCING_ENABLED
														output.instanceID = input.instanceID;
														#endif
														#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
														output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
														#endif
														#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
														output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
														#endif
														#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
														output.cullFace = input.cullFace;
														#endif
														return output;
													}

													// --------------------------------------------------
													// Graph

													// Graph Properties
													CBUFFER_START(UnityPerMaterial)
													float cutoff;
													float2 TilingA;
													float2 OffsetA;
													CBUFFER_END



													TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
													SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

													// Graph Functions

													void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
													{
														Out = UV * Tiling + Offset;
													}

													void Unity_Add_float3(float3 A, float3 B, out float3 Out)
													{
														Out = A + B;
													}

													// Graph Vertex
													struct VertexDescription
													{
														float3 Position;
														float3 Normal;
														float3 Tangent;
													};

													VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
													{
														VertexDescription description = (VertexDescription)0;
														description.Position = IN.ObjectSpacePosition;
														description.Normal = IN.ObjectSpaceNormal;
														description.Tangent = IN.ObjectSpaceTangent;
														return description;
													}

													// Graph Pixel
													struct SurfaceDescription
													{
														float3 BaseColor;
														float3 Emission;
														float Alpha;
														float AlphaClipThreshold;
													};

													SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
													{
														SurfaceDescription surface = (SurfaceDescription)0;
														float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
														float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
														float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
														Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
														float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
														float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
														float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
														float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
														float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
														float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
														float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
														Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
														surface.BaseColor = (_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.xyz);
														surface.Emission = IN.WorldSpacePosition;
														surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
														surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
														return surface;
													}

													// --------------------------------------------------
													// Build Graph Inputs

													VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
													{
														VertexDescriptionInputs output;
														ZERO_INITIALIZE(VertexDescriptionInputs, output);

														output.ObjectSpaceNormal = input.normalOS;
														output.ObjectSpaceTangent = input.tangentOS;
														output.ObjectSpacePosition = input.positionOS;

														return output;
													}

													SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
													{
														SurfaceDescriptionInputs output;
														ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





														output.WorldSpacePosition = input.positionWS;
														output.uv0 = input.texCoord0;
													#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
													#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
													#else
													#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
													#endif
													#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

														return output;
													}


													// --------------------------------------------------
													// Main

													#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
													#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
													#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

													ENDHLSL
												}
												Pass
												{
														// Name: <None>
														Tags
														{
															"LightMode" = "Universal2D"
														}

														// Render State
														Cull Back
														Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
														ZTest LEqual
														ZWrite Off

														// Debug
														// <None>

														// --------------------------------------------------
														// Pass

														HLSLPROGRAM

														// Pragmas
														#pragma target 2.0
														#pragma only_renderers gles gles3 glcore
														#pragma multi_compile_instancing
														#pragma vertex vert
														#pragma fragment frag

														// DotsInstancingOptions: <None>
														// HybridV1InjectedBuiltinProperties: <None>

														// Keywords
														// PassKeywords: <None>
														// GraphKeywords: <None>

														// Defines
														#define _SURFACE_TYPE_TRANSPARENT 1
														#define _AlphaClip 1
														#define _NORMALMAP 1
														#define _NORMAL_DROPOFF_TS 1
														#define ATTRIBUTES_NEED_NORMAL
														#define ATTRIBUTES_NEED_TANGENT
														#define ATTRIBUTES_NEED_TEXCOORD0
														#define VARYINGS_NEED_POSITION_WS
														#define VARYINGS_NEED_TEXCOORD0
														#define FEATURES_GRAPH_VERTEX
														/* WARNING: $splice Could not find named fragment 'PassInstancing' */
														#define SHADERPASS SHADERPASS_2D
														/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

														// Includes
														#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
														#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
														#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
														#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
														#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

														// --------------------------------------------------
														// Structs and Packing

														struct Attributes
														{
															float3 positionOS : POSITION;
															float3 normalOS : NORMAL;
															float4 tangentOS : TANGENT;
															float4 uv0 : TEXCOORD0;
															#if UNITY_ANY_INSTANCING_ENABLED
															uint instanceID : INSTANCEID_SEMANTIC;
															#endif
														};
														struct Varyings
														{
															float4 positionCS : SV_POSITION;
															float3 positionWS;
															float4 texCoord0;
															#if UNITY_ANY_INSTANCING_ENABLED
															uint instanceID : CUSTOM_INSTANCE_ID;
															#endif
															#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
															uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
															#endif
															#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
															uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
															#endif
															#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
															FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
															#endif
														};
														struct SurfaceDescriptionInputs
														{
															float3 WorldSpacePosition;
															float4 uv0;
														};
														struct VertexDescriptionInputs
														{
															float3 ObjectSpaceNormal;
															float3 ObjectSpaceTangent;
															float3 ObjectSpacePosition;
														};
														struct PackedVaryings
														{
															float4 positionCS : SV_POSITION;
															float3 interp0 : TEXCOORD0;
															float4 interp1 : TEXCOORD1;
															#if UNITY_ANY_INSTANCING_ENABLED
															uint instanceID : CUSTOM_INSTANCE_ID;
															#endif
															#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
															uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
															#endif
															#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
															uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
															#endif
															#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
															FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
															#endif
														};

														PackedVaryings PackVaryings(Varyings input)
														{
															PackedVaryings output;
															output.positionCS = input.positionCS;
															output.interp0.xyz = input.positionWS;
															output.interp1.xyzw = input.texCoord0;
															#if UNITY_ANY_INSTANCING_ENABLED
															output.instanceID = input.instanceID;
															#endif
															#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
															output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
															#endif
															#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
															output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
															#endif
															#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
															output.cullFace = input.cullFace;
															#endif
															return output;
														}
														Varyings UnpackVaryings(PackedVaryings input)
														{
															Varyings output;
															output.positionCS = input.positionCS;
															output.positionWS = input.interp0.xyz;
															output.texCoord0 = input.interp1.xyzw;
															#if UNITY_ANY_INSTANCING_ENABLED
															output.instanceID = input.instanceID;
															#endif
															#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
															output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
															#endif
															#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
															output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
															#endif
															#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
															output.cullFace = input.cullFace;
															#endif
															return output;
														}

														// --------------------------------------------------
														// Graph

														// Graph Properties
														CBUFFER_START(UnityPerMaterial)
														float cutoff;
														float2 TilingA;
														float2 OffsetA;
														CBUFFER_END



														TEXTURE2D(mask); SAMPLER(samplermask); float4 mask_TexelSize;
														SAMPLER(_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_Sampler_3_Linear_Repeat);

														// Graph Functions

														void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
														{
															Out = UV * Tiling + Offset;
														}

														void Unity_Add_float3(float3 A, float3 B, out float3 Out)
														{
															Out = A + B;
														}

														// Graph Vertex
														struct VertexDescription
														{
															float3 Position;
															float3 Normal;
															float3 Tangent;
														};

														VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
														{
															VertexDescription description = (VertexDescription)0;
															description.Position = IN.ObjectSpacePosition;
															description.Normal = IN.ObjectSpaceNormal;
															description.Tangent = IN.ObjectSpaceTangent;
															return description;
														}

														// Graph Pixel
														struct SurfaceDescription
														{
															float3 BaseColor;
															float Alpha;
															float AlphaClipThreshold;
														};

														SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
														{
															SurfaceDescription surface = (SurfaceDescription)0;
															float2 _Property_3d2f7a45533647099ed017de75e0e45e_Out_0 = TilingA;
															float2 _Property_a21c85c1a28c43a6998afecdcad70376_Out_0 = OffsetA;
															float2 _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3;
															Unity_TilingAndOffset_float(IN.uv0.xy, _Property_3d2f7a45533647099ed017de75e0e45e_Out_0, _Property_a21c85c1a28c43a6998afecdcad70376_Out_0, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
															float4 _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0 = SAMPLE_TEXTURE2D(mask, samplermask, _TilingAndOffset_65f364e07012467fa50e88579a44afb4_Out_3);
															float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_R_4 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.r;
															float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_G_5 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.g;
															float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_B_6 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.b;
															float _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7 = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.a;
															float _Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0 = cutoff;
															float3 _Add_68d42d7ed83442499e0803d102492c85_Out_2;
															Unity_Add_float3(IN.WorldSpacePosition, (_Property_07a09c4ea0024e7c83fd0f483d19b16e_Out_0.xxx), _Add_68d42d7ed83442499e0803d102492c85_Out_2);
															surface.BaseColor = (_SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_RGBA_0.xyz);
															surface.Alpha = _SampleTexture2D_03c50d720bb347d4a66e8e0b684c14a9_A_7;
															surface.AlphaClipThreshold = (_Add_68d42d7ed83442499e0803d102492c85_Out_2).x;
															return surface;
														}

														// --------------------------------------------------
														// Build Graph Inputs

														VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
														{
															VertexDescriptionInputs output;
															ZERO_INITIALIZE(VertexDescriptionInputs, output);

															output.ObjectSpaceNormal = input.normalOS;
															output.ObjectSpaceTangent = input.tangentOS;
															output.ObjectSpacePosition = input.positionOS;

															return output;
														}

														SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
														{
															SurfaceDescriptionInputs output;
															ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





															output.WorldSpacePosition = input.positionWS;
															output.uv0 = input.texCoord0;
														#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
														#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
														#else
														#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
														#endif
														#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

															return output;
														}


														// --------------------------------------------------
														// Main

														#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
														#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
														#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

														ENDHLSL
													}
								}
									CustomEditor "ShaderGraph.PBRMasterGUI"
															FallBack "Hidden/Shader Graph/FallbackError"
}