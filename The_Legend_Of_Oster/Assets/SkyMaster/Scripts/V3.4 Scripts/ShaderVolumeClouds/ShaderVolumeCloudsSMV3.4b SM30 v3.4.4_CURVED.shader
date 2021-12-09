// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld' //v3.4.6

Shader "SkyMaster/ShaderVolumeClouds-Desktop-SM3.0 v3.4.4 CURVED" {
    Properties {
        _SunColor ("_SunColor", Color) = (0.95,0.95,0.95,0.8)
        _ShadowColor ("_ShadowColor", Color) = (0.05,0.05,0.1,0.3)
        _ColorDiff ("_ColorDiff", Float ) = 0.5
        _CloudMap ("_CloudMap", 2D) = "white" {}
        _CloudMap1 ("_CloudMap1", 2D) = "white" {}
        _Density ("_Density", Float ) = -0.4
        _Coverage ("_Coverage", Float ) = 4250
        _Transparency ("_Transparency", Float ) = 1
        _Velocity1 ("_Velocity1", Vector ) = (1,23,0,0)
        _Velocity2 ("_Velocity2", Vector ) = (1,22,0,0)   
        _LightingControl ("_LightingControl", Vector) = (1,1,-1,0)       
        _HorizonFactor ("_HorizonFactor", Range(0, 10)) = 2    
        _EdgeFactors ("_EdgeFactor2", Vector) = (0,0.52,-1,0) 
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        //_Mode ("_Mode", Range(0,5)) = 0
        _CutHeight ("_CutHeight", Float) = 240

        Thickness ("Thickness", Float) = 1
        _CoverageOffset ("_Coverage Offset", Float ) = 0
        _ColorDiffOffset ("_ColorDiff Offset", Float ) = 0
        _TranspOffset ("_Transparency Offset", Float ) = 0

        //SCATTER
        _Control ("Control Color", COLOR) = (1,1,1)
        _Color ("Color", COLOR) = (1,1,1) 
        _FogColor ("Fog Color", COLOR) = (1,1,1) 
        _FogFactor ("Fog factor", float) = 1
        _FogUnity ("Fog on/off(1,0)", float) = 0
        _PaintMap ("_PaintMap", 2D) = "white" {}

		//HDRP
		pointLightPos("_pointLightPos", Vector) = (0,0,0,0)
		pointLightPower("pointLightPower", float) = 1
		pointLightColor ("_pointLightColor", Vector) = (0,0,0,0)

		//CURVED
		_Bend("Bend", Vector) = (1,1,1,1)
		_BendPos("Bend Center Position", Vector) = (0,0,0,0)
		_BendGradient("Gradient", float) = 1.0
		_BendGradientPower("Gradient Power", Range(0.000001,20)) = 1.0

			//SCATTER	
			_Color("_Color", Vector) = (1,1,1,1)
			_Control("_Control", Vector) = (1,1,1,1)
			up("up", Vector) = (0,1,0,1)
			sunPosition("sunPosition", Vector) = (1,1,1,1)
			//pi("pi", float) = 3.141592653589793238462643383279502884197169
			rayleighZenithLength("rayleighZenithLength", float) = 8400
			mieZenithLength("mieZenithLength", float) = 1250
			sunAngularDiameterCos("sunAngularDiameterCos", float) = 0.999956676946448443553574619906976478926848692873900859324 
			betaR("betaR", float) = 1.0
			betaM("betaM", float) = 0.1
			fog_depth("fog_depth", float) = 2.0
			mieCoefficient("mieCoefficient", float) = 0.05
			mieDirectionalG("mieDirectionalG", float) = 0.8
			ExposureBias("ExposureBias", float) = 1.0

			/*float3 _Color = float3(1.0, 1.0, 1.0);
			float3 _Control = float3(1.0, 1.0, 1.0);

			static const float pi = 3.141592653589793238462643383279502884197169;
			static const float rayleighZenithLength = 8.4E3;
			static const float mieZenithLength = 1.25E3;
			static const float3 up = float3(0.0, 1.0, 0.0);

			static const float sunAngularDiameterCos = 0.999956676946448443553574619906976478926848692873900859324;


			float3 sunPosition = float3(1.0, 1.0, 1.0);
			float3 betaR = 1;
			float3 betaM = 0.1;

			float fog_depth = 2.0;
			float mieCoefficient = 0.05;
			float mieDirectionalG = 0.8;
			float ExposureBias = 1.0;*/

			//v4.2
			[Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Int) = 2
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }       
        Pass {
            //Name "ForwardBase"
            Tags {
               // "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
			//v4.2
			Cull[_CullMode]
            
            CGPROGRAM
            #include "UnityCG.cginc"

            //SCATTER
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            #pragma vertex vert
            #pragma fragment frag
           // #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile_fog 
            #pragma multi_compile_fwdbase                      
            #pragma target 3.0
			#pragma multi_compile_fwdbase nolightmap //v4.1

			 sampler2D _CloudMap; 
             float4 _CloudMap_ST;
             sampler2D _CloudMap1; 
             float4 _CloudMap1_ST;
            //float4 _LightColor0;
             float4 _SunColor = float4(1.0, 1.0, 1.0,1.0);
             float4 _ShadowColor = float4(1.0, 1.0, 1.0, 1.0);
			 float _ColorDiff = 0.1;
             float _Density = 0.1;
             float _Coverage = 0.1;
             float _Transparency = 0.1;
             float _HorizonFactor = 0.1;
             float4 _LightingControl = float4(1.0, 1.0, 1.0, 1.0);
             float2 _EdgeFactors = float2(1.0, 1.0);
             float2 _Velocity1 = float2(1.0, 1.0);;
             float2 _Velocity2 = float2(1.0, 1.0);;
            // uniform int _Mode;
             float _CutHeight = 0.1;
             float4 _FogColor = float4(1.0, 1.0, 1.0, 1.0);;
             float _FogFactor = 0.1;
             float _FogUnity = 0.1;

            //uniform sampler2D _PaintMap;
              float  _ColorDiffOffset = 0.1;
              float Thickness = 0.1;
             float _CoverageOffset = 0.1;
             float _TranspOffset = 0.1;	

             sampler2D _PaintMap;
             float4 _PaintMap_ST;

			//HDRP
			float3 pointLightPos = float3(1.0, 1.0, 1.0);
			float pointLightPower = float3(0.0, 0.0, 0.0);
			float3 pointLightColor = float3(1.0, 1.0, 1.0);
			float fogPower = 1;
			float fogPowerExp = 1;

            //SCATTER			
			float3 _Color = float3(1.0, 1.0, 1.0);
			float3 _Control = float3(1.0, 1.0, 1.0);
			static  float pi = 3.141592653589793238462643383279502884197169;
			static  float rayleighZenithLength = 8.4E3;
			static  float mieZenithLength = 1.25E3;
			static  float3 up = float3(0.0, 1.0, 0.0);				
			
			static  float sunAngularDiameterCos = 0.999956676946448443553574619906976478926848692873900859324;	
			

			float3 sunPosition = float3(1.0, 1.0, 1.0);
			float3 betaR = 1;
			float3 betaM = 0.1;

				float fog_depth = 2.0;
				float mieCoefficient =0.05;
				float mieDirectionalG = 0.8;				
				float ExposureBias = 1.0;
				float rayleighPhase(float cosTheta)
				{
					return (3.0 / (16.0*pi)) * (1.0 + pow(cosTheta, 2.0));
				}		
								
				float hgPhase(float cosTheta, float g)
				{   float POW = pow(g, 2.0);
					return (0.25 / pi) * ((1.0 - POW) / pow(1.0 - 2.0*g*cosTheta + POW, 1.5)); 
				}


				//CURVED
				float3 _Bend;
				float3 _BendPos;
				float _BendGradient;
				float _BendGradientPower;
				float4 MakeCurved(float4 vertex)
				{
					float4 worldPos = mul(unity_ObjectToWorld, vertex); //Get world vertex position
					float distance = pow(max(0, length(_BendPos - worldPos) - _BendGradient), _BendGradientPower);					
					worldPos.xyz += _Bend * distance;
					return mul(unity_WorldToObject, worldPos); //Ruturn to object position
				}


            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;    
                float2 texcoord0 : TEXCOORD0;
				//v4.2
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 worldPos : TEXCOORD1;    
                   float3 ForwLight: TEXCOORD2;  
                   float3 camPos:TEXCOORD3;  
                     float3 normal : TEXCOORD4;                      
                LIGHTING_COORDS(5,6)                    
                UNITY_FOG_COORDS(7)
					//v4.2
					UNITY_VERTEX_OUTPUT_STEREO
            };
            VertexOutput vert (VertexInput v) {           
             	VertexOutput o;  

				//v4.2 - https://docs.unity3d.com/Manual/SinglePassInstancing.html
				UNITY_SETUP_INSTANCE_ID(v); //Insert
				UNITY_INITIALIZE_OUTPUT(VertexOutput, o); //Insert
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                o.uv0 = v.texcoord0;    

				//CURVED
				v.vertex = MakeCurved(v.vertex);

                o.pos = UnityObjectToClipPos(v.vertex);                         
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                //SCATTER
                //  TANGENT_SPACE_ROTATION;
                // o.ForwLight = mul(rotation,ObjSpaceLightDir(v.vertex)); //ObjSpaceLightDir(v.vertex);
                o.ForwLight =ObjSpaceLightDir(v.vertex); //ObjSpaceLightDir(v.vertex);
                o.camPos = normalize(WorldSpaceViewDir(v.vertex));
                o.normal = v.normal;
                TRANSFER_VERTEX_TO_FRAGMENT(o);		


                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

				/*
            	float change_h = _CutHeight;//240;
				float PosDiff = 0.0006*(i.worldPos.y-change_h);

                float2 UVs = _Density*float2(i.worldPos.x,i.worldPos.z);
                float4 TimingF = 0.0012;

                float2 UVs1 = _Velocity1*TimingF*_Time.y + UVs;

                float4 cloudTexture = tex2D(_CloudMap,UVs1+_CloudMap_ST);
                float4 cloudTexture1 = tex2D(_CloudMap1,UVs1+_CloudMap1_ST);

                //_PaintMap
               // float4 paintTexture1 = tex2D(_PaintMap,UVs1+_CloudMap1_ST);
                float4 paintTexture1 = tex2D(_PaintMap,UVs1*_PaintMap_ST.zw*_PaintMap_ST.xy);

                float2 UVs2 = (_Velocity2*TimingF*_Time.y/1.4 + float2(_EdgeFactors.x,_EdgeFactors.y) + UVs);

                float4 Texture1 = tex2D(_CloudMap,UVs2+_CloudMap_ST); 
                float4 Texture2 = tex2D(_CloudMap1,UVs2+_CloudMap1_ST); 

                float DER = i.worldPos.y*0.001;               
                float3 normalA = (((DER*((_Coverage+_CoverageOffset)+((cloudTexture.rgb*2)-1)))-(1-(Texture1.rgb*2))));             	
             	float3 normalN = normalize(normalA); 


             	//SCATTER
            	float3 Difference = i.worldPos.xyz - _WorldSpaceCameraPos;
					//float4 mainColor = tex2D(_CloudMap, UVs1); 
					float Norma = length(Difference);
					float3 BETAS = betaR + betaM;					
					float3 Fex = exp(-((BETAS) * Norma));
					//float cosTheta = dot(normalize(Difference), -sunPosition);
					//float cosTheta = dot(normalize(Difference), float3(sunPosition.x/1,sunPosition.y*2,-sunPosition.z)); //v3.3
					float cosTheta = dot(normalize(Difference), (float3(sunPosition.x,sunPosition.y,sunPosition.z)) ); //v3.3					
					float rPhase = rayleighPhase(cosTheta*0.5+0.5);
					float3 betaRTheta = betaR * rPhase;
					float mPhase = hgPhase(cosTheta, mieDirectionalG);
					float3 betaMTheta = betaM * mPhase;					
					float3 FEXX = 400 * Fex;					
					float3 Lin = ((betaRTheta + betaMTheta) / (BETAS)) * (400 - FEXX);
					float3 Combined = FEXX +  Lin; 
					float3 Scatter_power = Combined * 0.005;
					float diffuseFactor =  max(dot(normalN.xyz,  float3(sunPosition.x,sunPosition.y,sunPosition.z) ), 0) ;  //v3.3
					float3 lightDirection = _WorldSpaceLightPos0.xyz;         				
					float3 lightColor = _LightColor0.rgb;
				//	float3 LightAll = float3(_Color * (Texture2.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 				
					float3 LightAll = float3(_SunColor * (Texture2.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 
					* saturate(1-dot(normalN, lightDirection)));
					float3 FinalColor = (LightAll * Fex + Lin)*0.2; //(LightAll * Fex + Lin)*0.2;	


//            	float4 return1 = tex2D (_MainTex,IN.uv.xy )* tex2D(_BorderTex, IN.uv.xy+float2(_Time.x*_Speed.x,_Time.x*_Speed.y)) * IN.color * _Intensity;			
//				float4 Combined1 = (dot(IN.ViewDir,IN.PointLights));		
//				//Volume
//				fixed4 col = tex2D(_MainTex, IN.uv) * IN.color.a;              
//               	fixed lerp_vec = max (0.3, min(dot (IN.normal, IN.ForwLight),0.5)	);        

               	//fixed atten = LIGHT_ATTENUATION(i);       
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos); //v4.1

               //	float3 finalCol = (_Glow)* IN.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * atten * lerp_vec * 1* _LightIntensity * float4(Combined1.xyz,1)* _SunLightIntensity + _MinLight;			
				//float4 FINAL = _Control.x*return1 + _Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) + _Control.z*float4(min(finalCol.rgb,1),col.a);


                

//				float DER1 = -(i.worldPos.y-0)*PosDiff;
//             	float PosTDiff = i.worldPos.y*PosDiff;
             	float DER1 = -(i.worldPos.y-0)*1;
             	float PosTDiff = i.worldPos.y*1;
             	if(i.worldPos.y > change_h){             		
             		DER1 = (1-cloudTexture1.a) *  PosTDiff;
             		//DER1 =  PosTDiff;

             	}
//             	float diff3 = change_h+511 - i.worldPos.y;
//             	if(diff3 > 0){ 
//             		_Coverage = _Coverage - 0.001111*diff3;
//             	}

      //       	float shaper = (_Transparency-0.48+0.0)*((DER1*saturate((_Coverage+cloudTexture1.a)))-Texture2.a);		////////////////////////////////////////
             	//float shaper = ((_Transparency+_TranspOffset)-0.48+0.0)*((DER1*saturate(((_Coverage+_CoverageOffset)+cloudTexture1.a)))*Texture2.a);
             	float shaper = ((_Transparency+_TranspOffset)-0.48+0.0)*((DER1*(((_Coverage+_CoverageOffset)+cloudTexture1.a)))*Texture2.a);

                float3 lightDirect = normalize(_WorldSpaceLightPos0.xyz);
               	lightDirect.y = -lightDirect.y;
               
                float ColDiff =  ( (_ColorDiff+_ColorDiffOffset) -0.082)+((1+(DER*_LightingControl.r*_LightingControl.g))*0.5); 

                float verticalFactor = dot(lightDirect, float3(0,1,0));
    //         	float Lerpfactor = (ColDiff+(_ShadowColor.a*(dot(lightDirect,normalN)-1)*ColDiff));
     		    float Lerpfactor = (ColDiff+(_ShadowColor.a*(dot(i.ForwLight,normalN)-1)*ColDiff));

                float3	 ColB = _SunColor.rgb;
               // if(_Mode==0){
	                change_h =10;   
	                PosDiff =  Thickness*0.0006*(i.worldPos.y-change_h);  
	                PosTDiff = i.worldPos.y*PosDiff;          
	             	DER1 = -(i.worldPos.y+0)*PosDiff;

	             	if(i.worldPos.y > change_h){             		
	             		//DER1 = (1-cloudTexture1.a) *  PosTDiff;
	             		DER1 = (1-cloudTexture1.a) *  PosTDiff;
	             	}

	             	if(i.worldPos.y > 150){ 																																///////////////////////////// 650
	             		DER1 = (1-cloudTexture1.a) *  PosTDiff*0.07;
	             		shaper = shaper*shaper;
	             	}

	             	ColB =1*_SunColor.a*(1-cloudTexture1.a)*_SunColor.rgb*DER1*(1-verticalFactor);///////
             	//}

             	//SCATTER
             	float diff = saturate(dot(normalN, normalize(i.ForwLight)))+0.7;//worldPos
           //  	float diff = saturate(dot((-i.camPos), normalize(i.ForwLight)))+0.7;//worldPos

             	//float diff2 = (dot(float3(0,1,0), (i.normal)));//worldPos
             	float diff2 = distance(_WorldSpaceCameraPos,i.worldPos)*distance(_WorldSpaceCameraPos,i.worldPos);//worldPos

               // float3 endColor =   lerp(_ShadowColor.rgb*(diff*_LightColor0.rgb*atten),(FinalColor*_LightColor0.rgb*atten*diff)*ColB, Lerpfactor  );  //+ 11*float4(1.0-exp2(-FinalColor*0.04),0) ;

//               float3 finalCol = diff*_LightColor0.rgb* atten;	
                 float3 finalCol = diff*_SunColor.rgb* atten;	
              float3 endColor = _Control.x*_SunColor*lerp(_ShadowColor.rgb,saturate((FinalColor+0.5))*ColB, Lerpfactor)*diff*atten  +  (1.2-shaper*0.5)*_Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) + _Control.z*float4(min(finalCol.rgb,1),Texture1.a);


//				 float3 endColor = _Control.x*_SunColor*lerp(_ShadowColor.rgb,saturate((FinalColor+0.5))*ColB*0.1, Lerpfactor)*diff*atten  
//				 +  1.5*cloudTexture1.a*(1.2-shaper*0.5)*_Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) 
//				 +  2* cloudTexture1.a*(1)*_Control.z*float4(min(finalCol.rgb,1),Texture1.a);



              //    float3 endColor =   lerp(_ShadowColor.rgb*(diff*1*atten),(FinalColor*1*atten*diff)*ColB, Lerpfactor  ) + _LightColor0.rgb; 

              //  float4 Fcolor = float4((endColor),saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2)  ))*atten;///////////////////
          //     float4 Fcolor = float4(saturate(endColor + _FogFactor*diff2*_FogColor*0.00000001 + 0),saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2)  )) ;

          		//v3.3e
//               float4 Fcolor = float4(saturate(endColor*0.9 + 0.5*endColor*_FogFactor*1*diff2*_FogColor*0.00000001),saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2)  )) ;	///////////////////////////// 
//                float4 Fcolor = float4(saturate(endColor*0.9 + 0.1*endColor*_FogFactor*1*diff2*_FogColor*0.000000001*1),saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2*1)  )) ;
               // float4 Fcolor = float4(saturate(endColor*0.9 + 0.1*endColor*_FogFactor*1*diff2*_FogColor*0.00000001*1),saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2*1)  )) ;

			//	float4 Fcolor = float4(saturate(endColor*0.5 + 0.2*endColor*_FogFactor * 1 * diff2*_FogColor*0.0000001 * 1), saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2 * 1)));
			  float4 Fcolor = float4(saturate(endColor*ColB.rgb), saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2 * 1)));

               if(_FogUnity==1){
					UNITY_APPLY_FOG(i.fogCoord, Fcolor);
               }
             //   return Fcolor;
                return (float4(clamp(Fcolor.r, 0.02,1), clamp(Fcolor.g, 0.02, 1), clamp(Fcolor.b, 0.02, 1), Fcolor.a*paintTexture1.a));
				*/


					


				float change_h = _CutHeight;//240;
				float PosDiff = Thickness * 0.0006*(i.worldPos.y - change_h) - 0.4;

				float2 UVs = _Density * float2(i.worldPos.x, i.worldPos.z);
				float4 TimingF = 0.0012;//0.0012

				float2 UVs1 = _Velocity1 * TimingF*_Time.y + UVs;

				float4 cloudTexture = tex2D(_CloudMap, UVs1 + _CloudMap_ST);
				float4 cloudTexture1 = tex2D(_CloudMap1, UVs1 + _CloudMap1_ST);

				//_PaintMap
				//float4 paintTexture1 = tex2D(_PaintMap,float4(_PaintMap_ST.xy,UVs1*_PaintMap_ST.zw));
				float4 paintTexture1 = tex2D(_PaintMap, UVs1*_PaintMap_ST.xy + _PaintMap_ST.zw);

				float2 UVs2 = (_Velocity2*TimingF*_Time.y + float2(_EdgeFactors.x, _EdgeFactors.y) + UVs);

				float4 Texture1 = tex2D(_CloudMap, UVs2 * 1 + _CloudMap_ST);
				float4 Texture2 = tex2D(_CloudMap1, UVs2 * 1 + _CloudMap1_ST);

				float DER = i.worldPos.y*0.001;
				float3 normalA = (((DER*((_Coverage + _CoverageOffset) + ((cloudTexture.rgb * 2) - 1))) - (1 - (Texture1.rgb * 2)))) * 1;             /////// -0.25 coverage	(-0.35,5)
				float3 normalN = normalize(normalA);

				//SCATTER             
				//SCATTER
				float3 Difference = i.worldPos.xyz - _WorldSpaceCameraPos;
				//float4 mainColor = tex2D(_CloudMap, UVs1); 
				float Norma = length(Difference);
				float3 BETAS = betaR + betaM;
				float3 Fex = exp(-((BETAS)* Norma));
				//float cosTheta = dot(normalize(Difference), -sunPosition);
				//float cosTheta = dot(normalize(Difference), float3(sunPosition.x/1,sunPosition.y*2,-sunPosition.z)); //v3.3
				float cosTheta = dot(normalize(Difference), (float3(sunPosition.x, sunPosition.y, sunPosition.z))); //v3.3					
				float rPhase = rayleighPhase(cosTheta*0.5 + 0.5);
				float3 betaRTheta = betaR * rPhase;
				float mPhase = hgPhase(cosTheta, mieDirectionalG);
				float3 betaMTheta = betaM * mPhase;
				float3 FEXX = 400 * Fex;
				float3 Lin = ((betaRTheta + betaMTheta) / (BETAS)) * (400 - FEXX);
				float3 Combined = FEXX + Lin;
				float3 Scatter_power = Combined * 0.005;
				float diffuseFactor = max(dot(normalN.xyz, float3(sunPosition.x, sunPosition.y, sunPosition.z)), 0);  //v3.3
				float3 lightDirection = _WorldSpaceLightPos0.xyz;
				float3 lightColor = _LightColor0.rgb;
				//	float3 LightAll = float3(_Color * (Texture2.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 				
				float3 LightAll = float3(_SunColor * (Texture2.xyz * diffuseFactor * Combined  * lightColor + Scatter_power)
					* saturate(1 - dot(normalN, lightDirection)));
				float3 FinalColor = (LightAll * Fex + Lin)*0.2; //(LightAll * Fex + Lin)*0.2;


				//fixed atten = LIGHT_ATTENUATION(i);   
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos.xyz); //v4.1

																   //     	float DER1 = -(i.worldPos.y)*PosDiff-95;  //-95
				float DER1 = (i.worldPos.y)*PosDiff - 95;  //-95
				float PosTDiff = i.worldPos.y;
				if (i.worldPos.y > change_h) {
					DER1 = (1 - cloudTexture1.a);
				}

				float shaper = (_Transparency + 4.5 +_TranspOffset) *((DER1*saturate(((_Coverage + _CoverageOffset) - (0.8*PosDiff) + cloudTexture1.a* (Texture2.a))))); /////////////////// DIFERMCE			//////////////// * 30   _Transparency /////// -0.3 coverage	
																																						  //	float shaper = (_Transparency+4.5) *( (DER1*saturate(( (_Coverage +_CoverageOffset)   -(0.8*PosDiff)+cloudTexture1.a ))))* (Texture2.a)   ;

				float3 lightDirect = sunPosition;// normalize(_WorldSpaceLightPos0.xyz);
				lightDirect.y = -lightDirect.y;

				float ColDiff = (_ColorDiff + _ColorDiffOffset) + ((1 + (DER*_LightingControl.r*_LightingControl.g))*0.5);

				float verticalFactor = dot(lightDirect, float3(0, 1, 0));
				float Lerpfactor = (ColDiff + (_ShadowColor.a*(dot(lightDirect, normalN) - 1)*ColDiff));

				float ColB = _SunColor.rgb;

				change_h = 11;   //10
				PosDiff = 0.0004*(i.worldPos.y - change_h);
				PosTDiff = i.worldPos.y*PosDiff;
				DER1 = -(i.worldPos.y)*PosDiff;

				if (i.worldPos.y > change_h) {
					DER1 = (1 - cloudTexture1.a) *  PosTDiff;
				}
				ColB = 1 * _SunColor.a*(1 - cloudTexture1.a)*_SunColor.rgb*DER1*(1 - verticalFactor);


				//SCATTER
				//    	float diff = saturate(dot((-i.camPos), normalize(i.ForwLight)))+0.7;
				float diff = saturate(dot((normalN), normalize(i.ForwLight))) + 0.7;

				float diff2 = distance(_WorldSpaceCameraPos, i.worldPos)*distance(_WorldSpaceCameraPos, i.worldPos);

				float3 finalCol = diff * _LightColor0.rgb* atten;

				float3 endColor = (_Control.x*lerp(_ShadowColor.rgb, (0.7)*ColB, Lerpfactor) + _Control.z*float4(min(finalCol.rgb, 1), Texture1.a))*_SunColor.rgb;//_Color;

				float4 Fcolor = float4(saturate(endColor + (_FogFactor / 3)  *diff2*_FogColor*0.00000001 + 0), saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2))); //-8 _FogFactor


				if (_FogUnity == 1) {
					UNITY_APPLY_FOG(i.fogCoord, Fcolor);
				}
				float3 colAAA = float3(Fcolor.r, Fcolor.g, Fcolor.b);

				//POINT LIGHT
				float3 pointPos = pointLightPos;// float3(-450, 100, -1340);
				float dist = length(i.worldPos - pointPos);
				//colAAA = colAAA + pointLightPower * 0.7 * pow(100,2) * (1 / pow(dist, 2))*pointLightColor;// float3(1, 1, 1);
				colAAA = colAAA + pointLightPower * 0.7 * pow(100, 1.4) * (1 / pow(dist, 1.4))*pointLightColor;// float3(1, 1, 1);

				float3 colorOUT = (pow(colAAA, 1.1) + FinalColor * 1);
				if (saturate(colorOUT).r ==0) {
					colorOUT = float3(1, 1, 1);
				}

				////////// VOLUME FOG COMPENSATION
				float distToVertex = length(i.worldPos - _WorldSpaceCameraPos);
				
				colorOUT = saturate(lerp(colorOUT, _FogColor, fogPower * pow(distToVertex, fogPowerExp-0.3) * 0.0015));
				//if (distToVertex > 1000 && colorOUT.r < _FogColor.r) {
					//colorOUT = _FogColor;
				//}
				///////// END VOLUME FOG COMPENSATION

				return (float4(colorOUT, Fcolor.a*paintTexture1.a));



				




            }
            ENDCG
        }
