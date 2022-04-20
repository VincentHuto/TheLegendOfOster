// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkyMaster/StarsShader" {
    Properties {
        _Color ("Light Color", Color) = (1,1,1,1)
        _Ambient ("Ambient Color", Color) = (0,0,0,0)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Jitter ("Jitter", 2D) = "white" {}
        _SunDir ("Sun Direction", Vector) = (0,1,0,0)
        _CloudCover ("Cloud Cover", Range(0,1)) = 0.5
        _CloudSharpness ("Cloud Sharpness", Range(1,30)) = 8
        _CloudDensity ("Density", Range(0,5)) = 1
        _CloudSpeed ("Cloud Speed", Vector) = (0.001, 0, 0, 0)        
        
        _Light ("Sun Intensity", Range(0,10)) = 1
        
        _Bump ("Bump", 2D) = "white" {}
    }
    SubShader {
    Tags
        {
            "Queue"="Transparent-1450"
        
           //"Queue"="Geometry"
           // "RenderType"="Transparent"
           // "IgnoreProjector"="True"
        }
    Pass {
   
    //Blend SrcAlpha OneMinusSrcAlpha
   Blend One One
    Cull Front
    ZWrite Off
 
 
    CGPROGRAM
    //#pragma target 3.0
    #pragma vertex vert
    #pragma fragment frag
    #include "UnityCG.cginc"
 
 
    sampler2D _MainTex;
     float4 _MainTex_ST;
    float4 _Bump_ST;
    
    sampler2D _Jitter;
    sampler2D _Bump;
 
    float4 _Color;
    float4 _Ambient;
    float4 _SunDir;
    float4 _CloudSpeed;
 
 
  	float _Light;
 
    float _CloudCover;
   uniform float _CloudDensity;
    float _CloudSharpness;
 
 
    struct v2f {
        float4 pos : SV_POSITION;
        float4 tex : TEXCOORD0;
        
        
        float2 uv:TEXCOORD1;
        float3 dir:TEXCOORD2;
         float2 uv2:TEXCOORD3;
    };

  
    v2f vert (appdata_tan v) 
    {
        v2f o;
        o.pos = UnityObjectToClipPos (v.vertex); 
 
        float3 vertnorm = normalize(v.vertex.xyz);        
        
        float2 vertuv   = vertnorm.xz / (vertnorm.y + 0.2);       
     
        
        //o.uv = TRANSFORM_TEX(v.texcoord,_Bump);  

        o.uv = v.texcoord;  

        o.uv2 = TRANSFORM_TEX(v.texcoord,_MainTex);  
        
        o.tex = float4( vertuv.xy * 0.2, 0, 0 );               
        
        TANGENT_SPACE_ROTATION;
        o.dir = mul(rotation, _SunDir.xyz); 
        //TRANSFER_VERTEX_TO_FRAGMENT(o); 
        return o;
    }
 
 
    float4 frag (v2f i) : COLOR
    {       
    	float2 TexC = float2(i.uv.x,i.uv.y) ;


        float2 offset = _Time.y * _CloudSpeed.xy;
       // float4 tex = tex2D( _MainTex, (( i.uv2 ) + offset)*10.5  );       
          float4 tex = tex2D( _MainTex, TexC * _CloudDensity  );    
 
        //float Density = 0;
  
 
// fixed4 tex2N = tex2D( _Bump,i.uv2 + offset);
 //fixed3 UnpackedN = UnpackNormal(tex2N) * _Light;
 //fixed Bumped_lightingN = dot( UnpackedN, i.dir);
 fixed Bumped_lightingN =1;


  fixed4 tex2N = tex2D( _Jitter,(TexC + offset/15) * _CloudDensity*11 );
   //fixed4 tex2N = tex2D( _Jitter,TexC  );
        
        tex = max( tex - ( 1 - _CloudCover*2 ), 0 );
              
        float4 res = _Color.a * lerp(pow(tex2N,2),0.6,_CloudSharpness) * float4 (0.95 * _Color.r*Bumped_lightingN * tex.r, 0.95* _Color.g*Bumped_lightingN * tex.g, 0.95 * _Color.b*Bumped_lightingN * tex.b, _Color.a);
        
      //  res.xyz += _Ambient.xyz;
        return pow(abs(res),_Light);
    }
    ENDCG
 
 
    }
    }
}