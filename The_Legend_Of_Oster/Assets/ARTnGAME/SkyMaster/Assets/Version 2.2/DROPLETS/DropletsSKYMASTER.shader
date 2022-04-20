// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkyMaster/DropletsSM" {
Properties {
	_BumpAmt  ("Distortion", range (0,300)) = 10
	_MainTex ("Tint Color (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}	
	_Drops ("Water Drops", 2D) = "white" {}
	//_Drops2 ("Water Drops", 2D) = "white" {}
	_Speed ("Speed", Float) = 6
	//_Speed2 ("Slider Time", Float) = 3
	_WaterAmount ("Water intensity", Float) = 1
	_FreezeFacrorA ("Freeze outwards1 (1,1,0)", range (-1,1)) = 0
	_FreezeFacrorB ("Freeze outwards2 (1,0,0 = uniform)", range (-1,1)) = 0
	_FreezeFacrorC ("Freeze inwards (0,0,1)", range (-1,1)) = 0
}

Category {

	Tags { "Queue"="Transparent+2" "RenderType"="Opaque" "IgnoreProjector"="True"}

	SubShader {

		GrabPass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
		}		

		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
			
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord: TEXCOORD0;
};

struct v2f {
	float4 vertex : SV_POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvbump : TEXCOORD1;
	float2 uvmain : TEXCOORD2;
	float2 uvDrops : TEXCOORD3;	
	//float2 uvDrops2 : TEXCOORD4;	
	UNITY_FOG_COORDS(4)
};

float _BumpAmt;
float4 _BumpMap_ST;
float4 _MainTex_ST;
float4 _Drops_ST;

sampler2D _Drops;

//float4 _Drops2_ST;

//sampler2D _Drops2;

uniform float _Speed;
uniform float _Speed2;
uniform float _WaterAmount;
uniform float _FreezeFacrorA;
uniform float _FreezeFacrorB;
uniform float _FreezeFacrorC;

v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.zw;
	o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
	o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
	o.uvDrops = TRANSFORM_TEX( v.texcoord, _Drops );
	//o.uvDrops2 = TRANSFORM_TEX( v.texcoord, _Drops2 );
	UNITY_TRANSFER_FOG(o,o.vertex);
	return o;
}

sampler2D _GrabTexture;
float4 _GrabTexture_TexelSize;
sampler2D _BumpMap;
sampler2D _MainTex;

half4 frag (v2f i) : SV_Target
{	
	half4 Wmotion = tex2D(_Drops, float2(i.uvDrops.x,(_Speed*_Time.x)+i.uvDrops.y))*cos(_Time.x)*0.05;
	//half4 Wmotion2 = tex2D(_Drops2, float2(i.uvDrops.x,(_Speed2*_Time.x)+i.uvDrops2.y))*cos(_Time.x)*0.05;
	half4 tint = tex2D(_MainTex, i.uvmain);
	
	//	half2 bump = UnpackNormal(tex2D( _BumpMap, float2(i.uvDrops.x,(_Speed*_Time.x)+i.uvDrops.y) )).rg ;
	half2 bump = UnpackNormal(tex2D( _BumpMap, float2(i.uvDrops.x,(_Speed*_Time.x)+i.uvDrops.y) )).rg  *  (_FreezeFacrorA*(1-(_FreezeFacrorB*tint.r)) + ((_FreezeFacrorC*tint.r)) );
	
	float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
	i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
	
	
	//half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab)-float4(Wmotion.r+Wmotion2.r,Wmotion.r+Wmotion2.r,0,0));
	half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab)-(_WaterAmount*float4(Wmotion.r,Wmotion.r,0,0)  ) );
	
	//col *= tint;
	//	col *= 1;//tint ;//(1-tint.r);
	
	UNITY_APPLY_FOG(i.fogCoord, col);
	return col*1.0;
}
ENDCG
		}
	}

	// Fallback 
	SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}
}
}