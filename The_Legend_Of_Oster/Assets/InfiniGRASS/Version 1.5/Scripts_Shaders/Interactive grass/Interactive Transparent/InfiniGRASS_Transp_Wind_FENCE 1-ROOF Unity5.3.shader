// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_LightmapInd', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D
// Upgrade NOTE: replaced tex2D unity_LightmapInd with UNITY_SAMPLE_TEX2D_SAMPLER


Shader "InfiniGRASS/InfiniGrass Directional Wind ROOF Unity 5.3 (Legacy)" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        
        _BulgeScale ("Bulge Scale", Float ) = 0.2
        _BulgeShape ("Bulge Shape", Float ) = 5
        _BulgeScale_copy ("Bulge Scale_copy", Float ) = 1.2 // control turbulence
                               
        _WaveControl1("Waves", Vector) = (1, 0.01, 0.001, 0.41) // _WaveControl1.w controls interaction power
        _TimeControl1("Time", Vector) = (1, 1, 1, 100)
        _OceanCenter("Ocean Center", Vector) = (0, 0, 0, 0)
        
         _RandYScale ("Vary Height Ammount", Float ) = 1
         _RippleScale ("Vary Height", Float ) = 0     
        
           //INFINIGRASS - hero position for fading and lowering dynamics to reduce jitter while interacting  
           
           _InteractPos("Interact Position", Vector) = (0, 0, 0) //for lowering motion when interaction item is near
            _InteractSpeed("Interact Speed", Vector) = (0, 0, 0) //v1.5
           _FadeThreshold ("Fade out Threshold", Float ) = 100
           _StopMotionThreshold ("Stop motion Threshold", Float ) = 10
           
           _Color ("Grass tint", Color) = (0.5,0.8,0.5,0) //0.5,0.8,0.5
           _ColorGlobal ("Global tint", Color) = (0.5,0.5,0.5,0) //0.5,0.8,0.5
           _TintPower("tint power", Float) = 0
            _TintFrequency("tint frequency", Float) = 0.1
           _SpecularPower("Specular", Float) = 1
           
           _SmoothMotionFactor("Smooth wave motion", Float) = 105
           _WaveXFactor("Wave Control x axis", Float) = 1
           _WaveYFactor("Wave Control y axis", Float) = 1
           
           //SNOW
           _SnowTexture ("Snow texture", 2D) = "white" {}

           //ROOF version of FENCE shader - control local vs global wind
           //_TimeControl1.y for global, _TimeControl1.z for local
           _BaseLight("Grass Base light control", Float) = 0
            _BaseColorYShift("Shift Base color Y axis", Float) = 1
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fog //v1.7.7 - add Unity fog for forward rendering
            #pragma multi_compile_fwdbase_fullshadows
           // #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            #ifndef LIGHTMAP_OFF
                // float4 unity_LightmapST;
                // sampler2D unity_Lightmap;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            
            uniform float _BulgeScale; 
            uniform float _BulgeShape;
            uniform float _BulgeScale_copy;
            float4 _WaveControl1;
   			float4 _TimeControl1;
    		float3 _OceanCenter;
            uniform fixed _Cutoff;
             uniform float _RandYScale;
            uniform float _RippleScale;
            //float3 _CameraPos;
            float3 _InteractPos;
            float _FadeThreshold;
            float _StopMotionThreshold;
            
            float3 _Color;
            float3 _ColorGlobal;
           	float _TintPower; 
           	float _SpecularPower;
           	float _SmoothMotionFactor;
           	float _WaveXFactor;
           	float _WaveYFactor;
           	
           	sampler2D _SnowTexture;
           	float _SnowCoverage; 	
           	float _TintFrequency;

           	 float3 _InteractSpeed;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
                 UNITY_FOG_COORDS(7)//v1.7.7
                #ifndef LIGHTMAP_OFF
                    float2 uvLM : TEXCOORD8;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_389 = o.vertexColor;
                float4 node_392 = _Time + _TimeEditor;
           //     v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
                
                
                float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
                
                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                            
                       
                       //INIFNIGRASS
                       float4 modelY = float4(0.0,1.0,0.0,0.0);
                               float4 ModelYWorld =mul(unity_ObjectToWorld,modelY);
                               float scaleY = length(ModelYWorld);
                                
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
              // o.posWorld =  v.vertex;



                                                                                                  
