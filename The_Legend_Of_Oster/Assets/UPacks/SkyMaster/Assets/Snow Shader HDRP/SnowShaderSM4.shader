// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkyMaster/SnowShaderSM4 URP" {
	Properties{
		//_SnowCoverage ("Snow Coverage", Range(0, 1)) = 0   
		_SnowBlend("Snow Blend", Range(0, 50)) = 0.4
		_LightIntensity("Light Intensity", Range(0.5, 50)) = 1
		_SnowBumpDepth("Snow bump depth", Range(0, 5)) = 1
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Bump("Bump", 2D) = "bump" {}
	_SnowTexture("Snow texture", 2D) = "white" {}
	_Depth("Depth of Snow", Range(0, 0.02)) = 0.01
		_SnowBump("Snow Bump", 2D) = "bump" {}
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

		Category{
		Tags{ "RenderType" = "Opaque" }
		//Blend SrcAlpha OneMinusSrcAlpha
		//Cull Off
		//Lighting Off
		//Zwrite Off
		SubShader{
		Pass{

		//Tags { "LightMode"="ForwardBase"} //v3.3
		//Tags { "LightMode"="ForwardAdd"}
		//Blend One One

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_particles 
		//#pragma multi_compile_fwdadd_fullshadows
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"

		/*sampler2D _MainTex;
	sampler2D _FinalTex;
	float _Glow;
	float _LightIntensity;
	float _SunLightIntensity;
	float _MinLight;
	fixed4 _SunColor;*/

		float _SnowCoverage;
	float _SnowBlend;
	float _LightIntensity;
	float _SnowBumpDepth;

	sampler2D _MainTex;
	sampler2D _Bump;
	sampler2D _SnowTexture;
	float _Depth;
	sampler2D _SnowBump;
	float3 _Direction;
	half _Shininess;
	half _Wetness;

	sampler2D _MainTex2;
	sampler2D _NormalMap2;
	sampler2D _Water;
	sampler2D _SpecGlossMap;
	sampler2D _SpecGlossMap2;
	sampler2D _Mask;
	uniform float water_level;
	uniform float water_tiling;
	uniform float water_spec;

	float _BumpPower;
	float4 _Color;
	float Snow_Cover_offset;




	struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float3 normal: NORMAL;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float3 normal: TEXCOORD1;
#ifdef SOFTPARTICLES_ON               
		float4 position : TEXCOORD2;
#endif                              
		LIGHTING_COORDS(3,4)
			float3 ForwLight:TEXCOORD5;

	};

	float4 _MainTex_ST;
	float4 _FinalTex_ST;

	v2f vert(appdata_t v)
	{
		v2f o;
		float3 Snow = normalize(_Direction.xyz);

		if (dot(v.normal, Snow) >= lerp(1, -1, ((_SnowCoverage + Snow_Cover_offset) * 2) / 3))
		{
			v.vertex.xyz += normalize(v.normal + Snow)  * (_SnowCoverage + Snow_Cover_offset) * _Depth;
		}
		o.normal = v.normal;
		o.pos = UnityObjectToClipPos(v.vertex);
		/*
		o.pos = UnityObjectToClipPos(v.vertex);
		*/
#ifdef SOFTPARTICLES_ON
		o.position = ComputeScreenPos(o.pos);
		COMPUTE_EYEDEPTH(o.position.z);
#endif
		o.color = v.color;
		o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
		o.normal = v.normal;
		o.ForwLight = ObjSpaceLightDir(v.vertex);
		TRANSFER_VERTEX_TO_FRAGMENT(o);
		return o;
	}

	sampler2D_float _CameraDepthTexture;
	float _InvFade;

	fixed4 frag(v2f IN) : SV_Target
	{

		//PUDDLES
		float2 Mask_motion = float2(IN.texcoord.x,IN.texcoord.y + (_Time.x * 5))*0.05;
		float2 A_motion = float2(IN.texcoord.x,IN.texcoord.y + (_Time.x * 5))*0.005;
		float2 B_motion = float2(IN.texcoord.x,IN.texcoord.y + (_Time.x * 1));

		fixed blend = tex2D(_Mask, IN.texcoord).a * 1;
		fixed4 albedo1 = tex2D(_MainTex, IN.texcoord);
		fixed4 spec1 = tex2D(_SpecGlossMap, IN.texcoord);
		fixed3 normal1 = UnpackNormal(tex2D(_Bump, IN.texcoord)); //_NormalMap

		half4 flow = float4(1,1,1,1);
		half4 flow1 = tex2D(_Water, float2(IN.texcoord.x*water_tiling,IN.texcoord.y + (_Time.x*0.5)));

		fixed4 albedo2 = tex2D(_MainTex2, float2(IN.texcoord.x + flow.r,IN.texcoord.y + flow.r));
		fixed4 spec2 = tex2D(_SpecGlossMap2, float2(IN.texcoord.x + flow.r,IN.texcoord.y + flow.r)) * _Wetness;
		fixed3 normal2 = UnpackNormal(tex2D(_NormalMap2, float2(IN.texcoord.x + flow.r,IN.texcoord.y + flow.r) + (0.03)));
		fixed4 specGloss = lerp(spec1, spec2, blend);
		//END PUDDLES  

		//SNOW
		float4 SnowTexColor = tex2D(_SnowTexture, IN.texcoord);
		float4 MainTexColor = tex2D(_MainTex, IN.texcoord);
		float3 Normal = UnpackNormal(tex2D(_Bump, IN.texcoord));

		float Alpha = MainTexColor.a;
		//float DirN = dot(WorldNormalVector(IN, IN.normal), _Direction.xyz);
		float DirN = dot( IN.normal, _Direction.xyz);
		float Check = lerp(1,-1,(_SnowCoverage + Snow_Cover_offset) / 5.5); //divide by 6 to synch with the slower building of snow on Unity trees
		float3 fColor;
		if (DirN >= Check)
		{
			//o.Albedo = lerp(lerp(albedo1, albedo2, blend) , SnowTexColor.rgb*_LightIntensity,pow((1 - (Check / DirN)),_SnowBlend));
			//o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.texcoord))*_SnowBumpDepth);
			fColor = lerp(lerp(albedo1, albedo2, blend), SnowTexColor.rgb*_LightIntensity, pow((1 - (Check / DirN)), _SnowBlend));
		}
		else {
			//PUDDLES INTEGRATION
			fColor = lerp(albedo1, albedo2, blend);//o.Albedo = lerp(albedo1, albedo2, blend);
			//o.Specular = specGloss.rgb;
			//o.Smoothness = water_spec * specGloss.a + water_level * flow1*specGloss.a;
			//o.Normal = lerp(normal1, normal2, blend);
			//o.Normal.z = o.Normal.z*_BumpPower;
			//o.Normal = normalize(o.Normal);
			//END PUDDLES
		}
		//o.Specular = _Shininess;

		return float4(fColor.rgb, Alpha);

		//#ifdef SOFTPARTICLES_ON
		//		float Depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.position)));
		//	float DepthP = i.position.z;
		//	float fade = saturate((Depth - DepthP) * _InvFade);
		//	i.color.a *= fade;
		//#endif
		//	fixed4 col = tex2D(_MainTex, i.texcoord) * i.color.a;
		//	fixed lerp_vec = max(0, dot(i.normal, i.ForwLight));
		//	//fixed atten = LIGHT_ATTENUATION(i);               
		//	//float3 finalCol = (_Glow)* i.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * lerp_vec * 2 * _SunLightIntensity *_LightIntensity *atten * _LightColor0.rgb + _MinLight;
		//	//float3 finalCol = (_Glow)* i.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * lerp_vec * 2 * _SunLightIntensity *_LightIntensity  + _MinLight;
		//	float3 _LightColor = float3(1, 1, 1) * 0.3; //HDRP
		//	float3 finalCol = (_Glow)* i.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * lerp_vec * 2 * _SunLightIntensity *_LightIntensity* _LightColor + _MinLight; //v3.3

		//return float4(finalCol.rgb,col.a);
	}
		ENDCG
	}
	}
	}
}






