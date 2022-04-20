Shader "Hidden/Nature/Tree Creator Leaves Optimized" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
	_ShadowStrength("Shadow Strength", Range(0,1)) = 0.8
	_ShadowOffsetScale ("Shadow Offset Scale", Float) = 1
	
	_ShadowOffsetScale1 ("Shadow Offset Scale1", Float) = 1
	
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_ShadowTex ("Shadow (RGB)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R) Shadow Offset (B)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (B) Gloss(A)", 2D) = "white" {}
	
	
	
	////
	 _SnowBlend ("Snow Blend", Range(0, 50)) = 0.4     
        _LightIntensity ("Light Intensity", Range(0.5, 50)) = 1
        _SnowBumpDepth ("Snow bump depth", Range(0, 5)) = 1          
       
        _Bump ("Bump", 2D) = "bump" {}        
        _SnowTexture ("Snow texture", 2D) = "white" {}
        _Depth ("Depth of Snow", Range(0, 0.02)) = 0.01        
        _SnowBump ("Snow Bump", 2D) ="bump" {}        
        _Direction ("Direction of snow", Vector) = (0, 1, 0)
        _Power ("Snow,Main,Blend Factors", Vector) = (0.5, 0.5, 1,1)
          
        _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	

	// These are here only to provide default values
	[HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
	[HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
	[HideInInspector] _SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags {
		"IgnoreProjector"="True"
		"RenderType"="TreeLeaf"
	}
	LOD 200
	
CGPROGRAM
#pragma surface surf TreeLeaf1 alphatest:_Cutoff vertex:TreeVertLeaf1 nolightmap noforwardadd
#include "UnityBuiltin3xTreeLibrary.cginc"
//#pragma target 3.0

sampler2D _SnowTexture;
sampler2D _Bump;
sampler2D _SnowBump;  

float _SnowCoverage; 
        float _SnowBlend;  
        float _LightIntensity;
        float _SnowBumpDepth;        
        float _Depth;              
        float3 _Direction;
        float4 _Power;       
        half _Shininess; 

sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;

uniform float4 _UnityTerrainTreeTintColorSM;

void TreeVertLeaf1 (inout appdata_full v)
{
	ExpandBillboard (UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
	v.vertex.xyz *= _TreeInstanceScale.xyz;
	v.vertex = AnimateVertex (v.vertex,v.normal, float4(v.color.xy, v.texcoord1.xy));
	
	v.vertex = Squash(v.vertex);
	
	v.color.rgb = _TreeInstanceColor.rgb * _Color.rgb;
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);
	
//	 float3 Snow = normalize(_Direction.xyz);
//           
//            if (dot(v.normal, Snow) >= lerp(1, -1, (_SnowCoverage * 2) / 3))
//            {
//                v.vertex.xyz += normalize(v.normal + Snow)  * _SnowCoverage * _Depth;
//            }  
}

half4 LightingTreeLeaf1 (LeafSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	half3 h = normalize (lightDir + viewDir);
	
	half nl = dot (s.Normal, lightDir);
	
	half nh = max (0, dot (s.Normal, h));
	half spec = pow (nh, s.Specular * 128.0) * s.Gloss;
	
	// view dependent back contribution for translucency
	fixed backContrib = saturate(dot(viewDir, -lightDir));
	
	// normally translucency is more like -nl, but looks better when it's view dependent
	backContrib = lerp(saturate(-nl), backContrib, _TranslucencyViewDependency);
	
	fixed3 translucencyColor = backContrib * s.Translucency * _TranslucencyColor;
	
	// wrap-around diffuse
	nl = max(0, nl * 0.6 + 0.4);
	
	fixed4 c;
	/////@TODO: what is is this multiply 2x here???
	c.rgb = s.Albedo * (translucencyColor * 2 + nl);
	c.rgb = c.rgb * _LightColor0.rgb + spec;
	
	// For directional lights, apply less shadow attenuation
	// based on shadow strength parameter.
	#if defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
	c.rgb *= lerp(1, atten, _ShadowStrength);
	#else
	c.rgb *= atten;
	#endif

	c.a = s.Alpha;
	
	return c *1;
}

struct Input {
	float2 uv_MainTex;
	  float2 uv_Bump;
            float2 uv_SnowTexture;
            float2 uv_SnowBump;
	float4 color : COLOR; // color.a = AO
	INTERNAL_DATA
};

void surf (Input IN, inout LeafSurfaceOutput o) {
	//fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	
	float4 MainTexColor = tex2D(_MainTex, IN.uv_MainTex);
	float4 SnowTexColor = tex2D(_SnowTexture, IN.uv_SnowTexture);
            
    //        o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));           

    //        o.Alpha = MainTexColor.a;  
            float DirN = dot(WorldNormalVector(IN, o.Normal), _Direction.xyz)  ; 
            float Check = lerp(1,-1,_SnowCoverage); 
           if(IN.uv_MainTex.y >= 1-(0.1 * _SnowCoverage + (0.1*MainTexColor.r) ))
           // if(IN.uv_MainTex.y >= 1-(0.5 * _SnowCoverage))
            //if(DirN >= Check)
            {     
//            	if(DirN==0){
//            		DirN =11;
//            	  } 	   

     			//if(IN.uv_MainTex.y >= 1-(7 * _SnowCoverage) * o.Albedo.r* o.Albedo.r)     
	                //o.Albedo = lerp (  MainTexColor.rgb , SnowTexColor.rgb*_LightIntensity,pow((1-(Check/DirN)),_SnowBlend)) ;   
	                //o.Albedo = lerp (  MainTexColor.rgb , SnowTexColor.rgb*1,1-(Check/DirN)) ;        
	                 o.Albedo = lerp (  MainTexColor.rgb* IN.color.rgb/3  , SnowTexColor.rgb*0.99,1-(0.1 * _SnowCoverage)) ;   
	                //o.Albedo = lerp (  MainTexColor.rgb , SnowTexColor.rgb*_LightIntensity,pow((Check-(DirN)),_SnowBlend)) ;         
	                o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*1);    
                //}                            
            }
            else
            {
				o.Albedo = MainTexColor.rgb * IN.color.rgb * IN.color.a *_UnityTerrainTreeTintColorSM *1.5;
			}
//			if(o.Albedo.r > 0.99){
//				o.Albedo = o.Albedo/2;
//			}
	
	float4 trngls = tex2D (_TranslucencyMap, IN.uv_MainTex);
	o.Translucency = trngls.b/2.5;//trngls.b;
	o.Gloss = trngls.a * _Color.r;
	o.Alpha = MainTexColor.a/0.5;// STOP FLICKER !!!!!!!!!!!!!!!!!
	
	half4 norspc = tex2D (_BumpSpecMap, IN.uv_MainTex);
	//o.Specular = norspc.r;
	o.Normal = UnpackNormalDXT5nm(norspc);
}
ENDCG

	// Pass to render object as a shadow caster
	Pass {
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }
		
		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma multi_compile_shadowcaster
		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl

		#include "UnityBuiltin3xTreeLibrary.cginc"

		sampler2D _ShadowTex;

		struct Input {
			float2 uv_MainTex;
		};

		struct v2f_surf {
			V2F_SHADOW_CASTER;
			float2 hip_pack0 : TEXCOORD1;
		};
		float4 _ShadowTex_ST;
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			TreeVertLeaf (v);
			o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _ShadowTex);
			TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			return o;
		}
		fixed _Cutoff;
		float4 frag_surf (v2f_surf IN) : SV_Target {
			half alpha = tex2D(_ShadowTex, IN.hip_pack0.xy).r;
			clip (alpha - _Cutoff);
			SHADOW_CASTER_FRAGMENT(IN)
		}
		ENDCG
	}
	
}

Dependency "BillboardShader" = "Hidden/Nature/Tree Creator Leaves Rendertex"
}
