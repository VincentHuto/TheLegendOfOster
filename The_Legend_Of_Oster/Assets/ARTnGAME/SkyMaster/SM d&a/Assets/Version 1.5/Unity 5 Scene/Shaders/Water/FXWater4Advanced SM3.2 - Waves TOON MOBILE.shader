// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkyMaster/SM3.2 - MOBILE WATER v0.1" {

//No refraction

Properties {
	_ReflectionTex ("Internal reflection", 2D) = "white" {}
	//_RefractionTex ("Internal refraction", 2D) = "white" {}
	
	//TERRAIN DEPTH
	_DepthCameraPos ("Depth Camera Pos", Vector) = (240 ,100, 250, 3.0)	
	_ShoreContourTex ("_ShoreContourTex", 2D) = "white" {}
	_ShoreFadeFactor ("_ShoreFadeFactor", Float) = 1.0
	_TerrainScale ("Terrain Scale", Float) = 500.0
	
	_MainTex ("Fallback texture", 2D) = "black" {}
	_ShoreTex ("Shore & Foam texture ", 2D) = "black" {}
	_BumpMap ("Normals ", 2D) = "bump" {}
	
	_DistortParams ("Distortions (Bump waves, Reflection, Fresnel power, Fresnel bias)", Vector) = (1.0 ,1.0, 2.0, 1.15)
	_InvFadeParemeter ("Auto blend parameter (Edge, Shore, Distance scale)", Vector) = (0.15 ,0.15, 0.5, 1.0)
	
	_AnimationTiling ("Animation Tiling (Displacement)", Vector) = (2.2 ,2.2, -1.1, -1.1)
	_AnimationDirection ("Animation Direction (displacement)", Vector) = (1.0 ,1.0, 1.0, 1.0)

	_BumpTiling ("Bump Tiling", Vector) = (1.0 ,1.0, -2.0, 3.0)
	_BumpDirection ("Bump Direction & Speed", Vector) = (1.0 ,1.0, -1.0, 1.0)
	
	_FresnelScale ("FresnelScale", Range (0.15, 4.0)) = 0.75

	_BaseColor ("Base color", COLOR)  = ( .54, .95, .99, 0.5)
	_ReflectionColor ("Reflection color", COLOR)  = ( .54, .95, .99, 0.5)
	_SpecularColor ("Specular color", COLOR)  = ( .72, .72, .72, 1)
	
	_WorldLightDir ("Specular light direction", Vector) = (0.0, 0.1, -0.5, 0.0)
	_Shininess ("Shininess", Range (2.0, 500.0)) = 200.0
	
	_Foam ("Foam (intensity, cutoff)", Vector) = (0.1, 0.375, 0.0, 0.0)
	
	_GerstnerIntensity("Per vertex displacement", Float) = 1.0
	_GAmplitude ("Wave Amplitude", Vector) = (0.3 ,0.35, 0.25, 0.25)
	_GFrequency ("Wave Frequency", Vector) = (1.3, 1.35, 1.25, 1.25)
	_GSteepness ("Wave Steepness", Vector) = (1.0, 1.0, 1.0, 1.0)
	_GSpeed ("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)
	_GDirectionAB ("Wave Direction", Vector) = (0.3 ,0.85, 0.85, 0.25)
	_GDirectionCD ("Wave Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)
	
	_MultiplyEffect  ("Multiply Effect", Float) = 1
	
	_GerstnerIntensity1("Extra_GerstnerIntensity1", Float) = 0.0
	_GerstnerIntensity2("Extra_GerstnerIntensity2", Float) = 0.0
	
	_GerstnerIntensities ("Extra Wave Inclusion controls", Vector) = (0 ,0, 0, 0)
	_Gerstnerfactors ("Extra Wave Frequency controls", Vector) = (1 ,1, 1, 1)
	_Gerstnerfactors2 ("Extra Wave Amplitude controls", Vector) = (1 ,1, 1, 1)
	_GerstnerfactorsDir ("Extra Wave Direction controls", Vector) = (1 ,1, 1, 1)
	_GerstnerfactorsSteep ("Extra Wave Steepness controls", Vector) = (1 ,1, 1, 1)
	
	_LocalWaveVelocity ("Local Wave Velocity", Vector) = (0.1 ,0.9, 0.5, 0.5)
	_LocalWavePosition ("Local Wave Position", Vector) = (0.0 ,0.0, 0.0, 0.0)
	_LocalWaveParams ("Local Wave Height cutoff, Amp,Freq,Unaffected region", Vector) = (6 ,270, 10, 1)

	//v3.2 TOON
	_MultiplyTexture  ("Texture control", Vector) =(0.7 ,0.7, 0.5, 0.5) //v3.3
}


CGINCLUDE

	#include "UnityCG.cginc"
	#include "Lighting.cginc"
	#include "AutoLight.cginc"
	#include "./WaterIncludeSM.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	// interpolator structs
	
	struct v2f
	{
		float4 pos : SV_POSITION;
		float4 normalInterpolator : TEXCOORD0;
		float4 viewInterpolator : TEXCOORD1;
		float4 bumpCoords : TEXCOORD2;
		float4 screenPos : TEXCOORD3;
		float4 grabPassPos : TEXCOORD4;
//		float3 normal : NORMAL;
		UNITY_FOG_COORDS(5)
		
		//float3 lightDir: TEXCOORD6;
	
		//float3 vertPos: TEXCOORD7;
	//	float3 lightPos: TEXCOORD8;
	
		//float4 lightDirPosx: TEXCOORD6;
		//float4 lightPosYZvertXY: TEXCOORD7;//vertex z encoded in normalInterpolator
	
		LIGHTING_COORDS(6,7)
		
	};

	struct v2f_noGrab
	{
		float4 pos : SV_POSITION;
		float4 normalInterpolator : TEXCOORD0;
		float3 viewInterpolator : TEXCOORD1;
		float4 bumpCoords : TEXCOORD2;
		float4 screenPos : TEXCOORD3;
		UNITY_FOG_COORDS(4)
	};
	
	struct v2f_simple
	{
		float4 pos : SV_POSITION;
		float4 viewInterpolator : TEXCOORD0;
		float4 bumpCoords : TEXCOORD1;
		UNITY_FOG_COORDS(2)
	};

	// textures
	sampler2D _BumpMap;
	sampler2D _ReflectionTex;
	sampler2D _RefractionTex;
	sampler2D _ShoreTex;
	sampler2D_float _CameraDepthTexture;

	// colors in use
	uniform float4 _RefrColorDepth;
	uniform float4 _SpecularColor;
	uniform float4 _BaseColor;
	uniform float4 _ReflectionColor;
	
	// edge & shore fading
	uniform float4 _InvFadeParemeter;

	// specularity
	uniform float _Shininess;
	uniform float4 _WorldLightDir;

	// fresnel, vertex & bump displacements & strength
	uniform float4 _DistortParams;
	uniform float _FresnelScale;
	uniform float4 _BumpTiling;
	uniform float4 _BumpDirection;

	uniform float4 _GAmplitude;
	uniform float4 _GFrequency;
	uniform float4 _GSteepness;
	uniform float4 _GSpeed;
	uniform float4 _GDirectionAB;
	uniform float4 _GDirectionCD;
	
	//local
	uniform float4 _LocalWavePosition;
	uniform float4 _LocalWaveVelocity;
	uniform float4 _LocalWaveParams;
	
	// foam
	uniform float4 _Foam;
	
	//SM30
	uniform float _MultiplyEffect;
	
	//TERRAIN DEPTH
	sampler2D _ShoreContourTex;
	float3 _DepthCameraPos;
	float _ShoreFadeFactor;
	float _TerrainScale;

	//v3.2
	sampler2D _MainTex;
	float4 _MultiplyTexture;
	
	// shortcuts
	#define PER_PIXEL_DISPLACE _DistortParams.x
	#define REALTIME_DISTORTION _DistortParams.y
	#define FRESNEL_POWER _DistortParams.z
	#define VERTEX_WORLD_NORMAL i.normalInterpolator.xyz
	#define FRESNEL_BIAS _DistortParams.w
	#define NORMAL_DISPLACEMENT_PER_VERTEX _InvFadeParemeter.z
	
	
	//
	// UNDERWATER VERSION
	//
	
	uniform float4x4 _LightMatix0;
	
//	v2f vert600(appdata_full v)
//	{
//		v2f o;
//		
//		half4 worldSpaceVertex = mul(_Object2World,(v.vertex));
//		half3 vtxForAni = worldSpaceVertex.xzz;
//
//		half3 nrml;
//		half3 offsets;
//		Gerstner (
//			offsets, nrml, v.vertex.xyz, vtxForAni,						// offsets, nrml will be written
//			_GAmplitude,												// amplitude
//			_GFrequency,												// frequency
//			_GSteepness,												// steepness
//			_GSpeed,													// speed
//			_GDirectionAB,												// direction # 1, 2
//			_GDirectionCD												// direction # 3, 4
//		);
//		
//		v.vertex.xyz += offsets;
//		
//		// one can also use worldSpaceVertex.xz here (speed!), albeit it'll end up a little skewed
//		half2 tileableUv = mul(_Object2World,(v.vertex)).xz;
//		
//		o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;
//
//		o.viewInterpolator.xyz = worldSpaceVertex.xyz - _WorldSpaceCameraPos;
//
//		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//
//		ComputeScreenAndGrabPassPos(o.pos, o.screenPos, o.grabPassPos);
//		
																								//o.normalInterpolator.xyz = nrml;
																								//o.normalInterpolator.xyz =  v.normal;
//		
//		o.viewInterpolator.w = saturate(offsets.y);
//		o.normalInterpolator.w = 1;//GetDistanceFadeout(o.screenPos.w, DISTANCE_SCALE);
//		
//	//	o.normal = v.normal;
//		
//		
//		//o.lightDir = ObjSpaceLightDir(v.vertex);
//		
//	//	o.lightPos = mul(_LightMatix0,worldSpaceVertex);
//		//o.vertPos = worldSpaceVertex;
//		
//		o.lightDirPosx.xyz = ObjSpaceLightDir(v.vertex);
//		
//		o.lightPosYZvertXY.zw = worldSpaceVertex.xy;
//		o.normalInterpolator.w = worldSpaceVertex.z;
//		
//		float4 LightPos = mul(_LightMatix0,worldSpaceVertex);
//		
//		o.lightPosYZvertXY.xy = LightPos.yz;
//		o.lightDirPosx.w = LightPos.x;
//		
//		
//		UNITY_TRANSFER_FOG(o,o.pos);
//		return o;
//	}
//
//	half4 frag600( v2f i ) : SV_Target
//	{
//		half3 worldNormal = PerPixelNormal(_BumpMap, i.bumpCoords, VERTEX_WORLD_NORMAL, PER_PIXEL_DISPLACE);
//		half3 viewVector = -normalize(i.viewInterpolator.xyz);
//
//		half4 distortOffset = half4(worldNormal.xz * REALTIME_DISTORTION * 10.0, 0, 0);
//		half4 screenWithOffset = i.screenPos + distortOffset;
//		half4 grabWithOffset = i.grabPassPos + distortOffset;
//		
//		half4 rtRefractionsNoDistort = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(i.grabPassPos));
//		half refrFix = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(grabWithOffset));
//		half4 rtRefractions = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(grabWithOffset));
//		
//		#ifdef WATER_REFLECTIVE
//			half4 rtReflections = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(screenWithOffset));
//		#endif
//
//		#ifdef WATER_EDGEBLEND_ON
//		if (LinearEyeDepth(refrFix) < i.screenPos.z)
//			rtRefractions = rtRefractionsNoDistort;
//		#endif
//		
//		half3 reflectVector = normalize(reflect(viewVector, worldNormal));
//		half3 h = normalize ((_WorldLightDir.xyz) + viewVector.xyz);
//		float nh = max (0, dot (worldNormal, -h));
//		float spec = max(0.0,pow (nh, _Shininess));
//		
//		half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
//		
//		#ifdef WATER_EDGEBLEND_ON
//			half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
//			depth = LinearEyeDepth(depth);
//			edgeBlendFactors = saturate(_InvFadeParemeter * (depth-i.screenPos.w));
//			edgeBlendFactors.y = 1.0-edgeBlendFactors.y;
//		#endif
//		
//		// shading for fresnel term
//		worldNormal.xz *= _FresnelScale;
//		half refl2Refr = Fresnel(viewVector, worldNormal, FRESNEL_BIAS, FRESNEL_POWER);
//		
//		// base, depth & reflection colors
//		half4 baseColor = ExtinctColor (_BaseColor, i.viewInterpolator.w * _InvFadeParemeter.w);
//		#ifdef WATER_REFLECTIVE
//			half4 reflectionColor = lerp (rtReflections,_ReflectionColor,_ReflectionColor.a);
//		#else
//			half4 reflectionColor = _ReflectionColor;
//		#endif
//		
//		baseColor = lerp (lerp (rtRefractions, baseColor, baseColor.a), reflectionColor, refl2Refr);
//		baseColor = baseColor + spec * _SpecularColor;
//		
//		// handle foam
//		half4 foam = Foam(_ShoreTex, i.bumpCoords * 2.0);
//		baseColor.rgb += foam.rgb * _Foam.x * (edgeBlendFactors.y + saturate(i.viewInterpolator.w - _Foam.y));
//		
//		baseColor.a = edgeBlendFactors.x;
//		UNITY_APPLY_FOG(i.fogCoord, baseColor);
//		return baseColor;
//	}


half4 frag600( v2f i ) : SV_Target
	{
		half3 worldNormal = PerPixelNormal(_BumpMap, i.bumpCoords, VERTEX_WORLD_NORMAL, PER_PIXEL_DISPLACE);	

		half3 viewVector = -normalize(i.viewInterpolator.xyz);

		half4 distortOffset = half4(worldNormal.xz * REALTIME_DISTORTION , 0, 0);
		half4 screenWithOffset = i.screenPos + distortOffset;
		half4 grabWithOffset = i.grabPassPos + distortOffset;
		
		//half4 rtRefractionsNoDistort = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(i.grabPassPos));
		half refrFix = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(grabWithOffset));
		half4 rtRefractions = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(grabWithOffset));
		
		#ifdef WATER_REFLECTIVE
			half4 rtReflections = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(screenWithOffset));
		#endif

