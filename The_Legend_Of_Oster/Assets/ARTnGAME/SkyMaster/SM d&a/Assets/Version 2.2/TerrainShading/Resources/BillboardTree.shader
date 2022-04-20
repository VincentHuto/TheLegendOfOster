// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/TerrainEngine/BillboardTree" {
	Properties {
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		
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
	}
	
	
	
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector"="True" "RenderType"="TreeBillboard" }
		
		Pass {
			ColorMask rgb
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off 
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			// #include "AutoLight.cginc"
            //#include "Lighting.cginc"
			//#include "TerrainEngine.cginc"
			
			// ---- Billboarded tree helpers
	CBUFFER_START(UnityTerrain)
	// billboards
	float3 _TreeBillboardCameraRight;
	float4 _TreeBillboardCameraUp;
	float4 _TreeBillboardCameraFront;
	float4 _TreeBillboardCameraPos;
	float4 _TreeBillboardDistances; // x = max distance ^ 2
	CBUFFER_END

void TerrainBillboardTree( inout float4 pos, float2 offset, float offsetz )
{
	float3 treePos = pos.xyz - _TreeBillboardCameraPos.xyz;
	float treeDistanceSqr = dot(treePos, treePos);
	if( treeDistanceSqr > _TreeBillboardDistances.x )
		offset.xy = offsetz = 0.0;
		
	// positioning of billboard vertices horizontally
	pos.xyz += _TreeBillboardCameraRight.xyz * offset.x;
	
	// tree billboards can have non-uniform scale,
	// so when looking from above (or bellow) we must use
	// billboard width as billboard height
	
	// 1) non-compensating
	//pos.xyz += _TreeBillboardCameraUp.xyz * offset.y;
	
	// 2) correct compensating (?) 
	float alpha = _TreeBillboardCameraPos.w;
	float a = offset.y;
	float b = offsetz;
		// 2a) using elipse-radius formula
		//float r = abs(a * b) / sqrt(sqr(a * sin(alpha)) + sqr(b * cos(alpha))) * sign(b);
		//float r = abs(a) * b / sqrt(sqrt(a * sin(alpha)) + sqrt(b * cos(alpha)));
		// 2b) sin-cos lerp
		//float r = b * sin(alpha) + a * cos(alpha);	
	//pos.xyz += _TreeBillboardCameraUp.xyz * r;
	
	// 3) incorrect compensating (using lerp)
	// _TreeBillboardCameraPos.w contains ImposterRenderTexture::GetBillboardAngleFactor()
	//float billboardAngleFactor = _TreeBillboardCameraPos.w;
	//float r = lerp(offset.y, offsetz, billboardAngleFactor);	
	//pos.xyz += _TreeBillboardCameraUp.xyz * r;
	
	// so now we take solution #3 and complicate it even further...
	// 
	// case 49851: Flying trees
	// The problem was that tree billboard was fixed on it's center, which means
	// the root of the tree is not fixed and can float around. This can be quite visible
	// on slopes (checkout the case on fogbugz for screenshots).
	//
	// We're fixing this by fixing billboards to the root of the tree. 
	// Note that root of the tree is not necessary the bottom of the tree - 
	// there might be significant part of the tree bellow terrain.
	// This fixation mode doesn't work when looking from above/below, because
	// billboard is so close to the ground, so we offset it by certain distance
	// when viewing angle is bigger than certain treshold (40 deg at the moment)
	
	// _TreeBillboardCameraPos.w contains ImposterRenderTexture::billboardAngleFactor
	float billboardAngleFactor = _TreeBillboardCameraPos.w;
	// The following line performs two things:
	// 1) peform non-uniform scale, see "3) incorrect compensating (using lerp)" above
	// 2) blend between vertical and horizontal billboard mode
	float radius = lerp(offset.y, offsetz, billboardAngleFactor);
			
	// positioning of billboard vertices veritally
	pos.xyz += _TreeBillboardCameraUp.xyz * radius;
			
	// _TreeBillboardCameraUp.w contains ImposterRenderTexture::billboardOffsetFactor
	float billboardOffsetFactor = _TreeBillboardCameraUp.w;
	// Offsetting billboad from the ground, so it doesn't get clipped by ztest.
	// In theory we should use billboardCenterOffsetY instead of offset.x,
	// but we can't because offset.y is not the same for all 4 vertices, so 
	// we use offset.x which is the same for all 4 vertices (except sign). 
	// And it doesn't matter a lot how much we offset, we just need to offset 
	// it by some distance
	pos.xyz += _TreeBillboardCameraFront.xyz * abs(offset.x) * billboardOffsetFactor;
}

	struct v2f {
		float4 pos : SV_POSITION;
		fixed4 color : COLOR0;
		float2 uv : TEXCOORD0;
		float3 posi : TEXCOORD1;
		UNITY_FOG_COORDS(2)
		//LIGHTING_COORDS(2,3)
	};
			
			struct appdata_tree_billboard {
	float4 vertex : POSITION;
	fixed4 color : COLOR;			// Color
	float4 texcoord : TEXCOORD0;	// UV Coordinates 
	float2 texcoord1 : TEXCOORD1;	// Billboard extrusion
		
};

			
			
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

//sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;

			
			uniform float4 _UnityTerrainTreeTintColorSM;

			v2f vert (appdata_tree_billboard v) {
				v2f o;
				TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);	
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv.x = v.texcoord.x;
				o.uv.y = v.texcoord.y > 0;
				o.color = v.color;
				UNITY_TRANSFER_FOG(o,o.pos);
				
				o.posi=UnityObjectToClipPos (v.vertex);
				
				return o;
			}

			sampler2D _MainTex;
			fixed4 frag(v2f input) : SV_Target
			{
			
				fixed4 col = tex2D( _MainTex, input.uv);
				float4 SnowTexColor = tex2D(_SnowTexture, input.uv);
				
				if(input.uv.y >= 1-(3 * _SnowCoverage) * col.r* col.r)
	            {     	       
	                //col =  lerp (  col , SnowTexColor*0.9,1-(0.5 * _SnowCoverage)) ;   
	                //    col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;
	                //o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*1);   
	                col.rgb = float4(input.uv.y,input.uv.y,input.uv.y,1)*1;                             
	            }
	            else
	            {
					//col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;						
					col.rgb *= input.color.rgb;
					clip(col.a);
					col=col* _UnityTerrainTreeTintColorSM;
				}
				//float3 lightColor = _LightColor0.rgb;
				UNITY_APPLY_FOG(input.fogCoord, col);
				//return col * _UnityTerrainTreeTintColorSM * float4(_LightColor0.rgb,1);
				return col  ;
			}
			ENDCG			
		}
		
		
		
		
	}

	Fallback Off
}
