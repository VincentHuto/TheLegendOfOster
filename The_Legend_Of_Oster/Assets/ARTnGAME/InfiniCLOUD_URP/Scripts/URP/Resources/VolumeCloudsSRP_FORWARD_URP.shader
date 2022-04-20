Shader "Unlit/VolumeCloudsSRP_FORWARD_URP"
{
	Properties
	{
		//// CLOUDS
		_MainTex("Base (RGB)", 2D) = "black" {}
		_CloudTex("Base (RGB)", 2D) = "black" {}
		_CloudTexP("Base (RGB)", 2D) = "black" {}
		_SkyTex("Base (RGB)", 2D) = "black" {}//v2.1.15

		_ColorRamp("Colour Palette", 2D) = "gray" {}
		_Close("Close", float) = 0.0
		_Far("Far", float) = 0.0
		v3LightDir("v3LightDir", Vector) = (0,0,0)
		v3LightDirFOG("v3LightDirFOG", Vector) = (0,0,0)
		FogSky("FogSky",float) = 0.0
		_TintColor("Color Tint", Color) = (0,0,0,0)
		ClearSkyFac("Clear Sky Factor",float) = 1.0
		
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

		[Space]
		_Altitude0("Altitude (bottom)", Float) = 1500
		_Altitude1("Altitude (top)", Float) = 3500
		_FarDist("Far Distance", Float) = 30000

		[Space]
		_Scatter("Scattering Coeff", Float) = 0.008
		_HGCoeff("Henyey-Greenstein", Float) = 0.5
		_Extinct("Extinction Coeff", Float) = 0.01

		[Space]
		_SunSize("Sun Size", Range(0,1)) = 0.04
		_AtmosphereThickness("Atmoshpere Thickness", Range(0,5)) = 1.0
		_SkyTint("Sky Tint", Color) = (.5, .5, .5, 1)
		_GroundColor("Ground", Color) = (.369, .349, .341, 1)
		_Exposure("Exposure", Float) = 3
		//v3.5 clouds
		_BackShade("Back shade of cloud top", Float) = 1
		_UndersideCurveFactor("Underside Curve Factor", Float) = 0
		//v3.5.1
		_NearZCutoff("Away from camera Cutoff", Float) = -2
		_HorizonYAdjust("Adjust horizon Height", Float) = 0
		_FadeThreshold("Fade Near", Float) = 0
		//v3.5.3
		_InteractTexture("_Interact Texture", 2D) = "white" {}
		_InteractTexturePos("Interact Texture Pos", Vector) = (1 ,1, 0, 0)
		_InteractTextureAtr("Interact Texture Attributes - 2multi 2offset", Vector) = (1 ,1, 0, 0)
		_InteractTextureOffset("Interact Texture offsets", Vector) = (0 ,0, 0, 0) //v4.0
		//v2.1.19
		_fastest("Fastest mode", Int) = 0
		_LocalLightPos("Local Light Pos & Intensity", Vector) = (0 ,0, 0, 0) //local light position (x,y,z) and intensity (w)			 
		_LocalLightColor("Local Light Color & Falloff", Vector) = (0 , 0, 0, 2) //w = _LocalLightFalloff
		//v2.1.24
		_HorizonZAdjust("Adjust cloud depth", Float) = 1
		//v4.1f
		_mobileFactor("Adjust to 0 to fix Android lighting", Float) = 1
		_alphaFactor("Adjust to 0 to fix Android lighting", Float) = 1
		_invertX("Mirror X", Float) = 0
		_invertRay("Mirror Ray", Float) = 1
		_WorldSpaceCameraPosC("Camera", Vector) = (0 , 0, 0, 1)
		//v4.8
		varianceAltitude1("varianceAltitude1", Float) = 0
		//v4.8.6
		turbidity("turbidity", Float) = 2
		//// END CLOUDS

		//[HideInInspector]_MainTex("Base (RGB)", 2D) = "white" {}
		_SunThreshold("sun thres", Color) = (0.87, 0.74, 0.65,1)
		_SunColor("sun color", Color) = (1.87, 1.74, 1.65,1)
		_BlurRadius4("blur", Color) = (0.00325, 0.00325, 0,0)
		_SunPosition("sun pos", Color) = (111, 11,339, 11)
	}

		HLSLINCLUDE

		//#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl" //unity 2018.3
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			//FOG URP
//#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
//#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/Colors.hlsl"
#include "ClassicNoise3D.hlsl"
			//END FOG URP

		//#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/SurfaceInput.hlsl"
		//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		//#include "PostProcessing/Shaders/StdLib.hlsl" //unity 2018.1-2
		//#include "UnityCG.cginc"

	TEXTURE2D(_MainTex);
	TEXTURE2D(_ColorBuffer);
	TEXTURE2D(_Skybox);

	SAMPLER(sampler_MainTex);
	SAMPLER(sampler_ColorBuffer);
	SAMPLER(sampler_Skybox);

	float farCamDistFactor;
	////////// CLOUDS
	int4 _SceneFogMode;
	float4 _SceneFogParams;
	//v4.8
	float _invertX = 0;
	float _invertRay = 1;
	float3 _WorldSpaceCameraPosC;
	float varianceAltitude1 = 0;

	//v4.1f
	float _mobileFactor;
	float _alphaFactor;

	//v3.5.3
	sampler2D _InteractTexture;
	float4 _InteractTexturePos;
	float4 _InteractTextureAtr;
	float4 _InteractTextureOffset; //v4.0

	float _NearZCutoff;
	float _HorizonYAdjust;
	float _HorizonZAdjust;
	float _FadeThreshold;

	//v3.5 clouds	
	float _BackShade;
	float _UndersideCurveFactor;

	//VFOG
	float4x4 _WorldClip;

	float _SampleCount0 = 2;
	float _SampleCount1 = 3;
	int _SampleCountL = 4;

	sampler3D _NoiseTex1;
	sampler3D _NoiseTex2;
	float _NoiseFreq1 = 3.1;
	float _NoiseFreq2 = 35.1;
	float _NoiseAmp1 = 5;
	float _NoiseAmp2 = 1;
	float _NoiseBias = -0.2;

	float3 _Scroll1 = float3 (0.01, 0.08, 0.06);
	float3 _Scroll2 = float3 (0.01, 0.05, 0.03);

	float _Altitude0 = 1500;
	float _Altitude1 = 3500;
	float _FarDist = 30000;

	float _Scatter = 0.008;
	float _HGCoeff = 0.5;
	float _Extinct = 0.01;

	//float3 _SkyTint;
	float _SunSize;
	float3 _GroundColor; //v4.0
	float _Exposure; //v4.0

	uniform float4 _CloudTex_TexelSize;
	uniform float4 _CloudTexP_TexelSize;//v4.8.2

	//uniform sampler2D _MainTex;
	//uniform sampler2D _CloudTex;
	//uniform sampler2D _CloudTexP;
	TEXTURE2D(_CloudTex);
	SAMPLER(sampler_CloudTex);
	TEXTURE2D(_CloudTexP);
	SAMPLER(sampler_CloudTexP);
	TEXTURE2D(_SkyTex);
	SAMPLER(sampler_SkyTex);

	float frameFraction = 0;
	//uniform sampler2D _SkyTex;//v2.1.15
	//uniform sampler2D_float _CameraDepthTexture;

	//SM v1.7
	//uniform sampler2D _ColorRamp;
	//uniform float _Close;
	//uniform float _Far;
	//uniform float3 v3LightDir;		// light source
	//uniform float FogSky;
	//fixed4 _TintColor; //float3(680E-8, 1550E-8, 3450E-8);
	//uniform float ClearSkyFac;
	// x = fog height
	// y = FdotC (CameraY-FogHeight)
	// z = k (FdotC > 0.0)
	// w = a/2
	//uniform float4 _HeightParams;

	// x = start distance
	//uniform float4 _DistanceParams;

	//int4 _SceneFogMode; // x = fog mode, y = use radial flag
	//float4 _SceneFogParams;
//#ifndef UNITY_APPLY_FOG
	//half4 unity_FogColor;
	//half4 unity_FogDensity;
//#endif	

	//uniform float4 _MainTex_TexelSize;

	// for fast world space reconstruction
	//uniform float4x4 _FrustumCornersWS;
	//uniform float4 _CameraWS;

	//SM v1.7
	//uniform float luminance, Multiplier1, Multiplier2, Multiplier3, bias, lumFac, contrast, turbidity;
	//uniform float mieDirectionalG = 0.7,0.913; 
	//float mieDirectionalG;
	//float mieCoefficient;//0.054
	//float reileigh;

	//uniform float e = 2.71828182845904523536028747135266249775724709369995957;
	//uniform float pi = 3.141592653589793238462643383279502884197169;
	//uniform float n = 1.0003;
	//uniform float N = 2.545E25;
	//uniform float pn = 0.035;
	//uniform float3 lambda = float3(680E-9, 550E-9, 450E-9);
	//uniform float3 K = float3(0.686, 0.678, 0.666);//const vec3 K = vec3(0.686, 0.678, 0.666);
	//uniform float v = 4.0;
	//uniform float rayleighZenithLength = 8.4E3;
	//uniform float mieZenithLength = 1.25E3;
	//uniform float EE = 1000.0;
	//uniform float sunAngularDiameterCos = 0.999956676946448443553574619906976478926848692873900859324;
	//// 66 arc seconds -> degrees, and the cosine of that
	//float cutoffAngle = 3.141592653589793238462643383279502884197169 / 1.95;
	//float steepness = 1.5;
	//v2.1.19
	int _fastest;
	float4 _LocalLightPos;
	float4 _LocalLightColor;
	///////// END CLOUDS





	float _Blend;
	
	TEXTURE2D(_CameraDepthTexture);
	SAMPLER(sampler_CameraDepthTexture);
	half4 _CameraDepthTexture_ST;

	half4 _SunThreshold = half4(0.87, 0.74, 0.65, 1);

	half4 _SunColor = half4(0.87, 0.74, 0.65, 1);
	uniform half4 _BlurRadius4 = half4(2.5 / 768, 2.5 / 768, 0.0, 0.0);
	uniform half4 _SunPosition = half4(111, 11, 339, 11);
	uniform half4 _MainTex_TexelSize;

#define SAMPLES_FLOAT 16.0f
#define SAMPLES_INT 16
	

	//FOG URP /////////////////////
	// Vertex manipulation
	float2 TransformTriangleVertexToUV(float2 vertex)
	{
		float2 uv = (vertex + 1.0) * 0.5;
		return uv;
	}
	///////float4x4 unity_CameraInvProjection; //ALREADY DEFINED in LIBS ABOVE
	//TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	//TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
	TEXTURE2D(_NoiseTex);
	SAMPLER(sampler_NoiseTex);
	//TEXTURE2D_SAMPLER2D(_NoiseTex, sampler_NoiseTex);

	//URP v0.1
	// #pragma multi_compile FOG_LINEAR FOG_EXP FOG_EXP2
	#pragma multi_compile _ RADIAL_DIST
	#pragma multi_compile _ USE_SKYBOX

	float _DistanceOffset;
	float _Density;
	float _LinearGrad;
	float _LinearOffs;
	float _Height;
	float _cameraRoll;
	//WORLD RECONSTRUCT	
	//float4x4 _InverseView;
	//float4x4 _camProjection;	////TO REMOVE
	// Fog/skybox information
	half4 _FogColor;
	samplerCUBE _SkyCubemap;
	half4 _SkyCubemap_HDR;
	half4 _SkyTint;
	half _SkyExposure;
	float _SkyRotation;
	float4 _cameraDiff;
	float _cameraTiltSign;
	float _NoiseDensity;
	float _NoiseScale;
	float3 _NoiseSpeed;
	float _NoiseThickness;
	float _OcclusionDrop;
	float _OcclusionExp;
	int noise3D = 0;
	//END FOG URP /////////////////


	/////////////////// MORE FOG URP ///////////////////////////////////////////////////////
	/////////////////// MORE FOG URP ///////////////////////////////////////////////////////
	/////////////////// MORE FOG URP ///////////////////////////////////////////////////////
	// Applies one of standard fog formulas, given fog coordinate (i.e. distance)
	half ComputeFogFactorA(float coord) /// REDFINED, SO CHANGED NAME
	{
		float fog = 0.0;
#if FOG_LINEAR
		// factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
		fog = coord * _LinearGrad + _LinearOffs;
#elif FOG_EXP
		// factor = exp(-density*z)
		fog = _Density * coord;
		fog = exp2(-fog);
#else // FOG_EXP2
		// factor = exp(-(density*z)^2)
		fog = _Density * coord;
		fog = exp2(-fog * fog);
#endif
		return saturate(fog);
	}

	half ComputeFogFactorB(float coord)
	{
		float fogFac = 0.0;
		if (_SceneFogMode.x == 1) // linear
		{
			// factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
			fogFac = coord * _SceneFogParams.z + _SceneFogParams.w;
		}
		if (_SceneFogMode.x == 2) // exp
		{
			// factor = exp(-density*z)
			fogFac = _SceneFogParams.y * coord; fogFac = exp2(-fogFac);
		}
		if (_SceneFogMode.x == 3) // exp2
		{
			// factor = exp(-(density*z)^2)
			fogFac = _SceneFogParams.x * coord; fogFac = exp2(-fogFac * fogFac);
		}
		return saturate(fogFac);
	}

	// Distance-based fog
	float ComputeDistance(float3 ray, float depth)
	{
		float dist;
#if RADIAL_DIST
		dist = length(ray * depth);
#else
		dist = depth * _ProjectionParams.z;
#endif
		// Built-in fog starts at near plane, so match that by
		// subtracting the near value. Not a perfect approximation
		// if near plane is very large, but good enough.
		dist -= _ProjectionParams.y;
		return dist;
	}
	////LOCAL LIGHT
	float4 localLightColor;
	float4 localLightPos;
	/////////////////// SCATTER
	bool doDistance;
	bool doHeight;
	// Distance-based fog
	uniform float4 _CameraWS;
	uniform float4x4 _FrustumCornersWS;
	//SM v1.7
	uniform float luminance, Multiplier1, Multiplier2, Multiplier3, bias, lumFac, contrast, turbidity;
	//uniform float mieDirectionalG = 0.7,0.913; 
	float mieDirectionalG;
	float mieCoefficient;//0.054
	float reileigh;
	//SM v1.7
	uniform sampler2D _ColorRamp;
	uniform float _Close;
	uniform float _Far;
	uniform float3 v3LightDir;		// light source
	uniform float3 v3LightDirFOG;
	uniform float FogSky;
	float4 _TintColor; //float3(680E-8, 1550E-8, 3450E-8);
	float4 _TintColorL;
	float4 _TintColorK;
	uniform float ClearSkyFac;
	uniform float4 _HeightParams;
	// x = start distance
	uniform float4 _DistanceParams;

	//int4 _SceneFogMode; // x = fog mode, y = use radial flag
	//float4 _SceneFogParams;
#ifndef UNITY_APPLY_FOG
	//half4 unity_FogColor; //ALREADY DEFINED !!!!!!!!!!
	half4 unity_FogDensity;
#endif	

	uniform float e = 2.71828182845904523536028747135266249775724709369995957;
	uniform float pi = 3.141592653589793238462643383279502884197169;
	uniform float n = 1.0003;
	uniform float N = 2.545E25;
	uniform float pn = 0.035;
	uniform float3 lambda = float3(680E-9, 550E-9, 450E-9);
	uniform float3 K = float3(0.686, 0.678, 0.666);//const vec3 K = vec3(0.686, 0.678, 0.666);
	uniform float v = 4.0;
	uniform float rayleighZenithLength = 8.4E3;
	uniform float mieZenithLength = 1.25E3;
	uniform float EE = 1000.0;
	uniform float sunAngularDiameterCos = 0.999956676946448443553574619906976478926848692873900859324;
	// 66 arc seconds -> degrees, and the cosine of that
	float cutoffAngle = 3.141592653589793238462643383279502884197169 / 1.95;
	float steepness = 1.5;
	// Linear half-space fog, from https://www.terathon.com/lengyel/Lengyel-UnifiedFog.pdf
	float ComputeHalfSpace(float3 wsDir)
	{
		//float4 _HeightParams = float4(1,1,1,1);
		//wsDir.y = wsDir.y - abs(11.2*_cameraDiff.x);// -0.4;// +abs(11.2*_cameraDiff.x);

		float3 wpos = _CameraWS.xyz + wsDir; // _CameraWS + wsDir;
		float FH = _HeightParams.x;
		float3 C = _CameraWS.xyz;
		float3 V = wsDir;
		float3 P = wpos;
		float3 aV = _HeightParams.w * V;
		float FdotC = _HeightParams.y;
		float k = _HeightParams.z;
		float FdotP = P.y - FH;
		float FdotV = wsDir.y;
		float c1 = k * (FdotP + FdotC);
		float c2 = (1 - 2 * k) * FdotP;
		float g = min(c2, 0.0);
		g = -length(aV) * (c1 - g * g / abs(FdotV + 1.0e-5f));
		return g;
	}

	//SM v1.7
	float3 totalRayleigh(float3 lambda) {
		float pi = 3.141592653589793238462643383279502884197169;
		float n = 1.0003; // refraction of air
		float N = 2.545E25; //molecules per air unit volume 								
		float pn = 0.035;
		return (8.0 * pow(pi, 3.0) * pow(pow(n, 2.0) - 1.0, 2.0) * (6.0 + 3.0 * pn)) / (3.0 * N * pow(lambda, float3(4.0, 4.0, 4.0)) * (6.0 - 7.0 * pn));
	}

	float rayleighPhase(float cosTheta)
	{
		return (3.0 / 4.0) * (1.0 + pow(cosTheta, 2.0));
	}

	float3 totalMie(float3 lambda, float3 K, float T)
	{
		float pi = 3.141592653589793238462643383279502884197169;
		float v = 4.0;
		float c = (0.2 * T) * 10E-18;
		return 0.434 * c * pi * pow((2.0 * pi) / lambda, float3(v - 2.0, v - 2.0, v - 2.0)) * K;
	}

	float hgPhase(float cosTheta, float g)
	{
		float pi = 3.141592653589793238462643383279502884197169;
		return (1.0 / (4.0*pi)) * ((1.0 - pow(g, 2.0)) / pow(abs(1.0 - 2.0*g*cosTheta + pow(g, 2.0)), 1.5));
	}

	float sunIntensity(float zenithAngleCos)
	{
		float cutoffAngle = 3.141592653589793238462643383279502884197169 / 1.95;//pi/
		float steepness = 1.5;
		float EE = 1000.0;
		return EE * max(0.0, 1.0 - exp(-((cutoffAngle - acos(zenithAngleCos)) / steepness)));
	}

	float logLuminance(float3 c)
	{
		return log(c.r * 0.2126 + c.g * 0.7152 + c.b * 0.0722);
	}

	float3 tonemap(float3 HDR)
	{
		float Y = logLuminance(HDR);
		float low = exp(((Y*lumFac + (1.0 - lumFac))*luminance) - bias - contrast / 2.0);
		float high = exp(((Y*lumFac + (1.0 - lumFac))*luminance) - bias + contrast / 2.0);
		float3 ldr = (HDR.rgb - low) / (high - low);
		return float3(ldr);
	}
	/////////////////// END SCATTER
	half _Opacity;
	struct Varyings
	{
		//float2 uv        : TEXCOORD0;
		//float4 vertex : SV_POSITION;
		//UNITY_VERTEX_OUTPUT_STEREO

		float4 position : SV_POSITION;// SV_Position;
		float2 texcoord : TEXCOORD0;
		float3 ray : TEXCOORD1;
		float2 uvFOG : TEXCOORD2;
		float4 interpolatedRay : TEXCOORD3;

		float3 FarCam : TEXCOORD4; //URP
		//float2 uv_depth : TEXCOORD5; //URP

		UNITY_VERTEX_OUTPUT_STEREO
	};
	/////////////////// END MORE FOG URP ////////////////////////////////////////////////////
	/////////////////// END MORE FOG URP ////////////////////////////////////////////////////
	/////////////////// END MORE FOG URP ////////////////////////////////////////////////////

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
//#if UNITY_UV_STARTS_AT_TOP
		float2 uv1 : TEXCOORD1;
//#endif		
		float3 FarCam : TEXCOORD2; //URP
		///float2 uv_depth : TEXCOORD3; //URP
	};

	struct v2f_radial {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 blurVector : TEXCOORD1;
	};

	//FOR URP - REDEFINED ABOVE
	//struct Varyings
	//{
	//	float2 uv        : TEXCOORD0;
	//	float4 vertex : SV_POSITION;
	//	UNITY_VERTEX_OUTPUT_STEREO
	//};

	float Linear01DepthA(float2 uv)
	{
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		return SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
#else
		return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
#endif
	}

	float4 FragGrey(v2f i) : SV_Target
	{
		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
		half4 colorB = SAMPLE_TEXTURE2D(_ColorBuffer, sampler_ColorBuffer, i.uv.xy);		
		return color/2 + colorB/2;
	}

	half4 fragScreen(v2f i) : SV_Target{

			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			
			half4 colorA = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
		#if UNITY_UV_STARTS_AT_TOP
																				
			half4 colorB = SAMPLE_TEXTURE2D(_ColorBuffer, sampler_ColorBuffer, i.uv1.xy);
		#else
																				 
			half4 colorB = SAMPLE_TEXTURE2D(_ColorBuffer, sampler_ColorBuffer, i.uv.xy);//v1.1
		#endif
			half4 depthMask = saturate(colorB * _SunColor);
			return  1.0f - (1.0f - colorA) * (1.0f - depthMask);
	}


	half4 fragAdd(v2f i) : SV_Target{

		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		//half4 colorA = tex2D(_MainTex, i.uv.xy);
		half4 colorA = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
#if UNITY_UV_STARTS_AT_TOP
		//half4 colorB = tex2D(_ColorBuffer, i.uv1.xy);
		half4 colorB = SAMPLE_TEXTURE2D(_ColorBuffer, sampler_ColorBuffer, i.uv1.xy);
#else
		//half4 colorB = tex2D(_ColorBuffer, i.uv.xy);
		half4 colorB = SAMPLE_TEXTURE2D(_ColorBuffer, sampler_ColorBuffer, i.uv.xy);
#endif
		half4 depthMask = saturate(colorB * _SunColor);
		return 1 * colorA + depthMask;
	}

	struct Attributes
	{
		float4 positionOS       : POSITION;
		float2 uv               : TEXCOORD0;
	};

	v2f vert(Attributes v) {//v2f vert(AttributesDefault v) { //appdata_img v) {
							//v2f o;
		v2f o = (v2f)0;
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		
		VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);		

		o.pos = float4(vertexInput.positionCS.xyz, 1.0);
		float2 uv = v.uv;
		o.uv = uv;
		o.uv1 = uv.xy;

		return o;
	}

	v2f_radial vert_radial(Attributes v) {
	
		v2f_radial o = (v2f_radial)0;
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);		

		VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
		o.pos = float4(vertexInput.positionCS.xyz, 1.0);
		float2 uv = v.uv;		

		//#if UNITY_UV_STARTS_AT_TOP
				//uv = uv * float2(1.0, -1.0) + float2(0.0, 1.0);
		//#endif

		o.uv.xy = uv;
		o.blurVector = (_SunPosition.xy - uv.xy) * _BlurRadius4.xy;

		return o;
	}

	half4 frag_radial(v2f_radial i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		half4 color = half4(0,0,0,0);
		for (int j = 0; j < SAMPLES_INT; j++)
		{			
			half4 tmpColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
			color += tmpColor;
			i.uv.xy += i.blurVector;
		}
		return color / SAMPLES_FLOAT;
	}

	half TransformColor(half4 skyboxValue) {
		return dot(max(skyboxValue.rgb - _SunThreshold.rgb, half3(0, 0, 0)), half3(1, 1, 1)); // threshold and convert to greyscale
	}

	half4 frag_depth(v2f i) : SV_Target{

		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

	#if UNITY_UV_STARTS_AT_TOP			
			float depthSample = Linear01DepthA(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv1.xy));
	#else			
			float depthSample = Linear01DepthA(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv.xy));
	#endif
		
		half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
		//float depthSample = Linear01Depth(depthSample, _ZBufferParams);
		depthSample = Linear01Depth(depthSample, _ZBufferParams);

		// consider maximum radius
	#if UNITY_UV_STARTS_AT_TOP
		half2 vec = _SunPosition.xy - i.uv1.xy;
	#else
		half2 vec = _SunPosition.xy - i.uv.xy;
	#endif
		half dist = saturate(_SunPosition.w - length(vec.xy));

		half4 outColor = 0;
		
		if (depthSample < 0.018) {
			outColor = TransformColor(tex) * dist;
		}
		return outColor * 1;
	}

	inline half Luminance(half3 rgb)
	{
		//return dot(rgb, unity_ColorSpaceLuminance.rgb);
		return dot(rgb, rgb);
	}

	half4 frag_nodepth(v2f i) : SV_Target{

		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

	#if UNITY_UV_STARTS_AT_TOP
			//float4 sky = (tex2D(_Skybox, i.uv1.xy));
			float4 sky = SAMPLE_TEXTURE2D(_Skybox, sampler_Skybox, i.uv1.xy);
	#else
			//float4 sky = (tex2D(_Skybox, i.uv.xy));
			float4 sky = SAMPLE_TEXTURE2D(_Skybox, sampler_Skybox, i.uv.xy);
	#endif

			//float4 tex = (tex2D(_MainTex, i.uv.xy));
			half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
			//sky = float4(0.3, 0.05, 0.05,  1);
			/// consider maximum radius
	#if UNITY_UV_STARTS_AT_TOP
			half2 vec = _SunPosition.xy - i.uv1.xy;
	#else
			half2 vec = _SunPosition.xy - i.uv.xy;
	#endif
			half dist = saturate(_SunPosition.w - length(vec));

			half4 outColor = 0;

			// find unoccluded sky pixels
			// consider pixel values that differ significantly between framebuffer and sky-only buffer as occluded

			if (Luminance(abs(sky.rgb - tex.rgb)) < 0.2) {
				outColor = TransformColor(tex) * dist;
				//outColor = TransformColor(sky) * dist;
			}

			return outColor * 1;
	}


		/////// FOG URP //////////////////////////
		/////// FOG URP //////////////////////////
		/////// FOG URP //////////////////////////
		// Vertex shader that procedurally outputs a full screen triangle
	Varyings Vertex(uint vertexID : SV_VertexID) //Varyings Vertex(Attributes v)
	{
		//Varyings o = (Varyings)0;
		//UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		//VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);

		//float4 vpos = float4(vertexInput.positionCS.x, vertexInput.positionCS.y, vertexInput.positionCS.z, 1.0);
		//o.position = vpos;

		////o.position.z = 0.1;

		//float2 uv = v.uv;
		//
		//o.texcoord = float2(uv.x,uv.y);
		////o.texcoord = uv.xy;

		//float far = _ProjectionParams.z ;
		//float3 rayPers = -mul(unity_CameraInvProjection, vpos.xyzz * far).xyz;
		//rayPers.y = rayPers.y + 1* abs(_cameraDiff.x * 15111);

		//o.ray = rayPers;//lerp(rayPers, rayOrtho, isOrtho);
		//o.uvFOG = uv.xy;
		//half index = vpos.z;
		//o.interpolatedRay.xyz = _FrustumCornersWS[(int)index] ;// vpos;  // _FrustumCornersWS[(int)index];
		//o.interpolatedRay.w = index;
		//return o;

		// Render settings
		float far = _ProjectionParams.z;
		float2 orthoSize = unity_OrthoParams.xy;
		float isOrtho = _ProjectionParams.w; // 0: perspective, 1: orthographic
											 // Vertex ID -> clip space vertex position
		float x = (vertexID != 1) ? -1 : 3;
		float y = (vertexID == 2) ? -3 : 1;
		float3 vpos = float3(x, y, 1.0);

		// Perspective: view space vertex position of the far plane
		float3 rayPers = mul(unity_CameraInvProjection, vpos.xyzz * far).xyz;
		//rayPers.y = rayPers.y - abs(_cameraDiff.x * 15111);

		// Orthographic: view space vertex position
		float3 rayOrtho = float3(orthoSize * vpos.xy, 0);

		Varyings o;
		o.position = float4(vpos.x, -vpos.y, 1, 1);
		o.texcoord = (vpos.xy + 1) / 2;
		o.ray = lerp(rayPers, rayOrtho, isOrtho);

		//MINE
		float3 vA = vpos;
		float deg = _cameraRoll;
		float alpha = deg * 3.14 / 180.0;
		float sina, cosa;
		sincos(alpha, sina, cosa);
		float2x2 m = float2x2(cosa, -sina, sina, cosa);

		float3 tmpV = float3(mul(m, vA.xy), vA.z).xyz;
		float2 uvFOG = TransformTriangleVertexToUV(tmpV.xy);
		o.uvFOG = uvFOG.xy;

		half index = vpos.z;
		o.interpolatedRay.xyz = vpos;  // _FrustumCornersWS[(int)index];
		o.interpolatedRay.w = index;

		/// CLOUDS
		//half index = v.vertex.z;
		//v.vertex.z = 0.1;
		//o.vertex = UnityObjectToClipPos(v.vertex);
		//o.uv = v.texcoord.xy;
		//v3.5		
//		o.uv_depth = MultiplyUV(UNITY_MATRIX_TEXTURE0, o.texcoord);// v.texcoord);
//#if UNITY_UV_STARTS_AT_TOP
//		if (_MainTex_TexelSize.y < 0) {
//			o.uv_depth.y = 1 - o.uv_depth.y;
//		}
//#endif	
		float2 XY = vpos.xy / index * _HorizonZAdjust;// o.vertex.xy / o.vertex.w * _HorizonZAdjust; //v2.1.24

		//XY.y = XY.y + 0.25*_HorizonZAdjust*_cameraDiff.x*_cameraTiltSign; //0.5 - 0.03, 1 - 0.00,
		//XY.y = XY.y + 0.03*_HorizonZAdjust*_cameraDiff.x*_cameraTiltSign;
		float3 forward = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));
		//XY.y = XY.y - 1.3*_HorizonZAdjust*forward.y*1;
		//XY.y = XY.y - 1.8*_HorizonZAdjust*forward.y; //0.5 - 1.8, 0.25 - 5.1, 1 - 0
