// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld' //v3.4.6

Shader "SkyMaster/ShaderVolumeClouds-Desktop-SM3.0 v3.4.4 VORTEX ScriptLess" {
    Properties {
        _SunColor ("_SunColor", Color) = (0.95,0.95,0.95,0.8)
		_SunColor2("_SunColor2", Color) = (0.95,0.95,0.95,0.8)
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
        _PaintMap ("_CloudMap", 2D) = "white" {}

		//v4.2
		[Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Int) = 2

		//v0.7
		vortexControl("vortex Controls (rotation power, speed, thickness, cutoff distance", Vector) = (30,1,1,1)
		vortexPosRadius("vortex Center Position", Vector) = (0,0,0,1)

		//v0.8
		fog_depth("Fog Depth", float) = 0.39
		reileigh("Reileigh", float) = 1.44
		mieCoefficient("Mie Factor", float) = 0.01
		mieDirectionalG("Mie Directional G", float) = 0.71
		ExposureBias("Exposure Bias", float) = 0.01
		////const float n = 1.0003f;
		//const float N = 2.545E25f;
		//const float pn = 0.035f;
		lambda("lamda", float) = (1,1,1)
		KA("K", float) = (0.9, 0.5, 0.5)
		sunPosition("Sun Position - Power", float) = (0, 0, 0, 1)
		localLight("Local Light color-exponent", float) = (1, 1, 1, 1)
		localLightPos("Local Light Position-power", float) = (0, 0, 0, 1)
		localLightFreq("Local Light Frequencies", float) = (1, 1,1, 1)
		localLightPosRange("Local Light Position Range X-Z, Scale", float) = (-0.8, -0.8, 1, 1)

			fogPower("fog Power HDRP", float) = 1
			 fogPowerExp("fogPowerExp HDRP", float) = 1 
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
            //    "LightMode"="ForwardBase"
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
            //#define UNITY_PASS_FORWARDBASE
            #pragma multi_compile_fog 
            #pragma multi_compile_fwdbase                      
            #pragma target 3.0
			#pragma multi_compile_fwdbase nolightmap //v4.1

			uniform sampler2D _CloudMap; 
            uniform float4 _CloudMap_ST;
            uniform sampler2D _CloudMap1; 
            uniform float4 _CloudMap1_ST;
            //float4 _LightColor0;
            uniform float4 _SunColor;
			uniform float4 _SunColor2;
            uniform float4 _ShadowColor;
            uniform float _ColorDiff;
            uniform float _Density;
            uniform float _Coverage;
            uniform float _Transparency;         
            uniform float _HorizonFactor;
            uniform float4 _LightingControl;
            uniform float2 _EdgeFactors;
            uniform float2 _Velocity1;
            uniform float2 _Velocity2;
            // uniform int _Mode;
            uniform float _CutHeight;
            uniform float4 _FogColor ;
            uniform float _FogFactor;
            uniform float _FogUnity;

            //uniform sampler2D _PaintMap;
             uniform float  _ColorDiffOffset;
             uniform float Thickness;
            uniform float _CoverageOffset;
            uniform float _TranspOffset;

            uniform sampler2D _PaintMap;
            uniform float4 _PaintMap_ST;

            //SCATTER			
			float3 _Color;
			float3 _Control;
			static const float pi = 3.141592653589793238462643383279502884197169;
				static const float rayleighZenithLength = 8.4E3;
				static const float mieZenithLength = 1.25E3;
				static const float3 up = float3(0.0, 1.0, 0.0);				
			
				static const float sunAngularDiameterCos = 0.999956676946448443553574619906976478926848692873900859324;	
			

				float4 sunPosition;
				//float3 betaR;
				//float3 betaM;

				float fog_depth;
				float mieCoefficient; //= 0.05;
				float mieDirectionalG;// = 0.8;
				float ExposureBias;// = 1.0;

				//v0.8
				//public float fog_depth = 0.29f;// 1.5f;
				float reileigh;// = 1.3f;//2.0f;
				//public float mieCoefficient = 1;//0.1f;
				//public float mieDirectionalG = 0.1f;
				//public float ExposureBias = 0.11f;//
				float nA = 1.0003f;
				float NA = 2.545E25f;
				float pnA = 0.035f;
				float3 lambda;// = new Vector3(680E-9f, 550E-9f, 450E-9f);//new Vector3(680E-9f, 550E-9f, 450E-9f);
				float3 KA;// = new Vector3(0.9f, 0.5f, 0.5f);



				float rayleighPhase(float cosTheta)
				{
					return (3.0 / (16.0*pi)) * (1.0 + pow(cosTheta, 2.0));
				}		
								
				float hgPhase(float cosTheta, float g)
				{   float POW = pow(g, 2.0);
					return (0.25 / pi) * ((1.0 - POW) / pow(1.0 - 2.0*g*cosTheta + POW, 1.5)); 
				}

				

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;    
                float2 texcoord0 : TEXCOORD0;
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
            };
            VertexOutput vert (VertexInput v) {           
             	VertexOutput o;    
                o.uv0 = v.texcoord0;    
                o.pos = UnityObjectToClipPos(v.vertex );                         
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

			//v0.7
			float4 vortexControl;
			float4 vortexPosRadius;// = float4(0, 0, 0, 1000);
			//v0.7a - VORTEX
			float3x3 rotationMatrix(float3 axis, float angle)
			{
				axis = normalize(axis);
				float s = sin(angle);
				float c = cos(angle);
				float oc = 1.0 - c;

				return float3x3 (oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s,
					oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s,
					oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c);
			}


			//v0.8
			//UPDATE SCATTER
			float3 totalRayleigh(float3 lambdaAA)
			{
				float3 result = float3((8.0f * pow(pi, 3.0f) * pow(pow(nA, 2.0f) - 1.0f, 2.0f) * (6.0f + 3.0f * pnA)) / (3.0f * NA * pow(lambdaAA.x, 4.0f) * (6.0f - 7.0f * pnA)),
					(8.0f * pow(pi, 3.0f) * pow(pow(nA, 2.0f) - 1.0f, 2.0f) * (6.0f + 3.0f * pnA)) / (3.0f * NA * pow(lambdaAA.y, 4.0f) * (6.0f - 7.0f * pnA)),
					(8.0f * pow(pi, 3.0f) * pow(pow(nA, 2.0f) - 1.0f, 2.0f) * (6.0f + 3.0f * pnA)) / (3.0f * NA * pow(lambdaAA.z, 4.0f) * (6.0f - 7.0f * pnA)));
				return result;
			}

			float3 totalMie(float3 lambdaAA, float3 K, float T)
			{
				float cA = (0.2f * T) * 10E-17f;
				float3 result = float3(
					0.434f * cA * pi * pow((2.0f * pi) / lambdaAA.x, 2.0f) * K.x,
					0.434f * cA * pi * pow((2.0f * pi) / lambdaAA.y, 2.0f) * K.y,
					0.434f * cA * pi * pow((2.0f * pi) / lambdaAA.z, 2.0f) * K.z
				);
				return result;
			}

			//v0.8 - LIGHTNING
			float lightningTimerA = 0;
			float rand(float2 co) {
				return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
			}
			float3 lightPos;
			float4 localLight;
			float4 localLightPos;
			float4 localLightFreq;
			float4 localLightPosRange;

			float fogPower = 1;
			float fogPowerExp = 1;

            float4 frag(VertexOutput i) : COLOR {

				//v0.8 - local lights shader scriptless mode
				float3 localLightPower = 0;
				float3 boundngBoxEdgeA = float3(0,200,0);
				float3 boundngBoxEdgeB = float3(600, 600, 600);
				/*if (lightningTimerA == 0) {
					lightPos = float3(rand(float2(boundngBoxEdgeA.x, boundngBoxEdgeB.x)), rand(float2(boundngBoxEdgeA.y, boundngBoxEdgeB.y)), rand(float2(boundngBoxEdgeA.z, boundngBoxEdgeB.z)));
					lightningTimerA = 5;
				}
				else if (lightningTimerA > 0) {
					lightningTimerA = lightningTimerA - 0.01;
					localLightPower = length(lightPos - i.worldPos.xz);
				}*/
				lightPos = float3(localLightPos.x, localLightPos.y, localLightPos.z);//10000 * localLightPos.xyz;// float3(0, 0, 0);//
					//rand(float2(boundngBoxEdgeA.x - i.worldPos.x * 0.0000000100, boundngBoxEdgeB.x + i.worldPos.x * 0.0000000100)),
					//rand(float2(boundngBoxEdgeA.y, boundngBoxEdgeB.y)), 
					//rand(float2(boundngBoxEdgeA.z - i.worldPos.z * 0.0000000100, boundngBoxEdgeB.z + i.worldPos.z * 0.0000000100))
					//);

				float randomizePos = fmod((int)_Time.y, 10);
				randomizePos = rand(float2(randomizePos, randomizePos*2));
				lightPos = lightPos + localLightPosRange.z*21101*float3(randomizePos+localLightPosRange.x,0, randomizePos + localLightPosRange.y);
				
				localLightPower = (localLightPos.w*100000000* abs(cos(_Time.y*2 * localLightFreq.y) + 0.8*sin(_Time.y * (5 + 0.02*cos(_Time.y * localLightFreq.z)) )  )) /(pow(length(lightPos - i.worldPos.xyz),2.5 * localLight.w)) * localLight.rgb;
				
				//localLightFreq
				localLightPower = localLightPower * clamp((cos(_Time.y*localLightFreq.x)),0, 1);

				//v0.7a - VORTEX
				//float4 vortexPosRadius = float4(0, 0, 0, 1000);
				float distanceToVortexCenter = length(vortexPosRadius.xz - i.worldPos.xz);
				float3x3 rotator = rotationMatrix(float3(0, 1, 0), 1 * ((_Time.y * 0.5 * vortexControl.y) - vortexControl.x * 700 * (distanceToVortexCenter / 10000000)));
				//float3 posVertex = mul(float3(i.worldPos.x, 0, i.worldPos.z), rotator);
				float3 posVertexA = float3(i.worldPos.x, 0, i.worldPos.z);// vortexPosRadius;
				posVertexA = posVertexA - vortexPosRadius;
				float3 posVertex = mul(posVertexA, rotator);
				//posVertexA += vortexPosRadius;
				posVertex.y = i.worldPos.y;
				if (distanceToVortexCenter > 150000 * vortexControl.w * 0.01) {
					posVertex = i.worldPos;
				}
				else {
					Thickness = Thickness / vortexControl.z;
				}
				//float cloudSample = tex3Dlod(_ShapeTexture, float4(posVertex * _Scale, lod)).r;

            	float change_h = _CutHeight;//240;



				//v0.7a - VORTEX
				float PosDiff = Thickness * 0.0006*(posVertex.y - change_h) - 0.4;
				float2 UVs = _Density * float2(posVertex.x, posVertex.z);
				float4 TimingF = 0.0012;
				float2 UVs1 = _Velocity1 * TimingF*_Time.y + UVs;
				/*float PosDiff = 0.0006*(posVertex.y-change_h);
				float2 UVs = _Density*float2(posVertex.x, posVertex.z);
				float4 TimingF = 0.0012;
				float2 UVs1 = _Velocity1*TimingF*_Time.y + UVs;*/
				
				//float PosDiff = 0.0006*(i.worldPos.y-change_h);
                //float2 UVs = _Density*float2(i.worldPos.x,i.worldPos.z);
                //float4 TimingF = 0.0012;
                //float2 UVs1 = _Velocity1*TimingF*_Time.y + UVs;

                float4 cloudTexture = tex2D(_CloudMap,UVs1+_CloudMap_ST);
                float4 cloudTexture1 = tex2D(_CloudMap1,UVs1+_CloudMap1_ST);

                //_PaintMap
               // float4 paintTexture1 = tex2D(_PaintMap,UVs1+_CloudMap1_ST);
                float4 paintTexture1 = tex2D(_PaintMap,UVs1*_PaintMap_ST.xy + _PaintMap_ST.zw);//v5.0.9

                //float2 UVs2 = (_Velocity2*TimingF*_Time.y/1.4 + float2(_EdgeFactors.x,_EdgeFactors.y) + UVs);
				float2 UVs2 = (_Velocity2*TimingF*_Time.y  + float2(_EdgeFactors.x, _EdgeFactors.y) + UVs); ////

                float4 Texture1 = tex2D(_CloudMap,UVs2+_CloudMap_ST); 
                float4 Texture2 = tex2D(_CloudMap1,UVs2+_CloudMap1_ST); 

                float DER = posVertex.y*0.001;
                float3 normalA = (((DER*((_Coverage+_CoverageOffset)+((cloudTexture.rgb*2)-1)))-(1-(Texture1.rgb*2))));             	
             	float3 normalN = normalize(normalA); 


				//v0.8 - Script less mode
				///oceanMat.SetVector("betaR", totalRayleigh(lambda) * reileigh); 
				//oceanMat.SetVector("betaM", totalMie(lambda, K, fog_depth) * mieCoefficient);
				float3 lambdaA = float3(lambda.x * 0.000000068f, lambda.y * 0.000000055f, lambda.z * 0.000000045f);// float3(680E-9f, 550E-9f, 450E-9f);
				float3 betaR  =  clamp((totalRayleigh(lambdaA) * reileigh), -1000, 1000);
				float3 betaM  = totalMie(lambdaA, KA, fog_depth) * mieCoefficient;


             	//SCATTER
            	float3 Difference = posVertex.xyz - _WorldSpaceCameraPos;
					//float4 mainColor = tex2D(_CloudMap, UVs1); 
					float Norma = length(Difference);
					float3 BETAS = betaR + betaM;					
					float3 Fex = exp(-((BETAS) * Norma));
					//float cosTheta = dot(normalize(Difference), -sunPosition);
					//float cosTheta = dot(normalize(Difference), float3(sunPosition.x/1,sunPosition.y*2,-sunPosition.z)); //v3.3
					float cosTheta = dot(normalize(Difference),  (float3(sunPosition.x, sunPosition.y, sunPosition.z)) ); //v3.3					
					float rPhase = rayleighPhase(cosTheta*0.5+0.5);
					float3 betaRTheta = betaR * rPhase;
					float mPhase = hgPhase(cosTheta, mieDirectionalG);
					float3 betaMTheta = betaM * mPhase;					
					float3 FEXX = 400 * Fex;					
					float3 Lin = ((betaRTheta + betaMTheta) / (BETAS)) * (400 - FEXX);
					float3 Combined = FEXX +  Lin; 
					float3 Scatter_power = Combined * 0.005;
					float diffuseFactor = max(dot(normalN.xyz, float3(sunPosition.x, sunPosition.y, sunPosition.z) ), 0);  //v3.3
					float3 lightDirection = _WorldSpaceLightPos0.xyz;// float3(sunPosition.x, sunPosition.y, sunPosition.z);
					float3 lightColor = _SunColor2;// _LightColor0.rgb;
				//	float3 LightAll = float3(_Color * (Texture2.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 				
					float3 LightAll = float3(_SunColor * (Texture2.xyz * diffuseFactor * Combined  * lightColor  +  Scatter_power) 
					* saturate(1-dot(normalN, lightDirection)));
					float3 FinalColor =  (LightAll * Fex + Lin)*1.2; //(LightAll * Fex + Lin)*0.2;	


//            	float4 return1 = tex2D (_MainTex,IN.uv.xy )* tex2D(_BorderTex, IN.uv.xy+float2(_Time.x*_Speed.x,_Time.x*_Speed.y)) * IN.color * _Intensity;			
//				float4 Combined1 = (dot(IN.ViewDir,IN.PointLights));		
//				//Volume
//				fixed4 col = tex2D(_MainTex, IN.uv) * IN.color.a;              
//               	fixed lerp_vec = max (0.3, min(dot (IN.normal, IN.ForwLight),0.5)	);        

               	//fixed atten = LIGHT_ATTENUATION(i);       
				UNITY_LIGHT_ATTENUATION(atten, i, posVertex.xyz); //v4.1

               //	float3 finalCol = (_Glow)* IN.color.rgb + col * float3(_SunColor.r,_SunColor.g,_SunColor.b) * atten * lerp_vec * 1* _LightIntensity * float4(Combined1.xyz,1)* _SunLightIntensity + _MinLight;			
				//float4 FINAL = _Control.x*return1 + _Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) + _Control.z*float4(min(finalCol.rgb,1),col.a);


                

//				float DER1 = -(i.worldPos.y-0)*PosDiff;
//             	float PosTDiff = i.worldPos.y*PosDiff;
																   //     	float DER1 = -(i.worldPos.y)*PosDiff-95;  //-95
				float DER1 = (posVertex.y)*PosDiff - 95;  //-95
				float PosTDiff = posVertex.y;
				if (posVertex.y > change_h) {
					DER1 = (1 - cloudTexture1.a);
				}
//             	float diff3 = change_h+511 - i.worldPos.y;
//             	if(diff3 > 0){ 
//             		_Coverage = _Coverage - 0.001111*diff3;
//             	}

      //       	float shaper = (_Transparency-0.48+0.0)*((DER1*saturate((_Coverage+cloudTexture1.a)))-Texture2.a);		////////////////////////////////////////
             	//float shaper = ((_Transparency+_TranspOffset)-0.48+0.0)*((DER1*saturate(((_Coverage+_CoverageOffset)+cloudTexture1.a)))*Texture2.a);
             	//float shaper = ((_Transparency+_TranspOffset)-0.48+0.0)*((DER1*(((_Coverage+_CoverageOffset)+cloudTexture1.a)))*Texture2.a);
				float shaper = (_Transparency + 4.5 + _TranspOffset) *((DER1*saturate(((_Coverage + _CoverageOffset) - (0.8*PosDiff) + cloudTexture1.a* (Texture2.a))))); /////////////////// DIFERMCE			//////////////// * 30   _Transparency /////// -0.3 coverage	


				float3 lightDirect = sunPosition;// normalize(sunPosition.xyz);
               	lightDirect.y = -lightDirect.y;
               
               //// float ColDiff =  ( (_ColorDiff+_ColorDiffOffset) -0.082)+((1+(DER*_LightingControl.r*_LightingControl.g))*0.5); ////
				float ColDiff = (_ColorDiff + _ColorDiffOffset) + ((1 + (DER*_LightingControl.r*_LightingControl.g))*0.5);

                float verticalFactor = dot(lightDirect, float3(0,1,0));
    //         	float Lerpfactor = (ColDiff+(_ShadowColor.a*(dot(lightDirect,normalN)-1)*ColDiff));
     		    float Lerpfactor = (ColDiff+(_ShadowColor.a*(dot(i.ForwLight,normalN)-1)*ColDiff));

				float ColB = _SunColor.rgb;
               // if(_Mode==0){
	                change_h =11;   
	                ///PosDiff =  Thickness*0.0006*(posVertex.y-change_h);////
					PosDiff = 0.0004*(posVertex.y - change_h);
	                PosTDiff = posVertex.y*PosDiff;
	             	DER1 = -(posVertex.y+0)*PosDiff;

	             	if(posVertex.y > change_h){
	             		//DER1 = (1-cloudTexture1.a) *  PosTDiff;
	             		DER1 = (1-cloudTexture1.a) *  PosTDiff;
	             	}

	             	//if(posVertex.y > 150){ 																																///////////////////////////// 650
	             	//	DER1 = (1-cloudTexture1.a) *  PosTDiff*0.07; /////////////////////
	             	//	shaper = shaper*shaper;
	             	//}

	             	ColB =1*_SunColor.a*(1-cloudTexture1.a)*_SunColor.rgb*DER1*(1-verticalFactor);///////
             	//}

             	//SCATTER
             	float diff = saturate(dot(normalN, normalize(i.ForwLight)))+0.7;//worldPos
           //  	float diff = saturate(dot((-i.camPos), normalize(i.ForwLight)))+0.7;//worldPos

             	//float diff2 = (dot(float3(0,1,0), (i.normal)));//worldPos
             	float diff2 = distance(_WorldSpaceCameraPos, posVertex)*distance(_WorldSpaceCameraPos, posVertex);//worldPos

               // float3 endColor =   lerp(_ShadowColor.rgb*(diff*_LightColor0.rgb*atten),(FinalColor*_LightColor0.rgb*atten*diff)*ColB, Lerpfactor  );  //+ 11*float4(1.0-exp2(-FinalColor*0.04),0) ;

//               float3 finalCol = diff*_LightColor0.rgb* atten;	
                 ////float3 finalCol = diff*_SunColor.rgb* atten ;////////////
				 float3 finalCol = diff * _SunColor2* atten;//  _LightColor0.rgb* atten;

              //float3 endColor = _Control.x*_SunColor*lerp(_ShadowColor.rgb,saturate((FinalColor+0.5))*ColB, Lerpfactor)*diff*atten  +  (1.2-shaper*0.5)*_Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) + _Control.z*float4(min(finalCol.rgb,1),Texture1.a);
			  //endColor = FinalColor;// +0.5 * float3(1, 1, 1);
			  float3 endColor = (_Control.x*lerp(_ShadowColor.rgb, (0.7)*ColB, Lerpfactor) + _Control.z*float4(min(finalCol.rgb, 1), Texture1.a))*_SunColor.rgb;//_Color;
			  //////////
			  





//				 float3 endColor = _Control.x*_SunColor*lerp(_ShadowColor.rgb,saturate((FinalColor+0.5))*ColB*0.1, Lerpfactor)*diff*atten  
//				 +  1.5*cloudTexture1.a*(1.2-shaper*0.5)*_Control.y*float4(1.0-exp2(-FinalColor*ExposureBias),0) 
//				 +  2* cloudTexture1.a*(1)*_Control.z*float4(min(finalCol.rgb,1),Texture1.a);



              //    float3 endColor =   lerp(_ShadowColor.rgb*(diff*1*atten),(FinalColor*1*atten*diff)*ColB, Lerpfactor  ) + _LightColor0.rgb; 

              //  float4 Fcolor = float4((endColor),saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2)  ))*atten;///////////////////
          //     float4 Fcolor = float4(saturate(endColor + _FogFactor*diff2*_FogColor*0.00000001 + 0),saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2)  )) ;

          		//v3.3e
//               float4 Fcolor = float4(saturate(endColor*0.9 + 0.5*endColor*_FogFactor*1*diff2*_FogColor*0.00000001),saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2)  )) ;	///////////////////////////// 
//                float4 Fcolor = float4(saturate(endColor*0.9 + 0.1*endColor*_FogFactor*1*diff2*_FogColor*0.000000001*1),saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2*1)  )) ;
                //float4 Fcolor =  float4(saturate(endColor*0.9 + 0.1*endColor*_FogFactor*1*diff2*_FogColor*0.00000001*1),saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2*1)  )) ;
				float4 Fcolor = float4(saturate(endColor + (_FogFactor / 3)  *diff2*_FogColor*0.00000001 + 0), saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2))); //-8 _FogFactor

				//v0.8
				//https://forum.unity.com/threads/how-to-access-color-of-directional-light-in-cg-shader.137962/
				//RANDOM - https://www.ronja-tutorials.com/post/024-white-noise/
				//Fcolor.rgb = saturate(shaper - 0.01*(_HorizonFactor*0.00001*diff2 * 1));
				Fcolor.rgb = Fcolor.rgb * (pow(_SunColor2,0.2 + sunPosition.w*0.1))+ float3(1, 1, 1) * localLightPower;
				//Fcolor.rgb = Fcolor.rgb * (pow(float3(1,0,0), 0.2 + sunPosition.w*0.1)) + float3(1, 1, 1) * localLightPower;

               if(_FogUnity==1){
                UNITY_APPLY_FOG(i.fogCoord, Fcolor);
               }

			   float distToVertex = length(posVertex - _WorldSpaceCameraPos);
			   float3 colorOUT = saturate(lerp(Fcolor, _FogColor, fogPower * pow(distToVertex, fogPowerExp - 0.3) * 0.0015));

			   return (float4(colorOUT, Fcolor.a*paintTexture1.a));
                //return float4(endColor.rgb, Fcolor.a*paintTexture1.a);
                //return (float4(Fcolor.r,Fcolor.g,Fcolor.b, Fcolor.a*paintTexture1.a));

            }
            ENDCG
        }