//   
//    SubShader {
//       // Tags { "RenderType"="Opaque" }
//        LOD 200
//       
//        CGPROGRAM
//        #pragma target 3.0
//        #pragma surface surf StandardSpecular fullforwardshadows vertex:vert
// 		#pragma exclude_renderers gles
//        
//        float _SnowCoverage; 
//        float _SnowBlend;  
//        float _LightIntensity;
//        float _SnowBumpDepth;
//                  
//        sampler2D _MainTex;
//        sampler2D _Bump;        
//        sampler2D _SnowTexture;
//        float _Depth;        
//        sampler2D _SnowBump;        
//        float3 _Direction;
//        half _Shininess;
//        half _Wetness; 
// 		
// 		sampler2D _MainTex2;
//		sampler2D _NormalMap2;
//		sampler2D _Water;	
//		sampler2D _SpecGlossMap;
//		sampler2D _SpecGlossMap2;	
//		sampler2D _Mask;		
//		uniform float water_level;
//		uniform float water_tiling;
//		uniform float water_spec;
//		
//		fixed _BumpPower;
//		float4 _Color;
//		float Snow_Cover_offset;
// 
//        struct Input {        
//            float2 uv_MainTex;
//            float2 uv_Bump;
//            float2 uv_SnowTexture;
//            float2 uv_SnowBump;
//            float3 worldNormal;      
//            
//            float2 uv_MainTex2;
//			float2 uv_Water;
//			float2 uv_Mask;
//			
//			INTERNAL_DATA
//        };        
//       
//        void vert (inout appdata_full v) {            
//            float3 Snow = normalize(_Direction.xyz);
//           
//            if (dot(v.normal, Snow) >= lerp(1, -1, ((_SnowCoverage+Snow_Cover_offset) * 2) / 3))
//            {
//                v.vertex.xyz += normalize(v.normal + Snow)  * (_SnowCoverage+Snow_Cover_offset) * _Depth;
//            }           
//        }
// 
//        void surf (Input IN, inout SurfaceOutputStandardSpecular o) {   
//   
//   			//PUDDLES
//   			float2 Mask_motion = float2(IN.uv_Mask.x,IN.uv_Mask.y+(_Time.x*5))*0.05 ;
//			float2 A_motion = float2(IN.uv_MainTex.x,IN.uv_MainTex.y+(_Time.x*5))*0.005 ;
//			float2 B_motion = float2(IN.uv_MainTex2.x,IN.uv_MainTex2.y+(_Time.x*1)) ;
//		
//			fixed blend = tex2D(_Mask, IN.uv_Mask).a*1;	
//			fixed4 albedo1 = tex2D(_MainTex, IN.uv_MainTex);
//			fixed4 spec1	= tex2D(_SpecGlossMap, IN.uv_MainTex);
//		 	fixed3 normal1 = UnpackNormal (tex2D (_Bump, IN.uv_MainTex)); //_NormalMap
//
//			half4 flow = float4(1,1,1,1);
//			half4 flow1 = tex2D(_Water, float2(IN.uv_MainTex2.x*water_tiling,IN.uv_MainTex2.y+(_Time.x*0.5)) );
//
//			fixed4 albedo2 = tex2D(_MainTex2, float2(IN.uv_MainTex2.x+flow.r,IN.uv_MainTex2.y+flow.r));
//			fixed4 spec2 = tex2D(_SpecGlossMap2, float2(IN.uv_MainTex2.x+flow.r,IN.uv_MainTex2.y+flow.r)) * _Wetness;
//		 	fixed3 normal2 = UnpackNormal (tex2D (_NormalMap2, float2(IN.uv_MainTex2.x+flow.r,IN.uv_MainTex2.y+flow.r)+ (0.03)  ));
//		 	fixed4 specGloss = lerp (spec1, spec2, blend);
//   			//END PUDDLES  
//   
//   			//SNOW
//   			float4 SnowTexColor = tex2D(_SnowTexture, IN.uv_SnowTexture);
//            float4 MainTexColor = tex2D(_MainTex, IN.uv_MainTex);
//            o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));           
//
//            o.Alpha = MainTexColor.a;  
//            float DirN = dot(WorldNormalVector(IN, o.Normal), _Direction.xyz)  ; 
//            float Check = lerp(1,-1,(_SnowCoverage+Snow_Cover_offset)/5.5); //divide by 6 to synch with the slower building of snow on Unity trees
//            if(DirN >= Check)
//            {                
//                o.Albedo = lerp (  lerp (albedo1, albedo2, blend) , SnowTexColor.rgb*_LightIntensity,pow((1-(Check/DirN)),_SnowBlend)) ;              
//                o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*_SnowBumpDepth);                                
//            }
//            else{            	
//            	//PUDDLES INTEGRATION
//			 	o.Albedo 		= lerp (albedo1, albedo2, blend) ;
//			 	o.Specular 		= specGloss.rgb ;
//				o.Smoothness 	= water_spec*specGloss.a+ water_level*flow1*specGloss.a;
//			  	o.Normal 		= lerp (normal1, normal2, blend);
//			 	o.Normal.z = o.Normal.z*_BumpPower; 
//			 	o.Normal = normalize(o.Normal); 
//	   			//END PUDDLES
//            } 	
//			o.Specular = _Shininess;           
//        }
//        ENDCG
//    }
//    FallBack "Diffuse"
//}