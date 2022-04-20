// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "SkyMaster/ScatterShadedCloudMOTION Soft v3.3" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_BorderTex ("Border", 2D) = "white" {}
		_TintColor ("TintColor", Color) = (1.0, 1.0, 1.0, 1.0)
		_Intensity ("Intensity", float) = 1.0
		_Color ("Main Color", COLOR) = (1,1,1) 
		
		// Volume
		_FinalTex ("Final Texture", 2D) = "white" {}
   		_InvFade ("Soft Particles Factor", Range(0.0001,3.0)) = 1.0 //v3.3
	    _Glow ("Glow Factor", Range(0.0,10.0)) = 1.0
	    _LightIntensity ("Light Intensity", Range(0.0,5.0)) = 1.0
	    _SunLightIntensity ("Sun Intensity", Range(0.0,5.0)) = 1.0
	    _MinLight ("Mimize light", Range(-1.0,1.5)) = 0.0
	    _SunColor ("Sun Color", Color) = (0,0,0,0)
	    
	    _Control ("Main Color", COLOR) = (1,1,1)
	    _Speed ("Intensity", Float) = (-0.05,0,0) 
	}
	
		
		CGINCLUDE

//		#pragma fragmentoption ARB_precision_hint_fastest 
	
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"	
		#pragma multi_compile_fog 
		#pragma vertex vert
		#pragma fragment frag
		
		   #pragma multi_compile_particles 
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma target 3.0
		
		sampler2D _BorderTex;
		half4 _BorderTex_ST;
		sampler2D _MainTex;
		fixed4 _TintColor;
		float _Intensity;
			float2 _Speed;		
		//Scatter			
		float3 _Color;
		float3 _Control;
		
		//float4x4 _Camera2World;//v3.0.2
		
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
						 float4 _Dist_ST;																				
																																										
									
		struct v2f {
			half4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float2 uv : TEXCOORD0;
			float3 vWorldPosition:TEXCOORD1;
			float3 normal:TEXCOORD2;		 	
		//	half2 texcoord : TEXCOORD3;	
			
				//Volume	
			float3 ForwLight:TEXCOORD3;
			LIGHTING_COORDS(4,5) 
			UNITY_FOG_COORDS(6) 
			  float4 screenPos:TEXCOORD7;
                float3 PointLights:TEXCOORD8;
                float3 ViewDir:TEXCOORD9;

//			UNITY_FOG_COORDS(5) 
//			  float4 screenPos:TEXCOORD6;
//                float3 PointLights:TEXCOORD7;
//                float3 ViewDir:TEXCOORD8;
                
                //float4 projPos : TEXCOORD11;
		};

		v2f vert(appdata_full v) {
			v2f o;
			UNITY_INITIALIZE_OUTPUT(v2f,o); 
			
			o.pos = UnityObjectToClipPos (v.vertex);	
			
						
							
				//v.3.0.2	
				
//				float4x4 ViewM = float4x4(0,0,0,0,   0,0,0,0,  	 0,0,0,0,  	 0,0,0,0);
//    			float3 right = float3(1,0,0);
//    			float3 up = float3(0,1,0);
//    			float3 forward = float3(0,0,1);
//    			float3 CamPos = _WorldSpaceCameraPos;
//    			
//    			//right = UNITY_MATRIX_V[0].xyz;
//    			//up = UNITY_MATRIX_V[1].xyz;     			
//    			//forward = -UNITY_MATRIX_V[2].xyz;
//    			CamPos = _WorldSpaceCameraPos;    			
//    			
//    			ViewM  =  float4x4(
//    			right.x,right.y,right.z,-CamPos.x*right.x - CamPos.y * right.y - CamPos.z * right.z,   
//    			up.x,up.y,up.z,-CamPos.x*up.x - CamPos.y * up.y - CamPos.z * up.z,  	 
//    			-forward.x,-forward.y,-forward.z,-CamPos.x*(-forward.x) - CamPos.y * (-forward.y) - CamPos.z * (-forward.z),  	 
//    			0,0,0,1
//    			);
//    			
				//o.vertex = mul (mul(UNITY_MATRIX_P,mul(UNITY_MATRIX_V,_Camera2World)), v.vertex);
				
    			//END v.3.0.2
							
					
				
				o.screenPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.screenPos.z);
                o.ViewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.normal = v.normal;
                o.PointLights = ShadeVertexLights(v.vertex, dot(o.normal,o.ViewDir));	
						
								
			o.uv.xy = v.texcoord.xy;
			o.color = v.color* _TintColor;
			//o.texcoord = TRANSFORM_TEX(v.texcoord, _BorderTex);
			
			o.vWorldPosition = mul (unity_ObjectToWorld, v.vertex).xyz;					
			o.normal = v.normal;
			
				//Volume
				o.ForwLight = ObjSpaceLightDir(v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);	
				UNITY_TRANSFER_FOG(o,o.pos);
			return o; 
		}
		
		//Volume
		 sampler2D_float _CameraDepthTexture;
            float _InvFade;
		
		fixed4 frag( v2f IN ) : COLOR {	
		
			//#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)));
				float partZ = IN.screenPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				IN.color.a *= fade;
			//	#endif
		
		float3 Difference = IN.vWorldPosition - _WorldSpaceCameraPos +float3(300,1300,0);
					float4 mainColor = tex2D(_MainTex, _MainTex_ST*IN.uv); 
					float Norma = length(Difference);
					float3 BETAS = betaR + betaM;					
					float3 Fex = exp(-((BETAS) * Norma));
					//float cosTheta = dot(normalize(Difference), -sunPosition);
					//float cosTheta = dot(normalize(Difference), float3(sunPosition.x/1,sunPosition.y*2,-sunPosition.z)); //v3.3
					float cosTheta = dot(normalize(Difference), (float3(sunPosition.x,sunPosition.y,-sunPosition.z)) ); //v3.3
					
					float rPhase = rayleighPhase(cosTheta*0.5+0.5);
					float3 betaRTheta = betaR * rPhase;
					float mPhase = hgPhase(cosTheta, mieDirectionalG);
					float3 betaMTheta = betaM * mPhase;					
					float3 FEXX = 400 * Fex;					
					float3 Lin = ((betaRTheta + betaMTheta) / (BETAS)) * (400 - FEXX);
					float3 Combined = FEXX +  Lin; 
					float3 Scatter_power = Combined * 0.005;
					float diffuseFactor =  max(dot(IN.normal.xyz,  float3(sunPosition.x,sunPosition.y,-sunPosition.z) ), 0) ;  //v3.3
					float3 lightDirection = _WorldSpaceLightPos0.xyz;         				
					float3 lightColor = _LightColor0.rgb;
				
					float3 LightAll = float3(_Color * (mainColor.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 
					* saturate(dot(IN.normal, lightDirection)));
					float3 FinalColor = LightAll * Fex + Lin;					
				
		
		float4 return1 = tex2D (_MainTex,IN.uv.xy )* tex2D(_BorderTex, IN.uv.xy+float2(_Time.x*_Speed.x,_Time.x*_Speed.y)) * IN.color * _Intensity;
				
		
		float4 Combined1 = (dot(IN.ViewDir,IN.PointLights));	
		
		
		//Volume
		fixed4 col = tex2D(_MainTex, IN.uv) * IN.color.a;
              
               fixed lerp_vec = max (0.3, min(dot (IN.normal, IN.ForwLight),0.5)	);              
               //fixed atten = LIGHT_ATTENUATION(IN);
			   UNITY_LIGHT_ATTENUATION(atten, IN, IN.vWorldPosition.xyz); //v4.1
               float3 finalCol = (_Glow)* IN.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * atten * lerp_vec * 1* _LightIntensity * float4(Combined1.xyz,1)* _SunLightIntensity + _MinLight;
                           
			
			float4 FINAL = _Control.x*return1 + _Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) + _Control.z*float4(min(finalCol.rgb,1),col.a);
			//float4 FINAL =  _Control.z*float4(min(finalCol.rgb,1),col.a) + _Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) ;
					
					//FINAL.rgb *= IN.color.a;
//					half4 col1 = IN.color * tex2D(_MainTex, IN.texcoord);
//				col1.rgb *= col.a;
//				UNITY_APPLY_FOG_COLOR(IN.fogCoord, col1, fixed4(0,0,0,0)); // fog towards black due to our blend mode
//				return col1;
					
			UNITY_APPLY_FOG(IN.fogCoord,FINAL);
			return FINAL;	
			
		}	
			ENDCG
			SubShader {
			Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
				Cull Off
				Lighting Off
				ZWrite Off
				Fog{ Mode Off }
				Blend SrcAlpha OneMinusSrcAlpha

				Pass{

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