// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkyMaster/Cloud Dome" {
    Properties {
        _Color ("Light Color", Color) = (1,1,1,1)
        _Ambient ("Ambient Color", Color) = (0,0,0,0)
		_Brightness("Brightness Color", Vector) = (0,0,0,0)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Jitter ("Jitter", 2D) = "white" {}
        _SunDir ("Sun Direction", Vector) = (0,1,0,0)
        _CloudCover ("Cloud Cover", Range(0,1.2)) = 0.5
        _CloudSharpness ("Cloud Sharpness", Range(0.7,0.99999)) = 0.8       
        _CloudDensity ("Density", Range(0,1)) = 1
        _CloudSpeed ("Cloud Speed", Vector) = (0.001, 0, 0, 0)        
        _Bump ("Bump", 2D) = "white" {}
        _Forward ("Forward Mode (1)", Float) = 0 //use 1 in forward mode
        _CloudSize("Cloud size)", Float) = 1 //use 1 in forward mode
        _AmbientFactor("Reduce ambient)", Float) = 1 //use 1 in forward mode
        _HorizonCutoff("Horizon Cutoff Factor)", Float) = 0.48  //v3.3
    }
    SubShader {
    Tags
        {
           // "Queue"="Transparent-450"
           "Queue"="Transparent-1"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }
        
//         Pass {
//          	ColorMask 0
//          }
        
    Pass {
   
   // Blend SrcAlpha OneMinusSrcAlpha
   Blend SrcAlpha One
    Cull Front
    ZWrite Off 
 	//ColorMask RGB
 	//Lighting On
 
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #include "UnityCG.cginc" 
    //#pragma target 3.0
 
    sampler2D _MainTex;
    sampler2D _Jitter;
    sampler2D _Bump;
 
 //v3.0
   float4 _MainTex_ST;
    float4 _Bump_ST;
 
    float4 _Color;
    float4 _Ambient;
    float4 _SunDir;
    float4 _CloudSpeed; 
	float4 _Brightness;
 
    float _CloudCover;
    uniform float _CloudDensity;
    float _CloudSharpness; 
    float _Forward;
    float _CloudSize;
    float _AmbientFactor;
    float _HorizonCutoff;//v3.3
 
    struct v2f {
        float4 pos : SV_POSITION;
        float4 tex : TEXCOORD0;
        
        //v3.0
        float2 uv : TEXCOORD1;
      // float3 dir : TEXCOORD2;
        float2 uv2:TEXCOORD3; 
    };  

    v2f vert (appdata_tan v) 
    {
        v2f o;
        o.pos = UnityObjectToClipPos (v.vertex); 
        float3 vertnorm = normalize(v.vertex.xyz);        
       // float2 vertuv   = vertnorm.xz / (vertnorm.y + 0.2);
        float2 vertuv   = vertnorm.xz / (vertnorm.y*0.4 + 0.2);
        o.tex = float4( vertuv.xy * 0.3, 0, 0 );
        
        //v3.0
        
         o.uv = TRANSFORM_TEX(v.texcoord,_Bump);  
               o.uv2 = TRANSFORM_TEX(v.texcoord,_MainTex);  
        
        TANGENT_SPACE_ROTATION;
       // o.dir = mul(rotation,_SunDir);
      //  TRANSFER_VERTEX_TO_FRAGMENT(o);
        return o;
    } 
 
    float4 frag (v2f i) : COLOR
    {     
        float2 offset = _Time.y * _CloudSpeed.xy;
        float4 tex = tex2D( _MainTex, ( i.tex.xy ) + offset );        
        float2 offset1 = 0.5*_Time.y * _CloudSpeed.xy;
        float4 tex1 = tex2D( _MainTex, ( i.tex.xy ) + offset1 );        
        float2 offset2 = 0.7*_Time.y * _CloudSpeed.xy+_CloudDensity;
        float4 tex2 = tex2D( _MainTex, ( i.tex.xy )*_CloudSize + offset2 );
 //float2 offset3 = 1.7*_Time.y * _CloudSpeed.xy+_CloudDensity;
     //   float4 tex3 = tex2D( _MainTex, ( i.tex.xy ) + offset3 );
 
 //v3.0
  float Density = 0;  
 // _SunDir.z *= 128;
  fixed4 tex2N = tex2D( _Bump,i.uv + offset);
 fixed3 UnpackedN = UnpackNormal(tex2N) * 1;
 //fixed Bumped_lightingN = dot( UnpackedN, i.dir);
 
 
 
 		if(_Forward==0 || _Forward==2 || _Forward==3){
	        tex = max( tex - ( 1 - _CloudCover*2 ), 0 );
	        tex1 = max( tex1 - ( 1 - _CloudCover*2 ), 0 );
	        tex2 = max( tex2 - ( 1 - _CloudCover*2 ), 0 ); 		
	        tex.a = 1.0 - pow( _CloudSharpness, tex.a * 255 );
        }
        
        if(_Forward==4){
	        tex = max( tex - ( 1 - _CloudCover*1.4 ), 0 );
	        tex1 = max( tex1 - ( 1 - _CloudCover*1.4 ), 0 );
	        tex2 = max( tex2 - ( 1 - _CloudCover*1.4 ), 0 ); 		
	        tex.a = 1.0 - pow( _CloudSharpness, tex.a * 255 );
        }
       
             
       
        float Light = 1;
        float4 res = float4 (Light * _Color.r*1, Light * _Color.g*1, Light * _Color.b*1, tex.a*tex1.a*tex2.a);
       

        res *=_Ambient*_AmbientFactor;      

        if(i.uv.y < _HorizonCutoff){
        	res = float4(res.r,res.g,res.b,0);
        }

		return (res) - float4(_Brightness.x, _Brightness.y, _Brightness.z, 0);// -float4(_Brightness.x, _Brightness.x, _Brightness.x, 0);// float4(0.5, 0.5, 0.5, 1);
    }
    ENDCG 
 
    }
   }
}