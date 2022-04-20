// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SkyMaster/SMFoggyTerrainFP" {
Properties { 
    [HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
    [HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
    [HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
    [HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
    [HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}     
    [HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
     _Color ("Main Color", Color) = (1,1,1,1)     
   //// _Ramp ("Ramp (RGB)", 2D) = "gray" {}     
    _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    _Outline ("Outline width", Range (.002, 0.03)) = .005    
     _LowFogHeight ("LowFogHeight", Range (0, 150)) = 0
     _HighFogHeight ("HighFogHeight", Range (0, 200)) = 150
}
     
SubShader {
    Tags {
        "SplatCount" = "3"
        "Queue" = "Geometry-99"
        "RenderType" = "Opaque"
    }     
    CGPROGRAM
    #pragma surface surf Lambert exclude_path:prepass vertex:vert
    //#pragma exclude_renderers d3d11 d3d11_9x
    //#pragma target 3.0

    uniform sampler2D _Control;
	uniform sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
    uniform fixed4 _Color;
   // uniform sampler2D _Ramp;
    uniform float _LowFogHeight;
    uniform float _HighFogHeight;
    uniform float4 _OutlineColor;
 
    struct Input {
        float4 pos: POSITION;
        float2 tc_FirstTex : TEXCOORD0;
        float2 uv_Splat0 : TEXCOORD1;
        //float2 uv_Splat1 : TEXCOORD2;
        //float2 uv_Splat2 : TEXCOORD3;
        //float2 uv_Splat3 : TEXCOORD4;///
    };
    
	struct appdata_full2 {
		float4 vertex : POSITION;
		float4 tangent : TANGENT;
		float3 normal : NORMAL;
		float4 texcoord : TEXCOORD0;
		float4 texcoord1 : TEXCOORD1;
		float4 texcoord2 : TEXCOORD2;
		float4 texcoord3 : TEXCOORD3; 
		fixed4 color : COLOR;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};


    void vert (inout appdata_full2 v, out Input o)
    {
			UNITY_INITIALIZE_OUTPUT(Input, o);
            //float4 hpos = UnityObjectToClipPos (v.vertex);
            o.pos = mul(unity_ObjectToWorld, v.vertex);
            o.tc_FirstTex = v.texcoord.xy;     
    } 
 
    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 splat_control = tex2D (_Control, IN.tc_FirstTex);
        fixed4 col=float4(0,0,0,0);
        col += splat_control.r * tex2D (_Splat0, IN.tc_FirstTex);
        col += splat_control.g * tex2D (_Splat1, IN.tc_FirstTex);
        col += splat_control.b * tex2D (_Splat2, IN.tc_FirstTex);
        col += splat_control.a * tex2D (_Splat3, IN.tc_FirstTex);//v4.3 was IN.uv_Splat3
        o.Albedo = col * _Color;
        o.Alpha = 0.0; 
        
		//float4 c = tex2D (_Ramp, IN.uv_FirstTex);
		float Fogy = clamp((IN.pos.y - _LowFogHeight) / (_HighFogHeight - _LowFogHeight), 0, 1);
        o.Albedo =  lerp (_OutlineColor, (col * _Color), Fogy);
    }
    ENDCG
    UsePass "Toon/Basic Outline/OUTLINE"
 
}

Fallback "Diffuse"
 
} 