//		#ifdef WATER_EDGEBLEND_ON
//		if (LinearEyeDepth(refrFix) < i.screenPos.z)
//			rtRefractions = rtRefractionsNoDistort;
//		#endif
		
		half3 reflectVector = normalize(reflect(viewVector, worldNormal));
		half3 h = normalize ((_WorldLightDir.xyz) + viewVector.xyz);
		float nh = max (0, dot (worldNormal, -h));
		float spec = max(0.0,pow (nh, _Shininess));
		
		half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
		
		#ifdef WATER_EDGEBLEND_ON
			half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
			depth = LinearEyeDepth(depth);
			edgeBlendFactors = saturate(_InvFadeParemeter * (depth-i.screenPos.w));
			edgeBlendFactors.y = 1.0-edgeBlendFactors.y;
		#endif
		
		// shading for fresnel term
		//worldNormal.xz *= _FresnelScale;
		half refl2Refr = Fresnel(viewVector, worldNormal, FRESNEL_BIAS, FRESNEL_POWER);
		
		// base, depth & reflection colors
		//half4 baseColor = ExtinctColor (_BaseColor, i.viewInterpolator.w * _InvFadeParemeter.w + _BaseColor*11);////////// Sky Master 3.0
		
		half4 baseColor = _BaseColor - ( i.viewInterpolator.w * _InvFadeParemeter.w + _BaseColor*_MultiplyEffect) * half4(0.15, 0.03, 0.01, 0);
		
		#ifdef WATER_REFLECTIVE
			half4 reflectionColor = lerp (rtReflections,_ReflectionColor,_ReflectionColor.a);
		#else
			half4 reflectionColor = _ReflectionColor;
		#endif
		
		baseColor = lerp (lerp (rtRefractions, baseColor, baseColor.a), reflectionColor, refl2Refr);

		//baseColor = lerp (rtRefractions, baseColor, baseColor.a);

		baseColor = baseColor + spec * _SpecularColor;
		
		// handle foam
		half4 foam = Foam(_ShoreTex, i.bumpCoords);
		//baseColor.rgb += foam.rgb *_Foam.x * (edgeBlendFactors.y +  saturate(i.viewInterpolator.w - _Foam.y)) ;
		baseColor.rgb += foam.rgb *_Foam.x * (edgeBlendFactors.y) ;