//		XY.y = XY.y - 3.6*(1-_HorizonZAdjust)*forward.y*_HorizonZAdjust; //URP v1.0
//		XY.x = XY.x / 2;

		//PERSPECTIVE PROJECTION
		float4 farClip = float4(XY, 1, 1);
		float4 farWorld = mul(_WorldClip, farClip);
		float3 farWorldScaled = farWorld.xyz / farWorld.w * (30000/farCamDistFactor); //URP v0.1
		o.FarCam = farWorldScaled - _WorldSpaceCameraPosC;//-_CameraWS
		////END CLOUDS


		return o;
	}

	float3 ComputeViewSpacePosition(Varyings input, float z)
	{
		// Render settings
		float near = _ProjectionParams.y;
		float far = _ProjectionParams.z;
		float isOrtho = unity_OrthoParams.w; // 0: perspective, 1: orthographic
											 // Z buffer sample
											 // Far plane exclusion
#if !defined(EXCLUDE_FAR_PLANE)
		float mask = 1;
#elif defined(UNITY_REVERSED_Z)
		float mask = z > 0;
#else
		float mask = z < 1;
#endif

		// Perspective: view space position = ray * depth
		float lindepth = Linear01DepthA(input.texcoord.xy);
		lindepth = Linear01Depth(lindepth, _ZBufferParams);// Linear01Depth(lindepth);
		//float3 vposPers = input.ray * lindepth; //v5.0c
		float3 vposPers = input.ray * lindepth * ((30000/4) / far); //v5.0c

		//if (Linear01DepthA(input.texcoord.xy) ==0) {
		//	vposPers = input.ray;
		//}

		// Orthographic: linear depth (with reverse-Z support)
#if defined(UNITY_REVERSED_Z)
		float depthOrtho = -lerp(far, near, z);
#else
		float depthOrtho = -lerp(near, far, z);
#endif
		
		// Orthographic: view space position
		float3 vposOrtho = float3(input.ray.xy, depthOrtho);

		// Result: view space position
		return lerp(vposPers, vposOrtho, isOrtho) * mask;
	}

	half4 VisualizePosition(Varyings input, float3 pos)
	{
		const float grid = 5;
		const float width = 3;

		pos *= grid;

		// Detect borders with using derivatives.
		float3 fw = fwidth(pos);
		half3 bc = saturate(width - abs(1 - 2 * frac(pos)) / fw);

		// Frequency filter
		half3 f1 = smoothstep(1 / grid, 2 / grid, fw);
		half3 f2 = smoothstep(2 / grid, 4 / grid, fw);
		bc = lerp(lerp(bc, 0.5, f1), 0, f2);

		// Blend with the source color.
		half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord);
		c.rgb = SRGBToLinear(lerp(LinearToSRGB(c.rgb), bc, 0.5));

		return c;
	}

		///////////////// FRAGMENT /////////////////////////////////

	float4x4 _InverseView;

	float2 WorldToScreenPos(float3 pos) {
		pos = normalize(pos - _WorldSpaceCameraPos)*(_ProjectionParams.y + (_ProjectionParams.z - _ProjectionParams.y)) + _WorldSpaceCameraPos;
		float2 uv = float2(0, 0);
		float4 toCam = mul(unity_WorldToCamera, float4(pos.xyz, 1));
		float camPosZ = toCam.z;
		float height = 2 * camPosZ / unity_CameraProjection._m11;
		float width = _ScreenParams.x / _ScreenParams.y * height;
		uv.x = (toCam.x + width / 2) / width;
		uv.y = (toCam.y + height / 2) / height;
		return uv;
	}

	float2 raySphereIntersect(float3 r0, float3 rd, float3 s0, float sr) {

		float a = dot(rd, rd);
		float3 s0_r0 = r0 - s0;
		float b = 2.0 * dot(rd, s0_r0);
		float c = dot(s0_r0, s0_r0) - (sr * sr);
		float disc = b * b - 4.0 * a* c;
		if (disc < 0.0) {
			return float2(-1.0, -1.0);
		}
		else {
			return float2(-b - sqrt(disc), -b + sqrt(disc)) / (2.0 * a);
		}
	}

	float3x3 rotationMatrix(float3 axis, float angle)
	{
		axis = normalize(axis);
		float s = sin(angle);
		float c = cos(angle);
		float oc = 1.0 - c;

		return float3x3 (oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s,
			oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s,
			oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c);
	}
	float4x4 rotationMatrix4(float3 axis, float angle)
	{
		axis = normalize(axis);
		float s = sin(angle);
		float c = cos(angle);
		float oc = 1.0 - c;

		return float4x4 (oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s, 0.0,
			oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s, 0.0,
			oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c, 0.0,
			0.0, 0.0, 0.0, 1.0);

	}

	/////////////// CLOUDS 

	//v3.5
	//v3.5 clouds
	float UVRandom(float2 uv)
	{
		float f = dot(float2(12.9898, 78.233), uv);
		return frac(43758.5453 * sin(f));
	}

	float SampleNoise(float3 uvw, float _Altitude1, float _NoiseAmp1, float Alpha)//v3.5.3
	{

		float AlphaFactor = clamp(Alpha*_InteractTextureAtr.w, _InteractTextureAtr.x, 1);

		const float baseFreq = 1e-5;

		float4 uvw1 = float4(uvw * _NoiseFreq1 * baseFreq, 0);
		float4 uvw2 = float4(uvw * _NoiseFreq2 * baseFreq, 0);

		uvw1.xyz += _Scroll1.xyz * _Time.x;
		uvw2.xyz += _Scroll2.xyz * _Time.x;

		float n1 = tex3Dlod(_NoiseTex1, uvw1).a;
		float n2 = tex3Dlod(_NoiseTex2, uvw2).a;
		float n = n1 * _NoiseAmp1*AlphaFactor + n2 * _NoiseAmp2;//v3.5.3

		n = saturate(n + _NoiseBias);

		float y = uvw.y - _Altitude0;
		float h = _Altitude1 * 1 - _Altitude0;//v3.5.3
		n *= smoothstep(0, h * (0.1 + _UndersideCurveFactor), y);
		n *= smoothstep(0, h * 0.4, h - y);

		return n;
	}

	float HenyeyGreenstein(float cosine)
	{
		float g2 = _HGCoeff * _HGCoeff;
		return 0.5 * (1 - g2) / pow(abs(1 + g2 - 2 * _HGCoeff * cosine), 1.5);
	}

	float Beer(float depth)
	{
		return exp(-_Extinct * depth * _BackShade);  // return exp(-_Extinct * depth); //_BackShade v3.5
	}

	float BeerPowder(float depth)
	{
		return exp(-_Extinct * depth) * (1 - exp(-_Extinct * 2 * depth));
	}

	float MarchLight(float3 pos, float rand, float _Altitude1, float _NoiseAmp1, float Alpha)
	{
		float3 light = float3(v3LightDir.x, _invertRay * v3LightDir.y, v3LightDir.z);//v3LightDir;// _WorldSpaceLightPos0.xyz; //v4.8
		float stride = (_Altitude1 - pos.y) / (light.y * _SampleCountL);

		//v3.5.2
		if (_invertRay * v3LightDir.y < 0) {//if(_WorldSpaceLightPos0.y < 0){  //v4.8
											//if(_WorldSpaceLightPos0.y > -0.01){         
			stride = (_Altitude0 - pos.y + _WorldSpaceCameraPosC.y) / (light.y * _SampleCountL * 15); //higher helps frame rate A LOT
																									  //}
		}

		pos += light * stride * rand;

		float depth = 0;
		UNITY_LOOP for (int s = 0; s < _SampleCountL; s++)
		{
			depth += SampleNoise(pos, _Altitude1, _NoiseAmp1, Alpha) * stride;
			pos += light * stride;
		}

		return BeerPowder(depth);
	}
	//v3.5 clouds

	////////////// END CLOUDS


	half4 Fragment(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float3 forward = mul((float3x3)(unity_WorldToCamera), float3(0, 0, 1));
		//float zsample = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.texcoord);
		//float zsample = Linear01Depth(input.texcoord.xy); //CORRECT WAY

		float zsample = Linear01DepthA(input.texcoord.xy);
		float depth = Linear01Depth(zsample * (zsample < 1.0), _ZBufferParams);// Linear01Depth(lindepth);
		//float3 vposPers = input.ray * lindepth;

		

		float3 vpos = ComputeViewSpacePosition(input, zsample);
		float3 wpos = mul(_InverseView, float4(vpos, 1)).xyz;
		//float depth = Linear01Depth(zsample * (zsample < 1.0));
	//	float depth =  Linear01Depth(zsample * (zsample < 1.0)); //////// CHANGE 2 URP
		//float depth = Linear01DepthA(input.texcoord.xy);
		

		//float depthSample = Linear01Depth(input.texcoord.xy); //CORRECT WAY
		//if (depthSample > 0.00000001) { //affect frontal
		//if (depthSample == 0) { 		//affect background
		//if (depthSample < 0.00000001) {	//affect background
		//if (depth > 0.00000001) {
			//return float4(1, 0, 0, 1);
		//}
		//else {
			//return float4(1, 1, 0, 1);
		//}

		//if (depth ==0) {
			//depth = 1; //EXPOSE BACKGROUND AS FORGROUND TO GET SCATTERING
		//}

		float4 wsDir = depth * float4(input.ray, 1); // input.interpolatedRay;	

		//_CameraWS = float4(85.8, -102.19,-10,1);
		float4 wsPos = (_CameraWS) + wsDir;// _CameraWS + wsDir; //////// CHANGE 1 URP
		//return wsPos;
		

		//// CLOUDS
		float4 wsDirA = depth * input.interpolatedRay; //dpth * i.interpolatedRay;
		float4 wsPosA = _CameraWS + wsDirA;

		//return ((wsPos) *0.1);

		///// SCATTER
		//float3 lightDirection = float3(-v3LightDir.x - 0 * _cameraDiff.w * forward.x, -v3LightDir.y - 0 * _cameraDiff.w * forward.y, v3LightDir.z);
		//float3 lightDirection = float3(v3LightDir.x - 0 * _cameraDiff.w * forward.x, -v3LightDir.y - 0 * _cameraDiff.w * forward.y, -v3LightDir.z);
		//float3 lightDirection = float3(-v3LightDir.x - 0 * _cameraDiff.w * forward.x, -v3LightDir.y - 0 * _cameraDiff.w * forward.y, v3LightDir.z);
		float3 lightDirection = float3(-v3LightDirFOG.x - 0 * _cameraDiff.w * forward.x, -v3LightDirFOG.y - 0 * _cameraDiff.w * forward.y, v3LightDirFOG.z);


		//int noise3D = 0;
		half4 noise;
		half4 noise1;
		half4 noise2;
		if (noise3D == 0) {
			float fixFactor1 = 0;
			float fixFactor2 = 0;
			float dividerScale = 1; //1
			float scaler1 = 1.00; //0.05
			float scaler2 = 0.8; //0.01
			float scaler3 = 0.3; //0.01
			float signer1 = 0.004 / (dividerScale * 1.0);//0.4 !!!! (0.005 for 1) (0.4 for 0.05) //0.004
			float signer2 = 0.004 / (dividerScale * 1.0);//0.001

			if (_cameraDiff.w < 0) {
				fixFactor1 = -signer1 * 90 * 2 * 2210 / 1 * (dividerScale / 1);//2210
				fixFactor2 = -signer2 * 90 * 2 * 2210 / 1 * (dividerScale / 1);
			}
			float hor1 = -_cameraDiff.w * signer1 *_cameraDiff.y * 2210 / 1 * (dividerScale / 1) - 1.2 * _WorldSpaceCameraPos.x * 10 + fixFactor1;
			float hor2 = -_cameraDiff.w * signer2 *_cameraDiff.y * 2210 / 1 * (dividerScale / 1) - 1.2 * _WorldSpaceCameraPos.x * 10 + fixFactor2;
			float hor3 = -_cameraDiff.w * signer2 *_cameraDiff.y * 1210 / 1 * (dividerScale / 1) - 1.2 * _WorldSpaceCameraPos.x * 2 + fixFactor2;

			float vert1 = _cameraTiltSign * _cameraDiff.x * 0.77 * 1.05 * 160 + 0.0157*_cameraDiff.y * (pow((input.texcoord.x - 0.1), 2)) - 0.3 * _WorldSpaceCameraPos.y * 30
				- 2 * 0.33 * _WorldSpaceCameraPos.z * 2.13 + 50 * abs(cos(_WorldSpaceCameraPos.z * 0.01)) + 35 * abs(sin(_WorldSpaceCameraPos.z * 0.005));

			float vert2 = _cameraTiltSign * _cameraDiff.x * 0.20 * 1.05 * 160 + 0.0157*_cameraDiff.y * (pow((input.texcoord.x - 0.1), 2)) - 0.3 * _WorldSpaceCameraPos.y * 30
				- 1 * 0.33 * _WorldSpaceCameraPos.z * 3.24 + 75 * abs(sin(_WorldSpaceCameraPos.z * 0.02)) + 85 * abs(cos(_WorldSpaceCameraPos.z * 0.01));

			float vert3 = _cameraTiltSign * _cameraDiff.x * 0.10 * 1.05 * 70 + 0.0117*_cameraDiff.y * (pow((input.texcoord.x - 0.1), 2)) - 0.3 * _WorldSpaceCameraPos.y * 30
				- 1 * 1.03 * _WorldSpaceCameraPos.z * 3.24 + 75 * abs(sin(_WorldSpaceCameraPos.z * 0.02)) + 85 * abs(cos(_WorldSpaceCameraPos.z * 0.01));

			noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, 1 * (dividerScale * (float2(input.texcoord.x*scaler1 * 1, input.texcoord.y*scaler1))
				+ (-0.001*float2((0.94)*hor1, vert1)) + 3 * abs(cos(_Time.y *1.22* 0.012)))) * 2 * 9;
			noise1 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, 1 * (dividerScale * (input.texcoord.xy*scaler2)
				+ (-0.001*float2((0.94)*hor2, vert2) + 3 * abs(cos(_Time.y *1.22* 0.010))))) * 3 * 9;
			noise2 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, 1 * (dividerScale * (input.texcoord.xy*scaler3)
				+ (-0.001*float2((0.94)*hor3, vert3) + 1 * abs(cos(_Time.y *1.22* 0.006))))) * 3 * 9;
		}
		else {

			/////////// NOISE 3D //////////////
			const float epsilon = 0.0001;

			float2 uv = input.texcoord * 4.0 + float2(0.2, 1) * _Time.y * 0.01;

			/*#if defined(SNOISE_AGRAD) || defined(SNOISE_NGRAD)
			#if defined(THREED)
			float3 o = 0.5;
			#else
			float2 o = 0.5;
			#endif
			#else*/
			float o = 0.5 * 1.5;
			//#endif

			float s = 0.011;

			/*#if defined(SNOISE)
			float w = 0.25;
			#else*/
			float w = 0.02;
			//#endif

			//#ifdef FRACTAL
			for (int i = 0; i < 5; i++)
				//#endif
			{
				float3 coord = wpos + float3(_Time.y * 3 * _NoiseSpeed.x,
					_Time.y * _NoiseSpeed.y,
					_Time.y * _NoiseSpeed.z);
				float3 period = float3(s, s, 1.0) * 1111;



				//#if defined(CNOISE)
				o += cnoise(coord * 0.17 * _NoiseScale) * w;

				float3 pointToCamera = (wpos - _WorldSpaceCameraPos) * 0.47;
				int steps = 2;
				float stepCount = 1;
				float step = length(pointToCamera) / steps;
				for (int j = 0; j < steps; j++) {
					//ray trace noise												
					float3 coordAlongRay = _WorldSpaceCameraPos + normalize(pointToCamera) * step
						+ float3(_Time.y * 6 * _NoiseSpeed.x,
							_Time.y * _NoiseSpeed.y,
							_Time.y * _NoiseSpeed.z);
					o += 1.5*cnoise(coordAlongRay * 0.17 * _NoiseScale) * w * 1;
					//stepCount++;
					if (depth < 0.99999) {
						o += depth * 45 * _NoiseThickness;
					}
					step = step + step;
				}

				s *= 2.0;
				w *= 0.5;
			}
			noise = float4(o, o, o, 1);
			noise1 = float4(o, o, o, 1);
			noise2 = float4(o, o, o, 1);
		}

		float cosTheta = dot(normalize(wsDir.xyz), lightDirection);
		cosTheta = dot(normalize(wsDir.xyz), -lightDirection);

		float lumChange = clamp(luminance * pow(abs(((1 - depth) / (_OcclusionDrop * 0.1 * 2))), _OcclusionExp), luminance, luminance * 2);
		if (depth <= _OcclusionDrop * 0.1 * 1) {
			luminance = lerp(4 * luminance, 1 * luminance, (0.001 * 1) / (_OcclusionDrop * 0.1 - depth + 0.001));
		}

		float3 up = float3(0.0, 1.0, 0.0); //float3(0.0, 0.0, 1.0);			
		float3 lambda = float3(680E-8 + _TintColorL.r * 0.000001, 550E-8 + _TintColorL.g * 0.000001, 450E-8 + _TintColorL.b * 0.000001);
		float3 K = float3(0.686 + _TintColorK.r * 0.1, 0.678 + _TintColorK.g * 0.1, 0.666 + _TintColorK.b * 0.1);
		float  rayleighZenithLength = 8.4E3;
		float  mieZenithLength = 1.25E3;
		float  pi = 3.141592653589793238462643383279502884197169;
		float3 betaR = totalRayleigh(lambda) * reileigh * 1000;
		float3 lambda1 = float3(_TintColor.r, _TintColor.g, _TintColor.b)* 0.0000001;//  680E-8, 1550E-8, 3450E-8); //0.0001//0.00001
		lambda = lambda1;
		float3 betaM = totalMie(lambda1, K, turbidity * Multiplier2) * mieCoefficient;
		float zenithAngle = acos(max(0.0, dot(up, normalize(lightDirection))));
		float sR = rayleighZenithLength / (cos(zenithAngle) + 0.15 * pow(abs(93.885 - ((zenithAngle * 180.0) / pi)), -1.253));
		float sM = mieZenithLength / (cos(zenithAngle) + 0.15 * pow(abs(93.885 - ((zenithAngle * 180.0) / pi)), -1.253));
		float  rPhase = rayleighPhase(cosTheta*0.5 + 0.5);
		float3 betaRTheta = betaR * rPhase;
		float  mPhase = hgPhase(cosTheta, mieDirectionalG) * Multiplier1;
		float3 betaMTheta = betaM * mPhase;
		float3 Fex = exp(-(betaR * sR + betaM * sM));
		float  sunE = sunIntensity(dot(lightDirection, up));
		float3 Lin = ((betaRTheta + betaMTheta) / (betaR + betaM)) * (1 - Fex) + sunE * Multiplier3*0.0001;
		float  sunsize = 0.0001;
		float3 L0 = 1.5 * Fex + (sunE * 1.0 * Fex)*sunsize;
		float3 FragColor = tonemap(Lin + L0);//tonemap(Lin + L0);
		///// END SCATTER

		///////////////return float4(FragColor,1);

		//occlusion !!!!
		float4 sceneColor = float4(0, 0, 0, 0);// Multiplier3 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord.xy);


		//return sceneColor+ (float4(FragColor, 1));
		//return 0 + (float4(FragColor, 1));


		float3 subtractor = saturate(pow(abs(dot(normalize(input.ray), normalize(lightDirection))),36)) - (float3(1, 1, 1)*depth * 1);
		if (depth < _OcclusionDrop * 0.1) {
			FragColor = saturate(FragColor * pow(abs((depth / (_OcclusionDrop * 0.1))), _OcclusionExp));
		}
		else {
			if (depth < 0.9999) {
				FragColor = saturate(FragColor * pow(abs((depth / (_OcclusionDrop * 0.1))), 0.001));
			}
		}

		
		////return float4(FragColor, 1);


		//SCATTER
		int doHeightA = 1;
		int doDistanceA = 1;
		//float g = ComputeDistance(input.ray, depth) - _DistanceOffset/_WorldSpaceCameraPos.y;
		float g =  ComputeDistance(input.ray, depth) - _DistanceOffset;
		if (doDistanceA == 1) {
			//g += ComputeDistance(input.ray, depth) - (_DistanceOffset - 100*_WorldSpaceCameraPos.y);
			g += ComputeDistance(input.ray, depth) -_DistanceOffset ;
		}
		if (doHeightA == 1) {
			g +=  ComputeHalfSpace(wpos);
			//g += ComputeHalfSpace(wpos*0.5) - _DistanceOffset;
			//g += ComputeHalfSpace(wpos*0.05);
			//float4 wsDir1 = depth * input.interpolatedRay;
			//float3 wpos1 = _WorldSpaceCameraPos + (wsDir1);// +wsDir; // _CameraWS + wsDir;
			//float FH = _HeightParams.x;
			//float3 C = _WorldSpaceCameraPos.xyz;
			//float3 V = (wsDir1);
			//float3 P = wpos1;
			//float3 aV = _HeightParams.w * V			*		1;
			//float FdotC = _HeightParams.y;
			//float k = _HeightParams.z;
			//float FdotP = P.y - FH;
			//float FdotV = (wsDir1).y;
			//float c1 = k * (FdotP + FdotC);
			//float c2 = (1 - 2 * k) * FdotP;
			//float g1 = min(c2, 0.0);
			//g1 = -length(aV) * (c1 - g1 * g1 / abs(FdotV + 1.0e-5f));
			//g += g1 * 1;
		}

		g = g * pow(abs((noise.r + 1 * noise1.r + _NoiseDensity * noise2.r * 1)), 1.2)*0.3;
		
		half fogFac = ComputeFogFactorA(max(0.0, g));
		if (zsample <= 1-0.999995) {
			//if (zsample >= 0.999995) {
			if (FogSky <= 0) {
				fogFac = 1.0* ClearSkyFac;
			}
			else {
				if (doDistanceA) {
					fogFac = fogFac * ClearSkyFac;
				}
			}
		}

		float4 Final_fog_color = lerp(unity_FogColor + float4(FragColor * 1, 1), sceneColor, fogFac);

		//return Final_fog_color;

		float fogHeight = _Height;
		half fog = ComputeFogFactorA(max(0.0, g));

		//local light
		float3 visual = 0;// VisualizePosition(input, wpos);
		if (1 == 1) {

			float3 light1 = localLightPos.xyz;
			float dist1 = length(light1 - wpos);

			float2 screenPos = WorldToScreenPos(light1);
			float lightRadius = localLightColor.w;

			float dist2 = length(screenPos - float2(input.texcoord.x, input.texcoord.y * 0.62 + 0.23));
			if (
				length(_WorldSpaceCameraPos - wpos) < length(_WorldSpaceCameraPos - light1) - lightRadius
				&&
				dot(normalize(_WorldSpaceCameraPos - wpos), normalize(_WorldSpaceCameraPos - light1)) > 0.95// 0.999
				) { //occlusion
			}
			else {
				float factorOcclusionDist = length(_WorldSpaceCameraPos - wpos) - (length(_WorldSpaceCameraPos - light1) - lightRadius);
				float factorOcclusionDot = dot(normalize(_WorldSpaceCameraPos - wpos), normalize(_WorldSpaceCameraPos - light1));

				Final_fog_color = lerp(Final_fog_color,
					Final_fog_color  * (1 - ((11 - dist2) / 11))
					+ Final_fog_color * float4(2 * localLightColor.x, 2 * localLightColor.y, 2 * localLightColor.z, 1)*(11 - dist2) / 11,
					(localLightPos.w * saturate(1 * 0.1458 / pow(dist2, 0.95))
						+ 0.04*saturate(pow(1 - input.uvFOG.y * (1 - fogHeight), 1.0)) - 0.04)
				);
			}
		}

		//return sceneColor/2 + Final_fog_color/2;