//        Pass {
//            Name "ForwardAdd"
//            Tags {
//                "LightMode"="ForwardAdd"
//            }
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
//
//
//			//v0.7
//			float4 vortexControl;
//			float4 vortexPosRadius;// = float4(0, 0, 0, 1000);
//			//v0.7a - VORTEX
//			float3x3 rotationMatrix(float3 axis, float angle)
//			{
//				axis = normalize(axis);
//				float s = sin(angle);
//				float c = cos(angle);
//				float oc = 1.0 - c;
//
//				return float3x3 (oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s,
//					oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s,
//					oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c);
//			}
//
//			float4 frag(VertexOutput i) : COLOR{
//
//				//v0.7a - VORTEX
//				//float4 vortexPosRadius = float4(0, 0, 0, 1000);
//				float distanceToVortexCenter = length(vortexPosRadius.xz - i.worldPos.xz);
//				float3x3 rotator = rotationMatrix(float3(0, 1, 0), 1 * ((_Time.y * 0.5 * vortexControl.y) - vortexControl.x * 700 * (distanceToVortexCenter / 10000000)));
//				//float3 posVertex = mul(float3(i.worldPos.x, 0, i.worldPos.z), rotator);
//				float3 posVertexA = float3(i.worldPos.x, 0, i.worldPos.z);// vortexPosRadius;
//				posVertexA = posVertexA - vortexPosRadius;
//				float3 posVertex = mul(posVertexA, rotator);
//				//posVertexA += vortexPosRadius;
//				posVertex.y = i.worldPos.y;
//				if (distanceToVortexCenter > 150000 * vortexControl.w * 0.01) {
//					posVertex = i.worldPos;
//				}
//				else {
//					Thickness = Thickness / vortexControl.z;
//				}
//				//float cloudSample = tex3Dlod(_ShapeTexture, float4(posVertex * _Scale, lod)).r;
//
//				float change_h = _CutHeight;//240;
//
//
//
//				//v0.7a - VORTEX
//				float PosDiff = Thickness * 0.0006*(posVertex.y - change_h) - 0.4;
//				float2 UVs = _Density * float2(posVertex.x, posVertex.z);
//				float4 TimingF = 0.0012;
//				float2 UVs1 = _Velocity1 * TimingF*_Time.y + UVs;
//				//float PosDiff = 0.0006*(i.worldPos.y-change_h);
//               // float2 UVs = _Density*float2(i.worldPos.x,i.worldPos.z);
//               // float4 TimingF = 0.0012;
//               // float2 UVs1 = _Velocity1*TimingF*_Time.y + UVs;
//
//                float4 cloudTexture = tex2D(_CloudMap,UVs1+_CloudMap_ST);
//                float4 cloudTexture1 = tex2D(_CloudMap1,UVs1+_CloudMap1_ST);
//
//                //_PaintMap
//               // float4 paintTexture1 = tex2D(_PaintMap,UVs1+_CloudMap1_ST);
//               // float4 paintTexture1 = tex2D(_PaintMap,UVs1*_PaintMap_ST.zw*_PaintMap_ST.xy);
//				float4 paintTexture1 = tex2D(_PaintMap, UVs1*_PaintMap_ST.xy +_PaintMap_ST.zw);//v5.0.9
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


			//v0.8
				//public float fog_depth = 0.29f;// 1.5f;
			float reileigh;// = 1.3f;//2.0f;
			//public float mieCoefficient = 1;//0.1f;
			//public float mieDirectionalG = 0.1f;
			//public float ExposureBias = 0.11f;//
			float nA = 1.0003f;
			float NA = 2.545E25f;
			float pnA = 0.035f;
			float3 lambda;// = new Vector3(680E-9f, 550E-9f, 450E-9f);//new Vector3(680E-9f, 550E-9f, 450E-9f);
			float3 KA;// = new Vector3(0.9f, 0.5f, 0.5f);



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


			//v0.7
			float4 vortexControl;
			float4 vortexPosRadius;// = float4(0, 0, 0, 1000);
			//v0.7a - VORTEX
			float3x3 rotationMatrix(float3 axis, float angle)
			{
				axis = normalize(axis);
				float s = sin(angle);
				float c = cos(angle);
				float oc = 1.0 - c;

				return float3x3 (oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s,
					oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s,
					oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c);
			}

			float4 frag(VertexOutput i) : COLOR{

				//v0.7a - VORTEX
				//float4 vortexPosRadius = float4(0, 0, 0, 1000);
				float distanceToVortexCenter = length(vortexPosRadius.xz - i.worldPos.xz);
				float3x3 rotator = rotationMatrix(float3(0, 1, 0), 1 * ((_Time.y * 0.5 * vortexControl.y) - vortexControl.x * 700 * (distanceToVortexCenter / 10000000)));
				//float3 posVertex = mul(float3(i.worldPos.x, 0, i.worldPos.z), rotator);
				float3 posVertexA = float3(i.worldPos.x, 0, i.worldPos.z);// vortexPosRadius;
				posVertexA = posVertexA - vortexPosRadius;
				float3 posVertex = mul(posVertexA, rotator);
				//posVertexA += vortexPosRadius;
				posVertex.y = i.worldPos.y;
				if (distanceToVortexCenter > 150000 * vortexControl.w * 0.01) {
					posVertex = i.worldPos;
				}
				else {
					Thickness = Thickness / vortexControl.z;
				}
				//float cloudSample = tex3Dlod(_ShapeTexture, float4(posVertex * _Scale, lod)).r;

				float change_h = _CutHeight;//240;



				//v0.7a - VORTEX
				float PosDiff = Thickness * 0.0006*(posVertex.y - change_h) - 0.4;
				float2 UVs = _Density * float2(posVertex.x, posVertex.z);
				float4 TimingF = 0.0012;
				float2 UVs1 = _Velocity1 * TimingF*_Time.y + UVs;
				//float PosDiff = 0.0006*(i.worldPos.y-change_h);
                //float2 UVs = _Density*float2(i.worldPos.x,i.worldPos.z);
               //float4 TimingF = 0.0012;
               // float2 UVs1 = _Velocity1*TimingF*_Time.y + UVs;

                float4 cloudTexture = tex2D(_CloudMap,UVs1+_CloudMap_ST);
                float4 cloudTexture1 = tex2D(_CloudMap1,UVs1+_CloudMap1_ST);

                //_PaintMap
               // float4 paintTexture1 = tex2D(_PaintMap,UVs1+_CloudMap1_ST);
                float4 paintTexture1 = tex2D(_PaintMap,UVs1*_PaintMap_ST.xy + _PaintMap_ST.zw); //v5.0.9

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
             //	float shaper = ((_Transparency+_TranspOffset)-0.48+0.0)*((DER1*(((_Coverage+_CoverageOffset)+cloudTexture1.a)))*Texture2.a);
				float shaper = (_Transparency + 4.5 + _TranspOffset) *((DER1*saturate(((_Coverage + _CoverageOffset) - (0.8*PosDiff) + cloudTexture1.a* (Texture2.a)))));

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