//		
		baseColor.a = edgeBlendFactors.x;
		

		//v3.2
		float4 SnowTexColor = tex2D(_MainTex, i.bumpCoords);


		//MULTI LIGHTS
//		fixed atten = LIGHT_ATTENUATION(i);
//		i.lightDir = normalize(i.lightDir);
//		fixed diff = saturate(dot(i.normal,i.lightDir));
//		
//		float4 lighter = float4(_LightColor0.rgb*atten*2*diff*diff*1*diff*(1-refl2Refr)*1,1);
		
		UNITY_APPLY_FOG(i.fogCoord, baseColor);

		//v3.2
		return (baseColor+1)*SnowTexColor*0.7 ;
		//return baseColor * 1;
	}
	
	//
	// HQ VERSION
	//
	
	
	
	
	v2f vert(appdata_full v)
	{
		v2f o;
		
		half4 worldSpaceVertex = mul(unity_ObjectToWorld,(v.vertex));
		half3 vtxForAni = worldSpaceVertex.xzz;

		half3 nrml;
		half3 offsets;
		Gerstner (
			offsets, nrml, v.vertex.xyz, vtxForAni,						// offsets, nrml will be written
			_GAmplitude,												// amplitude
			_GFrequency,												// frequency
			_GSteepness,												// steepness
			_GSpeed,													// speed
			_GDirectionAB,												// direction # 1, 2
			_GDirectionCD												// direction # 3, 4
		);
		
		//half2 tileableUv = mul(_Object2World,(v.vertex)).xz;		
		
//		///MINE
//		//float2 coords = v.texcoord.xy/1;		
//		//coords = float2(tileableUv.x*11, tileableUv.y*10);		
//		float WorldScale=500;
//		WorldScale = _TerrainScale;
//		float3 CamPos = float3(250,0,250);//_WorldSpaceCameraPos;
//		CamPos =_DepthCameraPos;//_WorldSpaceCameraPos;
//		float2 Origin = float2(CamPos.x - WorldScale/2 , CamPos.z - WorldScale/2);
//		float2 UnscaledTexPoint = float2(tileableUv.x - Origin.x , tileableUv.y - Origin.y);
//		float2 ScaledTexPoint = float2(UnscaledTexPoint.x/WorldScale , UnscaledTexPoint.y/WorldScale);
//		
//		float4 tex = tex2Dlod(_ShoreContourTex,float4(ScaledTexPoint,0,0));
//		
		//v.vertex.xyz += float3(offsets.x, offsets.y *(pow(tex.r,_ShoreFadeFactor)),offsets.z);

		v.vertex.xyz += float3(offsets.x, offsets.y,offsets.z);
//		
//		
		//LOCAL WAVES
		//_LocalWaveParams ("Local Wave Height cutoff, Amp,Freq,"
		
		//if(LocalWaves == 1){
		v.vertex = mul(unity_ObjectToWorld,(v.vertex));
		float3 BoatPos = _LocalWavePosition.xyz;//float3(10,0,10);
		float3 BoatToVertex = v.vertex - BoatPos;
		float dist = length(BoatToVertex);
		
		float SpeedFactor = dot(normalize(BoatToVertex),normalize(_LocalWaveVelocity))*1.0+0.7;
		
		if(SpeedFactor<0){
			SpeedFactor=0;
		}
		
		float Yoffset =   (_LocalWaveParams.y*SpeedFactor-0) * sin(_Time.y*(_LocalWaveParams.z)+v.vertex.x+v.vertex.z)/(pow(dist,1.9)+0.1) 
							+ (_LocalWaveParams.y*SpeedFactor)* cos(_Time.y*(_LocalWaveParams.z)+v.vertex.z+1.14f+v.vertex.x)/(pow(dist,1.8)+0.1);
							
						//+ (_LocalWaveParams.y-20)* sin(_Time.y*(_LocalWaveParams.z)+v.vertex.z+1.14f)/(pow(dist,1.8)+0.1) ;
						//+ (_LocalWaveParams.y-40)* cos(_Time.y*(_LocalWaveParams.z)+v.vertex.z)/(pow(dist,2)+0.1);
						
						//_LocalWaveVelocity
		
		if(abs(Yoffset) > _LocalWaveParams.x - 0){
			Yoffset = sign(Yoffset)*(_LocalWaveParams.x + 0.08*(abs(Yoffset) - _LocalWaveParams.x));
		}
		if(dist > _LocalWaveParams.w - 0){
			v.vertex.y += 7*Yoffset;
		} 
		float _ScaleLocal = 6;
		float _SpeedLocal = 0.0005;
		float _FreqLocal = 0.0005;
		half offsetVert = ((v.vertex.x*v.vertex.x)*(v.vertex.z*v.vertex.z));
		half ValueOffset = _ScaleLocal * sin(_Time.w * _SpeedLocal + offsetVert * _FreqLocal);
		//v.vertex.y += ValueOffset;
		//}
		v.vertex = mul(unity_WorldToObject,(v.vertex));
		
		//END LOCAL WAVES
		
		
		half2 tileableUv = mul(unity_ObjectToWorld,(v.vertex)).xz;
		
//		v.vertex.xyz += offsets;		
		// one can also use worldSpaceVertex.xz here (speed!), albeit it'll end up a little skewed
//		half2 tileableUv = mul(_Object2World,(v.vertex)).xz;
		
		o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw * 0.02;

		o.viewInterpolator.xyz = worldSpaceVertex.xyz - _WorldSpaceCameraPos;

		o.pos = UnityObjectToClipPos(v.vertex);

		ComputeScreenAndGrabPassPos(o.pos, o.screenPos, o.grabPassPos);
		
	//	o.normalInterpolator.xyz = nrml;
	o.normalInterpolator.xyz =  v.normal+0.3*nrml;
		o.normalInterpolator.w = 0;

	//	o.viewInterpolator.w = 0;
		o.viewInterpolator.w = saturate(offsets.y);
	//	o.normalInterpolator.w = 1;//GetDistanceFadeout(o.screenPos.w, DISTANCE_SCALE);
		
		//MULTI LIGHTS
		//o.lightDir = ObjSpaceLightDir(v.vertex);
		
	//	o.lightPos = mul(_LightMatix0,worldSpaceVertex);
		//o.vertPos = mul(_Object2World,v.vertex);

	
		
		
//		o.lightDirPosx.xyz = ObjSpaceLightDir(v.vertex);
//		
//		o.lightPosYZvertXY.zw = worldSpaceVertex.xy;
//		o.normalInterpolator.w = worldSpaceVertex.z;
//		
//		float3 LightPos = mul(_LightMatix0,worldSpaceVertex);
//		o.lightDirPosx.w = LightPos.x;
//		o.lightPosYZvertXY.xy = LightPos.yz;
		
		
	//	o.normal = v.normal;
		//multilights
		TRANSFER_VERTEX_TO_FRAGMENT(o);
		
		UNITY_TRANSFER_FOG(o,o.pos);
		return o;
	}

