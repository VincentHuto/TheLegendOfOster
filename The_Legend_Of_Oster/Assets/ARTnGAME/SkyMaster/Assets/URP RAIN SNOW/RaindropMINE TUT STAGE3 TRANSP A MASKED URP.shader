//https://gamedev.stackexchange.com/questions/152496/how-can-i-make-natural-rain-drops-on-screen?noredirect=1&lq=1
//TUTORIAL - https://www.youtube.com/watch?v=EBrAdahFtuo
Shader "Custom/RaindropSM TUT STAGE3 TRANSP A MASKED URP" {
	Properties {
		_MainTex("_MainTex (RGB)", 2D) = "white" {}
		_Size("_Size", Float) = 1
		_Distortion("_Distortion", Float) = 1
		_Blur("_Blur", Float) = 0

			//v0.3
			_TileNumCausticRotMin("_TileNumCausticRotMin", float) = 1

			//v0.4
			_LoopNum("_LoopNum", Vector) = (1, 1, 1, 1)
			_RainSmallDirection("_RainSmallDirection", Vector) = (1, 1, 1, 1)

			//v0.6
			_TimeOffset("_TimeCenterOffsets", Vector) = (1, 1, 1, 1)
			_EraseCenterRadius("_EraseCenterRadius", Vector) = (0, 0, 1, 1)
			erasePower("ErasePower", float) = 1

			//MASKED
			maskPower("maskPower", float) = 1
			mainTexTilingOffset("main Texture Tiling and Offset", Vector) = (1, 1, 0,0)
	}
		SubShader{
			Tags{

				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"

			}
			//OD 200

				//v0.5
				//GrabPass{"_GrabTexture"} //HDRP

			Pass{
				CGPROGRAM

				//URP
				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/API/D3D11.hlsl"

				#pragma vertex vert
				#pragma fragment frag

				//TUTORIAL STAGE 1
				#define SS(a,b,t) smoothstep(a,b,t)

				#include "UnityCG.cginc"
				//#include "ShaderLibs/Framework2D.cginc"
				#include "ShaderLibs/FBM.cginc"
				#include "ShaderLibs/Feature.cginc"

				sampler2D _MainTex;// , _GrabTexture;

		//URP
		TEXTURE2D(_CameraOpaqueTexture);
		SAMPLER(sampler_CameraOpaqueTexture);
		float3 SampleSceneColor(float2 uv)
		{
			return SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, UnityStereoTransformScreenSpaceTex(uv)).rgb;
		}
		//UNITY_DECLARE_TEX2DARRAY(_ColorPyramidTexture);
		//SAMPLER(s_trilinear_clamp_sampler);


		//MASKED
		float4 mainTexTilingOffset;
		float maskPower;

		//TUTORIAL STAGE 1
		float4 _MainTex_ST;
		float _Size;
		float _Distortion;
		float _Blur;

		//v0.6
		float4 _TimeOffset;
		float4 _EraseCenterRadius;
		float erasePower;

		//v0.3
		float _TileNumCausticRotMin;
		float4 _RainSmallDirection;
		fixed2 Rains(fixed2 uv, fixed seed, fixed m) {
			float uvY = (uv.y - _RainSmallDirection.w);
			uv.x = uv.x + _RainSmallDirection.x * 0.5*uvY + _RainSmallDirection.y * 0.6 * uvY * uvY + _RainSmallDirection.z * 0.2 * uvY * uvY * uv.x;

			float period = 5;
			float2 retVal = float2(0.0, 0.0);
			float aspectRatio = 4.0;
			float tileNum = 5;
			float ySpd = 0.1;
			uv.y += ftime * 0.0618* _TimeOffset.x + _TimeOffset.y;
			uv *= fixed2(tileNum * aspectRatio, tileNum);			
			fixed idRand = Hash12(floor(uv));
			uv = frac(uv);
			float2 gridUV = uv;
			uv -= 0.5;
			float t = (ftime* _TimeOffset.x + _TimeOffset.y) * PI2 / period;
			t += idRand * PI2;
			uv.y += sin(t + sin(t + sin(t)*0.55))*0.45;
			uv.y *= aspectRatio;	

	//		uv.x -= 4*sin(t + sin(t + sin(t)*0.55))*0.45;
	//		uv.x *= aspectRatio;

			uv.x += (idRand - .5)*0.6;//0.6
			float r = length(uv);
			r = smoothstep(0.2, 0.1, r);
			float tailTileNum = 3.0;
			float2 tailUV = uv * float2(1.0, tailTileNum);
			tailUV.y = frac(tailUV.y) - 0.5;
			tailUV.x *= tailTileNum;			
			float rtail = length(tailUV);
			rtail *= uv.y * 1.5;
			rtail = smoothstep(0.2, 0.1, rtail);
			rtail *= smoothstep(0.3, 0.5, uv.y);
			retVal = float2(rtail*tailUV + r * uv);
			return retVal;
		}





			//https://www.shadertoy.com/view/MlSBzh
			float noise(float t) { return frac(sin(t*100.0)*1000.0); } //fract(sin(t*100.0)*1000.0); }
			float noise2(float2 p) { return noise(p.x + noise(p.y)); }

			float raindot(float2 uv, float2 id, float t) {
				float2 p = 0.1 + 0.8 * float2(noise2(id), noise2(id + float2(1.0, 0.0)));
				float r = clamp(0.5 - fmod(t + noise2(id), 1.0), 0.0, 1.0);
				return smoothstep(0.3 * r, 0.0, length(p - uv));
			}

			float trailDrop(float2 uv, float2 id, float t, float size) {
				float f = size*clamp(noise2(id) - 0.5, 0.0, 1.0);
				// wobbly path
				float wobble = 0.5 + 0.2
					* cos(12.0 * uv.y)
					* sin(50.0 * uv.y);
				float v = 1.0 - 300.0 / f * pow(uv.x - 0.5 + 0.2 * wobble, 2.0);
				// head
				v *= clamp(30.0 * uv.y, 0.0, 1.0);
				v *= clamp(uv.y + 7.0 * t - 0.6, 0.0, 1.0);
				// tail
				v *= clamp(1.0 - uv.y - pow(t, 2.0), 0.0, 1.0);

				//v0.6
				v *= clamp(0.82 - uv.y - pow(t, 1.2), 0.4, 1.0);

				return f * clamp(v * 10.0, 0.0, 1.0);
			}		

			float N21(float2 p) {
				p = frac(p*float2(123.34, 345.45));
				p += dot(p, p + 34.345);
				return frac(p.x * p.y);
			}

			float4 dropsLayer(float2 UV, float time) {
				
				float2 aspect = float2(2, 1);
				float2 uvs = UV * _Size * aspect;

				//move only downwards
				uvs.y += time * 0.25;

				float2 gv = frac(uvs) - 0.5;
				float2 id = floor(uvs); //quantize

				float n = N21(id);//NOISE - sudo random between 0 and 1
				time += n * 6.2831; //add noise to timing of drops
				float w = UV.y * 10;//3 - make wave horizontlly

				float x = (n - 0.5)*0.8; //sin(3 * w) * pow(sin(w),6)*0.45;// 0.2; //Make drops go left and right alignments
				x += (0.4 - abs(x)) * sin(3 * w) * pow(sin(w), 6)*0.45; // adjust with (0.4 -abs(x))  to avoid edges

				float y = -sin(time + sin(time + sin(time)*0.5))*0.45;
				y -= (gv.x - x) * (gv.x - x);//2 - make drop oval !!

				//move drops
				float2 dropPosition = (gv - float2(x, y)) / aspect; //length(gv / aspect)
				float drop = SS(0.05, 0.03, length(dropPosition));

				//TRAILS
				float2 trailPosition = (gv - float2(x, time * 0.25)) / aspect;
				trailPosition.y = (frac(trailPosition.y * 8) - 0.5) / 8; //go from +4 to -4 with 8
				float trail = SS(0.03, 0.01, length(trailPosition));
				float fogTrail = SS(-0.05, 0.05, dropPosition.y);//1//trail *= SS(-0.05, 0.05, dropPosition.y);//1
				fogTrail *= SS(0.5, y, gv.y);//2 - fade near the top
				trail *= fogTrail;

				//Limit trail in middle than all around !!! OR DISABLE TO HAVE new effect !!!
				fogTrail *= SS(0.05, 0.04, abs(dropPosition.x));
				//fogTrail *= SS(0.05, -0.04, abs(dropPosition.x* dropPosition.x));
				//fogTrail *= SS(0.05, -0.04, 2*abs(dropPosition.x* dropPosition.x));

	//			col += fogTrail * 0.5 ;
	//			col += trail;
	//			col += drop;
				//if (gv.x > 0.48 || gv.y > 0.49) { //red lines grid
				//	col = float4(1, 0, 0, 1);
				//}

				float2 offset = drop * dropPosition + trail * trailPosition - fogTrail * 0.005 *n;

				return float4(offset, fogTrail, n * dropPosition.x);
			}


			//v0.5
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD2;
				float4 grabUV : TEXCOORD1;
				
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = v.uv;
				o.grabUV = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));
				return o;
			}


			float4 frag(v2f i) : SV_Target{

				float4 colSTART = 0;// tex2D(_MainTex, i.uv2 * mainTexTilingOffset.xy + mainTexTilingOffset.zw) * maskPower; //MASKED

				//v0.6
				float eraser = 1;
				float distErase = sqrt((i.uv.x - _EraseCenterRadius.x)*(i.uv.x - _EraseCenterRadius.x) + (i.uv.y - _EraseCenterRadius.y)*(i.uv.y - _EraseCenterRadius.y));
				float fadeErase = 1-saturate(pow((distErase - _EraseCenterRadius.z),erasePower) / _EraseCenterRadius.w);
				if (distErase > _EraseCenterRadius.z) {
					eraser = fadeErase;
				}

				//v0.6
				float2 center = _TimeOffset.zw;
				float dist = sqrt((i.uv.x - center.x)*(i.uv.x - center.x) + (i.uv.y - center.y)*(i.uv.y - center.y));
				//i.uv.y = 1 - i.uv.y;
				//i.uv.x = 1 - 0.5*(i.uv.x * dist) / i.uv.y;
				i.uv.x = (i.uv.x + i.uv.x * dist);
				i.uv.y = (i.uv.y + i.uv.y * dist);

				float iTime = _Time.y  * _TimeOffset.x + _TimeOffset.y;
				float2 uv = i.uv.xy;
				float2 uv1 = float2(uv.x * 30.0, uv.y * 4.3 + noise(floor(uv.x * 20.0)));
				//float2 uv1 = float2(uv.x * 20.0, uv.y * 1.3 + noise(floor(uv.x * 20.0)));
				float2 uvi = floor(float2(uv1.x, uv1.y));
				float2 uvf =( uv1 - uvi);
				float v =  trailDrop(uvf, uvi, fmod(iTime + noise(floor(uv.x * 20.0)), 3.0) / 3.0, 12);
				float v2 =  trailDrop(uvf + float2(0.056, 0), uvi + float2(0.0156, 0), fmod(iTime * 2 + noise(floor(uv.x * 10.0)), 11.0) / 3.0, 1);
				//float v = trailDrop(uvf, uvi, fmod(iTime + noise(floor(uv.x * 10.0)), 3.0) / 13.0, 3);
				//float v2 = 1 * trailDrop(uvf + float2(1.056, 1), uvi + float2(1.0156, 2), fmod(iTime * 12 + noise(floor(uv.x * 10.0)), 11.0) / 3.0, 11);
				
				//v += 0.3*trailDrop(uvf, uvi, fmod(iTime * 0.5 + 1*noise(floor(uv.x * 20.0)),1) / 9.0, 51);
				v += raindot(frac(uv * 20.0 + float2(0, 0.1 * iTime)), floor(uv * 20.0 + float2(0, 0.1 * iTime)), iTime);				
		//		v2 += raindot(frac(uv * 30.0  + float2(0, 0.1 * iTime))* abs(cos(iTime*0.2)), floor(uv * 30.0 + float2(0, 0.1 * iTime)), iTime * 2);
				
				//float3 fragColor = tex2Dlod(_MainTex, float4(uv + float2(ddx(v), ddy(v)) + float2(ddx(v2), ddy(v2)), 0,1)  );
				//float3 fragColor = tex2D(_MainTex, float4(uv + float2(ddx(v), ddy(v)) + float2(ddx(v2), ddy(v2)), 0, 1));
							   

				//TUTORIAL STAGE 1
				float time = fmod(_Time.y * _TimeOffset.x + _TimeOffset.y, 7200); //_Time.y;
				float4 col = 0; 

				float4 drops = dropsLayer(i.uv, time);
				drops += dropsLayer(i.uv * 1.25 + 7.45, time);
				drops += dropsLayer(i.uv * 1.36 + 1.5, time * 1.2);
				drops += dropsLayer(i.uv * 1.57 - 7.45, time * 0.8);


				//v0.3 - caustic Rot min ------------------------------------ Add extra noise to trails
				float2 uvCaustA = _TileNumCausticRotMin * i.uv * 15;
				float timeCaustA = _Time.y * _TimeOffset.x + _TimeOffset.y;
				float val = CausticVoronoi(uvCaustA, timeCaustA); //CausticTriTwist //CausticVoronoi
				//_Distortion = _Distortion + 100*val;
				drops *= float4(val, val, val,1) + float4(val, val, val, 1)*2;

				//v0.4 - SMALL RAIN
				float baseOffset = 0.1;
				float2 uv4 = i.uv;
				//uv4 *= float2(_ScreenParams.x / _ScreenParams.y, 1.0);
				uv4 *= 1.5;
				float x4 = (sin((_Time.y)*.1)*.5 + .5)*.3;
				x4 = x4 * x4;
				x4 += baseOffset;
				float s = sin(x4);
				float c = cos(x4);
				float2x2 rot = float2x2(c, -s, s, c);
				uv4 = mul(rot, uv4);
				float moveSpd = 0.1;
				float2 rainUV = float2(0., 0.);
				rainUV += Rains(uv4, 152.12, moveSpd);
				rainUV += Rains(uv4*2.32, 25.23, moveSpd);
				//fixed4 finalColor = tex2D(_MainTex, i.uv + rainUV * 2.);
				drops.xy += rainUV * 0.35;


				//LONG RAIN
				float2 finalUVs = i.uv * _MainTex_ST.xy + _MainTex_ST.zw + drops.xy * _Distortion;//offset * _Distortion;
				finalUVs = float2(finalUVs + 0.05*(float2(ddx(v), ddy(v)) + float2(ddx(v2), ddy(v2)))); //ADD PREVIOUS RAIN SYSTEM - SMALL DROPS
				finalUVs += 0.5*(float2(ddx(v*0.4), ddy(v * 0.4)) + float2(ddx(v2 * 0.4), ddy(v2 * 0.4))) * drops.w;// n * dropPosition;
								
				float blur = _Blur * 7 * (1 - drops.z);// (1 - fogTrail);
		//		col = tex2Dlod(_MainTex, float4(finalUVs,0, blur));
		//		col = tex2D(_MainTex, i.uv * mainTexTilingOffset.xy + mainTexTilingOffset.zw); //MASKED
				col = colSTART;
				//col *= 0; col = N21(id); //col.rg = id * 0.1; //NOISE !!! Test


				//v0.5
				float fade = 1 - saturate(fwidth(i.uv) * 50);
				float2 projUV = i.grabUV.xy / i.grabUV.w;
				projUV += drops.xy * _Distortion * fade;
		
				//LONG RAIN
				float halfShift = fwidth(v) / 2;
				float lowEdge = 0.5 - halfShift;
				float upEdge = 0.5 + halfShift;				
				float stepAA = (v - lowEdge) / (upEdge - lowEdge);
				stepAA = saturate(stepAA);
				//v = stepAA;
				//v = v / fwidth(v);
				projUV = float2(projUV + 0.012*(float2(abs(ddy(v))- 0.05 * v, abs(ddy(v))- 0.05*v )));
				projUV = float2(projUV + 0.012*(float2((ddy(v)) - 0.05 * v, (ddy(v)) - 0.05*v)));
				projUV = float2(projUV + 0.012*(float2(abs(ddy(v)) - 0.05 * v, (ddy(v)) - 0.05*v)));
				projUV = float2(projUV + 0.002*(float2((ddy(v)) - 0.05 * v, abs(ddy(v)) - 0.05*v)));
				
				projUV = float2(projUV + 0.002*(float2(((v)) - 0.05 * v, ((v)) - 0.05*v)));
				projUV = float2(projUV + 0.003*(float2((ddy(v)) - 0.05 * v, ((v)) - 0.05*v)));
				//projUV = float2(projUV + 0.004*(float2(((v)) - 0.05 * v, abs((v)) - 0.05*v)));

			//	projUV = smoothstep(projUV,  0.112*(float2(abs(ddy(v)) - 0.05 * v, abs(ddy(v)) - 0.05*v)), 0.5);
		//		projUV += 0.2*(float2(ddx(v*1), ddy(v * 1)) ) * drops.w;// n * dropPosition;
				//projUV = float2(projUV + 0.005*(float2(ddx(v), ddy(v)) + float2(ddx(v2), ddy(v2)))); //ADD PREVIOUS RAIN SYSTEM - SMALL DROPS
		//		//projUV = float2(projUV + 1.014*(float2(ddx(v+1.1), ddy(v + 1.1)) + float2(ddx(v2 + 0.1), ddy(v2 + 0.1))));
		//		projUV += 0.05*(float2(ddx(v*0.4), ddy(v * 0.4)) + float2(ddx(v2 * 0.4), ddy(v2 * 0.4))) * drops.w;// n * dropPosition;

				blur *= 0.01;
				const float numSamples = 32;
				float a = N21(i.uv) * 6.2831;
				for (float j = 0; j < numSamples; j++) {

					float2 offs = float2(sin(a), cos(a))*blur;
					float  d = frac(sin((j + 1)*546.0)*5424.0);
					d = sqrt(d);
					offs *= d;
					
					//HDRP
					//col += tex2D(_MainTex, projUV + offs * eraser);//      _GrabTexture, projUV+offs * eraser);
					//_CameraOpaqueTexture
					//half2 offset = i.normal.xz  * _GrabTexture_TexelSize.xy * _Distortion * 1000;
					//half4 grabUV = half4(offset * i.grabUV.z + i.grabUV.xy, i.grabUV.zw);
					float2 samplingPositionNDC = float4(projUV + offs * eraser, 0, 0).xy;// float4(grabUV.xy / grabUV.w, 0, 0).xy;
						//float4 refraction = float4(SampleSceneColor(float2(samplingPositionNDC.x, clamp(samplingPositionNDC.y, 0, 1))),1);
						//float4 refraction = float4(SAMPLE_TEXTURE2D_LOD(_ColorPyramidTexture, s_trilinear_clamp_sampler, //URP v0.1
						//	float4(samplingPositionNDC.x, clamp(samplingPositionNDC.y, 0, 1), 0, 0) * 1 + float4(0, 0, 0, 0), 0).rgb, 1);
					float4 refraction = float4(SampleSceneColor(float2(samplingPositionNDC.x, clamp(samplingPositionNDC.y, 0, 1))), 1);
					col += refraction;				
						
					//MASKED
					

					a++;
				}
				//col = tex2D(_GrabTexture, projUV);
				col /= numSamples;
			
				col += tex2D(_MainTex, i.uv2 * mainTexTilingOffset.xy + mainTexTilingOffset.zw + 0) * maskPower;


				return col;
																
				//return fixed4(fragColor, 1);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}