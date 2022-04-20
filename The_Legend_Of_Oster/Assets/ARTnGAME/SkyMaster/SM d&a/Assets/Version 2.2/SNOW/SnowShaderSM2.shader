Shader "SkyMaster/SnowShaderSM2" {
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
        _Power ("Snow,Main,Blend Factors", Vector) = (0.5, 0.5, 1,1)
      //  worldPos ("Pos", Vector) = (0.5, 0.5, 1,1)        
        _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
    }
   
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
       
        CGPROGRAM
        #pragma surface surf BlinnPhong //vertex:vert
        //#pragma target 3.0
         
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
        float4 _Power;
       // float3 worldPos; 
        half _Shininess; 
        
        struct Input {        	
            float2 uv_MainTex;
           // float2 uv_Bump;
            float2 uv_SnowTexture;
            float2 uv_SnowBump;
            float3 worldNormal; //v3.3
            //float3 worldPos;
            INTERNAL_DATA         
        };        
       
       // void vert (inout appdata_full v) {         	
            
            //float3 Snow = normalize(_Direction.xyz);
           
            //if (dot(v.normal, Snow) >= lerp(1, -1, (_SnowCoverage * 2) / 3))
           // {
            //    v.vertex.xyz += normalize(v.normal + Snow)  * _SnowCoverage * 1;//_SnowBumpDepth
            //}           
       // }
 
        void surf (Input IN, inout SurfaceOutput o) {       
   			
   			float4 SnowTexColor = tex2D(_SnowTexture, IN.uv_SnowTexture);
            float4 MainTexColor = tex2D(_MainTex, IN.uv_MainTex);
            //o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));   
            o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_MainTex));  //v3.3         

            o.Alpha = MainTexColor.a;  
            float DirN = dot(WorldNormalVector(IN, o.Normal), _Direction.xyz)  ; 
            float Check = lerp(1,-1,_SnowCoverage/7); 
            float diff = Check - DirN;
            if(0 >= diff)
            {      
            	//v3.3
	            if(DirN == 0)
	            {
	             	DirN =  0.00001;
	            }    
	                 
             		//  o.Albedo = lerp (  MainTexColor.rgb , SnowTexColor.rgb*_LightIntensity,pow((1-(Check/(DirN))),_SnowBlend)) ;   
                	o.Albedo = lerp (  MainTexColor.rgb , SnowTexColor.rgb*_LightIntensity,pow((1-(diff)),_SnowBlend)) ;               
               		//	o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*_SnowBumpDepth);   
                  	o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*_SnowBumpDepth);                           
            }
            else{
            	o.Albedo = MainTexColor;
            } 
			//o.Specular = _Shininess;	//v3.4		
        }
        ENDCG
    }
    FallBack "Diffuse"
}