//fixed4 _LightColor0;

	half4 frag( v2f i ) : SV_Target
	{
		half3 worldNormal = PerPixelNormal(_BumpMap, i.bumpCoords, VERTEX_WORLD_NORMAL, PER_PIXEL_DISPLACE);



		half3 viewVector = normalize(i.viewInterpolator.xyz);

		half4 distortOffset = half4(worldNormal.xz * REALTIME_DISTORTION , 0, 0);
		half4 screenWithOffset = i.screenPos + distortOffset;
		half4 grabWithOffset = i.grabPassPos + distortOffset;
		
		//half4 rtRefractionsNoDistort = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(i.grabPassPos));
		half refrFix = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(grabWithOffset));
		half4 rtRefractions = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(grabWithOffset));
		
		#ifdef WATER_REFLECTIVE
			half4 rtReflections = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(screenWithOffset));
		#endif

//		#ifdef WATER_EDGEBLEND_ON
//		if (LinearEyeDepth(refrFix) < i.screenPos.z)
//			rtRefractions = rtRefractionsNoDistort;
//		#endif
		
		half3 reflectVector = normalize(reflect(viewVector, worldNormal));
		half3 h = normalize ((_WorldLightDir.xyz) + viewVector.xyz);
		float nh = max (0, dot (worldNormal, -h));
		float spec = max(0.0,pow (nh, _Shininess));
		
		half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
		
		#ifdef WATER_EDGEBLEND_ON
			half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
			depth = LinearEyeDepth(depth);
			edgeBlendFactors = saturate(_InvFadeParemeter * (depth-i.screenPos.w));
			edgeBlendFactors.y = 1.0-edgeBlendFactors.y;
		#endif
		
		// shading for fresnel term
		//worldNormal.xz *= _FresnelScale;
		half refl2Refr = Fresnel(viewVector, worldNormal, FRESNEL_BIAS, FRESNEL_POWER);
		
		// base, depth & reflection colors
		//half4 baseColor = ExtinctColor (_BaseColor, i.viewInterpolator.w * _InvFadeParemeter.w + _BaseColor*11);////////// Sky Master 3.0

		//v3.2
		float4 SnowTexColor = tex2D(_MainTex, i.bumpCoords);

		//half4 baseColor = _BaseColor - ( i.viewInterpolator.w * _InvFadeParemeter.w + _BaseColor*_MultiplyEffect ) * half4(0.15, 0.03, 0.01, 0);
		half4 baseColor = _BaseColor+SnowTexColor*_MultiplyTexture.x - ( i.viewInterpolator.w * _InvFadeParemeter.w + _BaseColor*_MultiplyEffect ) * half4(0.15, 0.03, 0.01, 0); //v3.3
		
		#ifdef WATER_REFLECTIVE
			half4 reflectionColor = lerp (rtReflections,_ReflectionColor,_ReflectionColor.a);
		#else
			half4 reflectionColor = _ReflectionColor;
		#endif
		
		baseColor = lerp (lerp (rtRefractions, baseColor, baseColor.a), reflectionColor, refl2Refr);

		//baseColor = lerp (rtRefractions, baseColor, baseColor.a);

		baseColor = baseColor + spec * _SpecularColor;
		
		// handle foam
		half4 foam = Foam(_ShoreTex, i.bumpCoords);
		//baseColor.rgb += foam.rgb *_Foam.x * (edgeBlendFactors.y +  saturate(i.viewInterpolator.w - _Foam.y)) ;
		baseColor.rgb += foam.rgb *_Foam.x * (edgeBlendFactors.y) ;
