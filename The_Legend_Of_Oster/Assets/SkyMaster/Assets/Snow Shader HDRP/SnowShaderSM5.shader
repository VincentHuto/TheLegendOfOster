// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
 
Shader "SkyMaster/SnowShaderSM5 URP" {
	Properties{
		_SnowCoverage ("Snow Coverage", Range(0, 1)) = 0   
		_SnowBlend("Snow Blend", Range(0, 50)) = 0.4
		_LightIntensity("Light Intensity", Range(0.5, 50)) = 1
		_SnowBumpDepth("Snow bump depth", Range(0, 5)) = 1
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Bump("Bump", 2D) = "bump" {}
	_SnowTexture("Snow texture", 2D) = "white" {}
	_Depth("Depth of Snow", Range(0, 0.02)) = 0.01
		//_SnowBump("Snow Bump", 2D) = "bump" {}
	_Direction("Direction of snow", Vector) = (0, 1, 0)
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
		_Wetness("Wetness", Range(-0, 20)) = 1

		_Mask("Mask", 2D) = "black" {}
	_MainTex2("Albedo 1", 2D) = "white" {}
	_NormalMap2("NormalMap 1", 2D) = "bump" {}
	_Water("Water", 2D) = "white" {}
	water_level("Water level", Float) = 1
		water_spec("Water Spec Focus", Float) = 1.2
		water_tiling("Water tiling", Float) = 2
		_BumpPower("Bump Power", Range(3, 0.01)) = 1

		_Color("Tint", Color) = (1,1,1,1)

		//v3.0.2
		Snow_Cover_offset("Snow coverage offset", Float) = 0

		//[LM_Specular][LM_Glossiness] _SpecGlossMap("Specular 0", 2D) = "white" {}
	//[LM_Specular][LM_Glossiness] _SpecGlossMap2("Specular 1", 2D) = "white" {}
	}

		//Category{
		//Tags{ "RenderType" = "Opaque" }
		//Blend SrcAlpha OneMinusSrcAlpha
		//Cull Off
		//Lighting Off
		//Zwrite Off
		SubShader
	{
			Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
		}

		Pass
		{
			Name "Universal Forward"
			Tags
			{
				"LightMode" = "UniversalForward"
			}

		// Render State
		//Blend One Zero, One Zero
		Cull Off
		//ZTest LEqual
		//Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On
		// ColorMask: <None>

		//CGPROGRAM
		HLSLPROGRAM

		//Tags{ "RenderType" = "Opaque" }
		//Blend SrcAlpha OneMinusSrcAlpha
		//Cull Off
		//Lighting Off
		

		#pragma vertex vert
		#pragma fragment frag

		#pragma multi_compile_fog
		#pragma multi_compile_instancing

		//#include "UnityCG.cginc"
		//#include "AutoLight.cginc"
		//#include "Lighting.cginc"	

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
		#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"


		CBUFFER_START(UnityPerMaterial)
		float _SnowCoverage;
	float _SnowBlend;
	float _LightIntensity;
	float _SnowBumpDepth;
	float _Depth;
	//sampler2D _SnowBump;
	float3 _Direction;
	half _Shininess;
	half _Wetness;
	uniform float water_level;
	uniform float water_tiling;
	uniform float water_spec;

	float _BumpPower;
	float4 _Color;
	float Snow_Cover_offset;
	float4 _MainTex_ST;
	float4 _FinalTex_ST;
		CBUFFER_END
	

	//sampler2D _MainTex;
	//sampler2D _Bump;
	//sampler2D _SnowTexture;
	TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
	TEXTURE2D(_Bump); SAMPLER(sampler_Bump);
	TEXTURE2D(_SnowTexture); SAMPLER(sampler_SnowTexture);

	

	TEXTURE2D(_MainTex2); SAMPLER(sampler_MainTex2); //float4 _texture_TexelSize;
	TEXTURE2D(_NormalMap2); SAMPLER(sampler_NormalMap2); //float4 _texture_TexelSize;
	TEXTURE2D(_Water); SAMPLER(sampler_Water);// float4 _texture_TexelSize;
	TEXTURE2D(_SpecGlossMap); SAMPLER(sampler_SpecGlossMap); //float4 _texture_TexelSize;
	TEXTURE2D(_SpecGlossMap2); SAMPLER(sampler_SpecGlossMap2); //float4 _texture_TexelSize;
	TEXTURE2D(_Mask); SAMPLER(sampler_Mask); //float4 _texture_TexelSize;

	/*sampler2D _MainTex2;
	sampler2D _NormalMap2;
	sampler2D _Water;
	sampler2D _SpecGlossMap;
	sampler2D _SpecGlossMap2;
	sampler2D _Mask;*/


	
	struct appdata_t {
		float4 vertex : POSITION;
		//float4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float3 normal: NORMAL;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		//float4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float3 normal: TEXCOORD1;
		/*#ifdef SOFTPARTICLES_ON               
				float4 position : TEXCOORD2;
		#endif  */                            
		//LIGHTING_COORDS(3,4)
		//float3 ForwLight:TEXCOORD5;

	};



	//https://forum.unity.com/threads/can-anyone-give-me-a-quick-rundown-of-the-basics-of-a-handwritten-urp-shader-vs-in-built.757823/
	//This is a replacement for the old 'UnityObjectToClipPos()'
	float4 UnityObjectToClipPos(float3 pos)
	{
		return mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4 (pos, 1)));
	}

	v2f vert(appdata_t v)
	{
		v2f o;
		float3 Snow = normalize(_Direction.xyz);



		//URP
		VertexPositionInputs vertInputs = GetVertexPositionInputs(v.vertex.xyz);    //This function calculates all the relative spaces of the objects vertices
		o.pos = vertInputs.positionCS;
		//o.vertex = ObjectToClipPos (v.vertex);
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);



		if (dot(v.normal, Snow) >= lerp(1, -1, ((_SnowCoverage + Snow_Cover_offset) * 2) / 3))
		{
			v.vertex.xyz += normalize(v.normal + Snow)  * (_SnowCoverage + Snow_Cover_offset) * _Depth;
		}
		o.normal = v.normal;
		o.pos = UnityObjectToClipPos(v.vertex);
		/*
		o.pos = UnityObjectToClipPos(v.vertex);
		*/
		//#ifdef SOFTPARTICLES_ON
		//		o.position = ComputeScreenPos(o.pos);
		//		COMPUTE_EYEDEPTH(o.position.z);
		//#endif
		//o.color = v.color;
		//o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
		o.normal = v.normal;
		//o.ForwLight = ObjSpaceLightDir(v.vertex);
		//TRANSFER_VERTEX_TO_FRAGMENT(o);
		return o;
	}

	//sampler2D_float _CameraDepthTexture;
	float _InvFade;

	float4 frag(v2f IN) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(IN);

		//PUDDLES
		float2 Mask_motion = float2(IN.texcoord.x,IN.texcoord.y + (_Time.x * 5))*0.05;
		float2 A_motion = float2(IN.texcoord.x,IN.texcoord.y + (_Time.x * 5))*0.005;
		float2 B_motion = float2(IN.texcoord.x,IN.texcoord.y + (_Time.x * 1));

		//float blend = tex2D(_Mask, IN.texcoord).a * 1;
		float blend = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, IN.texcoord);

		//float4 albedo1 = tex2D(_MainTex, IN.texcoord);
		//float4 spec1 = tex2D(_SpecGlossMap, IN.texcoord);
		float4 albedo1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);
		float4 spec1 = SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, IN.texcoord);


		//float3 normal1 = UnpackNormal(tex2D(_Bump, IN.texcoord)); //_NormalMap

		half4 flow = float4(1,1,1,1);
		//half4 flow1 = tex2D(_Water, float2(IN.texcoord.x*water_tiling,IN.texcoord.y + (_Time.x*0.5)));
		float4 flow1 = SAMPLE_TEXTURE2D(_Water, sampler_Water, float2(IN.texcoord.x*water_tiling, IN.texcoord.y + (_Time.x*0.5)));

		//float4 albedo2 = tex2D(_MainTex2, float2(IN.texcoord.x + flow.r,IN.texcoord.y + flow.r));
		//float4 spec2 = tex2D(_SpecGlossMap2, float2(IN.texcoord.x + flow.r,IN.texcoord.y + flow.r)) * _Wetness;
		float4 albedo2 = SAMPLE_TEXTURE2D(_MainTex2, sampler_MainTex2, float2(IN.texcoord.x + flow.r, IN.texcoord.y + flow.r));
		float4 spec2 = SAMPLE_TEXTURE2D(_SpecGlossMap2, sampler_SpecGlossMap2, float2(IN.texcoord.x + flow.r, IN.texcoord.y + flow.r))* _Wetness;


		//float3 normal2 = UnpackNormal(tex2D(_NormalMap2, float2(IN.texcoord.x + flow.r,IN.texcoord.y + flow.r) + (0.03)));
		float4 specGloss = lerp(spec1, spec2, blend);
		//END PUDDLES  

		//SNOW
		//float4 SnowTexColor = tex2D(_SnowTexture, IN.texcoord);
		//float4 MainTexColor = tex2D(_MainTex, IN.texcoord);
		float4 SnowTexColor = SAMPLE_TEXTURE2D(_SnowTexture, sampler_SnowTexture, IN.texcoord);
		float4 MainTexColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);

		//float3 Normal = UnpackNormal(tex2D(_Bump, IN.texcoord));

		float Alpha = MainTexColor.a;
		//float DirN = dot(WorldNormalVector(IN, IN.normal), _Direction.xyz);
		float DirN = dot( IN.normal, _Direction.xyz);
		float Check = lerp(1,-1,(_SnowCoverage + Snow_Cover_offset) / 5.5); //divide by 6 to synch with the slower building of snow on Unity trees
		float3 fColor;
		if (DirN >= Check)
		{
			fColor = lerp(lerp(albedo1, albedo2, blend), SnowTexColor.rgb*_LightIntensity, pow((1 - (Check / DirN)), _SnowBlend));
		}
		else {			
			fColor = lerp(albedo1, albedo2, blend);	
		}


		//return float4(1,1,1,1);
		return float4(fColor.rgb, Alpha);				
	}
		//ENDCG
		ENDHLSL
	}
	}
	//}
	FallBack "Hidden/Universal Render Pipeline/FallbackError"
}