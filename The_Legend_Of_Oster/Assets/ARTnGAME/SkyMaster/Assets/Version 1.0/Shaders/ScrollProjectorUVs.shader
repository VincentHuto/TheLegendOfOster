// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/Animated" {
	Properties {
		_xSpeed ("xSpeed", Float) = 1.0
		_ySpeed ("ySpeed", Float) = 1.0
		_Color ("Color", Color) = (1,1,1,1)
		_ShadowTex ("Projected Image", 2D) = "white" {}
	}
	SubShader {
		Pass {      
			Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM
	 
			#pragma vertex vert  
			#pragma fragment frag 
			#include "UnityCG.cginc"
			
			fixed _xSpeed;
			fixed _ySpeed;
			fixed4 _Color;
			uniform sampler2D _ShadowTex; 
	 
			uniform fixed4x4 unity_Projector; 
	 
			struct vertexInput {
				fixed4 vertex : POSITION;
			};
			
			struct vertexOutput {
				fixed4 pos : SV_POSITION;
				fixed4 posProj : TEXCOORD0;
			};
	 
			vertexOutput vert(vertexInput input) {
				vertexOutput output;
				output.posProj = mul(unity_Projector, input.vertex);
				output.pos = UnityObjectToClipPos(input.vertex);
				return output;
			}
	 
			fixed4 frag(vertexOutput input) : COLOR{
				if (input.posProj.w > 0.0)  { 
					fixed2 anim;
					anim.x = _xSpeed * _Time.y;
					anim.y = _ySpeed * _Time.y;
					return _Color * tex2D(_ShadowTex ,(fixed2(input.posProj.xy) / input.posProj.w) )+ 0.1*float4(anim.x,anim.x,anim.x,anim.y); 
				}
				else { 
					return fixed4(0.0,0.0,0.0,0.0);
				}
			}
 
			ENDCG
		}
	}
   // Fallback "Projector/Light"
}