//		
		baseColor.a = edgeBlendFactors.x;
		




		//MULTI LIGHTS
//		fixed atten = LIGHT_ATTENUATION(i);
//		i.lightDir = normalize(i.lightDir);
//		fixed diff = saturate(dot(i.normal,i.lightDir));
//		
//		float4 lighter = float4(_LightColor0.rgb*atten*2*diff*diff*1*diff*(1-refl2Refr)*1,1);
		
		UNITY_APPLY_FOG(i.fogCoord, baseColor);

		//v3.2
		//return (baseColor+1)*SnowTexColor*0.7 ;
		//return (baseColor+_MultiplyTexture.x)+SnowTexColor*_MultiplyTexture.y;
		//return (baseColor+_MultiplyTexture.x)+baseColor*SnowTexColor*_MultiplyTexture.y ;

		//return baseColor*SnowTexColor*+_MultiplyTexture.x + SnowTexColor*+_MultiplyTexture.y ;
		//return (baseColor+baseColor.r+_MultiplyTexture.x)*SnowTexColor*0.7; 

		//return (baseColor+_MultiplyTexture.x)*SnowTexColor*0.7;

		return (baseColor);

	}
//	half4 fragADD( v2f i ) : SV_Target
//	{
//		half3 worldNormal = PerPixelNormal(_BumpMap, i.bumpCoords, VERTEX_WORLD_NORMAL, PER_PIXEL_DISPLACE);
//		half3 viewVector = normalize(i.viewInterpolator.xyz);
//
//		half4 distortOffset = half4(worldNormal.xz * REALTIME_DISTORTION * 10.0, 0, 0);
//		half4 screenWithOffset = i.screenPos + distortOffset;
//		half4 grabWithOffset = i.grabPassPos + distortOffset;
//		
//		half4 rtRefractionsNoDistort = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(i.grabPassPos));
//		half refrFix = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(grabWithOffset));
//		half4 rtRefractions = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(grabWithOffset));
//		
//		#ifdef WATER_REFLECTIVE
//			half4 rtReflections = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(screenWithOffset));
//		#endif
//
//		#ifdef WATER_EDGEBLEND_ON
//		if (LinearEyeDepth(refrFix) < i.screenPos.z)
//			rtRefractions = rtRefractionsNoDistort;
//		#endif
//		
//		
//		
//		
//			
//		
//		half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
//		
//		#ifdef WATER_EDGEBLEND_ON
//			half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
//			depth = LinearEyeDepth(depth);
//			edgeBlendFactors = saturate(_InvFadeParemeter * (depth-i.screenPos.w));
//			edgeBlendFactors.y = 1.0-edgeBlendFactors.y;
//		#endif
//		
//		// shading for fresnel term
//		worldNormal.xz *= _FresnelScale;
//		half refl2Refr = Fresnel(viewVector, worldNormal, FRESNEL_BIAS, FRESNEL_POWER);
//		
//		// base, depth & reflection colors
//		//half4 baseColor = ExtinctColor (_BaseColor, i.viewInterpolator.w * _InvFadeParemeter.w + _BaseColor*11);////////// Sky Master 3.0
//		
//		half4 baseColor = _BaseColor - ( i.viewInterpolator.w * _InvFadeParemeter.w + _BaseColor*_MultiplyEffect) * half4(0.15, 0.03, 0.01, 0);
//		
//		#ifdef WATER_REFLECTIVE
//			half4 reflectionColor = lerp (rtReflections,_ReflectionColor,_ReflectionColor.a);
//		#else
//			half4 reflectionColor = _ReflectionColor;
//		#endif
//		
//		baseColor = lerp (lerp (rtRefractions, baseColor, baseColor.a), reflectionColor, refl2Refr);
//		//baseColor = baseColor + spec * _SpecularColor;
//		
//		// handle foam
//		half4 foam = Foam(_ShoreTex, i.bumpCoords * 2.0);
//		baseColor.rgb += foam.rgb * _Foam.x * (edgeBlendFactors.y + saturate(i.viewInterpolator.w - _Foam.y)) ;
//		
//		baseColor.a = edgeBlendFactors.x;
//		
//		
//		///MULTI LIGHTS
//		
//		fixed atten = LIGHT_ATTENUATION(i);
//		//i.lightDir = normalize(i.lightDir);
//		
//		float3 LightDirN = normalize(i.lightDirPosx.xyz);
//		float3 LightDir = i.lightDirPosx.xyz;
//		
//		//fixed diff = saturate(dot(i.normal,i.lightDir));	
//		//fixed diff = saturate(dot(worldNormal,i.lightDir));
//		fixed diff = saturate(dot(worldNormal,LightDirN));
//		
//		float3 DotDir = dot(i.normal,LightDir);
//		fixed diff2 = max(0.0,dot(worldNormal,LightDir));
//		fixed diff3 = max(0.0,dot(i.normal,LightDir));
//			
//		//fixed diff = saturate(dot(worldNormal/2 + i.normal/2,i.lightDir));			
//					
//	//	float4 lighter = float4(_LightColor0.rgb*1*4*diff*diff*diff*diff*(1-refl2Refr),1);
//		
//		//lightPos
//		//float4 lighter = float4(_LightColor0.rgb*diff*1/length(i.vertPos -  mul(_LightMatix0,i.vertPos)  ),1);
//		//float4 lighter = float4(_LightColor0.rgb*diff*1/length(i.vertPos -  mul(_LightMatix0,i.vertPos)  ),1);
//		//float4 lighter = float4(_LightColor0.rgb*diff,1);		
//		
//		float3 vertexPos = float3(i.lightPosYZvertXY.zw,i.normalInterpolator.w);		
//		float3 vertexToLight = _WorldSpaceLightPos0.xyz - vertexPos;
//		//float4 lighter = float4(_LightColor0.rgb*diff*1/length(vertexPos -  mul(_LightMatix0,vertexPos)  ),1);
//		//float4 lighter = float4(_LightColor0.rgb*atten*diff3/length(_WorldSpaceLightPos0.xyz - vertexPos ),1);	
//		float4 lighter = float4(_LightColor0.rgb*atten*((diff*diff2/5)+diff3)/length( vertexToLight ),1);
//		
//		
//		
//	
//		half3 viewVector1 = normalize(_WorldSpaceCameraPos - vertexPos); //normalize(-i.viewInterpolator.xyz);				
//		half3 reflectVector = normalize(reflect(viewVector1, worldNormal));
//		//half3 h = normalize ((_WorldLightDir.xyz) + viewVector.xyz);		
//		half3 h  = normalize ((LightDir.xyz) + viewVector1.xyz);		
//		float nh = max (0, dot (reflectVector, -h));
//		
//		
//		float3 LightDir2 = normalize(vertexToLight);
////		if(DotDir < 0){
////		
////		}
//		reflectVector = reflect(-LightDir2, worldNormal);
//		nh = max (0.0, dot (reflectVector, viewVector1));
//		
//		float spec = max(0.0,pow (nh, _Shininess));
//		
//		
////		float3 SpecularPoint;
////		if(DotDir < 0.0){
////			SpecularPoint = float3(0,0,0);
////		}else{
//		//	float3 SpecularPoint = atten * _LightColor0.rgb * pow(max(0.0,dot(reflect(-LightDir,i.normal),normalize(i.viewInterpolator.xyz))),  _Shininess*1 )/length(_WorldSpaceLightPos0.xyz - vertexPos );
//		//}
//		
//		//UNITY_APPLY_FOG(i.fogCoord, baseColor);
//		//return baseColor * lighter;
//		return float4(lighter.xyz + (_LightColor0.rgb *spec * _SpecularColor),1);
//	}
	//
	// MQ VERSION
	//
	
