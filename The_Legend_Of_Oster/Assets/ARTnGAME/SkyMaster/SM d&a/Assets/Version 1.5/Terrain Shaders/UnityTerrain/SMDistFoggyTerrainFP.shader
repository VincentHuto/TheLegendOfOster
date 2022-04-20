// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkyMaster/SMDistFoggyTerrainFP" {
Properties { 
    [HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
    [HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
    [HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
    [HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
    [HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}     
    [HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
     _Color ("Main Color", Color) = (1,1,1,1)     
    _Ramp ("Ramp (RGB)", 2D) = "gray" {}     
    _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    _Outline ("Outline width", Range (.002, 0.03)) = .005    
     _LowFogHeight ("LowFogHeight", Range (0, 150)) = 0
     _HighFogHeight ("HighFogHeight", Range (0, 200)) = 150
     

}
     
SubShader {
    
    
    	Tags {"SplatCount" = "4" "RenderType"="Opaque" "Queue"="Geometry-100" }
		LOD 200

Pass {
			Tags {"LightMode" = "ForwardBase"}
         	Lighting On
			CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"			
				#pragma multi_compile_fwdbase 
				float3 _Color;
				sampler2D _MainTex;		
				
						
	uniform sampler2D _Control;
    uniform sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
   
    uniform sampler2D _Ramp;
    uniform float _LowFogHeight;
    uniform float _HighFogHeight;
    uniform float4 _OutlineColor;
												
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
				uniform float4 _Ramp_ST;

				struct v2f {
					float4 position:SV_POSITION;
						
					float2 uv:TEXCOORD0;					
					float2 uv_Splat0 : TEXCOORD1;
			        float2 uv_Splat1 : TEXCOORD2;
			        float2 uv_Splat2 : TEXCOORD3;
			        float2 uv_Splat3 : TEXCOORD4;				
					
					float3 vWorldPosition:TEXCOORD5;
					float3 normals:TEXCOORD6;
		 			LIGHTING_COORDS(7,8)  
				};
			
			
			
				float4 _Splat0_ST;
				float4 _Splat1_ST;
				float4 _Splat2_ST;
				float4 _Splat3_ST;
				
				
				v2f vert(appdata_full v) {
					v2f o;
					
					UNITY_INITIALIZE_OUTPUT(v2f,o);
					
					o.position = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord.xy;	
					
					o.uv_Splat0 = TRANSFORM_TEX(v.texcoord.xy, _Splat0);// TRANSFORM_TEX(v.uv_Splat0, _Splat0);
					o.uv_Splat1 = TRANSFORM_TEX(v.texcoord.xy, _Splat1);
					o.uv_Splat2 = TRANSFORM_TEX(v.texcoord.xy, _Splat2);
					o.uv_Splat3 = TRANSFORM_TEX(v.texcoord.xy, _Splat3);		
																	
					o.vWorldPosition = mul (unity_ObjectToWorld, v.vertex).xyz;					
					o.normals = v.normal;				
					return o;
				}
			
			
			
			
			
				float4 frag(v2f IN) : COLOR {
					float3 Difference = IN.vWorldPosition - _WorldSpaceCameraPos;
					
				
					
        fixed4 splat_control = tex2D (_Control, IN.uv);
        fixed3 col;
        col  = splat_control.r * tex2D (_Splat0, IN.uv_Splat0).rgb;
        col += splat_control.g * tex2D (_Splat1, IN.uv_Splat1).rgb;
        col += splat_control.b * tex2D (_Splat2, IN.uv_Splat2).rgb;
        col += splat_control.a * tex2D (_Splat3, IN.uv_Splat3).rgb;


				
				float4 mainColor = float4(col.xyz,1);
					  
					    
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
    

 
}

Fallback "Diffuse"
 
} 