//        Pass {
//           // Name "ForwardAdd"
//           // Tags {
//           //     "LightMode"="ForwardAdd"
//           // }
//            Blend One One
//            //ZWrite Off
//            
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//          //  #define UNITY_PASS_FORWARDADD
//            #include "UnityCG.cginc"
//            #include "AutoLight.cginc"
//             #include "Lighting.cginc"
//            #pragma multi_compile_fwdadd
//            #pragma multi_compile_fog
//            #pragma target 3.0
//			#pragma multi_compile_fwdbase nolightmap //v4.1
//
//            uniform sampler2D _CloudMap; 
//            uniform float4 _CloudMap_ST;
//            uniform sampler2D _CloudMap1; 
//            uniform float4 _CloudMap1_ST;
//          //  uniform float4 _LightColor0;
//            uniform float4 _SunColor;
//            uniform float4 _ShadowColor;
//            uniform float _ColorDiff;
//            uniform float _Density;
//            uniform float _Coverage;
//            uniform float _Transparency;           
//            uniform float _HorizonFactor;
//            uniform float4 _LightingControl;         
//            uniform float2 _EdgeFactors;
//            uniform float2 _Velocity1;
//            uniform float2 _Velocity2;
//            uniform float _CutHeight;
//
//            uniform float _ColorDiffOffset;
//            uniform float Thickness;
//            uniform float _CoverageOffset;
//            uniform float _TranspOffset;
//               uniform float4 _FogColor ;
//            uniform float _FogFactor;
//
//
//            uniform sampler2D _PaintMap;
//            uniform float4 _PaintMap_ST;
//
//
//            struct VertexInput {
//                float4 vertex : POSITION;
//                float3 normal : NORMAL;    
//                float2 texcoord0 : TEXCOORD0;
//                float4 tangent: TANGENT;
//            };
//            struct VertexOutput {
//                float4 pos : SV_POSITION;
//                float2 uv0 : TEXCOORD0;
//                float4 worldPos : TEXCOORD1;
//                float3 ForwLight: TEXCOORD2;                         
//                LIGHTING_COORDS(3,4)                    
//                UNITY_FOG_COORDS(5)
//            };
//            VertexOutput vert (VertexInput v) {           
//             	VertexOutput o;    
//
//
//
//                o.uv0 = v.texcoord0;    
//                o.pos = UnityObjectToClipPos(v.vertex );                         
//                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
//
//                 //SCATTER
//               //  TANGENT_SPACE_ROTATION;
//                // o.ForwLight = mul(rotation,ObjSpaceLightDir(v.vertex)); //ObjSpaceLightDir(v.vertex);
//
//                o.ForwLight =ObjSpaceLightDir(v.vertex); //ObjSpaceLightDir(v.vertex);
//
//                TRANSFER_VERTEX_TO_FRAGMENT(o);	
//
//                UNITY_TRANSFER_FOG(o,o.pos);
//                return o;
//            }
//            float4 frag(VertexOutput i) : COLOR {
//                            
//                 float change_h = _CutHeight;//240;
//				float PosDiff = 0.0006*(i.worldPos.y-change_h);
//
//                float2 UVs = _Density*float2(i.worldPos.x,i.worldPos.z);
//                float4 TimingF = 0.0012;
//
//                float2 UVs1 = _Velocity1*TimingF*_Time.y + UVs;
//
//                float4 cloudTexture = tex2D(_CloudMap,UVs1+_CloudMap_ST);
//                float4 cloudTexture1 = tex2D(_CloudMap1,UVs1+_CloudMap1_ST);
//
//                //_PaintMap
//               // float4 paintTexture1 = tex2D(_PaintMap,UVs1+_CloudMap1_ST);
//                float4 paintTexture1 = tex2D(_PaintMap,UVs1*_PaintMap_ST.zw*_PaintMap_ST.xy);
//
//                float2 UVs2 = (_Velocity2*TimingF*_Time.y/1.4 + float2(_EdgeFactors.x,_EdgeFactors.y) + UVs);
//
//                float4 Texture1 = tex2D(_CloudMap,UVs2+_CloudMap_ST); 
//                float4 Texture2 = tex2D(_CloudMap1,UVs2+_CloudMap1_ST); 
//
//                float DER = i.worldPos.y*0.001;               
//                float3 normalA = (((DER*((_Coverage+_CoverageOffset)+((cloudTexture.rgb*2)-1)))-(1-(Texture1.rgb*2))));             	
//             	float3 normalN = normalize(normalA); 
//
//
//             	//SCATTER
////            	float3 Difference = i.worldPos.xyz - _WorldSpaceCameraPos;
////					//float4 mainColor = tex2D(_CloudMap, UVs1); 
////					float Norma = length(Difference);
////					float3 BETAS = betaR + betaM;					
////					float3 Fex = exp(-((BETAS) * Norma));
////					//float cosTheta = dot(normalize(Difference), -sunPosition);
////					//float cosTheta = dot(normalize(Difference), float3(sunPosition.x/1,sunPosition.y*2,-sunPosition.z)); //v3.3
////					float cosTheta = dot(normalize(Difference), (float3(sunPosition.x,sunPosition.y,sunPosition.z)) ); //v3.3					
////					float rPhase = rayleighPhase(cosTheta*0.5+0.5);
////					float3 betaRTheta = betaR * rPhase;
////					float mPhase = hgPhase(cosTheta, mieDirectionalG);
////					float3 betaMTheta = betaM * mPhase;					
////					float3 FEXX = 400 * Fex;					
////					float3 Lin = ((betaRTheta + betaMTheta) / (BETAS)) * (400 - FEXX);
////					float3 Combined = FEXX +  Lin; 
////					float3 Scatter_power = Combined * 0.005;
////					float diffuseFactor =  max(dot(normalN.xyz,  float3(sunPosition.x,sunPosition.y,sunPosition.z) ), 0) ;  //v3.3
////					float3 lightDirection = _WorldSpaceLightPos0.xyz;         				
////					float3 lightColor = _LightColor0.rgb;
////				//	float3 LightAll = float3(_Color * (Texture2.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 				
////					float3 LightAll = float3(_SunColor * (Texture2.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 
////					* saturate(1-dot(normalN, lightDirection)));
////					float3 FinalColor = (LightAll * Fex + Lin)*0.2; //(LightAll * Fex + Lin)*0.2;	
//
//
////            	float4 return1 = tex2D (_MainTex,IN.uv.xy )* tex2D(_BorderTex, IN.uv.xy+float2(_Time.x*_Speed.x,_Time.x*_Speed.y)) * IN.color * _Intensity;			
////				float4 Combined1 = (dot(IN.ViewDir,IN.PointLights));		
////				//Volume
////				fixed4 col = tex2D(_MainTex, IN.uv) * IN.color.a;              
////               	fixed lerp_vec = max (0.3, min(dot (IN.normal, IN.ForwLight),0.5)	);       
//
//               	//fixed atten = LIGHT_ATTENUATION(i);    
//				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos.xyz); //v4.1
//
//               //	float3 finalCol = (_Glow)* IN.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * atten * lerp_vec * 1* _LightIntensity * float4(Combined1.xyz,1)* _SunLightIntensity + _MinLight;			
//				//float4 FINAL = _Control.x*return1 + _Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) + _Control.z*float4(min(finalCol.rgb,1),col.a);
//
//
//                
//
////				float DER1 = -(i.worldPos.y-0)*PosDiff;
////             	float PosTDiff = i.worldPos.y*PosDiff;
//             	float DER1 = -(i.worldPos.y-0)*1;
//             	float PosTDiff = i.worldPos.y*1;
//             	if(i.worldPos.y > change_h){             		
//             		DER1 = (1-cloudTexture1.a) *  PosTDiff;
//             		//DER1 =  PosTDiff;
//
//             	}
////             	float diff3 = change_h+511 - i.worldPos.y;
////             	if(diff3 > 0){ 
////             		_Coverage = _Coverage - 0.001111*diff3;
////             	}
//
//      //       	float shaper = (_Transparency-0.48+0.0)*((DER1*saturate((_Coverage+cloudTexture1.a)))-Texture2.a);		////////////////////////////////////////
//             	//float shaper = ((_Transparency+_TranspOffset)-0.48+0.0)*((DER1*saturate(((_Coverage+_CoverageOffset)+cloudTexture1.a)))*Texture2.a);
//             	float shaper = ((_Transparency+_TranspOffset)-0.48+0.0)*((DER1*(((_Coverage+_CoverageOffset)+cloudTexture1.a)))*Texture2.a);
//
//                float3 lightDirect = normalize(_WorldSpaceLightPos0.xyz);
//               	lightDirect.y = -lightDirect.y;
//               
//                float ColDiff =  ( (_ColorDiff+_ColorDiffOffset) -0.082)+((1+(DER*_LightingControl.r*_LightingControl.g))*0.5); 
//
//                float verticalFactor = dot(lightDirect, float3(0,1,0));
//    //         	float Lerpfactor = (ColDiff+(_ShadowColor.a*(dot(lightDirect,normalN)-1)*ColDiff));
//     		    float Lerpfactor = (ColDiff+(_ShadowColor.a*(dot(i.ForwLight,normalN)-1)*ColDiff));
//
//                float ColB = _SunColor.rgb;
//               // if(_Mode==0){
//	                change_h =10;   
//	                PosDiff =  Thickness*0.0006*(i.worldPos.y-change_h);  
//	                PosTDiff = i.worldPos.y*PosDiff;          
//	             	DER1 = -(i.worldPos.y+0)*PosDiff;
//
//	             	if(i.worldPos.y > change_h){             		
//	             		//DER1 = (1-cloudTexture1.a) *  PosTDiff;
//	             		DER1 = (1-cloudTexture1.a) *  PosTDiff;
//	             	}
//
//	             	if(i.worldPos.y > 150){ 																																///////////////////////////// 650
//	             		DER1 = (1-cloudTexture1.a) *  PosTDiff*0.07;
//	             		shaper = shaper*shaper;
//	             	}
//
//	             	ColB =1*_SunColor.a*(1-cloudTexture1.a)*_SunColor.rgb*DER1*(1-verticalFactor);///////
//             	//}
//
//             	//SCATTER
//             	float diff = saturate(dot(normalN, normalize(i.ForwLight)))+0.7;//worldPos
//           //  	float diff = saturate(dot((-i.camPos), normalize(i.ForwLight)))+0.7;//worldPos
//
//             	//float diff2 = (dot(float3(0,1,0), (i.normal)));//worldPos
//             	float diff2 = distance(_WorldSpaceCameraPos,i.worldPos)*distance(_WorldSpaceCameraPos,i.worldPos);//worldPos
//
//
//
//               	//float3 endColor =  (0+0.5*atten*_LightColor0.rgb)*_ShadowColor.rgb*(1-(Texture1.g*1));
//               //	float3 endColor = lerp((Texture1.g)*2.5*(Texture1.g*atten+0.015)*_LightColor0.rgb*_ShadowColor.rgb*(1-(Texture1.g*1)),   11*Texture1.g*0.05*atten*_LightColor0.rgb*ColB,    Lerpfactor);
// //              	float3 endColor = _LightColor0.rgb*lerp(2.5*(atten+0.015)*_LightColor0.rgb*_ShadowColor.rgb*(1-(Texture1.g)),   11*Texture1.g*0.05*atten*_LightColor0.rgb*ColB,    Lerpfactor);
//
//
//				//IF DIRECTIONAL, exclude - Unity Multiple Lights
//				float noDirLight=1;
//				if(_WorldSpaceLightPos0.w == 0.0){
//				 	  noDirLight=0;
//				}
//
//               	//float3 endColor =diff*_LightColor0.rgb*lerp(2.5*(atten+0.015)*_LightColor0.rgb*_ShadowColor.rgb*(1-(Texture1.g)),   1*Texture1.g*0.05*atten*_LightColor0.rgb*ColB,    saturate(Lerpfactor)) *atten;
//               	float3 endColor =diff*_LightColor0.rgb*lerp(_ShadowColor.rgb,  ColB,    (1-Lerpfactor)) *atten;
//
//               // float3 endColor =  (diff*atten*_LightColor0.rgb);
//
// //               float4 Fcolor = float4(saturate((shaper - 0.01*(_HorizonFactor*0.00001*diff2)   )*endColor)/3,shaper - 0.01*(_HorizonFactor*0.00001*diff2)    )*(atten*0.1+0.0);  
//                float4 Fcolor = float4((endColor*0.2 + 0.1*endColor*_FogFactor*1*diff2*_FogColor*0.000000001*1)*shaper,shaper - 0.01*(_HorizonFactor*0.00001*diff2)    );              
//
//                //return (Fcolor*0.7);
//
//                 return saturate(float4(Fcolor.r,Fcolor.g,Fcolor.b, Fcolor.a*paintTexture1.a)*0.7*noDirLight);
//
//            }
//            ENDCG 
//        }

        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
                       
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_fog 
            #pragma multi_compile_shadowcaster                    
            #pragma target 3.0
			#pragma multi_compile_fwdbase nolightmap //v4.1

            uniform sampler2D _CloudMap; 
            uniform float4 _CloudMap_ST;
            uniform sampler2D _CloudMap1; 
            uniform float4 _CloudMap1_ST;
            //uniform float4 _LightColor0;          
            uniform float _Density;
            uniform float _Coverage;
            uniform float _Transparency;         
            uniform float2 _EdgeFactors;
            uniform float2 _Velocity1;
            uniform float2 _Velocity2;
            uniform float _Cutoff;
            uniform float _CutHeight;

            uniform float Thickness;
            uniform float _CoverageOffset;
            uniform float _TranspOffset;

             uniform sampler2D _PaintMap;
            uniform float4 _PaintMap_ST;

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;    
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;               
                float2 uv0 : TEXCOORD1;
                float4 worldPos : TEXCOORD2;               
            };
            VertexOutput vert (VertexInput v) {           
             	VertexOutput o;    
                o.uv0 = v.texcoord0;    
                o.pos = UnityObjectToClipPos(v.vertex );                         
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                            
//                 float2 UVs = _Density*float2(i.worldPos.x,i.worldPos.z);
//                float4 TimingF = 0.0012;
//                float2 UVs1 = _Velocity1*TimingF*_Time.y + UVs;
//                float4 cloudTexture = tex2D(_CloudMap,UVs1+_CloudMap_ST);
//                float4 cloudTexture1 = tex2D(_CloudMap1,UVs1+_CloudMap1_ST);
//                float2 UVs2 = (_Velocity2*TimingF*_Time.y + float2(_EdgeFactors.x,_EdgeFactors.y) + UVs);
//                float4 Texture1 = tex2D(_CloudMap,UVs2+_CloudMap_ST); 
//                float4 Texture2 = tex2D(_CloudMap1,UVs2+_CloudMap1_ST); 
//
//                float DER = i.worldPos.y*0.001;               
//                float3 normalA = (((DER*(_Coverage+((cloudTexture.rgb*2)-1)))-(1-(Texture1.rgb*2))));             	
//             	float3 normalN = normalize(normalA); 
//
//             	float4 paintTexture1 = tex2D(_PaintMap,UVs1*_PaintMap_ST.zw*_PaintMap_ST.xy);
//
//				float change_h =_CutHeight;
//				float PosDiff = Thickness*0.0006*(i.worldPos.y-change_h);
//             	float DER1 = -(i.worldPos.y+0)*PosDiff;
//             	float PosTDiff = i.worldPos.y*PosDiff;
//             	if(i.worldPos.y > change_h){             		
//             		DER1 = (1-cloudTexture1.a) *  PosTDiff;
//             		//DER1 =  PosTDiff;
//             	}
//
//             	float shaper = ((_Transparency+_TranspOffset)-0.48)*((DER1*(((_Coverage+0)+cloudTexture1.a)))-Texture2.a);
//              
//                clip(shaper*paintTexture1 - _Cutoff+0.4);
//                SHADOW_CASTER_FRAGMENT(i)






//             float change_h = _CutHeight;//240;
//				float PosDiff = Thickness*0.0006*(i.worldPos.y-change_h)-0.4;
//
//                float2 UVs = _Density*float2(i.worldPos.x,i.worldPos.z);
//                float4 TimingF = 0.0012;//0.0012
//
//                float2 UVs1 = _Velocity1*TimingF*_Time.y + UVs;
//
//                float4 cloudTexture = tex2D(_CloudMap,UVs1+_CloudMap_ST);
//                float4 cloudTexture1 = tex2D(_CloudMap1,UVs1+_CloudMap1_ST);
//
//                //_PaintMap
//                //float4 paintTexture1 = tex2D(_PaintMap,float4(_PaintMap_ST.xy,UVs1*_PaintMap_ST.zw));
//                float4 paintTexture1 = tex2D(_PaintMap,UVs1*_PaintMap_ST.zw*_PaintMap_ST.xy);
//
//                float2 UVs2 = (_Velocity2*TimingF*_Time.y + float2(_EdgeFactors.x,_EdgeFactors.y) + UVs);
//
//                float4 Texture1 = tex2D(_CloudMap,UVs2*_Velocity1.w+_CloudMap_ST); 
//                float4 Texture2 = tex2D(_CloudMap1,UVs2*_Velocity2.w+_CloudMap1_ST); 
//
//                float DER = i.worldPos.y*0.001;               
//                float3 normalA = (((DER*( (_Coverage +_CoverageOffset) +((cloudTexture.rgb*2)-1)))-(1-(Texture1.rgb*2)))) * 1;             /////// -0.25 coverage	(-0.35,5)
//             	float3 normalN = normalize(normalA); 
//
//             	//SCATTER              
//               	//fixed atten = LIGHT_ATTENUATION(i);               
//              		
//        //     	float DER1 = -(i.worldPos.y)*PosDiff-95;  //-95
//        		float DER1 = (i.worldPos.y)*PosDiff-95;  //-95
//             	float PosTDiff = i.worldPos.y;
//             	if(i.worldPos.y > change_h){             		
//             		DER1 = (1-cloudTexture1.a);
//             	}
//
//             	float shaper = (_Transparency+4.5) *( (DER1*saturate(( (_Coverage +_CoverageOffset)   -(0.8*PosDiff)+cloudTexture1.a* (Texture2.a) ))))   ; 


                            
                float change_h = _CutHeight;//240;
				float PosDiff = 0.0006*(i.worldPos.y-change_h);

                float2 UVs = _Density*float2(i.worldPos.x,i.worldPos.z);
                float4 TimingF = 0.0012;

                float2 UVs1 = _Velocity1*TimingF*_Time.y + UVs;

                float4 cloudTexture = tex2D(_CloudMap,UVs1+_CloudMap_ST);
                float4 cloudTexture1 = tex2D(_CloudMap1,UVs1+_CloudMap1_ST);

                //_PaintMap
               // float4 paintTexture1 = tex2D(_PaintMap,UVs1+_CloudMap1_ST);
                float4 paintTexture1 = tex2D(_PaintMap,UVs1*_PaintMap_ST.xy + _PaintMap_ST.zw);

                float2 UVs2 = (_Velocity2*TimingF*_Time.y/1.4 + float2(_EdgeFactors.x,_EdgeFactors.y) + UVs);

                float4 Texture1 = tex2D(_CloudMap,UVs2+_CloudMap_ST); 
                float4 Texture2 = tex2D(_CloudMap1,UVs2+_CloudMap1_ST); 

                float DER = i.worldPos.y*0.001;               
                float3 normalA = (((DER*((_Coverage+_CoverageOffset)+((cloudTexture.rgb*2)-1)))-(1-(Texture1.rgb*2))));             	
             	float3 normalN = normalize(normalA); 


             	//SCATTER
//            	float3 Difference = i.worldPos.xyz - _WorldSpaceCameraPos;
//					//float4 mainColor = tex2D(_CloudMap, UVs1); 
//					float Norma = length(Difference);
//					float3 BETAS = betaR + betaM;					
//					float3 Fex = exp(-((BETAS) * Norma));
//					//float cosTheta = dot(normalize(Difference), -sunPosition);
//					//float cosTheta = dot(normalize(Difference), float3(sunPosition.x/1,sunPosition.y*2,-sunPosition.z)); //v3.3
//					float cosTheta = dot(normalize(Difference), (float3(sunPosition.x,sunPosition.y,sunPosition.z)) ); //v3.3					
//					float rPhase = rayleighPhase(cosTheta*0.5+0.5);
//					float3 betaRTheta = betaR * rPhase;
//					float mPhase = hgPhase(cosTheta, mieDirectionalG);
//					float3 betaMTheta = betaM * mPhase;					
//					float3 FEXX = 400 * Fex;					
//					float3 Lin = ((betaRTheta + betaMTheta) / (BETAS)) * (400 - FEXX);
//					float3 Combined = FEXX +  Lin; 
//					float3 Scatter_power = Combined * 0.005;
//					float diffuseFactor =  max(dot(normalN.xyz,  float3(sunPosition.x,sunPosition.y,sunPosition.z) ), 0) ;  //v3.3
//					float3 lightDirection = _WorldSpaceLightPos0.xyz;         				
//					float3 lightColor = _LightColor0.rgb;
//				//	float3 LightAll = float3(_Color * (Texture2.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 				
//					float3 LightAll = float3(_SunColor * (Texture2.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 
//					* saturate(1-dot(normalN, lightDirection)));
//					float3 FinalColor = (LightAll * Fex + Lin)*0.2; //(LightAll * Fex + Lin)*0.2;	


//            	float4 return1 = tex2D (_MainTex,IN.uv.xy )* tex2D(_BorderTex, IN.uv.xy+float2(_Time.x*_Speed.x,_Time.x*_Speed.y)) * IN.color * _Intensity;			
//				float4 Combined1 = (dot(IN.ViewDir,IN.PointLights));		
//				//Volume
//				fixed4 col = tex2D(_MainTex, IN.uv) * IN.color.a;              
//               	fixed lerp_vec = max (0.3, min(dot (IN.normal, IN.ForwLight),0.5)	);              
           //    	fixed atten = LIGHT_ATTENUATION(i);               
               //	float3 finalCol = (_Glow)* IN.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * atten * lerp_vec * 1* _LightIntensity * float4(Combined1.xyz,1)* _SunLightIntensity + _MinLight;			
				//float4 FINAL = _Control.x*return1 + _Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) + _Control.z*float4(min(finalCol.rgb,1),col.a);


                

//				float DER1 = -(i.worldPos.y-0)*PosDiff;
//             	float PosTDiff = i.worldPos.y*PosDiff;
             	float DER1 = -(i.worldPos.y-0)*1;
             	float PosTDiff = i.worldPos.y*1;
             	if(i.worldPos.y > change_h){             		
             		DER1 = (1-cloudTexture1.a) *  PosTDiff;
             		//DER1 =  PosTDiff;

             	}
//             	float diff3 = change_h+511 - i.worldPos.y;
//             	if(diff3 > 0){ 
//             		_Coverage = _Coverage - 0.001111*diff3;
//             	}

      //       	float shaper = (_Transparency-0.48+0.0)*((DER1*saturate((_Coverage+cloudTexture1.a)))-Texture2.a);		////////////////////////////////////////
             	//float shaper = ((_Transparency+_TranspOffset)-0.48+0.0)*((DER1*saturate(((_Coverage+_CoverageOffset)+cloudTexture1.a)))*Texture2.a);
             	float shaper = ((_Transparency+_TranspOffset)-0.48+0.0)*((DER1*(((_Coverage+_CoverageOffset)+cloudTexture1.a)))*Texture2.a);

             //   float3 lightDirect = normalize(_WorldSpaceLightPos0.xyz);
            //   	lightDirect.y = -lightDirect.y;
               
             //   float ColDiff =  ( (_ColorDiff+_ColorDiffOffset) -0.082)+((1+(DER*_LightingControl.r*_LightingControl.g))*0.5); 

              //  float verticalFactor = dot(lightDirect, float3(0,1,0));
    //         	float Lerpfactor = (ColDiff+(_ShadowColor.a*(dot(lightDirect,normalN)-1)*ColDiff));
     		   // float Lerpfactor = (ColDiff+(_ShadowColor.a*(dot(i.ForwLight,normalN)-1)*ColDiff));

               // float ColB = _SunColor.rgb;
               // if(_Mode==0){
	                change_h =10;   
	                PosDiff =  Thickness*0.0006*(i.worldPos.y-change_h);  
	                PosTDiff = i.worldPos.y*PosDiff;          
	             	DER1 = -(i.worldPos.y+0)*PosDiff;

	             	if(i.worldPos.y > change_h){             		
	             		//DER1 = (1-cloudTexture1.a) *  PosTDiff;
	             		DER1 = (1-cloudTexture1.a) *  PosTDiff;
	             	}

	             	if(i.worldPos.y > 150){ 																																///////////////////////////// 650
	             		DER1 = (1-cloudTexture1.a) *  PosTDiff*0.07;
	             		shaper = shaper*shaper;
	             	}
              
                clip(shaper*paintTexture1 - _Cutoff+0.4);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}