//	v2f_noGrab vert300(appdata_full v)
//	{
//		v2f_noGrab o;
//		
//		half3 worldSpaceVertex = mul(_Object2World,(v.vertex)).xyz;
//		half3 vtxForAni = (worldSpaceVertex).xzz;
//
//		half3 nrml;
//		half3 offsets;
//		Gerstner (
//			offsets, nrml, v.vertex.xyz, vtxForAni,						// offsets, nrml will be written
//			_GAmplitude,												// amplitude
//			_GFrequency,												// frequency
//			_GSteepness,												// steepness
//			_GSpeed,													// speed
//			_GDirectionAB,												// direction # 1, 2
//			_GDirectionCD												// direction # 3, 4
//		);
//		
//		v.vertex.xyz += offsets;
//		
//		// one can also use worldSpaceVertex.xz here (speed!), albeit it'll end up a little skewed
//		half2 tileableUv = mul(_Object2World,v.vertex).xz;
//		o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;
//
//		o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;
//
//		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//
//		o.screenPos = ComputeScreenPos(o.pos);
//		
//		o.normalInterpolator.xyz = nrml;
//		o.normalInterpolator.w = 1;//GetDistanceFadeout(o.screenPos.w, DISTANCE_SCALE);
//		
//		UNITY_TRANSFER_FOG(o,o.pos);
//		return o;
//	}
//
//	half4 frag300( v2f_noGrab i ) : SV_Target
//	{
//		half3 worldNormal = PerPixelNormal(_BumpMap, i.bumpCoords, normalize(VERTEX_WORLD_NORMAL), PER_PIXEL_DISPLACE);
//
//		half3 viewVector = normalize(i.viewInterpolator.xyz);
//
//		half4 distortOffset = half4(worldNormal.xz * REALTIME_DISTORTION * 10.0, 0, 0);
//		half4 screenWithOffset = i.screenPos + distortOffset;
//		
//		#ifdef WATER_REFLECTIVE
//			half4 rtReflections = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(screenWithOffset));
//		#endif
//		
//		half3 reflectVector = normalize(reflect(viewVector, worldNormal));
//		half3 h = normalize (_WorldLightDir.xyz + viewVector.xyz);
//		float nh = max (0, dot (worldNormal, -h));
//		float spec = max(0.0,pow (nh, _Shininess));
//		
//		half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
//		
//		#ifdef WATER_EDGEBLEND_ON
//			half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
//			depth = LinearEyeDepth(depth);
//			edgeBlendFactors = saturate(_InvFadeParemeter * (depth-i.screenPos.z));
//			edgeBlendFactors.y = 1.0-edgeBlendFactors.y;
//		#endif
//		
//		worldNormal.xz *= _FresnelScale;
//		half refl2Refr = Fresnel(viewVector, worldNormal, FRESNEL_BIAS, FRESNEL_POWER);
//		
//		half4 baseColor = _BaseColor;
//		#ifdef WATER_REFLECTIVE
//			baseColor = lerp (baseColor, lerp (rtReflections,_ReflectionColor,_ReflectionColor.a), saturate(refl2Refr * 2.0));
//		#else
//			baseColor = lerp (baseColor, _ReflectionColor, saturate(refl2Refr * 2.0));
//		#endif
//		
//		baseColor = baseColor + spec * _SpecularColor;
//		
//		baseColor.a = edgeBlendFactors.x * saturate(0.5 + refl2Refr * 1.0);
//		UNITY_APPLY_FOG(i.fogCoord, baseColor);
//		return baseColor;
//	}
//	
//	//
//	// LQ VERSION
//	//
//	
//	v2f_simple vert200(appdata_full v)
//	{
//		v2f_simple o;
//		
//		half3 worldSpaceVertex = mul(_Object2World, v.vertex).xyz;
//		half2 tileableUv = worldSpaceVertex.xz;
//
//		o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;
//
//		o.viewInterpolator.xyz = worldSpaceVertex-_WorldSpaceCameraPos;
//		
//		o.pos = mul(UNITY_MATRIX_MVP,  v.vertex);
//		
//		o.viewInterpolator.w = 1;//GetDistanceFadeout(ComputeScreenPos(o.pos).w, DISTANCE_SCALE);
//		
//		UNITY_TRANSFER_FOG(o,o.pos);
//		return o;
//
//	}
//
//	half4 frag200( v2f_simple i ) : SV_Target
//	{
//		half3 worldNormal = PerPixelNormal(_BumpMap, i.bumpCoords, half3(0,1,0), PER_PIXEL_DISPLACE);
//		half3 viewVector = normalize(i.viewInterpolator.xyz);
//
//		half3 reflectVector = normalize(reflect(viewVector, worldNormal));
//		half3 h = normalize ((_WorldLightDir.xyz) + viewVector.xyz);
//		float nh = max (0, dot (worldNormal, -h));
//		float spec = max(0.0,pow (nh, _Shininess));
//
//		worldNormal.xz *= _FresnelScale;
//		half refl2Refr = Fresnel(viewVector, worldNormal, FRESNEL_BIAS, FRESNEL_POWER);
//
//		half4 baseColor = _BaseColor;
//		baseColor = lerp(baseColor, _ReflectionColor, saturate(refl2Refr * 2.0));
//		baseColor.a = saturate(2.0 * refl2Refr + 0.5);
//
//		baseColor.rgb += spec * _SpecularColor.rgb;
//		UNITY_APPLY_FOG(i.fogCoord, baseColor);
//		return baseColor;
//	}
	