//                if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold){                 
//                	_BulgeScale = 0;
//                	_BulgeScale_copy = 0;
//                }
//
  float3 SpeedFac = float3(0,0,0);  //	SpeedFac =  _InteractSpeed;  
                float distA =  distance(_InteractPos,o.posWorld)/ (_StopMotionThreshold*1);      
                  if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold*1){ 
               // if( distance(_InteractPos.x,o.posWorld.x) < _StopMotionThreshold/2 ){    
               //      if( distance(_InteractPos.z,o.posWorld.z) < _StopMotionThreshold/2){                  
                	//_BulgeScale = 0;
                	//_BulgeScale_copy = 0;                	
                	SpeedFac =  _InteractSpeed *_WaveControl1.w;
//                		if( o.uv0.y > 0.2){
//							o.posWorld.x += (_InteractSpeed.x*22+0.1)*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
//							o.posWorld.z += (_InteractSpeed.z+0.1)*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
//						}
                	if( o.uv0.y < 0.3){
                	//_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
                	//_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
                	}
                	if( o.uv0.y > 0.19){
						_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
                		_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
                	}
                	if( o.uv0.y > 0.5){
						o.posWorld.y = o.posWorld.y - (1-distA)*3*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ;//+_BulgeScale*0.5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) ;
					}             
               }




	                if( o.uv0.y > 0.1){
	       //         	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) /scaleY;// unity_Scale.w;
					}
	                if( o.uv0.y >= 0.01){
	       //         	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
	                }

	                //v1.5
	           _BulgeScale= _BulgeScale* _BulgeScale_copy;
	            //  _OceanCenter.x = 0.0;
	          // _OceanCenter.z = 0.0;

	                     dist = 90* (cos(_BulgeShape+_Time.y/15))-_SmoothMotionFactor;
				///////////////////////// 
				if( o.uv0.y > 0.1){
					o.posWorld.x += _BulgeScale*1*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
					o.posWorld.z += _BulgeScale*1*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
				}
				if( o.uv0.y > 0.2){					
					o.posWorld.x += _BulgeScale*2*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/3);
					o.posWorld.z += _BulgeScale*2*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);	
				}
				if( o.uv0.y > 0.3){
					
					o.posWorld.x += _BulgeScale*3*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/4);
					o.posWorld.z += _BulgeScale*3*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);
				}
				if( o.uv0.y > 0.4){
					
					o.posWorld.x += _BulgeScale*4*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/2) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/2);
					o.posWorld.z += _BulgeScale*4*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/2) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/2);	
				}		
				if( o.uv0.y > 0.96){
					
					o.posWorld.x += _BulgeScale*5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/0.9)	+ _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/1);
					o.posWorld.z += _BulgeScale*5*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/0.9) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/1);
				}	
				//ADD GLOBAL ROTATION - WIND						
				v.vertex = mul(unity_WorldToObject, o.posWorld);	            
	           // v.vertex =  o.posWorld;
                                                                                

                
                
                o.pos = UnityObjectToClipPos(v.vertex);
                #ifndef LIGHTMAP_OFF
                    o.uvLM = v.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                UNITY_TRANSFER_FOG(o, o.pos);//v1.7.7
                return o;
            }
            
            
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_582 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_582.rg, _Normal))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_582.rg, _Diffuse));
                
                
                
                clip(node_1.a - _Cutoff);
                
                //DEFINE FADE BASED ON CAMERA - INFINIGRASS
				float Aplha = 1;
				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
					 clip(-1);
				}
                
                #ifndef LIGHTMAP_OFF
                    float4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uvLM);
                    #ifndef DIRLIGHTMAP_OFF
                        float3 lightmap = DecodeLightmap(lmtex);
                        float3 scalePerBasisVector = DecodeLightmap(UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap,i.uvLM));
                        UNITY_DIRBASIS
                        half3 normalInRnmBasis = saturate (mul (unity_DirBasis, normalLocal));
                        lightmap *= dot (normalInRnmBasis, scalePerBasisVector);
                    #else
                        float3 lightmap = DecodeLightmap(lmtex);
                    #endif
                #endif
                #ifndef LIGHTMAP_OFF
                    #ifdef DIRLIGHTMAP_OFF
                        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                    #else
                        float3 lightDirection = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
                        lightDirection = mul(lightDirection,tangentTransform); // Tangent to world
                    #endif
                #else
                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                #endif
                
                
                lightDirection =  normalize(reflect(_WorldSpaceLightPos0.xyz,normalDirection));
                
                
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                //float attenuation = LIGHT_ATTENUATION(i);
				UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWorld.xyz); //v1.9 
                
                   //INFINIGRASS
                 float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);
                 float node_133 = pow((abs((frac((i.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                   float dist = distance(_OceanCenter, float3(_WaveControl1.x*i.posWorld.y,_WaveControl1.y*i.posWorld.x,_WaveControl1.z*i.posWorld.z) );
               
               float3 attenColor = attenuation * (_LightColor0.xyz);
               
               
               
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(0.9,0.9,0.8)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) ;//* float3(0.9,1,0.5); //v1.4
                #ifndef LIGHTMAP_OFF
                    float3 diffuse = lightmap.rgb;
                #else
                    float3 diffuse = (forwardLight+backLight) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
                #endif
///////// Gloss:
                float gloss = 0.4;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_3 = 0.2;
                float3 specularColor = float3(node_3,node_3,node_3);
                float3 specular = 3 * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
//                #ifndef LIGHTMAP_OFF
//                    #ifndef DIRLIGHTMAP_OFF
//                        specular *= lightmap;
//                    #else
//                        specular *= (floor(attenuation) * _LightColor0.xyz);
//                    #endif
//                #else
//                    specular *= (floor(attenuation) * _LightColor0.xyz);
//                #endif
                specular *= ((attenuation) * _LightColor0.xyz);
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_331 = 1.0;
             //   finalColor += diffuseLight * (lerp(float3(node_331,node_331,node_331),float3(0.9632353,0.8224623,0.03541304),i.vertexColor.b)*node_1.rgb);
                finalColor += diffuseLight * (node_1.rgb)*_ColorGlobal; //v1.4
                
                specular = specular * (i.uv0.y*2-0.5) ;
                finalColor = lerp(finalColor, finalColor*_Color,_TintPower*i.uv0.y*(0.9+0.6*cos(i.posWorld.x*2*_TintFrequency)+0.6*sin(i.posWorld.z*3*_TintFrequency)+0.6*sin(i.posWorld.z*1*_TintFrequency+0.1)));
                
                finalColor += specular * _SpecularPower;
/// Final Color:

				//SNOW
                float3 col = finalColor;
                
                float4 SnowTexColor = tex2D(_SnowTexture,  i.uv0);
				
				//if(i.uv0.y >= 1-(3 * (_SnowCoverage+_TimeControl1.y-1)) * col.r* col.r)
				if(i.uv0.y >= 1-(4 * (_SnowCoverage+_TimeControl1.y-1)) * col.r* col.r* col.r+0.01) //v1.7.6
	            {     	  
	            	if(i.uv0.y < 0.99 ){    //v1.7.6
		                //col =  lerp (  col , SnowTexColor*0.9,1-(0.5 * _SnowCoverage)) ;   
		                //    col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;
		                //o.Normal = normalize(o.Normal + UnpackNormal(tex2D(_SnowBump, IN.uv_SnowBump))*1);   
		                //col.rgb = float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z; 
		                //v1.7.6
		                col.rgb = (float4(i.uv0.y,i.uv0.y,i.uv0.y,1)*4*+_TimeControl1.z)*(1+finalColor)*1.6;   
	                }                      
	            }
	            else
	            {
					//col = col * input.color * input.color.a *_UnityTerrainTreeTintColorSM *1.5;						
				//	col.rgb *= input.color.rgb;
				//	clip(col.a);
				//	col=col* _UnityTerrainTreeTintColorSM;
				}
                
                //END SNOW
                UNITY_APPLY_FOG(i.fogCoord, col); //v1.7.7
                return fixed4(col,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fog //v1.7.7 - add Unity fog for forward rendering
            #pragma multi_compile_fwdadd_fullshadows
           // #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            #ifndef LIGHTMAP_OFF
                // float4 unity_LightmapST;
                // sampler2D unity_Lightmap;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            
               uniform float _BulgeScale; 
            uniform float _BulgeShape;
            uniform float _BulgeScale_copy;
            float4 _WaveControl1;
   			float4 _TimeControl1;
    		float3 _OceanCenter;
            uniform fixed _Cutoff;
             uniform float _RandYScale;
            uniform float _RippleScale;
            
            float3 _InteractPos;
            float _FadeThreshold;
            float _StopMotionThreshold;
            float _SmoothMotionFactor;
            float _WaveXFactor;
           	float _WaveYFactor;

           	 float3 _InteractSpeed;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)//v1.7.7
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_389 = o.vertexColor;
                float4 node_392 = _Time + _TimeEditor;
            //    v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
                
                float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
                
                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                               
                                 //INIFNIGRASS
                       float4 modelY = float4(0.0,1.0,0.0,0.0);
                               float4 ModelYWorld =mul(unity_ObjectToWorld,modelY);
                               float scaleY = length(ModelYWorld);     
                               
                  o.posWorld = mul(unity_ObjectToWorld, v.vertex);
             //  o.posWorld =  v.vertex;
                                      
//                if( distance(_InteractPos,o.posWorld) <  _StopMotionThreshold){                 
//                	_BulgeScale = 0;
//                	_BulgeScale_copy = 0;
//                }

float3 SpeedFac = float3(0,0,0);  //	SpeedFac =  _InteractSpeed;  
                float distA =  distance(_InteractPos,o.posWorld)/ (_StopMotionThreshold*1);      
                  if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold*1){ 
               // if( distance(_InteractPos.x,o.posWorld.x) < _StopMotionThreshold/2 ){    
               //      if( distance(_InteractPos.z,o.posWorld.z) < _StopMotionThreshold/2){                  
                	//_BulgeScale = 0;
                	//_BulgeScale_copy = 0;                	
                	SpeedFac =  _InteractSpeed * _WaveControl1.w;
//                		if( o.uv0.y > 0.2){
//							o.posWorld.x += (_InteractSpeed.x*22+0.1)*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
//							o.posWorld.z += (_InteractSpeed.z+0.1)*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
//						}
                	if( o.uv0.y < 0.3){
                	//_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
                	//_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
                	}
                	if( o.uv0.y > 0.19){
						_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
                		_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
                	}
                	if( o.uv0.y > 0.5){
						o.posWorld.y = o.posWorld.y - (1-distA)*3*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ;//+_BulgeScale*0.5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) ;
					}             
               }
                               
                if( o.uv0.y > 0.1){
            //    	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) / scaleY;
				}
                if( o.uv0.y >= 0.01){
           //     	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
                }
                
                  //v1.5
	           _BulgeScale= _BulgeScale* _BulgeScale_copy;
	           //   _OceanCenter.x = 0.0;
	          // _OceanCenter.z = 0.0;

                  dist = 90* (cos(_BulgeShape+_Time.y/15))-_SmoothMotionFactor;
				///////////////////////// 
				if( o.uv0.y > 0.1){
					o.posWorld.x += _BulgeScale*1*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
					o.posWorld.z += _BulgeScale*1*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
				}
				if( o.uv0.y > 0.2){					
					o.posWorld.x += _BulgeScale*2*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/3);
					o.posWorld.z += _BulgeScale*2*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);	
				}
				if( o.uv0.y > 0.3){
					
					o.posWorld.x += _BulgeScale*3*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/4);
					o.posWorld.z += _BulgeScale*3*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);
				}
				if( o.uv0.y > 0.4){
					
					o.posWorld.x += _BulgeScale*4*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/2) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/2);
					o.posWorld.z += _BulgeScale*4*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/2) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/2);	
				}		
				if( o.uv0.y > 0.96){
					
					o.posWorld.x += _BulgeScale*5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/0.9)	+ _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/1);
					o.posWorld.z += _BulgeScale*5*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/0.9) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/1);
				}				
				//ADD GLOBAL ROTATION - WIND						
			v.vertex = mul(unity_WorldToObject, o.posWorld);	            
	          //  v.vertex =  o.posWorld;
                
                
                //o.posWorld = mul(_Object2World, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                  UNITY_TRANSFER_FOG(o, o.pos);//v1.7.7
                return o;
            }
            
            fixed4 frag(VertexOutput i) : COLOR {
            
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_583 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_583.rg, _Normal))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_583.rg, _Diffuse));
                clip(node_1.a - _Cutoff);
                
                 //DEFINE FADE BASED ON CAMERA - INFINIGRASS
				float Aplha = 1;
				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
					 clip(-1);
				}
                
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                //float attenuation = LIGHT_ATTENUATION(i);
				UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWorld.xyz); //v1.9 
                
                //INFINIGRASS
                 float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);
                  float node_133 = pow((abs((frac((i.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                
      //          float3 attenColor = attenuation * (_LightColor0.xyz*(node_133+1)  + _LightColor0.xyz*dot(viewDirection,lightDirection)/1);
                
                float3 attenColor = attenuation * _LightColor0.xyz ;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = float3(0.9,0.9,0.8)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(0.9,1,0.5);
                float3 diffuse = (forwardLight+backLight) * attenColor;
///////// Gloss:
                float gloss = 0.4;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_3 = 0.2;
                float3 specularColor = float3(node_3,node_3,node_3);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_331 = 1.0;
                finalColor += float3(0,0,0);//diffuseLight * (lerp(float3(node_331,node_331,node_331),float3(0.9632353,0.8224623,0.03541304),i.vertexColor.b)*node_1.rgb);
                
                
                //INFINIGRASS
                //if( i.uv0.y > 0.6){
//                if(dot(viewDirection,lightDirection) > 0.9){
//                	finalColor += (i.uv0.y)/6 ;
//                }
                //}
                
                
                finalColor += specular ;
/// Final Color:

				
				 UNITY_APPLY_FOG(i.fogCoord, finalColor); //v1.7.7
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
//        Pass {
//            Name "ShadowCollector"
//            Tags {
//                "LightMode"="ShadowCollector"
//            }
//            Cull Off
//            
//            Fog {Mode Off}
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #define UNITY_PASS_SHADOWCOLLECTOR
//            #define SHADOW_COLLECTOR_PASS
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #pragma multi_compile_shadowcollector
//            #pragma exclude_renderers gles xbox360 ps3 flash 
//            #pragma target 3.0
//            uniform float4 _TimeEditor;
//            #ifndef LIGHTMAP_OFF
//                // float4 unity_LightmapST;
//                // sampler2D unity_Lightmap;
//                #ifndef DIRLIGHTMAP_OFF
//                    // sampler2D unity_LightmapInd;
//                #endif
//            #endif
//            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
//            
//            uniform float _BulgeScale; 
//            uniform float _BulgeShape;
//            uniform float _BulgeScale_copy;
//            float4 _WaveControl1;
//   			float4 _TimeControl1;
//    		float3 _OceanCenter;
//    		uniform fixed _Cutoff;
//    		 uniform float _RandYScale;
//            uniform float _RippleScale;
//            
//            float3 _InteractPos;
//            float _FadeThreshold;
//            
//            struct VertexInput {
//                float4 vertex : POSITION;
//                float3 normal : NORMAL;
//                float2 texcoord0 : TEXCOORD0;
//                float4 vertexColor : COLOR;
//            };
//            struct VertexOutput {
//                V2F_SHADOW_COLLECTOR;
//                float2 uv0 : TEXCOORD5;
//                float3 normalDir : TEXCOORD6;
//                float4 vertexColor : COLOR;
//            };
//            VertexOutput vert (VertexInput v) {
//                VertexOutput o;
//                o.uv0 = v.texcoord0;
//                o.vertexColor = v.vertexColor;
//                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
//                float4 node_389 = o.vertexColor;
//                float4 node_392 = _Time + _TimeEditor;
//            //    v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
//                
//                  float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(_Object2World, v.vertex).y,_WaveControl1.y*mul(_Object2World, v.vertex).x,_WaveControl1.z*mul(_Object2World, v.vertex).z) );
//                float dist2 = distance(_OceanCenter, float3(mul(_Object2World, v.vertex).y,mul(_Object2World, v.vertex).x*0.10,0.1*mul(_Object2World, v.vertex).z) );
//                
//                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
//                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
//                              
//                               //INIFNIGRASS
//                       float4 modelY = float4(0.0,1.0,0.0,0.0);
//                               float4 ModelYWorld =mul(_Object2World,modelY);
//                               float scaleY = length(ModelYWorld);
//                                 
//                if( o.uv0.y > 0.1){
//                	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) /scaleY;
//				}
//				if( o.uv0.y >= 0.01){
//                	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
//                }
//                
//                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//                TRANSFER_SHADOW_COLLECTOR(o)
//                return o;
//            }
//            fixed4 frag(VertexOutput i) : COLOR {
//                i.normalDir = normalize(i.normalDir);
//                float2 node_584 = i.uv0;
//                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_584.rg, _Diffuse));
//                clip(node_1.a - _Cutoff);
//                
//                 //DEFINE FADE BASED ON CAMERA - INFINIGRASS
//				float Aplha = 1;
//				//if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
//				//	 clip(-1);
//				//}
//                
//                
//                SHADOW_COLLECTOR_FRAGMENT(i)
//            }
//            ENDCG
//        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
        //    #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            #ifndef LIGHTMAP_OFF
                // float4 unity_LightmapST;
                // sampler2D unity_Lightmap;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            
            uniform float _BulgeScale; 
            uniform float _BulgeShape;
            uniform float _BulgeScale_copy;
            float4 _WaveControl1;
   			float4 _TimeControl1;
    		float3 _OceanCenter;
    		uniform fixed _Cutoff;
    		 uniform float _RandYScale;
            uniform float _RippleScale;
            
            float3 _InteractPos;
            float _FadeThreshold;
            float _StopMotionThreshold;
            float _SmoothMotionFactor;
            float _WaveXFactor;
           	float _WaveYFactor;

           	 float3 _InteractSpeed;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                float4 node_389 = o.vertexColor;
                float4 node_392 = _Time + _TimeEditor;
              //  v.vertex.xyz += (normalize((float3(1,0.5,0.5)+v.normal))*node_389.r*sin(((node_389.b*3.141592654)+node_392.g+node_392.b))*0.16);
                
                  float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
                
                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
                float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                            
                                //INIFNIGRASS
                       float4 modelY = float4(0.0,1.0,0.0,0.0);
                               float4 ModelYWorld =mul(unity_ObjectToWorld,modelY);
                               float scaleY = length(ModelYWorld);
                                  
                    o.posWorld = mul(unity_ObjectToWorld, v.vertex);
              // o.posWorld =  v.vertex;
                                      
//                if( distance(_InteractPos,o.posWorld) <  _StopMotionThreshold){                 
//                	_BulgeScale = 0;
//                	_BulgeScale_copy = 0;
//                }   

float3 SpeedFac = float3(0,0,0);  //	SpeedFac =  _InteractSpeed;  
                float distA =  distance(_InteractPos,o.posWorld)/ (_StopMotionThreshold*1);      
                  if( distance(_InteractPos,o.posWorld) < _StopMotionThreshold*1){ 
               // if( distance(_InteractPos.x,o.posWorld.x) < _StopMotionThreshold/2 ){    
               //      if( distance(_InteractPos.z,o.posWorld.z) < _StopMotionThreshold/2){                  
                	//_BulgeScale = 0;
                	//_BulgeScale_copy = 0;                	
                	SpeedFac =  _InteractSpeed * _WaveControl1.w;
//                		if( o.uv0.y > 0.2){
//							o.posWorld.x += (_InteractSpeed.x*22+0.1)*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
//							o.posWorld.z += (_InteractSpeed.z+0.1)*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
//						}
                	if( o.uv0.y < 0.3){
                	//_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
                	//_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
                	}
                	if( o.uv0.y > 0.19){
						_WaveXFactor = _WaveXFactor - (1-distA)*(1-distA)*SpeedFac.z;
                		_WaveYFactor = _WaveYFactor - (1-distA)*(1-distA)*SpeedFac.x;
                	}
                	if( o.uv0.y > 0.5){
						o.posWorld.y = o.posWorld.y - (1-distA)*3*(o.uv0.y-0.5)*sin(o.posWorld.z+_Time.y) ;//+_BulgeScale*0.5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) ;
					}             
               }
                                                                                         
                if( o.uv0.y > 0.1){
          //      	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy)) / scaleY;
				}
				if( o.uv0.y >= 0.01){
          //      	v.vertex.y = v.vertex.y *_RandYScale* abs(cos(((_TimeControl1.w*_Time.y +_TimeControl1.z)*0.2 + 2*dist)*_RippleScale));
                }

                  //v1.5
	           _BulgeScale= _BulgeScale* _BulgeScale_copy;
	         //  _OceanCenter.x = 0.0;
	          // _OceanCenter.z = 0.0;
                
               dist = 90* (cos(_BulgeShape+_Time.y/15))-_SmoothMotionFactor;
				///////////////////////// 
				if( o.uv0.y > 0.1){
					o.posWorld.x += _BulgeScale*1*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/5) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/5);
					o.posWorld.z += _BulgeScale*1*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/5) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/6);
				}
				if( o.uv0.y > 0.2){					
					o.posWorld.x += _BulgeScale*2*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/3);
					o.posWorld.z += _BulgeScale*2*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);	
				}
				if( o.uv0.y > 0.3){
					
					o.posWorld.x += _BulgeScale*3*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/3) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/4);
					o.posWorld.z += _BulgeScale*3*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/3) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/3);
				}
				if( o.uv0.y > 0.4){
					
					o.posWorld.x += _BulgeScale*4*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/2) + _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/2);
					o.posWorld.z += _BulgeScale*4*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/2) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/2);	
				}		
				if( o.uv0.y > 0.96){
					
					o.posWorld.x += _BulgeScale*5*cos(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*sin(o.posWorld.z+_Time.y) + _WaveXFactor*((2+cos(o.posWorld.x/dist))*_OceanCenter.x/0.9)	+ _WaveYFactor*((3+sin(2*o.posWorld.z/dist))*_OceanCenter.z/1);
					o.posWorld.z += _BulgeScale*5*sin(o.posWorld.x*_WaveControl1.x+_Time.y*_TimeControl1.x + o.posWorld.z*_WaveControl1.z)*0.1*cos(o.posWorld.z+_Time.y) + _WaveXFactor*((2+sin(o.posWorld.z/dist))*_OceanCenter.z/0.9) + _WaveYFactor*((3+cos(3*o.posWorld.x/dist))*_OceanCenter.x/1);
				}					
				//ADD GLOBAL ROTATION - WIND						
				v.vertex = mul(unity_WorldToObject, o.posWorld);	            
	            //v.vertex =  o.posWorld;
                
                
                
                
                o.pos = UnityObjectToClipPos(v.vertex);
               // o.posWorld = mul(_Object2World, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float2 node_585 = i.uv0;
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_585.rg, _Diffuse));
                clip(node_1.a - _Cutoff);
                
                 //DEFINE FADE BASED ON CAMERA - INFINIGRASS
				float Aplha = 1;
				if(distance(i.posWorld, _WorldSpaceCameraPos) > _FadeThreshold){
					 clip(-1);
				}
                
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/Diffuse"
   
}
