Shader "SkyMaster/Terrain SNOW Lite" {
Properties {
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125

	_SnowCoverage ("Snow Coverage", Range(0, 1)) = 0
	_SnowBlend ("Snow Blend", Range(0, 50)) = 0.4     
    _LightIntensity ("Light Intensity", Range(0.5, 50)) = 1
    _SnowBumpDepth ("Snow bump depth", Range(0, 5)) = 1

	_SnowTexture ("Snow texture", 2D) = "white" {}
	_SnowBump ("Snow Bump", 2D) ="bump" {} 	 
	_Direction ("Direction of snow", Vector) = (0, 1, 0)
	_Depth ("Depth of Snow", Range(0, 0.02)) = 0.01 

	// set by terrain engine
	[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
	[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
	[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
	[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
	[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
	[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
	[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
	[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
	[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
	// used in fallback on old cards & base map
	[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
	
}
	
SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert
#pragma target 3.0
// needs more than 8 texcoords
#pragma exclude_renderers gles

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 uv_Splat0 : TEXCOORD1;
	float2 uv_Splat1 : TEXCOORD2;
	float2 uv_Splat2 : TEXCOORD3;
	float2 uv_Splat3 : TEXCOORD4;
            
    float2 uv_SnowTexture: TEXCOORD5;
    float2 uv_SnowBump: TEXCOORD6;            
    float3 worldNormal: TEXCOORD7; 
                   
    INTERNAL_DATA            
};

sampler2D _Control;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
sampler2D _Normal0,_Normal1,_Normal2,_Normal3;
half _Shininess;

        float _SnowCoverage;
		float _SnowBlend;  
        float _LightIntensity;
        float _SnowBumpDepth;

		sampler2D _SnowTexture;
        sampler2D _SnowBump;

        float3 _Direction;
        float _Depth;
              float4 _Color;  
        
void vert (inout appdata_full v)
{
v.tangent.xyz = cross(v.normal, float3(0,0,1));
v.tangent.w = -1;             
             
            float3 Snow = normalize(_Direction.xyz);
           
            if (dot(v.normal, Snow) >= lerp(1, -1, (_SnowCoverage * 2) / 3))
            {
                v.vertex.xyz += normalize(v.normal + Snow)  * _SnowCoverage * _Depth;
            }             
}

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.uv_Control);
	fixed4 col;
	col  = splat_control.r * tex2D (_Splat0, IN.uv_Splat0)*(_Color*2);
	col += splat_control.g * tex2D (_Splat1, IN.uv_Splat1);
	col += splat_control.b * tex2D (_Splat2, IN.uv_Splat2);
	col += splat_control.a * tex2D (_Splat3, IN.uv_Splat3);
//	o.Albedo = col.rgb;

	fixed4 nrm;
	nrm  = splat_control.r * tex2D (_Normal0, IN.uv_Splat0);
	nrm += splat_control.g * tex2D (_Normal1, IN.uv_Splat1);
	nrm += splat_control.b * tex2D (_Normal2, IN.uv_Splat2);
	nrm += splat_control.a * tex2D (_Normal3, IN.uv_Splat3);
	// Sum of our four splat weights might not sum up to 1, in
	// case of more than 4 total splat maps. Need to lerp towards
	// "flat normal" in that case.
	fixed splatSum = dot(splat_control, fixed4(1,1,1,1));
	fixed4 flatNormal = fixed4(0.5,0.5,1,0.5); // this is "flat normal" in both DXT5nm and xyz*2-1 cases
	nrm = lerp(flatNormal, nrm, splatSum);
	o.Normal = UnpackNormal(nrm);

	o.Gloss = col.a * splatSum;
	o.Specular = _Shininess;
	//o.Alpha = 0.0;	
	
			//SNOW                      
            float4 SnowTexColor = tex2D(_SnowTexture, IN.uv_SnowTexture);
            float4 MainTexColor = col ;             
            o.Alpha = MainTexColor.a;  
            float DirN = dot(WorldNormalVector(IN, o.Normal), _Direction.xyz)  ; 
            float Check = lerp(1,-1,_SnowCoverage); 
            if(DirN >= Check)
            {
               //o.Albedo = lerp (  MainTexColor.rgb , SnowTexColor.rgb*_LightIntensity,pow((1-(Check/DirN)),_SnowBlend));     
               o.Albedo = lerp (  MainTexColor.rgb , SnowTexColor.rgb*_LightIntensity,pow((1-(Check/DirN)),_SnowBlend))* 1;           
			   o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowTexture)));                                                 
            }
            else{
            	//o.Albedo = MainTexColor.rgb*0.5+_Color/2;
            	o.Albedo = col.rgb;//+_Color/2;
            } 
			o.Specular = _Shininess;			
}
ENDCG  
}

Dependency "AddPassShader" = "Hidden/Nature/Terrain/Bumped Specular AddPass"
Dependency "BaseMapShader" = "Specular"

Fallback "Nature/Terrain/Diffuse"
}