ENDCG

//Subshader
//{
//	Tags {"RenderType"="Transparent-1" "Queue"="Transparent-1" "IgnoreProjector"="True"}
//	
//	Lod 600
//	ColorMask RGB
//	
//	GrabPass { "_RefractionTex" }
//	
//	Pass {
//	
//	Tags {"LightMode"="ForwardBase"}
//	
//			Blend SrcAlpha OneMinusSrcAlpha
//			ZTest LEqual
//			//ZWrite Off
//			Cull Off
//		
//			CGPROGRAM
//		
//			#pragma target 3.0
//		
//			#pragma vertex vert600
//			#pragma fragment frag600
//			#pragma multi_compile_fog
//		
//			#pragma multi_compile WATER_VERTEX_DISPLACEMENT_ON WATER_VERTEX_DISPLACEMENT_OFF
//			#pragma multi_compile WATER_EDGEBLEND_ON WATER_EDGEBLEND_OFF
//			#pragma multi_compile WATER_REFLECTIVE WATER_SIMPLE
//		
//			ENDCG
//	}
//	
//		Pass {
//		
//		Tags {"LightMode"="ForwardAdd"}
//		
//			Blend One One
//			ZTest LEqual
//			//ZWrite Off
//			Cull Off
//		
//			CGPROGRAM
//		
//			#pragma target 3.0
//		
//			#pragma vertex vert600
//			#pragma fragment frag600
//			#pragma multi_compile_fog
//		
//			#pragma multi_compile WATER_VERTEX_DISPLACEMENT_ON WATER_VERTEX_DISPLACEMENT_OFF
//			#pragma multi_compile WATER_EDGEBLEND_ON WATER_EDGEBLEND_OFF
//			#pragma multi_compile WATER_REFLECTIVE WATER_SIMPLE
//		
//			ENDCG
//	}
//	
//}

