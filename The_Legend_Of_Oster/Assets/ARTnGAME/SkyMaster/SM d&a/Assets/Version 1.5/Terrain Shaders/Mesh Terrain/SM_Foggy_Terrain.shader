// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkyMaster/SM_Foggy_Terrain" {
	Properties {
		_Color ("Main Color", COLOR) = (1,1,1) 
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry" }
		LOD 200
	
		Pass {
			Tags {"LightMode" = "ForwardBase"}
         	Lighting On
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"			
				#pragma multi_compile_fwdbase 
				float3 _Color;
				sampler2D _MainTex;				
				///////////////////
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

				struct v2f {
					float4 position:SV_POSITION;	
					float2 uv:TEXCOORD0;
					float3 vWorldPosition:TEXCOORD1;
					float3 normals:TEXCOORD2;
		 			LIGHTING_COORDS(3,4) 
				};
			
				v2f vert(appdata_full v) {
					v2f o;
					
					UNITY_INITIALIZE_OUTPUT(v2f,o);
					
					o.position = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord.xy;					
					o.vWorldPosition = mul (unity_ObjectToWorld, v.vertex).xyz;					
					o.normals = v.normal;				
					return o;
				}
			
				float4 frag(v2f IN) : COLOR {
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
					float diffuseFactor =  max(dot(IN.normals.xyz, sunPosition ), 0) ; 
					float3 lightDirection = _WorldSpaceLightPos0.xyz;         				
					float3 lightColor = _LightColor0.rgb;
				
					float3 LightAll = float3(_Color * (mainColor.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 
					* saturate(dot(IN.normals, lightDirection)));
					float3 FinalColor = LightAll * Fex + Lin;					
					return  float4(1.0-exp2(-FinalColor*ExposureBias),0);
				}
			ENDCG
		}

		//v3.3
		Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag           
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster                                   
           
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                //float2 uv0 : TEXCOORD1;
                //float3 normalDir : TEXCOORD0;
            };
            VertexOutput vert (appdata_base v) {
                VertexOutput o;
                //o.uv0 = v.texcoord0;               
                //o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;               
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
//                i.normalDir = normalize(i.normalDir);
//                float2 node_585 = i.uv0;
//                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_585.rg, _Diffuse));
//                clip(node_1.a - _Cutoff);      

                
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }

	}
	Fallback "Diffuse"
}