#if USE_SKYBOX
		// Look up the skybox color.
		//half3 skyColor = DecodeHDR(texCUBE(_SkyCubemap, input.ray), _SkyCubemap_HDR);
		half3 skyColor = half3(1, 1, 1);
		skyColor *= _SkyTint.rgb * _SkyExposure;// *unity_ColorSpaceDouble;
		// Lerp between source color to skybox color with fog amount.
		return lerp(half4(skyColor, 1), sceneColor, fog);
#else
		// Lerp between source color to fog color with the fog amount.
		half4 skyColor = lerp(_FogColor, sceneColor, saturate(fog));
		float distToWhite = (Final_fog_color.r - 0.99) + (Final_fog_color.g - 0.99) + (Final_fog_color.b - 0.99);		



		/////////////////////////////////// CLOUDS
		//ALL PROJECTIONS
		//float depthVOLIN = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,i.uv_depth));  //SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv_depth);
		//float4 PixelWorldW = (i.worldMul*depthVOLIN) + i.worldAdd;
		//float3 PixelWorld = PixelWorldW.xyz/PixelWorldW.w;

		//PERSPECTIVE PROJECTION
		float depthVOLIN = depth; //URP

		//v2.1.19
		if (_fastest == 0) {
			depthVOLIN = 1;
		}

		

		//wsPos + _WorldSpaceCameraPosC + float3(0, _HorizonYAdjust, 0);//
		//float3 PixelWorld =  (input.FarCam * depthVOLIN) + _WorldSpaceCameraPosC + float3(0, _HorizonYAdjust, 0); //ORTHO -14000 //v3.5.1
		float3 PixelWorld = wpos - _WorldSpaceCameraPosC; //URP v0.2

		//float3 PixelWorld = (input.FarCam * depthVOLIN) + 0 + float3(0, _HorizonYAdjust + 1700*_cameraDiff.x * _cameraTiltSign, 0); //ORTHO -14000 //v3.5.1	
		//*_cameraDiff.x*_cameraTiltSign

		float3 sky = Final_fog_color.xyz;//v3.5a
		//float3 ray = -(PixelWorld)* 0.00001; //input.ray;// -(PixelWorld)*0.00001;
		float3 ray = (PixelWorld)*0.0001; //URP v0.2

		int samples = lerp(_SampleCount1, _SampleCount0, ray.y);


	

	
		//v4.8.2
		float dist0 = _Altitude0 / ray.y;
		float3 pos = ray * (dist0);
		pos = pos + _WorldSpaceCameraPosC;
		
		_Altitude1 = _Altitude1 + varianceAltitude1 * (cos(pos.x * 0.001) * 1500 + cos(pos.z * 0.001) * 2000 + cos(pos.x * 0.0005) * 300 + cos(pos.z * 0.0003) * 600);
		_Altitude1 = _Altitude1 + varianceAltitude1 * (abs(cos(pos.x * 0.001)) * 1500 + abs(sin(pos.z * 0.001)) * 2000);

		float dist1 = _Altitude1 / ray.y;
		float stride = (dist1 - dist0) / samples;


		

		//v2.1.19
		float alphaFactor = 1;
		if (_fastest == 0) {
			alphaFactor = _alphaFactor; //4.1f
		}

		//URP v0.1
		if (length(wpos - _WorldSpaceCameraPosC) < _NearZCutoff) {
			return float4(0, 0, 0, 0);
		}
		
		//if (ray.y < 0.01 || dist0 >= _FarDist || ray.z < _NearZCutoff) return float4(0, 0, 0, alphaFactor); //v4.1
		if (ray.y < 0.01 || dist0 >= _FarDist || length(wpos - _WorldSpaceCameraPosC) < _NearZCutoff) return float4(0, 0, 0, alphaFactor); //v4.1
		//if (ray.y < 0.1 || dist0 >= _FarDist || length(wpos - _WorldSpaceCameraPosC) < _NearZCutoff) return float4(0, 0, 0, alphaFactor); //v4.1 //URP v0.1

		float3 light = v3LightDir;// 4.8
		float hg = HenyeyGreenstein(dot(ray, light));
		float2 uv = input.texcoord + _Time.x;
		float offs = UVRandom(uv) * (dist1 - dist0) / samples;
		
		//v4.8.2
		pos = pos + ray * (offs);


		

				
		//v3.5.1
		float dist = length(wsPosA.xyz - _WorldSpaceCameraPosC.xyz);
		if (dist < _FadeThreshold) {
			return float4(sky, 1);
		}


		

		float3 acc = float3(0,0,0);

		//v2.1.19
		float3 intensityMod = _LocalLightPos.w * _LocalLightColor.xyz * pow(10, 7);
		float depthA = 0;		
		float preDevide = samples / _Exposure;
		float3 groundColor1 = _GroundColor.rgb*0.0006;

		float3 _LightColor0 = float3(1, 1, 1);

		float3 light1 =  _LightColor0.rgb * _SkyTint.rgb; // URP
		float scatterHG = _Scatter * hg;

		

		UNITY_LOOP for (int s = 0; s < samples; s++)
		{		    
			float4 texInteract = tex2Dlod(_InteractTexture, 0.0003*float4(
				_InteractTexturePos.x*pos.x + _InteractTexturePos.z*-_Scroll1.x * _Time.x + _InteractTextureOffset.x,
				_InteractTexturePos.y*pos.z + _InteractTexturePos.w*-_Scroll1.z * _Time.x + _InteractTextureOffset.y,
				0, 0));
			//return float4(texInteract * Final_fog_color);
			
			float diffPos = length(_LocalLightPos.xyz - pos);
			texInteract.a = texInteract.a + clamp(_InteractTextureAtr.z * 0.1*(1 - 0.00024*diffPos), -1.5, 0);

			_NoiseAmp2 = _NoiseAmp2 * clamp(texInteract.a*_InteractTextureAtr.w, _InteractTextureAtr.y, 1);

			float n = SampleNoise(pos, _Altitude1, _NoiseAmp1, texInteract.a);
			float expose = 0.00001;
			if (s < preDevide) {  
				expose = 0;
			}			

			if (n >= expose) //v4.0 added >= than only >, for better underlight control
			{
				float density = n * stride;
				float rand = UVRandom(uv + s + 1);			
				float scatter = density * scatterHG * MarchLight(pos, rand * 0.001, _Altitude1, _NoiseAmp1, texInteract.a); //v4.0
				float3 beer1 = BeerPowder(depthA) * intensityMod / pow(diffPos, _LocalLightColor.w);
				float beer2 = 1 - Beer(depthA);
				acc += light1 * scatter * BeerPowder(depthA) + beer2 * groundColor1 + (beer2*0.01*_LightColor0 + scatter) * beer1;//v2.1.19
				depthA += density;
			}
			pos += ray * stride;
		}

		//return float4(zsample, zsample, zsample, 1);
		//return float4(depth, depth, depth, 1);
		//return float4(dist, dist, dist, 1);
		//return float4(0, 0, 0, 1);
		//return float4(texInteract.rgb, 1);
		//return float4(stride, stride, stride, 1);
		//return float4(ray, 1);
		//return float4(PixelWorld, 1);
		//return float4(wsPos);
		//return float4(acc, 1);
		
		if (_mobileFactor > 0) { //v4.1f
			acc += Beer(depthA) * sky + FragColor * _SunSize*acc;
			acc = lerp(acc, sky*0.96, saturate(((dist0) / (_FarDist*0.5))) + 0.03);			
		}

		float4 finalColor = float4(acc, 1);
		
		return float4(finalColor.rgb, pow(lerp(Beer(depthA), 0.96, saturate(((dist0) / (_FarDist*0.5))) + 0.03), 2));
		/////////// END CLOUDS

		//return Final_fog_color * _FogColor + float4(visual, 0);		
