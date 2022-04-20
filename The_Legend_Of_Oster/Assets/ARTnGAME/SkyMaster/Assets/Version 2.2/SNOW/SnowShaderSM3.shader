Shader "SkyMaster/SnowShaderSM3" {
    Properties {    	
        //_SnowCoverage ("Snow Coverage", Range(0, 1)) = 0   
        _SnowBlend ("Snow Blend", Range(0, 50)) = 0.4     
        _LightIntensity ("Light Intensity", Range(0.5, 50)) = 1
        _SnowBumpDepth ("Snow bump depth", Range(0, 5)) = 1          
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Bump ("Bump", 2D) = "bump" {}        
        _SnowTexture ("Snow texture", 2D) = "white" {}
        _Depth ("Depth of Snow", Range(0, 0.02)) = 0.01        
        _SnowBump ("Snow Bump", 2D) ="bump" {}        
        _Direction ("Direction of snow", Vector) = (0, 1, 0)         
        _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
        _Wetness ("Wetness", Range (-0, 20)) = 1
        
        _Mask ("Mask", 2D) = "black" {}
        _MainTex2 ("Albedo 1", 2D) = "white" {}
        _NormalMap2 ("NormalMap 1", 2D) = "bump" {}		
		_Water ("Water", 2D) = "white" {}		
		water_level ("Water level", Float) = 1
		water_spec ("Water Spec Focus", Float) = 1.2		
		water_tiling ("Water tiling", Float) = 2		
		_BumpPower ("Bump Power", Range (3, 0.01)) = 1 
		
		_Color ("Tint", Color) = (1,1,1,1) 
		
		//v3.0.2
		Snow_Cover_offset ("Snow coverage offset", Float) = 0
		
		[LM_Specular] [LM_Glossiness] _SpecGlossMap("Specular 0", 2D) = "white" {}
		[LM_Specular] [LM_Glossiness] _SpecGlossMap2("Specular 1", 2D) = "white" {}
    }
   
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
       
        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf StandardSpecular fullforwardshadows vertex:vert
 		#pragma exclude_renderers gles
        
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
		
		fixed _BumpPower;
		float4 _Color;
		float Snow_Cover_offset;
 
        struct Input {        
            float2 uv_MainTex;
            float2 uv_Bump;
            float2 uv_SnowTexture;
            float2 uv_SnowBump;
            float3 worldNormal;      
            
            float2 uv_MainTex2;
			float2 uv_Water;
			float2 uv_Mask;
			
			INTERNAL_DATA
        };        
       
        void vert (inout appdata_full v) {            
            float3 Snow = normalize(_Direction.xyz);
           
            if (dot(v.normal, Snow) >= lerp(1, -1, ((_SnowCoverage+Snow_Cover_offset) * 2) / 3))
            {
                v.vertex.xyz += normalize(v.normal + Snow)  * (_SnowCoverage+Snow_Cover_offset) * _Depth;
            }           
        }
 
        void surf (Input IN, inout SurfaceOutputStandardSpecular o) {   
   
   			//PUDDLES
   			float2 Mask_motion = float2(IN.uv_Mask.x,IN.uv_Mask.y+(_Time.x*5))*0.05 ;
			float2 A_motion = float2(IN.uv_MainTex.x,IN.uv_MainTex.y+(_Time.x*5))*0.005 ;
			float2 B_motion = float2(IN.uv_MainTex2.x,IN.uv_MainTex2.y+(_Time.x*1)) ;
		
			fixed blend = tex2D(_Mask, IN.uv_Mask).a*1;	
			fixed4 albedo1 = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 spec1	= tex2D(_SpecGlossMap, IN.uv_MainTex);
		 	fixed3 normal1 = UnpackNormal (tex2D (_Bump, IN.uv_MainTex)); //_NormalMap

			half4 flow = float4(1,1,1,1);
			half4 flow1 = tex2D(_Water, float2(IN.uv_MainTex2.x*water_tiling,IN.uv_MainTex2.y+(_Time.x*0.5)) );

			fixed4 albedo2 = tex2D(_MainTex2, float2(IN.uv_MainTex2.x+flow.r,IN.uv_MainTex2.y+flow.r));
			fixed4 spec2 = tex2D(_SpecGlossMap2, float2(IN.uv_MainTex2.x+flow.r,IN.uv_MainTex2.y+flow.r)) * _Wetness;
		 	fixed3 normal2 = UnpackNormal (tex2D (_NormalMap2, float2(IN.uv_MainTex2.x+flow.r,IN.uv_MainTex2.y+flow.r)+ (0.03)  ));
		 	fixed4 specGloss = lerp (spec1, spec2, blend);
   			//END PUDDLES  
   
   			//SNOW
   			float4 SnowTexColor = tex2D(_SnowTexture, IN.uv_SnowTexture);
            float4 MainTexColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));           

            o.Alpha = MainTexColor.a;  
            float DirN = dot(WorldNormalVector(IN, o.Normal), _Direction.xyz)  ; 
            float Check = lerp(1,-1,(_SnowCoverage+Snow_Cover_offset)/5.5); //divide by 6 to synch with the slower building of snow on Unity trees
            if(DirN >= Check)
            {                
                o.Albedo = lerp (  lerp (albedo1, albedo2, blend) , SnowTexColor.rgb*_LightIntensity,pow((1-(Check/DirN)),_SnowBlend)) ;              
                o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*_SnowBumpDepth);                                
            }
            else{            	
            	//PUDDLES INTEGRATION
			 	o.Albedo 		= lerp (albedo1, albedo2, blend) ;
			 	o.Specular 		= specGloss.rgb ;
				o.Smoothness 	= water_spec*specGloss.a+ water_level*flow1*specGloss.a;
			  	o.Normal 		= lerp (normal1, normal2, blend);
			 	o.Normal.z = o.Normal.z*_BumpPower; 
			 	o.Normal = normalize(o.Normal); 
	   			//END PUDDLES
            } 	
			o.Specular = _Shininess;           
        }
        ENDCG
    }
    FallBack "Diffuse"
}