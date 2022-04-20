// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Skybox/Procedural_SkyMaster" {
Properties {
	_SunSize ("Sun Size", Range (0.0, 30.0)) = 1
	_SunTint ("Sun Tint", Color) = (1, .925, .737, 1)
	_SkyExponent ("Sky Gradient", Float) = 1.5
	_SkyTopColor ("Sky Top", Color) = (.008, .296, .586, 1)
	_SkyMidColor ("Sky Middle", Color) = (.570, .734, 1, 1)
	_SkyEquatorColor ("Sky Equator", Color) = (.917, .992, 1, 1)
	_GroundColor ("Ground", Color) = (.369, .349, .341, 1)	
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Off ZWrite Off

	Pass {
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		//#pragma target 3.0
		//#pragma glsl	
	
		half _SunSize =1;
		half4 _SunTint;
		half _SkyExponent;
		half4 _SkyTopColor;
		half4 _SkyEquatorColor;
		half4 _SkyMidColor;
		half4 _GroundColor;
		
			struct fragIO 
			{				
    			float4 pos : SV_POSITION;    			
    			float2 uv : TEXCOORD1;
    			half4 normalAndSunExp : TEXCOORD2;     			
			};		

		fragIO vert (appdata_base v)
		{				
		    fragIO OUTPUT;
    		OUTPUT.pos = UnityObjectToClipPos(v.vertex);
    		OUTPUT.uv = v.texcoord.xy;
			OUTPUT.normalAndSunExp.xyz = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
			OUTPUT.normalAndSunExp.w = (_SunSize > 0)? (256.0/_SunSize) : 0.0;
			return OUTPUT;
		}	
 	
		fixed4 frag (fragIO IN) : COLOR // SV_Target
		{	
			half3 normal = normalize(IN.normalAndSunExp.xyz);
			half t = normal.y;

			half3 sunColor =  _LightColor0.rgb * 2 * _SunTint * 2 *_SunSize;
			half3 sunDir =_WorldSpaceLightPos0.xyz; //v3LightDir;//
			half3 sun = (IN.normalAndSunExp.w > 0) ? pow(max(0.0, dot(normal, sunDir)), IN.normalAndSunExp.w) : 0.0;

			half3 c; 
			if (t > 0)
			{
				half skyT = 1-pow (1-t, _SkyExponent);
				if (skyT < 0.25){
					c = lerp (_SkyEquatorColor.rgb, _SkyMidColor.rgb,skyT*4);					
				}
				else{
					c = lerp (_SkyMidColor.rgb, _SkyTopColor.rgb, (skyT-0.25)*(4.0/3.0));	
					}
			}
			else
			{
				half groundT = 1-pow (1+t, 10.0);
				c = lerp (_SkyEquatorColor.rgb, _GroundColor.rgb, groundT);
				sun *= (1-groundT);
			}

			c = lerp(c, max(c, sunColor), sun);
			return half4(c, 1);
		}
		ENDCG 
	}
} 	

Fallback Off

}
