// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Particle Dynamic Magic/VolumeLitParticlesPDM" {
	Properties {
    _MainTex ("Particle Texture", 2D) = "white" {}
    _FinalTex ("Final Texture", 2D) = "white" {}
    _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
    _Glow ("Glow Factor", Range(0.0,10.0)) = 1.0
    _LightIntensity ("Light Intensity", Range(0.0,5.0)) = 1.0
    _SunLightIntensity ("Sun Intensity", Range(0.0,5.0)) = 1.0
    _MinLight ("Mimize light", Range(-1.0,1.5)) = 0.0
    _SunColor ("Sun Color", Color) = (0,0,0,0)

		_EmisColor("_Emis Color", Color) = (0,0,0,0)
} 
Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }    
    Blend SrcAlpha OneMinusSrcAlpha
	Cull Back	
 	Lighting Off
 	Zwrite Off  
    SubShader {
        Pass {    

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
 
            sampler2D _MainTex;
            sampler2D _FinalTex;
            float _Glow;
            float _LightIntensity;
            float _SunLightIntensity;
            float _MinLight;
            fixed4 _SunColor;	
			float4 _EmisColor;

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
 			
            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                #ifdef SOFTPARTICLES_ON
                o.position = ComputeScreenPos (o.pos);
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
           
            fixed4 frag (v2f i) : SV_Target
            {
               #ifdef SOFTPARTICLES_ON
                float Depth = LinearEyeDepth ( SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD (i.position)) );
                float DepthP=i.position.z; 
                float fade = saturate (( Depth-DepthP ) * _InvFade);                               
                i.color.a *= fade;
               #endif
               fixed4 col = tex2D(_MainTex, i.texcoord) * i.color.a;
               fixed lerp_vec = max (0, dot (i.normal, i.ForwLight)	);               
               //fixed atten = LIGHT_ATTENUATION(i);               
               //float3 finalCol = (_Glow)* i.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * lerp_vec * 2 * _SunLightIntensity *_LightIntensity *atten * _LightColor0.rgb + _MinLight;
               //float3 finalCol = (_Glow)* i.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * lerp_vec * 2 * _SunLightIntensity *_LightIntensity  + _MinLight;
			   float3 _LightColor = float3(1, 1, 1) * 0.3; //HDRP
               float3 finalCol = (_Glow)* i.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * lerp_vec * 2 * _SunLightIntensity *_LightIntensity* _LightColor + _MinLight; //v3.3

               return float4(finalCol.rgb,col.a) ; 
            }
            ENDCG
        }
    }
}
}