Subshader
{
	Tags {"RenderType"="Transparent-1" "Queue"="Transparent-1" "IgnoreProjector"="True"}
	
	Lod 600
	ColorMask RGB
	
	GrabPass { "_RefractionTex" }
	
	Pass {
	
	Tags {"LightMode"="ForwardBase"}
	
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest LEqual
			//ZWrite Off
			Cull Off
		
			CGPROGRAM
		
			#pragma target 2.0
			//#pragma only_renderers gles gles3 [opengl,d3d9]
			//#pragma glsl
		
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			//#pragma multi_compile_fwdadd
		
			#pragma multi_compile WATER_VERTEX_DISPLACEMENT_ON WATER_VERTEX_DISPLACEMENT_OFF
			#pragma multi_compile WATER_EDGEBLEND_ON WATER_EDGEBLEND_OFF
			#pragma multi_compile WATER_REFLECTIVE WATER_SIMPLE
		
			ENDCG
	}

}

Subshader
{
	Tags {"RenderType"="Transparent-1" "Queue"="Transparent-1" "IgnoreProjector"="True"}
	
	Lod 500
	ColorMask RGB
	
	GrabPass { "_RefractionTex" }
	
	Pass {
	
	Tags {"LightMode"="ForwardBase"}
	
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest LEqual
			//ZWrite Off
			Cull Off
		
			CGPROGRAM
		
			#pragma target 2.0
			//#pragma only_renderers gles gles3 [opengl,d3d9]
			//#pragma glsl
		
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			//#pragma multi_compile_fwdadd
		
			#pragma multi_compile WATER_VERTEX_DISPLACEMENT_ON WATER_VERTEX_DISPLACEMENT_OFF
			#pragma multi_compile WATER_EDGEBLEND_ON WATER_EDGEBLEND_OFF
			#pragma multi_compile WATER_REFLECTIVE WATER_SIMPLE
		
			ENDCG
	}
	
//	Pass {
//	
//	Tags {"LightMode"="ForwardAdd"}
//	
//			Blend One One
//			ZTest LEqual
//			//ZWrite Off
//			Cull Off
//		
//			CGPROGRAM
//		
//			#pragma target 3.0
//		
//			#pragma vertex vert
//			#pragma fragment fragADD
//			#pragma multi_compile_fog
//			#pragma multi_compile_fwdadd
//		
//			#pragma multi_compile WATER_VERTEX_DISPLACEMENT_ON WATER_VERTEX_DISPLACEMENT_OFF
//			#pragma multi_compile WATER_EDGEBLEND_ON WATER_EDGEBLEND_OFF
//			#pragma multi_compile WATER_REFLECTIVE WATER_SIMPLE
//		
//			ENDCG
//	}
}

//Subshader
//{
//	Tags {"RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
//	
//	Lod 300
//	ColorMask RGB
//	
//	Pass {
//			Blend SrcAlpha OneMinusSrcAlpha
//			ZTest LEqual
//			ZWrite Off
//			Cull Off
//		
//			CGPROGRAM
//		
//			#pragma target 3.0
//		
//			#pragma vertex vert300
//			#pragma fragment frag300
//			#pragma multi_compile_fog
//		
//			#pragma multi_compile WATER_VERTEX_DISPLACEMENT_ON WATER_VERTEX_DISPLACEMENT_OFF
//			#pragma multi_compile WATER_EDGEBLEND_ON WATER_EDGEBLEND_OFF
//			#pragma multi_compile WATER_REFLECTIVE WATER_SIMPLE
//		
//			ENDCG
//	}
//}
//
//Subshader
//{
//	Tags {"RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
//	
//	Lod 200
//	ColorMask RGB
//	
//	Pass {
//			Blend SrcAlpha OneMinusSrcAlpha
//			ZTest LEqual
//			ZWrite Off
//			Cull Off
//		
//			CGPROGRAM
//		
//			#pragma vertex vert200
//			#pragma fragment frag200
//			#pragma multi_compile_fog
//		
//			ENDCG
//	}
//}

Fallback "Transparent/Diffuse"
}