#endif	
	}
	
	//v3.5.2
	float gaussed(float sig, float pos) {
		//from "Fitting a Gaussion Function to binned data" paper
		float numer = pos * pos;
		float denom = sig * sig;
		float ratio = numer / denom;
		return (0.39894 / sig) * exp(-0.5*ratio);
	}

	half4 ComputeFogAddCombine(v2f i) : SV_Target
	{
		//v2.1.19
		float fac1 = _WorldClip[2][3];
		float fac2 = _WorldClip[0][3];
		float fac3 = _WorldClip[1][3];	
		half4 cloudColor = SAMPLE_TEXTURE2D(_SkyTex, sampler_SkyTex,  i.uv.xy + float2(1 * 0 * 0.0014 , -1 * 0 * 0.0553));
		//half4 cloudColor = SAMPLE_TEXTURE2D(_SkyTex, sampler_SkyTex, i.uv.xy*fac1 + float2(fac2* 0.002, -fac3* 0.004));
		//half4 cloudColor = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex,  i.uv.xy + float2(1 * 0 * 0.0014 , -1 * 0 * 0.0553));
		//v4.8.3 - interpolate colors																								
		//half4 cloudColorP = SAMPLE_TEXTURE2D(_CloudTexP, sampler_CloudTexP, i.uv +float2(-fac2 * 0.002, fac3 * 0.004));// i.uv + float2(0 * 0.019, 0 * 0.022));
		half4 cloudColorP = SAMPLE_TEXTURE2D(_CloudTexP, sampler_CloudTexP, i.uv + float2(0 * 0.019, 0 * 0.022));
		cloudColor = lerp(cloudColorP, cloudColor, frameFraction);		
		return float4(cloudColor.rgb, cloudColor.a);
	}

	half4 ComputeFogAdd(v2f i, bool distance, bool height, bool splitFrames) : SV_Target
	{
		//v2.1.19
		float fac1 = 1;
		float fac2 = 0;
		float fac3 = 0;
		if (splitFrames) {
			fac1 = _WorldClip[2][3];
			fac2 = _WorldClip[0][3];
			fac3 = _WorldClip[1][3];
		}

		float2 iuvs = i.uv.xy;
		half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, iuvs);// i.uv); //v4.8.3

		float2 usvINNER = iuvs * fac1 + float2(fac2, fac3);
		half4 cloudColor = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, usvINNER); ///URP

		//v2.1.19
		float2 uvs = iuvs; //v4.8
		if (_invertX == 1) {
			uvs = float2(1 - iuvs.x, iuvs.y);
		}
		half4 skybox = SAMPLE_TEXTURE2D(_SkyTex, sampler_SkyTex, uvs*fac1 + half2(fac2, fac3)); //v4.8.3
	
		//v2.1.13 - Gauss automated
		if (1 == 1) {
			int rows = 5;
			int iterations = 0.5*(rows - 1);
			float blurStrength = 0.4;
			UNITY_LOOP for (int i1 = -iterations; i1 <= iterations; ++i1) {
				UNITY_LOOP for (int j = -iterations; j <= iterations; ++j) {				
					//v2.1.19
					cloudColor += gaussed(3, float(i1))*SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, float2(fac2, fac3) + fac1 * float2(iuvs.x + blurStrength * i1*_CloudTex_TexelSize.x, iuvs.y + blurStrength * j*_CloudTex_TexelSize.y));
				}
			}
			cloudColor = 1.2*cloudColor / (rows);
		}

		//v2.1.19
		if (splitFrames) {
			if (iuvs.x < -2 * fac2) {
				float distFromEdge1 = iuvs.x + 2 * fac2;			
				cloudColor = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, float2((0 - fac2) + (distFromEdge1 / 2), iuvs.y + fac3));
			}
			if (iuvs.x > 1 - 2 * fac2) {			
				float distFromEdge2 = iuvs.x - (1 - 2 * fac2);
				cloudColor = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, float2((1 - (1 * fac2)) + (distFromEdge2 / 2), iuvs.y + fac3));
			}
			if (iuvs.y > 1 - 2 * fac3) {		
				//depending on distance 
				float distFromEdge = iuvs.y - (1 - 2 * fac3);
				cloudColor = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, float2(iuvs.x + fac2, (1 - (1 * fac3)) + (distFromEdge / 2)));
			}	
			if (iuvs.y < -2 * fac3) {
				float distFromEdge3 = iuvs.y + 2 * fac3;				
				cloudColor = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, float2(iuvs.x + fac2,  (0 - fac3) + (distFromEdge3 / 2)));
			}

			//CORNERS - sample from edge
			if (iuvs.y >  1 - 2 * fac3) {
				if (iuvs.x <  -2 * fac2) {
					cloudColor = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, float2(0 - 2 * fac2, 1 - 2 * fac3));
				}
			}
			if (iuvs.y >  1 - 2 * fac3) {
				if (iuvs.x > 1 - 2 * fac2) {
					cloudColor = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, float2(1 - 2 * fac2, 1 - 2 * fac3));
				}
			}
			if (iuvs.y < -2 * fac3) {
				if (iuvs.x <  -2 * fac2) {
					cloudColor = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, float2(0 - 2 * fac2,  -2 * fac3));
				}
			}
			if (iuvs.y < -2 * fac3) {
				if (iuvs.x > 1 - 2 * fac2) {
					cloudColor = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, float2(1 - 2 * fac2,  -2 * fac3));
				}
			}		
		}
		
		//v2.1.14
		float4 sum = float4 (0,0,0,0);
		float w = 0;
		float weights = 0;
		const float G_WEIGHTS[9] = { 1.0, 0.8, 0.65, 0.5, 0.4, 0.2, 0.1, 0.05, 0.025 };

		float4 sampleA = cloudColor;// SAMPLE_TEXTURE2D(_CloudTex, i.uv.xy); // v2.1.15
		float texelX = _CloudTex_TexelSize.x / 6.0;
		float texelY = _CloudTex_TexelSize.y / 6.0;
		float4 sampleB = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, iuvs.xy + float2(texelX,texelY));
		float4 sampleC = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, iuvs.xy + float2(-texelX,-texelY));
		float4 sampleD = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, iuvs.xy + float2(2 * texelX,2 * texelY));
		float4 sampleE = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, iuvs.xy + float2(-2 * texelX,-2 * texelY));
		float4 sampleF = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, iuvs.xy + float2(3 * texelX,3 * texelY));
		float4 sampleG = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, iuvs.xy + float2(-3 * texelX,-3 * texelY));
		float4 sampleH = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, iuvs.xy + float2(4 * texelX,4 * texelY));
		float4 sampleI = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, iuvs.xy + float2(-4 * texelX,-4 * texelY));
		float4 sampleJ = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, iuvs.xy + float2(5 * texelX,5 * texelY));
		float4 sampleK = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, iuvs.xy + float2(-5 * texelX,-5 * texelY));

		//v2.1.15
		//if pixel is white and has at least one black pixel near, darken it
		if (sampleA.a > 0.5) {//0.5 for first if //0.35 lst if
			if (sampleB.a * sampleC.a * sampleD.a * sampleE.a * sampleF.a * sampleG.a * sampleH.a * sampleI.a < 0.004) {			
				sampleA = sampleA / 2;
				sampleB = sampleB / 2;
				sampleC = sampleC / 2;
				sampleD = sampleD / 2;
				sampleE = sampleE / 2;
				sampleF = sampleF / 2;
				sampleG = sampleG / 2;			
			}
		}

		w = sampleA.a * G_WEIGHTS[0]; sum += sampleA * w; weights += w;
		w = sampleB.a * G_WEIGHTS[1]; sum += sampleB * w; weights += w;
		w = sampleC.a * G_WEIGHTS[1]; sum += sampleC * w; weights += w;
		w = sampleD.a * G_WEIGHTS[2]; sum += sampleD * w; weights += w;
		w = sampleE.a * G_WEIGHTS[2]; sum += sampleE * w; weights += w;
		w = sampleF.a * G_WEIGHTS[3]; sum += sampleF * w; weights += w;
		w = sampleG.a * G_WEIGHTS[3]; sum += sampleG * w; weights += w;
		w = sampleH.a * G_WEIGHTS[4]; sum += sampleH * w; weights += w;
		w = sampleI.a * G_WEIGHTS[4]; sum += sampleI * w; weights += w;
		w = sampleJ.a * G_WEIGHTS[5]; sum += sampleJ * w; weights += w;
		w = sampleK.a * G_WEIGHTS[5]; sum += sampleK * w; weights += w;
		sum /= weights + 1e-4f;

		//v2.1.19
		if (_fastest == 0) {
			//return float4(sceneColor.a, sceneColor.a, sceneColor.a, 1);
			//return clamp(1.5 * cloudColor*(1 - sceneColor.a), 0, 1) + clamp(1 * sceneColor, 0, 1) + clamp(float4(2 * skybox.rgb*(cloudColor.a)*(1 - sceneColor.a), 0), 0, 1);
			return clamp(1.6 * cloudColor*(1 - sceneColor.a), 0, 1) + clamp(sceneColor, 0, 1) + clamp(float4(2.2 * skybox.rgb*(cloudColor.a)*(1 - sceneColor.a), 0), 0, 1);
		}
		else {
			float4 ds1 = sum + float4((sceneColor.rgb) * (sampleA.a * 2 + sum.a / 2) / 2, 0);  //v2.1.19
			return ds1; 
		}
	}
	////// END BLEND

	half4 FragmentTWO(Varyings input) : SV_Target
	{
		float z =SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.texcoord.xy);
		float3 vpos = ComputeViewSpacePosition(input,z);
		//vpos.z = vpos.z +11110;
		float3 wpos = mul(_InverseView, float4(vpos, 1)).xyz;	
		return VisualizePosition(input, wpos);
	}

	half4 frag7(v2f i) : SV_Target{ return ComputeFogAdd(i, false, true,false); }
	half4 frag8(v2f i) : SV_Target{ return ComputeFogAdd(i, false, true,true); }
	half4 frag9(v2f i) : SV_Target{ return ComputeFogAddCombine(i); }
		/////// END FOG URP //////////////////////
		/////// END FOG URP //////////////////////
		/////// END FOG URP //////////////////////
		
		ENDHLSL
				
		Subshader {
		
			Pass{ //0
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment fragScreen

			ENDHLSL
		}

			Pass{//1
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert_radial
#pragma fragment frag_radial

			ENDHLSL
		}

			Pass{//2
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment frag_depth

			ENDHLSL
		}

			Pass{//3
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment frag_nodepth

			ENDHLSL
		}

			Pass{//4
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment fragAdd

			ENDHLSL
		}

			//PASS5
			Pass{
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM

#pragma vertex vert
#pragma fragment FragGrey

			ENDHLSL
		}


				///////////// PASS FOG
				Pass{ // 6
					ZTest Always Cull Off ZWrite Off
					HLSLPROGRAM

					#pragma vertex Vertex
					#pragma fragment Fragment

					ENDHLSL
				}
				/// 3: combine // 7
				Pass
				{
				Cull Off //v0.1 - solve issue with GL.invertculling !!!
				HLSLPROGRAM
					#pragma vertex vert
					#pragma fragment frag7
					#pragma target 3.0					
				ENDHLSL
				}
					// 4: combine split frames // 8
					Pass
				{
				HLSLPROGRAM
					#pragma vertex vert
					#pragma fragment frag8
					#pragma target 3.0				
				ENDHLSL
				}
					// 5: combine previous and current frames // 9
					Pass
				{
				HLSLPROGRAM
					#pragma vertex vert//vert
					#pragma fragment frag9
					#pragma target 3.0					
				ENDHLSL
				}
				///////////// END PASS FOG

	}
}