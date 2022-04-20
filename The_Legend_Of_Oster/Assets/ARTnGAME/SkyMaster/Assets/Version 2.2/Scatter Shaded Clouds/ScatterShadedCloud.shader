// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "SkyMaster/ScatterShadedCloud" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_BorderTex ("Border", 2D) = "white" {}
		_TintColor ("TintColor", Color) = (1.0, 1.0, 1.0, 1.0)
		_Intensity ("Intensity", float) = 1.0
		_Color ("Main Color", COLOR) = (1,1,1) 
		
		// Volume
		_FinalTex ("Final Texture", 2D) = "white" {}
   		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	    _Glow ("Glow Factor", Range(0.0,10.0)) = 1.0
	    _LightIntensity ("Light Intensity", Range(0.0,5.0)) = 1.0
	    _SunLightIntensity ("Sun Intensity", Range(0.0,5.0)) = 1.0
	    _MinLight ("Mimize light", Range(-1.0,1.5)) = 0.0
	    _SunColor ("Sun Color", Color) = (0,0,0,0)
	    
	    _Control ("Main Color", COLOR) = (1,1,1) 
	}
	
		CGINCLUDE
	
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"	
		
		sampler2D _BorderTex;
		half4 _BorderTex_ST;
		sampler2D _MainTex;
		fixed4 _TintColor;
		float _Intensity;
					
		//Scatter			
		float3 _Color;
		float3 _Control;
		static const float pi = 3.141592653589793238462643383279502884197169;
				static const float rayleighZenithLength = 8.4E3;
				static const float mieZenithLength = 1.25E3;
				static const float3 up = float3(0.0, 1.0, 0.0);				
			
				static const float sunAngularDiameterCos = 0.999956676946448443553574619906976478926848692873900859324;	
			

				float3 sunPosition;
				float3 betaR;
				float3 betaM;

				float fog_depth = 2.0;
				float mieCoefficient =0.05;
				float mieDirectionalG = 0.8;				
				float ExposureBias = 1.0;
				float rayleighPhase(float cosTheta)
				{
					return (3.0 / (16.0*pi)) * (1.0 + pow(cosTheta, 2.0));
				}		
								
				float hgPhase(float cosTheta, float g)
				{   float POW = pow(g, 2.0);
					return (0.25 / pi) * ((1.0 - POW) / pow(1.0 - 2.0*g*cosTheta + POW, 1.5)); 
				}			
	
				///////////////////
				uniform float4 _MainTex_ST;
							
				//Volume								
				sampler2D _FinalTex;
	            float _Glow;
	            float _LightIntensity;
	            float _SunLightIntensity;
	            float _MinLight;
	            fixed4 _SunColor;												
																										
																																										
									
		struct v2f {
			half4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			float2 uv : TEXCOORD0;
			float3 vWorldPosition:TEXCOORD1;
			float3 normal:TEXCOORD2;		 	
			half2 texcoord : TEXCOORD3;	
			
				//Volume	
			float3 ForwLight:TEXCOORD4;
			LIGHTING_COORDS(5,6) 
		};

		v2f vert(appdata_full v) {
			v2f o;
			UNITY_INITIALIZE_OUTPUT(v2f,o);
			
			o.vertex = UnityObjectToClipPos (v.vertex);	
			o.uv.xy = v.texcoord.xy;
			o.color = v.color * _TintColor;
			o.texcoord = TRANSFORM_TEX(v.texcoord, _BorderTex);
			
			o.vWorldPosition = mul (unity_ObjectToWorld, v.vertex).xyz;					
			o.normal = v.normal;
			
				//Volume
				o.ForwLight = ObjSpaceLightDir(v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);	
			
			return o; 
		}
		
		//Volume
		 sampler2D_float _CameraDepthTexture;
            float _InvFade;
		
		fixed4 frag( v2f IN ) : COLOR {	
		
		float3 Difference = IN.vWorldPosition - _WorldSpaceCameraPos;
					float4 mainColor = tex2D(_MainTex, _MainTex_ST*IN.uv); 
					float Norma = length(Difference);
					float3 BETAS = betaR + betaM;					
					float3 Fex = exp(-((BETAS) * Norma));
					float cosTheta = dot(normalize(Difference), sunPosition);
					
					float rPhase = rayleighPhase(cosTheta*0.5+0.5);
					float3 betaRTheta = betaR * rPhase;
					float mPhase = hgPhase(cosTheta, mieDirectionalG);
					float3 betaMTheta = betaM * mPhase;					
					float3 FEXX = 400 * Fex;					
					float3 Lin = ((betaRTheta + betaMTheta) / (BETAS)) * (400 - FEXX);
					float3 Combined = FEXX +  Lin; 
					float3 Scatter_power = Combined * 0.005;
					float diffuseFactor =  max(dot(IN.normal.xyz, sunPosition ), 0) ; 
					float3 lightDirection = _WorldSpaceLightPos0.xyz;         				
					float3 lightColor = _LightColor0.rgb;
				
					float3 LightAll = float3(_Color * (mainColor.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 
					* saturate(dot(IN.normal, lightDirection)));
					float3 FinalColor = LightAll * Fex + Lin;					
				//	return  float4(1.0-exp2(-FinalColor*ExposureBias),0);
		
		float4 return1 = tex2D (_MainTex, IN.uv.xy)* tex2D(_BorderTex, IN.texcoord.xy) * IN.color * _Intensity;
		
		//float4 return1 = tex2D (_MainTex, IN.uv.xy)* tex2D(_BorderTex, IN.texcoord.xy) * float4(1.0-exp2(-FinalColor*ExposureBias),0.5) * _Intensity;
		
		
		//Volume
		fixed4 col = tex2D(_MainTex, IN.texcoord) * IN.color.a;
               fixed lerp_vec = max (0, dot (IN.normal, IN.ForwLight)	);               
               //fixed atten = LIGHT_ATTENUATION(i);     
			   UNITY_LIGHT_ATTENUATION(atten, IN, IN.vWorldPosition.xyz); //v4.1

               //float3 finalCol = (_Glow)* IN.color.rgb + col * _LightColor0.rgb * atten * lerp_vec * 2 * _LightIntensity;
               float3 finalCol = (_Glow)* IN.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * lerp_vec * 2 * _SunLightIntensity *_LightIntensity + _MinLight;
              // return float4(finalCol.rgb,col.a) ; 
		
			return _Control.x*return1 + _Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) + _Control.z*float4(finalCol.rgb,col.a);
		}
	
	ENDCG
		 
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
	Pass {
	
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"			
		#pragma multi_compile_fwdbase 
		#pragma target 3.0
		ENDCG
		 
		}
				
	} 
	FallBack Off
}
