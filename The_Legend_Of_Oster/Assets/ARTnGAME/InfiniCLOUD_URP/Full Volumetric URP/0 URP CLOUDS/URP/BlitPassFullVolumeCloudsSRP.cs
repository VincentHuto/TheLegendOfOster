using Artngame.SKYMASTER;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the destination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    internal class BlitPassFullVolumeCloudsSRP : UnityEngine.Rendering.Universal.ScriptableRenderPass
    {
        //v0.4  - Unity 2020.1
#if UNITY_2020_2_OR_NEWER
        public BlitFullVolumeCloudsSRP.BlitSettings settings;
        UnityEngine.Rendering.Universal.RenderTargetHandle _handle;
        public override void OnCameraSetup(CommandBuffer cmd, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            _handle.Init(settings.textureId);
            destination = (settings.destination == BlitFullVolumeCloudsSRP.Target.Color)
                ? UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget
                : _handle;

            var renderer = renderingData.cameraData.renderer;
            source = renderer.cameraColorTarget;
        }
#endif
        //v0.6
        public Vector3 cloudDistanceParams = new Vector3(0,0,1);
        public float controlBackAlphaPower = 1;
        public float controlCloudAlphaPower = 0.001f;
        public Vector4 controlCloudEdgeA = new Vector4(1, 1, 1, 1);
        public float controlCloudEdgeOffset = 1;
        public float depthDilation = 1;
        public bool enabledTemporalAA = false;
        public float TemporalResponse = 1;
        public float TemporalGain = 1;

        //v0.5
        public Vector4 YCutHeightDepthScale = new Vector4(0,1,0,1);

        //v0.4
        //PASS same shader time in clouds and shadows
        public bool useGlobalTime = true; //_Time since level load (t/20, t, t*2, t*3)
        public Material shadowsMaterial;
        public bool updateShadows = false;
        //Adjust for Unity 2020.1
        //public bool unity2020 = false; //apply Unity 2020.1 specific changes
        //Add posterize effect
        public Vector4 specialFX = new Vector4(0, 0, 0, 0); //posterizeClouds = 0.0f;

        public Vector4 raysResolution = new Vector4(1, 1, 1, 1);
        public Vector4 rayShadowing = new Vector4(1, 1, 1, 1);

        public Texture2D WeatherTexture;
        public float cameraScale = 1; //use 80 for correct cloud scaling in relation to land
        public float extendFarPlaneAboveClouds = 1;

        public int cloudChoice = 0;
        /////// FULL VOLUMETRIC CLOUDS
        [Tooltip("Fog top Y coordinate")]
        public float height = 1.0f;
        [Tooltip("Distance fog is based on radial distance from camera when checked")]
        public bool useRadialDistance = false;
        //v0.3
        public int scatterOn = 1;
        public int sunRaysOn = 1;
        public float zeroCountSteps = 0;
        public int sunShaftSteps = 5;

        //v0.1
        public int renderInFront = 0;

        public enum RandomJitter
        {
            Off,
            Random,
            BlueNoise
        }

        [HeaderAttribute("Debugging")]
        public bool debugNoLowFreqNoise = false;
        public bool debugNoHighFreqNoise = false;
        public bool debugNoCurlNoise = false;

        [HeaderAttribute("Performance")]
        [Range(1, 256)]
        public int steps = 128;
        public bool adjustDensity = true;
        public AnimationCurve stepDensityAdjustmentCurve = new AnimationCurve(new Keyframe(0.0f, 3.019f), new Keyframe(0.25f, 1.233f), new Keyframe(0.5f, 1.0f), new Keyframe(1.0f, 0.892f));
        public bool allowFlyingInClouds = false;
        [Range(1, 8)]
        public int downSample = 1;
        public Texture2D blueNoiseTexture;
        public RandomJitter randomJitterNoise = RandomJitter.BlueNoise;
        public bool temporalAntiAliasing = true;

        [HeaderAttribute("Cloud modeling")]
        public Gradient gradientLow;
        public Gradient gradientMed;
        public Gradient gradientHigh;
        public Texture2D curlNoise;
        public TextAsset lowFreqNoise;
        public TextAsset highFreqNoise;
        public float startHeight = 1500.0f;
        public float thickness = 4000.0f;
        public float planetSize = 35000.0f;
        public Vector3 planetZeroCoordinate = new Vector3(0.0f, 0.0f, 0.0f);
        [Range(0.0f, 1.0f)]
        public float scale = 0.3f;
        //
        public float noiseCuttoff=1;
        [Range(0.0f, 32.0f)]
        public float detailScale = 13.9f;
        [Range(0.0f, 1.0f)]
        public float lowFreqMin = 0.366f;
        [Range(0.0f, 1.0f)]
        public float lowFreqMax = 0.8f;
        [Range(0.0f, 1.0f)]
        public float highFreqModifier = 0.21f;
        [Range(0.0f, 10.0f)]
        public float curlDistortScale = 7.44f;
        [Range(0.0f, 1000.0f)]
        public float curlDistortAmount = 407.0f;
        [Range(0.0f, 1.0f)]
        public float weatherScale = 0.1f;
        [Range(0.0f, 2.0f)]
        public float coverage = 0.92f;
        [Range(0.0f, 2.0f)]
        public float cloudSampleMultiplier = 1.0f;

        [HeaderAttribute("High altitude clouds")]
        public Texture2D cloudsHighTexture;
        [Range(0.0f, 2.0f)]
        public float coverageHigh = 1.0f;
        [Range(0.0f, 2.0f)]
        public float highCoverageScale = 1.0f;
        [Range(0.0f, 1.0f)]
        public float highCloudsScale = 0.5f;

        [HeaderAttribute("Cloud Lighting")]
        public Light sunLight;
        public Color cloudBaseColor = new Color32(199, 220, 255, 255);
        public Color cloudTopColor = new Color32(255, 255, 255, 255);
        [Range(0.0f, 1.0f)]
        public float ambientLightFactor = 0.551f;
        [Range(0.0f, 5f)]//1.5
        public float sunLightFactor = 0.79f;
        public Color highSunColor = new Color32(255, 252, 210, 255);
        public Color lowSunColor = new Color32(255, 174, 0, 255);
        [Range(0.0f, 1.0f)]
        public float henyeyGreensteinGForward = 0.4f;
        [Range(0.0f, 1.0f)]
        public float henyeyGreensteinGBackward = 0.179f;
        [Range(0.0f, 200.0f)]
        public float lightStepLength = 64.0f;
        [Range(0.0f, 1.0f)]
        public float lightConeRadius = 0.4f;
        public bool randomUnitSphere = true;
        [Range(0.0f, 4.0f)]
        public float density = 1.0f;
        public bool aLotMoreLightSamples = false;

        [HeaderAttribute("Animating")]
        public float globalMultiplier = 1.0f;
        public float windSpeed = 15.9f;
        public float windDirection = -22.4f;
        public float coverageWindSpeed = 25.0f;
        public float coverageWindDirection = 5.0f;
        public float highCloudsWindSpeed = 49.2f;
        public float highCloudsWindDirection = 77.8f;

        public Vector3 _windOffset;
        public Vector2 _coverageWindOffset;
        public Vector2 _highCloudsWindOffset;
        public Vector3 _windDirectionVector;
        public float _multipliedWindSpeed;

        private Texture3D _cloudShapeTexture;
        private Texture3D _cloudDetailTexture;
        //private CloudTemporalAntiAliasing _temporalAntiAliasing;
        private Vector4 gradientToVector4(Gradient gradient)
        {
            if (gradient.colorKeys.Length != 4)
            {
                return new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            }
            float x = gradient.colorKeys[0].time;
            float y = gradient.colorKeys[1].time;
            float z = gradient.colorKeys[2].time;
            float w = gradient.colorKeys[3].time;
            return new Vector4(x, y, z, w);
        }
        /////// END FULL VOLUMETRIC CLOUDS







        public float farCamDistFactor = 1;
        ////// VOLUME CLOUDS 
        public Texture2D colourPalette;
        public bool Made_texture = false;
        public Gradient DistGradient = new Gradient();
        public Vector2 GradientBounds = Vector2.zero;
        //v4.8.6
        public bool adjustNightLigting = true;
        public float backShadeNight = 0.5f; //use this at night for more dense clouds
        public float turbidityNight = 2f;
        public float extinctionNight = 0.01f;
        public float shift_dawn = 0; //add shift to when cloud lighting changes vs the TOD of sky master
        public float shift_dusk = 0;
        //v4.8.4
        //public bool adjustNightLigting = true;
        public Vector3 groundColorNight = new Vector3(0.5f, 0.5f, 0.5f);
        public float scatterNight = 0.12f; //use this at night
        public float reflectFogHeight = 1;
        //v2.1.20
        public bool WebGL = false;
        //v2.1.19
        public bool fastest = false;
        public Light localLight;
        public float localLightFalloff = 2;
        public float localLightIntensity = 1;
        public float localLightPosScaling = 1;//v0.2 URP
        public float currentLocalLightIntensity = 0;
        public Vector3 _SkyTint = new Vector3(0.5f, 0.5f, 0.5f);
        public float _AtmosphereThickness = 1.0f;
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        ////////// CLOUDS
        //public bool useRadialDistance = false;
        public bool isForReflections = false;
        //v4.8
        public float _invertX = 0;
        public float _invertRay = 1;
        public Vector3 _WorldSpaceCameraPosC;
        public float varianceAltitude1 = 0;
        //v4.1f
        public float _mobileFactor=1;
        public float _alphaFactor= 0.96f;
        //v3.5.3
        public Texture2D _InteractTexture;
        public Vector4 _InteractTexturePos;
        public Vector4 _InteractTextureAtr;
        public Vector4 _InteractTextureOffset; //v4.0
        public float _NearZCutoff=-2;
        public float _HorizonYAdjust=0;
        public float _HorizonZAdjust=0;
        public float _FadeThreshold=0;
        //v3.5 clouds	
        public float _BackShade;
        public float _UndersideCurveFactor;
        public Matrix4x4 _WorldClip;
        public float _SampleCount0 = 2;
        public float _SampleCount1 = 3;
        public int _SampleCountL = 4;
        public Texture3D _NoiseTex1;
        public Texture3D _NoiseTex2;
        public float _NoiseFreq1 = 3.1f;
        public float _NoiseFreq2 = 35.1f;
        public float _NoiseAmp1 = 5;
        public float _NoiseAmp2 = 1;
        public float _NoiseBias = -0.2f;
        public Vector3 _Scroll1 = new Vector3(0.01f, 0.08f, 0.06f);
        public Vector3 _Scroll2 = new Vector3(0.01f, 0.05f, 0.03f);
        public float _Altitude0 = 1500;
        public float _Altitude1 = 3500;
        public float _FarDist = 30000;
        public float _Scatter = 0.008f;
        public float _HGCoeff = 0.5f;
        public float _Extinct = 0.01f;
        public float _SunSize = 0.04f;
        public Vector3 _GroundColor = new Vector3(0.8f, 0.8f, 0.8f); //v4.0
        public float _ExposureUnder=3; //v4.0
        public float frameFraction = 0;
        //v2.1.19
       // public bool _fastest = false;
        public Vector4 _LocalLightPos;
        public Vector4 _LocalLightColor;
        ///////// END CLOUDS
        ////// END VOLUME CLOUDS

        //FOG URP /////////////
        //FOG URP /////////////
        //FOG URP /////////////
        //public float blend =  0.5f;
        public Color _FogColor = Color.white / 2;
        //fog params
        public Texture2D noiseTexture;
        public float _startDistance = 30f;
        public float _fogHeight = 0.75f;
        public float _fogDensity = 1f;
        public float _cameraRoll = 0.0f;
        public Vector4 _cameraDiff = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        public float _cameraTiltSign = 1;
        public float heightDensity = 1;
        public float noiseDensity = 1;
        public float noiseScale = 1;
        public float noiseThickness = 1;
        public Vector3 noiseSpeed = new Vector4(1f, 1f, 1f);
        public float occlusionDrop = 1f;
        public float occlusionExp = 1f;
        public int noise3D = 1;
        public float startDistance = 1;
        public float luminance = 1;
        public float lumFac = 1;
        public float ScatterFac = 1;
        public float TurbFac = 1;
        public float HorizFac = 1;
        public float turbidity = 1;
        public float reileigh = 1;
        public float mieCoefficient = 1;
        public float mieDirectionalG = 1;
        public float bias = 1;
        public float contrast = 1;
        public Color TintColor = new Color(1, 1, 1, 1);
        public Vector3 TintColorK = new Vector3(0, 0, 0);
        public Vector3 TintColorL = new Vector3(0, 0, 0);
        public Vector4 Sun = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        public Vector4 SunFOG = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        public bool FogSky = true;
        public float ClearSkyFac = 1f;
        public Vector4 PointL = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        public Vector4 PointLParams = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

        bool _useRadialDistance = false;
        bool _fadeToSkybox = true;

        bool allowHDR = false;
        //END FOG URP //////////////////
        //END FOG URP //////////////////
        //END FOG URP //////////////////


        //SUN SHAFTS         
        public BlitFullVolumeCloudsSRP.BlitSettings.SunShaftsResolution resolution = BlitFullVolumeCloudsSRP.BlitSettings.SunShaftsResolution.Normal;
        public BlitFullVolumeCloudsSRP.BlitSettings.ShaftsScreenBlendMode screenBlendMode = BlitFullVolumeCloudsSRP.BlitSettings.ShaftsScreenBlendMode.Screen;
        public Vector3 sunTransform = new Vector3(0f, 0f, 0f); // Transform sunTransform;
        public int radialBlurIterations = 2;
        public Color sunColor = Color.white;
        public Color sunThreshold = new Color(0.87f, 0.74f, 0.65f);
        public float sunShaftBlurRadius = 2.5f;
        public float sunShaftIntensity = 1.15f;
        public float maxRadius = 0.75f;
        public bool useDepthTexture = true;
        public float blend = 0.5f;
                
        public enum RenderTarget
        {
            Color,
            RenderTexture,
        }
        public bool inheritFromController = true;

        public bool enableFog = false;//v0.1 off by default

        public Material blitMaterial = null;
        //public Material blitMaterialFOG = null;
        public int blitShaderPassIndex = 0;
        public FilterMode filterMode { get; set; }

        private RenderTargetIdentifier source { get; set; }
        private UnityEngine.Rendering.Universal.RenderTargetHandle destination { get; set; }

        UnityEngine.Rendering.Universal.RenderTargetHandle m_TemporaryColorTexture;
        string m_ProfilerTag;


        //SUN SHAFTS
        RenderTexture lrColorB;
        UnityEngine.Rendering.Universal.RenderTargetHandle lrDepthBuffer;

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public BlitPassFullVolumeCloudsSRP(UnityEngine.Rendering.Universal.RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag, BlitFullVolumeCloudsSRP.BlitSettings settings)
        {
            this.enableFog = settings.enableFog;

            this.inheritFromController = settings.inheritFromController;
            this.renderPassEvent = renderPassEvent;
            this.blitMaterial = blitMaterial;
            this.blitShaderPassIndex = blitShaderPassIndex;
            m_ProfilerTag = tag;
            m_TemporaryColorTexture.Init("_TemporaryColorTexture");

            //SUN SHAFTS
            this.resolution = settings.resolution;
            this.screenBlendMode = settings.screenBlendMode;
            this.sunTransform = settings.sunTransform;
            this.radialBlurIterations = settings.radialBlurIterations;
            this.sunColor = settings.sunColor;
            this.sunThreshold = settings.sunThreshold;
            this.sunShaftBlurRadius = settings.sunShaftBlurRadius;
            this.sunShaftIntensity = settings.sunShaftIntensity;
            this.maxRadius = settings.maxRadius;
            this.useDepthTexture = settings.useDepthTexture;
            this.blend = settings.blend;

            ////// VOLUME FOG URP /////////////////
            //FOG URP /////////////
            //FOG URP /////////////
            //FOG URP /////////////
            //this.blend =  0.5f;
            this._FogColor = settings._FogColor;
            //fog params
            this.noiseTexture = settings.noiseTexture;
            this._startDistance = settings._startDistance;
            this._fogHeight = settings._fogHeight;
            this._fogDensity = settings._fogDensity;
            this._cameraRoll = settings._cameraRoll;
            this._cameraDiff = settings._cameraDiff;
            this._cameraTiltSign = settings._cameraTiltSign;
            this.heightDensity = settings.heightDensity;
            this.noiseDensity = settings.noiseDensity;
            this.noiseScale = settings.noiseScale;
            this.noiseThickness = settings.noiseThickness;
            this.noiseSpeed = settings.noiseSpeed;
            this.occlusionDrop = settings.occlusionDrop;
            this.occlusionExp = settings.occlusionExp;
            this.noise3D = settings.noise3D;
            this.startDistance = settings.startDistance;
            this.luminance = settings.luminance;
            this.lumFac = settings.lumFac;
            this.ScatterFac = settings.ScatterFac;
            this.TurbFac = settings.TurbFac;
            this.HorizFac = settings.HorizFac;
            this.turbidity = settings.turbidity;
            this.reileigh = settings.reileigh;
            this.mieCoefficient = settings.mieCoefficient;
            this.mieDirectionalG = settings.mieDirectionalG;
            this.bias = settings.bias;
            this.contrast = settings.contrast;
            this.TintColor = settings.TintColor;
            this.TintColorK = settings.TintColorK;
            this.TintColorL = settings.TintColorL;
            this.Sun = settings.Sun;
            this.FogSky = settings.FogSky;
            this.ClearSkyFac = settings.ClearSkyFac;
            this.PointL = settings.PointL;
            this.PointLParams = settings.PointLParams;
            this._useRadialDistance = settings._useRadialDistance;
            this._fadeToSkybox = settings._fadeToSkybox;
            //END FOG URP //////////////////
            //END FOG URP //////////////////           
            //END FOG URP //////////////////
            ////// END VOLUME FOG URP /////////////////

            //CLOUDS
            isForReflections = settings.isForReflections;

        }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
        public void Setup(RenderTargetIdentifier source, UnityEngine.Rendering.Universal.RenderTargetHandle destination)
        {
            this.source = source;
            this.destination = destination;
        }
        
        connectSuntoFullVolumeCloudsURP connector;

        Camera currentCamera;
        float prevDownscaleFactor;//v0.1
        float prevlowerRefrReflResFactor;

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {

            //Debug.Log(renderingData.cameraData.camera.gameObject.name);
            connector = renderingData.cameraData.camera.GetComponent<connectSuntoFullVolumeCloudsURP>();
           // currentCamera = Camera.main;// renderingData.cameraData.camera;
            currentCamera =  renderingData.cameraData.camera;
            //context.SetupCameraProperties.
            //if (connector != null)
            //{
            //Debug.Log(connector.gameObject.name);
            //}

            //grab settings if script on scene camera
            if (connector == null)
            {
                connector = renderingData.cameraData.camera.gameObject.GetComponent<connectSuntoFullVolumeCloudsURP>();
                
                if (connector == null && Camera.main != null)
                {
                    connector = Camera.main.GetComponent<connectSuntoFullVolumeCloudsURP>(); //v0.1
                }
            }

            //URP
            if (_needsReset) ResetResources();

            prevDownscaleFactor = downScaleFactor;//v0.1 - check what scale was before get from connector
            prevlowerRefrReflResFactor = lowerRefrReflResFactor;

            //Debug.Log(Camera.main.GetComponent<connectSuntoVolumeFogURP>().sun.transform.position);
            if (inheritFromController && connector != null)
            {
                this.enableFog = connector.enableFog;


                cloudChoice = connector.cloudChoice;
                //////////////// FULL VOLUMETRIC CLOUDS

                //v0.6
                cloudDistanceParams = connector.cloudDistanceParams;
                controlBackAlphaPower = connector.controlBackAlphaPower;
                controlCloudAlphaPower = connector.controlCloudAlphaPower;
                controlCloudEdgeA = connector.controlCloudEdgeA;
                controlCloudEdgeOffset = connector.controlCloudEdgeOffset;
                depthDilation = connector.depthDilation;
                enabledTemporalAA = connector.enabledTemporalAA;
                TemporalResponse = connector.TemporalResponse;
                TemporalGain = connector.TemporalGain;

                //v0.5
                YCutHeightDepthScale = connector.YCutHeightDepthScale;

                //v0.4
                useGlobalTime = connector.useGlobalTime;
                shadowsMaterial = connector.shadowsMaterial;
                updateShadows = connector.updateShadows;
                //unity2020 = connector.unity2020;
                specialFX = connector.specialFX;

                WeatherTexture = connector.WeatherTexture;
                cameraScale = connector.cameraScale;
                extendFarPlaneAboveClouds = connector.extendFarPlaneAboveClouds;

                height = connector.height;
                useRadialDistance = connector.useRadialDistance;
                //v0.3
                scatterOn = connector.scatterOn;
                sunRaysOn = connector.sunRaysOn;
                zeroCountSteps = connector.zeroCountSteps;
                sunShaftSteps = connector.sunShaftSteps;

                //v0.1
                renderInFront = connector.renderInFront ;


                debugNoLowFreqNoise = connector.debugNoLowFreqNoise;
                debugNoHighFreqNoise = connector.debugNoHighFreqNoise ;
                debugNoCurlNoise = connector.debugNoCurlNoise;


                steps = connector.steps;
                adjustDensity = connector.adjustDensity;
                stepDensityAdjustmentCurve = connector.stepDensityAdjustmentCurve ;
                allowFlyingInClouds = connector.allowFlyingInClouds ;

                downSample = connector.downSample ;
                blueNoiseTexture = connector.blueNoiseTexture;

                if(connector.randomJitterNoise == connectSuntoFullVolumeCloudsURP.RandomJitterChoice.BlueNoise)
                {
                    randomJitterNoise = RandomJitter.BlueNoise;
                }
                if (connector.randomJitterNoise == connectSuntoFullVolumeCloudsURP.RandomJitterChoice.Off)
                {
                    randomJitterNoise = RandomJitter.Off;
                }
                if (connector.randomJitterNoise == connectSuntoFullVolumeCloudsURP.RandomJitterChoice.Random)
                {
                    randomJitterNoise = RandomJitter.Random;
                }
                // randomJitterNoise = connector.randomJitterNoise ;


                temporalAntiAliasing = connector.temporalAntiAliasing ;
                
                gradientLow = connector.gradientLow;
                gradientMed = connector.gradientMed;
                gradientHigh = connector.gradientHigh;
                curlNoise = connector.curlNoise;
                lowFreqNoise = connector.lowFreqNoise;
                highFreqNoise = connector.highFreqNoise;
                startHeight = connector.startHeight ;
                thickness= connector.thickness;
                planetSize = connector.planetSize ;
                planetZeroCoordinate = connector.planetZeroCoordinate;

                scale = connector.scale;
                noiseCuttoff = connector.noiseCuttoff;

                detailScale = connector.detailScale ;

                lowFreqMin = connector.lowFreqMin ;

                lowFreqMax = connector.lowFreqMax ;

                highFreqModifier = connector.highFreqModifier ;

                curlDistortScale = connector.curlDistortScale ;

                curlDistortScale = connector.curlDistortScale;

                weatherScale = connector.weatherScale;

                coverage = connector.coverage ;

                cloudSampleMultiplier = connector.cloudSampleMultiplier ;
                
                cloudsHighTexture = connector.cloudsHighTexture;

                coverageHigh = connector.coverageHigh;

                highCoverageScale = connector.highCoverageScale ;

                highCloudsScale = connector.highCloudsScale ;
                
                sunLight = connector.sunLight;
                cloudBaseColor = connector.cloudBaseColor ;
                cloudTopColor = connector.cloudTopColor;

                ambientLightFactor = connector.ambientLightFactor;

                sunLightFactor = connector.sunLightFactor ;
                highSunColor = connector.highSunColor ;
                lowSunColor = connector.lowSunColor;

                henyeyGreensteinGForward = connector.henyeyGreensteinGForward ;

                henyeyGreensteinGBackward = connector.henyeyGreensteinGBackward ;

                lightStepLength = connector.lightStepLength;

                lightConeRadius = connector.lightConeRadius ;
                randomUnitSphere = connector.randomUnitSphere ;

                density = connector.density;
                aLotMoreLightSamples = connector.aLotMoreLightSamples ;
                
                globalMultiplier = connector.globalMultiplier ;
                windSpeed = connector.windSpeed ;
                windDirection = connector.windDirection;
                coverageWindSpeed = connector.coverageWindSpeed ;
                coverageWindDirection = connector.coverageWindDirection;
                highCloudsWindSpeed = connector.highCloudsWindSpeed ;
                highCloudsWindDirection=connector.highCloudsWindDirection;

                _windOffset= connector._windOffset;
                _coverageWindOffset= connector._coverageWindOffset;
                _highCloudsWindOffset= connector._highCloudsWindOffset;
                _windDirectionVector= connector._windDirectionVector;
                _multipliedWindSpeed= connector._multipliedWindSpeed;

                raysResolution = connector.raysResolution;
                rayShadowing = connector.rayShadowing;
                /////////////// END FULL VOLUMETRIC CLOUDS







                this.sunTransform = new Vector3(connector.Sun.x, connector.Sun.y, connector.Sun.z);// connector.sun.transform.position;
                this.screenBlendMode = connector.screenBlendMode;
                //public Vector3 sunTransform = new Vector3(0f, 0f, 0f); 
                this.radialBlurIterations = connector.radialBlurIterations;
                this.sunColor = connector.sunColor;
                this.sunThreshold = connector.sunThreshold;
                this.sunShaftBlurRadius = connector.sunShaftBlurRadius;
                this.sunShaftIntensity = connector.sunShaftIntensity;
                this.maxRadius = connector.maxRadius;
                this.useDepthTexture = connector.useDepthTexture;

                ////// VOLUME FOG URP /////////////////
                //FOG URP /////////////
                //FOG URP /////////////
                //FOG URP /////////////
                //this.blend =  0.5f;
                this._FogColor = connector._FogColor;
                //fog params
                this.noiseTexture = connector.noiseTexture;
                this._startDistance = connector._startDistance;
                this._fogHeight = connector._fogHeight;
                this._fogDensity = connector._fogDensity;
                this._cameraRoll = connector._cameraRoll;
                this._cameraDiff = connector._cameraDiff;
                this._cameraTiltSign = connector._cameraTiltSign;
                this.heightDensity = connector.heightDensity;
                this.noiseDensity = connector.noiseDensity;
                this.noiseScale = connector.noiseScale;
                this.noiseThickness = connector.noiseThickness;
                this.noiseSpeed = connector.noiseSpeed;
                this.occlusionDrop = connector.occlusionDrop;
                this.occlusionExp = connector.occlusionExp;
                this.noise3D = connector.noise3D;
                this.startDistance = connector.startDistance;
                this.luminance = connector.luminance;
                this.lumFac = connector.lumFac;
                this.ScatterFac = connector.ScatterFac;
                this.TurbFac = connector.TurbFac;
                this.HorizFac = connector.HorizFac;
                this.turbidity = connector.turbidity;
                this.reileigh = connector.reileigh;
                this.mieCoefficient = connector.mieCoefficient;
                this.mieDirectionalG = connector.mieDirectionalG;
                this.bias = connector.bias;
                this.contrast = connector.contrast;
                this.TintColor = connector.TintColor;
                this.TintColorK = connector.TintColorK;
                this.TintColorL = connector.TintColorL;
                this.Sun = connector.Sun;

                this.SunFOG = connector.SunFOG;

                this.FogSky = connector.FogSky;
                this.ClearSkyFac = connector.ClearSkyFac;
                this.PointL = connector.PointL;
                this.PointLParams = connector.PointLParams;
                this._useRadialDistance = connector._useRadialDistance;
                this._fadeToSkybox = connector._fadeToSkybox;

                this.allowHDR = connector.allowHDR;
                //END FOG URP //////////////////
                //END FOG URP //////////////////
                //END FOG URP //////////////////
                ////// END VOLUME FOG URP /////////////////


                farCamDistFactor = connector.farCamDistFactor;

                ////// VOLUME CLOUDS
                blendBackground = connector.blendBackground;
                backgroundCam = connector.backgroundCam;
                backgroundMat = connector.backgroundMat;

                DistGradient = connector.DistGradient;
                GradientBounds = connector.GradientBounds;
                //v4.8.6
                adjustNightLigting = connector.adjustNightLigting;
                backShadeNight = connector.backShadeNight; //use this at night for more dense clouds
                turbidityNight = connector.turbidityNight;
                extinctionNight = connector.extinctionNight;
                shift_dawn = connector.shift_dawn; //add shift to when cloud lighting changes vs the TOD of sky master
                shift_dusk = connector.shift_dusk;
                //v4.8.4
                //public bool adjustNightLigting = true;
                groundColorNight = connector.groundColorNight;
                scatterNight = connector.scatterNight; //use this at night
                reflectFogHeight = connector.reflectFogHeight;
                //v2.1.20
                WebGL = connector.WebGL;
                //v2.1.19
                this.fastest = connector.fastest; //Debug.Log(fastest);
                localLight= connector.localLight;
                localLightFalloff = connector.localLightFalloff;
                //public float localLightIntensity = 1;
                currentLocalLightIntensity = connector.currentLocalLightIntensity;

                localLightPosScaling = connector.localLightPosScaling;//v0.2

                _SkyTint = connector._SkyTint;
                _AtmosphereThickness = connector._AtmosphereThickness;
                ////////// CLOUDS
     //           isForReflections = connector.isForReflections;
                //v4.8
                _invertX = connector._invertX;
                _invertRay = connector._invertRay;
                _WorldSpaceCameraPosC = connector._WorldSpaceCameraPosC;
                varianceAltitude1 = connector.varianceAltitude1;
                //v4.1f
                _mobileFactor= connector._mobileFactor;
                _alphaFactor= connector._alphaFactor;
                //v3.5.3
                _InteractTexture= connector._InteractTexture;
                _InteractTexturePos= connector._InteractTexturePos;
                _InteractTextureAtr= connector._InteractTextureAtr;
                _InteractTextureOffset= connector._InteractTextureOffset; //v4.0
                _NearZCutoff= connector._NearZCutoff;
                _HorizonYAdjust= connector._HorizonYAdjust;
                _HorizonZAdjust= connector._HorizonZAdjust;
                _FadeThreshold= connector._FadeThreshold;
                //v3.5 clouds	
                _BackShade = connector._BackShade;
                _UndersideCurveFactor = connector._UndersideCurveFactor;
                _WorldClip = connector._WorldClip;
                _SampleCount0 = connector._SampleCount0;
                _SampleCount1 = connector._SampleCount1;
                _SampleCountL = connector._SampleCountL;
                _NoiseTex1= connector._NoiseTex1;
                _NoiseTex2= connector._NoiseTex2;
                _NoiseFreq1 = connector._NoiseFreq1;
                _NoiseFreq2 = connector._NoiseFreq2;
                _NoiseAmp1 = connector._NoiseAmp1;
                _NoiseAmp2 = connector._NoiseAmp2;
                _NoiseBias = connector._NoiseBias;
                _Scroll1 = connector._Scroll1;
                _Scroll2 = connector._Scroll2;
                _Altitude0 = connector._Altitude0;
                _Altitude1 = connector._Altitude1;
                _FarDist = connector._FarDist;
                _Scatter = connector._Scatter;
                _HGCoeff = connector._HGCoeff;
                _Extinct = connector._Extinct;
                _SunSize= connector._SunSize;
                _GroundColor= connector._GroundColor; //v4.0
                _ExposureUnder = connector._ExposureUnder; //v4.0
                //frameFraction = connector.frameFraction;
                //v2.1.19
                //_fastest= connector._fastest;
                _LocalLightPos= connector._LocalLightPos;
                _LocalLightColor= connector._LocalLightColor;
                ///////// END CLOUDS
                splitPerFrames = connector.splitPerFrames; //v2.1.19
                cameraMotionCompensate = connector.cameraMotionCompensate;//v2.1.19    
                updateRate = connector.updateRate;
                // public int resolution = 256;
                downScaleFactor = connector.downScaleFactor;
                downScale = connector.downScale;
                _needsReset = connector._needsReset;
                if (!connector.autoReproject)
                {
                    enableReproject = connector.enableReproject;
                }
                autoReproject = connector.autoReproject;

                //v0.1
                lowerRefrReflResFactor = connector.lowerRefrReflResFactor;
        ////// END VOLUME CLOUDS
    }

            //v0.1 - after get connector, check if resolution matches, otherwise reset
            if (prevDownscaleFactor != downScaleFactor || lowerRefrReflResFactor != prevlowerRefrReflResFactor) ResetResources();

            //if still null, disable effect
            bool connectorFound = true;
            if (connector == null)
            {
                connectorFound = false;
            }

            if (enableFog && connectorFound)
            {
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDesc.depthBufferBits = 0;



                // CameraClearFlags previousFlag;
                //v2.1.15
                //Camera cam;
                //void OnPreRender()
                //{
                if (!fastest && downScale)
                {
                    if (cloudChoice == 0)
                    {
                        renderingData.cameraData.camera.backgroundColor = new Color(0, 0, 0, 0);
                        renderingData.cameraData.camera.clearFlags = CameraClearFlags.SolidColor;
                    }
                    //cam = GetComponent<Camera>(); //v4.2
                    //previousFlag = cam.clearFlags; //v2.1.20
                    //cam.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0);
                    //  cam.clearFlags = CameraClearFlags.SolidColor;
                }
                else {
                    renderingData.cameraData.camera.clearFlags = CameraClearFlags.Skybox;
                }
                //}




                // Can't read and write to same color target, create a temp render target to blit. 
                if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
                {
                    cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);
                    //RenderShafts(context, renderingData, cmd, opaqueDesc);

                    if (cloudChoice == 0)
                    {
                        RenderFog(context, renderingData, cmd, opaqueDesc);
                    }
                    else if(cloudChoice == 1)
                    {
                        RenderFullVolumetricClouds(context, renderingData, cmd, opaqueDesc);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);            
                cmd.ReleaseTemporaryRT(lrDepthBuffer.id);               
            }
        }    

        //SUN SHAFTS
        public void RenderShafts(ScriptableRenderContext context, UnityEngine.Rendering.Universal.RenderingData renderingData, CommandBuffer cmd, RenderTextureDescriptor opaqueDesc)
        {           
            opaqueDesc.depthBufferBits = 0;          
            Material sheetSHAFTS = blitMaterial;
           
            sheetSHAFTS.SetFloat("_Blend", blend);

            Camera camera = currentCamera;// Camera.main;
            // we actually need to check this every frame
            if (useDepthTexture)
            {               
                camera.depthTextureMode |= DepthTextureMode.Depth;
            }           

            Vector3 v = Vector3.one * 0.5f;
            if (sunTransform != Vector3.zero) {
                v = camera.WorldToViewportPoint(sunTransform);
            }
            else {
                v = new Vector3(0.5f, 0.5f, 0.0f);
            }
            
            //v0.1
            int rtW = opaqueDesc.width;
            int rtH = opaqueDesc.height;

            cmd.GetTemporaryRT(lrDepthBuffer.id, opaqueDesc, filterMode);

            sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(1.0f, 1.0f, 0.0f, 0.0f) * sunShaftBlurRadius);
            sheetSHAFTS.SetVector("_SunPosition", new Vector4(v.x, v.y, v.z, maxRadius));
            sheetSHAFTS.SetVector("_SunThreshold", sunThreshold);

            if (!useDepthTexture)
            {               
                var format = camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default; //v3.4.9
                RenderTexture tmpBuffer = RenderTexture.GetTemporary(rtW, rtH, 0, format);
                RenderTexture.active = tmpBuffer;
                GL.ClearWithSkybox(false, camera);
             
                sheetSHAFTS.SetTexture("_Skybox", tmpBuffer);               
                Blit(cmd, source, lrDepthBuffer.Identifier(), sheetSHAFTS, 3);

                RenderTexture.ReleaseTemporary(tmpBuffer);
            }
            else
            {               
                Blit(cmd, source, lrDepthBuffer.Identifier(), sheetSHAFTS, 2);
            }            

            Blit(cmd, source, m_TemporaryColorTexture.Identifier()); //KEEP BACKGROUND
           
            radialBlurIterations = Mathf.Clamp(radialBlurIterations, 1, 4);
            float ofs = sunShaftBlurRadius * (1.0f / 768.0f);

            sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            sheetSHAFTS.SetVector("_SunPosition", new Vector4(v.x, v.y, v.z, maxRadius));

            for (int it2 = 0; it2 < radialBlurIterations; it2++)
            {
                // each iteration takes 2 * 6 samples
                // we update _BlurRadius each time to cheaply get a very smooth look
                lrColorB = RenderTexture.GetTemporary(rtW, rtH, 0); 
                Blit(cmd, lrDepthBuffer.Identifier(), lrColorB, sheetSHAFTS, 1);
                cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
                ofs = sunShaftBlurRadius * (((it2 * 2.0f + 1.0f) * 6.0f)) / 768.0f;             
                sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f)); 
                cmd.GetTemporaryRT(lrDepthBuffer.id, opaqueDesc, filterMode);                
                Blit(cmd, lrColorB, lrDepthBuffer.Identifier(), sheetSHAFTS, 1);
                RenderTexture.ReleaseTemporary(lrColorB);  
                ofs = sunShaftBlurRadius * (((it2 * 2.0f + 2.0f) * 6.0f)) / 768.0f;              
                sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            }
            
            // put together:
            if (v.z >= 0.0f)
            {              
                sheetSHAFTS.SetVector("_SunColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, sunColor.a) * sunShaftIntensity);
            }
            else
            {
                sheetSHAFTS.SetVector("_SunColor", Vector4.zero); // no backprojection !
            }
          
            cmd.SetGlobalTexture("_ColorBuffer", lrDepthBuffer.Identifier());          
            Blit(cmd, m_TemporaryColorTexture.Identifier(), source, sheetSHAFTS, (screenBlendMode == BlitFullVolumeCloudsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);
           
            cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);           

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
           
            RenderTexture.ReleaseTemporary(lrColorB);
        }



        //OPTIMIZE
        int toggleCounter = 0;
        public int splitPerFrames = 1; //v2.1.19
        public bool cameraMotionCompensate = true;//v2.1.19
        RenderTexture _cloudBuffer;
        RenderTexture _cloudBufferP;
        RenderTexture _cloudBufferP1;
        public float updateRate = 0.3f;
       // public int resolution = 256;
        public float downScaleFactor = 2; //v0.1 lower by default
        public bool downScale = false;
        //RenderTexture _prevcloudBuffer;
        void ResetResources()
        {
            //if (_cloudBuffer) DestroyImmediate(_cloudBuffer);
            //if (_cloudBuffer) { _cloudBuffer.DiscardContents(); _cloudBuffer.Release(); }
            if (_cloudBuffer == null) { 
                _cloudBuffer = CreateBuffer();
            }
            // if (_cloudBufferP) { _cloudBufferP.DiscardContents(); _cloudBufferP.Release(); }
            if (_cloudBufferP == null)
            {
                _cloudBufferP = CreateBuffer();
            }
                // if (_cloudBufferP1) { _cloudBufferP1.DiscardContents(); _cloudBufferP1.Release(); }
                if (_cloudBufferP1 == null)
                {
                    _cloudBufferP1 = CreateBuffer();
                }

            //if (_cloudBufferP) DestroyImmediate(_cloudBufferP);
            //_cloudBufferP = CreateBuffer();

            //if (_cloudBufferP1) DestroyImmediate(_cloudBufferP1);
            //_cloudBufferP1 = CreateBuffer();

            ////v4.5
            //if (new1) DestroyImmediate(new1);
            //new1 = CreateBuffer();

            if (new1) { new1.DiscardContents();  new1.Release(); }
            new1 = CreateBuffer();

            _needsReset = false;
            if (connector != null)
            {
                connector._needsReset = false;
            }
        }
        void LateUpdate()
        {
            if (_needsReset) ResetResources();
        }
        void Reset()
        {
            _needsReset = true;
        }
        public bool _needsReset = true;
        Vector3 prevCameraRot;
        Vector3 prevCameraRotP;//v4.8.3 previous position when frame grabbed
        RenderTexture new1; //v4.5
        //v4.8.3
        int countCameraSteady = 0;
        //v4.8
        public bool enableReproject = false;
        public bool autoReproject = false;
        public float lowerRefrReflResFactor = 3;
        RenderTexture CreateBuffer()
        {
            //var width = (_columns + 1) * 2;
            //var height = _totalRows + 1;
            float lowerRefrRefl = 1; //v0.1
            //if (currentCamera.name.Contains("refract") || currentCamera.name.Contains("reflect") || currentCamera.name.Contains("Refl")
            //    || currentCamera.name.Contains("Refract"))

            if(isForReflections)
            {
                lowerRefrRefl = lowerRefrReflResFactor;
            }

            //if(currentCamera.gameObject.name == Camera.main.gameObject.name)
            // {
            //     lowerRefrRefl = 2;
            //}
            //Debug.Log(downScaleFactor);
            RenderTexture buffer = new RenderTexture((int)(1280 / (downScaleFactor* lowerRefrRefl)), (int)(720 / (downScaleFactor * lowerRefrRefl)), 0, RenderTextureFormat.ARGB32); //SM v4.0
            //buffer.hideFlags = HideFlags.DontSave;
            //buffer.hideFlags = HideFlags.None;
            buffer.filterMode = FilterMode.Bilinear; //FilterMode.Point; //v4.8.8
            buffer.wrapMode = TextureWrapMode.Repeat;          
            return buffer;
        }
        //END OPTIMIZE


        /////////////////////// FULL VOLUMETRIC CLOUDS /////////////////////////////////////

        public void RenderFullVolumetricClouds(ScriptableRenderContext context, UnityEngine.Rendering.Universal.RenderingData renderingData, CommandBuffer cmd, RenderTextureDescriptor opaqueDesc)
        //public override void Render(PostProcessRenderContext context)
        {
            Material CloudMaterial = blitMaterial;

            if (_cloudShapeTexture == null) // if shape texture is missing load it in
            {
                _cloudShapeTexture = TGALoader.load3DFromTGASlices(lowFreqNoise);
            }

            if (_cloudDetailTexture == null) // if detail texture is missing load it in
            {
                _cloudDetailTexture = TGALoader.load3DFromTGASlices(highFreqNoise);
            }
            Camera CurrentCamera = currentCamera;
            Vector3 cameraPos = CurrentCamera.transform.position;
            // sunLight.rotation.x 364 -> 339, 175 -> 201

            float sunLightFactorUpdated = sunLightFactor;
            float ambientLightFactorUpdated = ambientLightFactor;
            float sunAngle = sunLight.transform.eulerAngles.x;
            Color sunColor = highSunColor;
            float henyeyGreensteinGBackwardLerp = henyeyGreensteinGBackward;

            float noiseScale = 0.00001f * noiseCuttoff + scale * 0.0004f;

            if (sunAngle > 170.0f) // change sunlight color based on sun's height.
            {
                float gradient = Mathf.Max(0.0f, (sunAngle - 330.0f) / 30.0f);
                float gradient2 = gradient * gradient;
                sunLightFactorUpdated *= gradient;
                ambientLightFactorUpdated *= gradient;
                henyeyGreensteinGBackwardLerp *= gradient2 * gradient;
                ambientLightFactorUpdated = Mathf.Max(0.02f, ambientLightFactorUpdated);
                sunColor = Color.Lerp(lowSunColor, highSunColor, gradient2);
            }

            updateMaterialKeyword(debugNoLowFreqNoise, "DEBUG_NO_LOW_FREQ_NOISE", CloudMaterial);
            updateMaterialKeyword(debugNoHighFreqNoise, "DEBUG_NO_HIGH_FREQ_NOISE", CloudMaterial);
            updateMaterialKeyword(debugNoCurlNoise, "DEBUG_NO_CURL", CloudMaterial);
            updateMaterialKeyword(allowFlyingInClouds, "ALLOW_IN_CLOUDS", CloudMaterial);
            updateMaterialKeyword(randomUnitSphere, "RANDOM_UNIT_SPHERE", CloudMaterial);
            updateMaterialKeyword(aLotMoreLightSamples, "SLOW_LIGHTING", CloudMaterial);

            switch (randomJitterNoise)
            {
                case RandomJitter.Off:
                    updateMaterialKeyword(false, "RANDOM_JITTER_WHITE", CloudMaterial);
                    updateMaterialKeyword(false, "RANDOM_JITTER_BLUE", CloudMaterial);
                    break;
                case RandomJitter.Random:
                    updateMaterialKeyword(true, "RANDOM_JITTER_WHITE", CloudMaterial);
                    updateMaterialKeyword(false, "RANDOM_JITTER_BLUE", CloudMaterial);
                    break;
                case RandomJitter.BlueNoise:
                    updateMaterialKeyword(false, "RANDOM_JITTER_WHITE", CloudMaterial);
                    updateMaterialKeyword(true, "RANDOM_JITTER_BLUE", CloudMaterial);
                    break;
            }

            // send uniforms to shader
            CloudMaterial.SetVector("_SunDir", sunLight.transform ? (-sunLight.transform.forward).normalized : Vector3.up);
            //Debug.Log("_SunDir:" + (-sunLight.transform.forward).normalized);
            CloudMaterial.SetVector("_PlanetCenter", planetZeroCoordinate - new Vector3(0, planetSize, 0));
            CloudMaterial.SetVector("_ZeroPoint", planetZeroCoordinate);
            CloudMaterial.SetColor("_SunColor", sunColor);
            //CloudMaterial.SetColor("_SunColor", sunLight.color);

            //v0.5
            CloudMaterial.SetVector("YCutHeightDepthScale", YCutHeightDepthScale);

            //v0.4
            if (useGlobalTime)
            {
                CloudMaterial.SetFloat("globalTIME", Time.fixedTime);
            }
            else
            {
                CloudMaterial.SetFloat("globalTIME", 0.0f);
            }
            if (updateShadows && shadowsMaterial != null)
            {
                if (useGlobalTime)
                {
                    shadowsMaterial.SetFloat("globalTIME", Time.fixedTime);
                }
                else
                {
                    shadowsMaterial.SetFloat("globalTIME", 0.0f);
                }
                //shadowsMaterial.SetFloat("_WeatherScale", weatherScale * 0.00025f);
                //shadowsMaterial.SetFloat("_CoverageA", 1.0f - coverage);
                //shadowsMaterial.SetFloat("_WindSpeed", _multipliedWindSpeed);
                //shadowsMaterial.SetVector("_WindDirection", _windDirectionVector);
                //shadowsMaterial.SetVector("_WindOffset", _windOffset);
                //shadowsMaterial.SetVector("_CoverageWindOffset", _coverageWindOffset);
                //shadowsMaterial.SetVector("_HighCloudsWindOffset", _highCloudsWindOffset);
            }
            CloudMaterial.SetVector("specialFX", specialFX);


            CloudMaterial.SetTexture("_WeatherTexture", WeatherTexture);
            CloudMaterial.SetFloat("cameraScale", cameraScale);
            CloudMaterial.SetFloat("extendFarPlaneAboveClouds", extendFarPlaneAboveClouds);

            CloudMaterial.SetVector("raysResolution",raysResolution);
            CloudMaterial.SetVector("rayShadowing", rayShadowing);

            CloudMaterial.SetColor("_CloudBaseColor", cloudBaseColor);
            CloudMaterial.SetColor("_CloudTopColor", cloudTopColor);
            CloudMaterial.SetFloat("_AmbientLightFactor", ambientLightFactorUpdated);
            CloudMaterial.SetFloat("_SunLightFactor", sunLightFactorUpdated);
            //CloudMaterial.SetFloat("_AmbientLightFactor", sunLight.intensity * ambientLightFactor * 0.3f);
            //CloudMaterial.SetFloat("_SunLightFactor", sunLight.intensity * sunLightFactor);

            CloudMaterial.SetTexture("_ShapeTexture", _cloudShapeTexture);
            CloudMaterial.SetTexture("_DetailTexture", _cloudDetailTexture);
            CloudMaterial.SetTexture("_CurlNoise", curlNoise);
            CloudMaterial.SetTexture("_BlueNoise", blueNoiseTexture);
            CloudMaterial.SetVector("_Randomness", new Vector4(Random.value, Random.value, Random.value, Random.value));
            CloudMaterial.SetTexture("_AltoClouds", cloudsHighTexture);

            CloudMaterial.SetFloat("_CoverageHigh", 1.0f - coverageHigh);
            CloudMaterial.SetFloat("_CoverageHighScale", highCoverageScale * weatherScale * 0.001f);
            CloudMaterial.SetFloat("_HighCloudsScale", highCloudsScale * 0.002f);

            CloudMaterial.SetFloat("_CurlDistortAmount", 150.0f + curlDistortAmount);
            CloudMaterial.SetFloat("_CurlDistortScale", curlDistortScale * noiseScale);

            CloudMaterial.SetFloat("_LightConeRadius", lightConeRadius);
            CloudMaterial.SetFloat("_LightStepLength", lightStepLength);
            CloudMaterial.SetFloat("_SphereSize", planetSize);
            CloudMaterial.SetVector("_CloudHeightMinMax", new Vector2(startHeight, startHeight + thickness));
            CloudMaterial.SetFloat("_Thickness", thickness);
            CloudMaterial.SetFloat("_Scale", noiseScale);
            CloudMaterial.SetFloat("_DetailScale", detailScale * noiseScale);
            CloudMaterial.SetVector("_LowFreqMinMax", new Vector4(lowFreqMin, lowFreqMax));
            CloudMaterial.SetFloat("_HighFreqModifier", highFreqModifier);
            CloudMaterial.SetFloat("_WeatherScale", weatherScale * 0.00025f);
            CloudMaterial.SetFloat("_Coverage", 1.0f - coverage);
            CloudMaterial.SetFloat("_HenyeyGreensteinGForward", henyeyGreensteinGForward);
            CloudMaterial.SetFloat("_HenyeyGreensteinGBackward", -henyeyGreensteinGBackwardLerp);
            if (adjustDensity)
            {
                CloudMaterial.SetFloat("_SampleMultiplier", cloudSampleMultiplier * stepDensityAdjustmentCurve.Evaluate(steps / 256.0f));
            }
            else
            {
                CloudMaterial.SetFloat("_SampleMultiplier", cloudSampleMultiplier);
            }

            CloudMaterial.SetFloat("_Density", density);

            CloudMaterial.SetFloat("_WindSpeed", _multipliedWindSpeed);
            CloudMaterial.SetVector("_WindDirection", _windDirectionVector);
            CloudMaterial.SetVector("_WindOffset", _windOffset);
            CloudMaterial.SetVector("_CoverageWindOffset", _coverageWindOffset);
            CloudMaterial.SetVector("_HighCloudsWindOffset", _highCloudsWindOffset);

            CloudMaterial.SetVector("_Gradient1", gradientToVector4(gradientLow));
            CloudMaterial.SetVector("_Gradient2", gradientToVector4(gradientMed));
            CloudMaterial.SetVector("_Gradient3", gradientToVector4(gradientHigh));

            CloudMaterial.SetInt("_Steps", steps);
            CloudMaterial.SetInt("_renderInFront", renderInFront);//v0.1 choose to render in front of objects for reflections

            CloudMaterial.SetMatrix("_FrustumCornersES", GetFrustumCorners(CurrentCamera));
            CloudMaterial.SetMatrix("_CameraInvViewMatrix", CurrentCamera.cameraToWorldMatrix);
            CloudMaterial.SetVector("_CameraWS", cameraPos); //Debug.Log("cameraPos:" + cameraPos);
            CloudMaterial.SetFloat("_FarPlane", Camera.main.farClipPlane * 1);// 0.016f );////CurrentCamera.farClipPlane);

            //v0.2
            //v3.5.3			
            CloudMaterial.SetTexture("_InteractTexture", _InteractTexture);
            CloudMaterial.SetVector("_InteractTexturePos", _InteractTexturePos);
            CloudMaterial.SetVector("_InteractTextureAtr", _InteractTextureAtr);
            CloudMaterial.SetVector("_InteractTextureOffset", _InteractTextureOffset); //v4.0
            
            //////// SCATTER  
            CloudMaterial.SetInt("scatterOn", scatterOn);//v0.3
            CloudMaterial.SetInt("sunRaysOn", sunRaysOn);//v0.3
            CloudMaterial.SetFloat("zeroCountSteps", zeroCountSteps);//v0.3
            CloudMaterial.SetInt("sunShaftSteps", sunShaftSteps);//v0.3
                       
            float FdotC = CurrentCamera.transform.position.y - height;
            float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
          
            CloudMaterial.SetVector("_HeightParams", new Vector4(height, FdotC, paramK, heightDensity * 0.5f));
            CloudMaterial.SetVector("_DistanceParams", new Vector4(-Mathf.Max(startDistance, 0.0f), cloudDistanceParams.x, cloudDistanceParams.y, cloudDistanceParams.z)); //0, 0, 0));

            CloudMaterial.SetFloat("_Scatter", _Scatter);
            CloudMaterial.SetFloat("_HGCoeff", _HGCoeff);
            CloudMaterial.SetFloat("_Extinct", _Extinct);


            CloudMaterial.SetVector("_SkyTint", _SkyTint);
           
            //v3.5
            CloudMaterial.SetFloat("_BackShade", _BackShade);
          

            //v2.1.19
            if (localLight != null)
            {
                Vector3 localLightPos = localLight.transform.position;

                //v0.2 - scale light position
                if(localLightPosScaling != 1)
                {
                   // localLightPos = localLightPos + Camera.main.transform.position;
                }
                localLightPos = new Vector3(localLightPos.x * localLightPosScaling, localLightPos.y * localLightPosScaling, localLightPos.z * localLightPosScaling);

                //float intensity = Mathf.Pow(10, 3 + (localLightFalloff - 3) * 3);
                //  currentLocalLightIntensity = Mathf.Pow(10, 3 + (localLightFalloff - 3) * 3);
                //fogMaterial.SetVector ("_LocalLightPos", new Vector4 (localLightPos.x, localLightPos.y, localLightPos.z, localLight.intensity * localLightIntensity * intensity));
                CloudMaterial.SetVector("_LocalLightPos", new Vector4(localLightPos.x, localLightPos.y, localLightPos.z, localLight.intensity * localLightIntensity * currentLocalLightIntensity));
                CloudMaterial.SetVector("_LocalLightColor", new Vector4(localLight.color.r, localLight.color.g, localLight.color.b, localLightFalloff));
            }
            else
            {
                if (currentLocalLightIntensity > 0)
                {
                    currentLocalLightIntensity = 0;
                    //fogMaterial.SetVector ("_LocalLightPos", new Vector4 (localLightPos.x, localLightPos.y, localLightPos.z, localLight.intensity * localLightIntensity * intensity));
                    CloudMaterial.SetVector("_LocalLightColor", Vector4.zero);
                }
            }

            //SM v1.7
            CloudMaterial.SetFloat("luminance", luminance);
            CloudMaterial.SetFloat("lumFac", lumFac);
            CloudMaterial.SetFloat("Multiplier1", ScatterFac);
            CloudMaterial.SetFloat("Multiplier2", TurbFac);
            CloudMaterial.SetFloat("Multiplier3", HorizFac);
            CloudMaterial.SetFloat("turbidity", turbidity);
            CloudMaterial.SetFloat("reileigh", reileigh);
            CloudMaterial.SetFloat("mieCoefficient", mieCoefficient);
            CloudMaterial.SetFloat("mieDirectionalG", mieDirectionalG);
            CloudMaterial.SetFloat("bias", bias);
            CloudMaterial.SetFloat("contrast", contrast);
            CloudMaterial.SetVector("v3LightDir", -sunLight.transform.forward); //Debug.Log("v3LightDir:" + sunLight.transform.forward);
            //CloudMaterial.SetVector("_TintColor", new Vector4(TintColor.x, TintColor.y, TintColor.z, 1));//68, 155, 345
            CloudMaterial.SetVector("_TintColor", new Vector4(TintColor.r, TintColor.g, TintColor.b, 1));//68, 155, 345

            float Foggy = 0;
            if (FogSky)
            {
                Foggy = 1;
            }
            CloudMaterial.SetFloat("FogSky", Foggy);
            CloudMaterial.SetFloat("ClearSkyFac", ClearSkyFac);

            var sceneMode = RenderSettings.fogMode;
            var sceneDensity = 0.01f; //RenderSettings.fogDensity;//v3.0
            var sceneStart = RenderSettings.fogStartDistance;
            var sceneEnd = RenderSettings.fogEndDistance;
            Vector4 sceneParams;
            bool linear = (sceneMode == FogMode.Linear);
            float diff = linear ? sceneEnd - sceneStart : 0.0f;
            float invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;
            sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
            sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
            sceneParams.z = linear ? -invDiff : 0.0f;
            sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;
            CloudMaterial.SetVector("_SceneFogParams", sceneParams);
            CloudMaterial.SetVector("_SceneFogMode", new Vector4((int)sceneMode, useRadialDistance ? 1 : 0, 0, 0));


            ////////// END SCATTER

            //v0.6
            CloudMaterial.SetFloat("controlBackAlphaPower", controlBackAlphaPower);
            CloudMaterial.SetFloat("controlCloudAlphaPower", controlCloudAlphaPower);
            CloudMaterial.SetVector("controlCloudEdgeA", controlCloudEdgeA);
            CloudMaterial.SetFloat("controlCloudEdgeOffset", controlCloudEdgeOffset);
            CloudMaterial.SetFloat("depthDilation", depthDilation);
            CloudMaterial.SetFloat("_TemporalResponse", TemporalResponse);
            CloudMaterial.SetFloat("_TemporalGain", TemporalGain);

            // get cloud render texture and render clouds to it

            int rtW = opaqueDesc.width;
            int rtH = opaqueDesc.height;

            var format = CurrentCamera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;//v0.4
            if (CurrentCamera.name.Contains("Refl"))
            {
                format = allowHDR ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;//v0.4
            }
            RenderTexture tmpBuffer1 = RenderTexture.GetTemporary(rtW, rtH, 0, format);
            Blit(cmd, source, tmpBuffer1);


            // RenderTexture rtClouds = RenderTexture.GetTemporary((int)(rtW / ((float)downSample)), (int)(rtH / ((float)downSample)), 0, 
            //     source.format, RenderTextureReadWrite.Default, source.antiAliasing);
           
            RenderTexture rtClouds = RenderTexture.GetTemporary((int)(rtW / ((float)downSample)), (int)(rtH / ((float)downSample)), 0, format);
            ////         RenderTexture rtClouds = RenderTexture.GetTemporary((int)(source.width / ((float)downSample)), (int)(source.height / ((float)downSample)), 0, source.format, RenderTextureReadWrite.Default, source.antiAliasing);
            ///       CustomGraphicsBlit(source, rtClouds, CloudMaterial, 0);
            //CustomGraphicsBlit(null, rtClouds, CloudMaterial, 10);
            //CloudMaterial.SetTexture("_MainTex", tmpBuffer1);
            //CloudMaterial.SetTexture("_Clouds", tmpBuffer1);
            CloudMaterial.SetTexture("_MainTex", tmpBuffer1);
            Blit(cmd, destination.id, rtClouds, CloudMaterial, 10);
            //if (temporalAntiAliasing) // if TAA is enabled, then apply it to cloud render texture
            //{
            //    RenderTexture rtTemporal = RenderTexture.GetTemporary(rtClouds.width, rtClouds.height, 0, rtClouds.format, RenderTextureReadWrite.Default, source.antiAliasing);
            //    _temporalAntiAliasing.TemporalAntiAliasing(rtClouds, rtTemporal);
            //    UpscaleMaterial.SetTexture("_Clouds", rtTemporal);
            //    RenderTexture.ReleaseTemporary(rtTemporal);
            //}
            //else
            //{
            /////        ////////UpscaleMaterial.SetTexture("_Clouds", rtClouds);
            //CloudMaterial.SetTexture("_Clouds", rtClouds);
            //}
            // Apply clouds to background
            //Graphics.Blit(source, destination, UpscaleMaterial, 0);
            ////        Graphics.Blit(source, destination, CloudMaterial, 11);
            //       Blit(cmd, backgroundCam.targetTexture, tmpBuffer, CloudMaterial, 11);

            //Debug.Log(destination.id);

            //v0.6
            // if (previousFrameTexture != null) { previousFrameTexture.Release(); }
            // if (previousDepthTexture != null) { previousDepthTexture.Release(); }
            if (previousFrameTexture == null)
            {
                previousFrameTexture = new RenderTexture((int)(rtW / ((float)downSample)), (int)(rtH / ((float)downSample)), 0, RenderTextureFormat.DefaultHDR);
                previousFrameTexture.filterMode = FilterMode.Point;
                previousFrameTexture.Create();
            }
            if (previousDepthTexture == null)
            {
                previousDepthTexture = new RenderTexture((int)(rtW / ((float)downSample)), (int)(rtH / ((float)downSample)), 0, RenderTextureFormat.RFloat);
                previousDepthTexture.filterMode = FilterMode.Point;
                previousDepthTexture.Create();
            }
            RenderTexture tmpBuffer3 = RenderTexture.GetTemporary((int)(rtW / ((float)downSample)), (int)(rtH / ((float)downSample)), 0, format);
            if (enabledTemporalAA && Time.fixedTime > 0.05f)
            {
                //Debug.Log("AA Enabled");
                var worldToCameraMatrix = Camera.main.worldToCameraMatrix;
                var projectionMatrix = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);
                CloudMaterial.SetMatrix("_InverseProjectionMatrix", projectionMatrix.inverse);
                viewProjectionMatrix = projectionMatrix * worldToCameraMatrix;
                CloudMaterial.SetMatrix("_InverseViewProjectionMatrix", viewProjectionMatrix.inverse);
                CloudMaterial.SetMatrix("_LastFrameViewProjectionMatrix", lastFrameViewProjectionMatrix);
                CloudMaterial.SetMatrix("_LastFrameInverseViewProjectionMatrix", lastFrameInverseViewProjectionMatrix);
                CloudMaterial.SetTexture("_CloudTex", rtClouds);//CloudMaterial.SetTexture("_CloudTex", rtClouds);
                CloudMaterial.SetTexture("_PreviousColor", previousFrameTexture);
                CloudMaterial.SetTexture("_PreviousDepth", previousDepthTexture);
                cmd.SetRenderTarget(tmpBuffer3);
                if (mesh == null)
                {
                    Awake();
                }
                cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, CloudMaterial, 0, 12);// (int)RenderPass.TemporalReproj);
                //cmd.blit(context.source, context.destination, _material, 0);
                cmd.Blit(tmpBuffer3, previousFrameTexture);
                cmd.Blit(tmpBuffer3, rtClouds);

                cmd.SetRenderTarget(previousDepthTexture);
                cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, CloudMaterial, 0, 13);//(int)RenderPass.GetDepth);

                //DEBUG TAA
                //cmd.Blit(previousDepthTexture, rtClouds);
                //Blit(cmd, previousDepthTexture, source); //Blit(cmd, rtClouds, source);
                //context.ExecuteCommandBuffer(cmd);
                //CommandBufferPool.Release(cmd);
                //RenderTexture.ReleaseTemporary(rtClouds);
                //RenderTexture.ReleaseTemporary(tmpBuffer1);
                //return;
                //END DEBUG TAA
            }

            CloudMaterial.SetTexture("_MainTex", tmpBuffer1);
            CloudMaterial.SetTexture("_CloudTex", rtClouds);
            // Blit(cmd, tmpBuffer2, source);
            // Blit(cmd, source, destination.Identifier(), CloudMaterial, 11);
            Blit(cmd, tmpBuffer1, source, CloudMaterial, 11);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            RenderTexture.ReleaseTemporary(rtClouds);
            RenderTexture.ReleaseTemporary(tmpBuffer1);

            //v0.6
            RenderTexture.ReleaseTemporary(tmpBuffer3);
            lastFrameViewProjectionMatrix = viewProjectionMatrix;
            lastFrameInverseViewProjectionMatrix = viewProjectionMatrix.inverse;
        }

        //v0.6
        Matrix4x4 lastFrameViewProjectionMatrix;
        Matrix4x4 viewProjectionMatrix;
        Matrix4x4 lastFrameInverseViewProjectionMatrix;
        void OnDestroy()
        {
            if (previousFrameTexture != null)
            {
                previousFrameTexture.Release();
                previousFrameTexture = null;
            }

            if (previousDepthTexture != null)
            {
                previousDepthTexture.Release();
                previousDepthTexture = null;
            }
        }
        void OnDisable()
        {
            if (previousFrameTexture != null)
            {
                previousFrameTexture.Release();
                previousFrameTexture = null;
            }

            if (previousDepthTexture != null)
            {
                previousDepthTexture.Release();
                previousDepthTexture = null;
            }
        }
        RenderTexture previousFrameTexture;
        RenderTexture previousDepthTexture;
        Mesh mesh;
        void Awake()
        {
            mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
            new Vector3(-1, -1, 1),
            new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, -1, 1)
            };
            mesh.uv = new Vector2[]
            {
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1)
            };
            mesh.SetIndices(new int[] { 0, 1, 2, 3 }, MeshTopology.Quads, 0);
        }

        private void updateMaterialKeyword(bool b, string keyword, Material CloudMaterial)
        {
            if (b != CloudMaterial.IsKeywordEnabled(keyword))
            {
                if (b)
                {
                    CloudMaterial.EnableKeyword(keyword);
                }
                else
                {
                    CloudMaterial.DisableKeyword(keyword);
                }
            }
        }
        /// \brief Stores the normalized rays representing the camera frustum in a 4x4 matrix.  Each row is a vector.
        /// 
        /// The following rays are stored in each row (in eyespace, not worldspace):
        /// Top Left corner:     row=0
        /// Top Right corner:    row=1
        /// Bottom Right corner: row=2
        /// Bottom Left corner:  row=3
        private Matrix4x4 GetFrustumCorners(Camera cam)
        {
            float camFov = cam.fieldOfView;
            float camAspect = cam.aspect;

            Matrix4x4 frustumCorners = Matrix4x4.identity;

            float fovWHalf = camFov * 0.5f;

            float tan_fov = Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            Vector3 toRight = Vector3.right * tan_fov * camAspect;
            Vector3 toTop = Vector3.up * tan_fov;

            Vector3 topLeft = (-Vector3.forward - toRight + toTop);
            Vector3 topRight = (-Vector3.forward + toRight + toTop);
            Vector3 bottomRight = (-Vector3.forward + toRight - toTop);
            Vector3 bottomLeft = (-Vector3.forward - toRight - toTop);

            frustumCorners.SetRow(0, topLeft);
            frustumCorners.SetRow(1, topRight);
            frustumCorners.SetRow(2, bottomRight);
            frustumCorners.SetRow(3, bottomLeft);

            return frustumCorners;
        }

        /// \brief Custom version of Graphics.Blit that encodes frustum corner indices into the input vertices.
        /// 
        /// In a shader you can expect the following frustum cornder index information to get passed to the z coordinate:
        /// Top Left vertex:     z=0, u=0, v=0
        /// Top Right vertex:    z=1, u=1, v=0
        /// Bottom Right vertex: z=2, u=1, v=1
        /// Bottom Left vertex:  z=3, u=1, v=0
        /// 
        /// \warning You may need to account for flipped UVs on DirectX machines due to differing UV semantics
        ///          between OpenGL and DirectX.  Use the shader define UNITY_UV_STARTS_AT_TOP to account for this.
        static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
        {
            RenderTexture.active = dest;

            //fxMaterial.SetTexture("_MainTex", source);

            GL.PushMatrix();
            GL.LoadOrtho(); // Note: z value of vertices don't make a difference because we are using ortho projection

            fxMaterial.SetPass(passNr);

            GL.Begin(GL.QUADS);

            // Here, GL.MultitexCoord2(0, x, y) assigns the value (x, y) to the TEXCOORD0 slot in the shader.
            // GL.Vertex3(x,y,z) queues up a vertex at position (x, y, z) to be drawn.  Note that we are storing
            // our own custom frustum information in the z coordinate.
            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

            GL.End();
            GL.PopMatrix();
        }
        /////////////////////// END FULL VOLUMETRIC CLOUDS /////////////////////////////////////





        /////////////////////// VOLUME FOG SRP /////////////////////////////////////
        public void RenderFog(ScriptableRenderContext context, UnityEngine.Rendering.Universal.RenderingData renderingData, CommandBuffer cmd, RenderTextureDescriptor opaqueDesc)
        //public override void Render(PostProcessRenderContext context)
        {
            //var _material = context.propertySheets.Get(Shader.Find("Hidden/InverseProjectVFogLWRP"));
            Material _material = blitMaterial;
            _material.SetFloat("_DistanceOffset", _startDistance);
            _material.SetFloat("_Height", _fogHeight); //v0.1                                                                      
            _material.SetFloat("_cameraRoll", _cameraRoll);
            _material.SetVector("_cameraDiff", _cameraDiff);
            _material.SetFloat("_cameraTiltSign", _cameraTiltSign);

            var mode = RenderSettings.fogMode;
            if (mode == FogMode.Linear)
            {
                var start = RenderSettings.fogStartDistance;//RenderSettings.RenderfogStartDistance;
                var end = RenderSettings.fogEndDistance;
                var invDiff = 1.0f / Mathf.Max(end - start, 1.0e-6f);
                _material.SetFloat("_LinearGrad", -invDiff);
                _material.SetFloat("_LinearOffs", end * invDiff);
                _material.DisableKeyword("FOG_EXP");
                _material.DisableKeyword("FOG_EXP2");
            }
            else if (mode == FogMode.Exponential)
            {
                const float coeff = 1.4426950408f; // 1/ln(2)
                var density = RenderSettings.fogDensity;// RenderfogDensity;
                _material.SetFloat("_Density", coeff * density * _fogDensity);
                _material.EnableKeyword("FOG_EXP");
                _material.DisableKeyword("FOG_EXP2");
            }
            else // FogMode.ExponentialSquared
            {
                const float coeff = 1.2011224087f; // 1/sqrt(ln(2))
                var density = RenderSettings.fogDensity;//RenderfogDensity;
                _material.SetFloat("_Density", coeff * density * _fogDensity);
                _material.DisableKeyword("FOG_EXP");
                _material.EnableKeyword("FOG_EXP2");
            }
            if (_useRadialDistance)
                _material.EnableKeyword("RADIAL_DIST");
            else
                _material.DisableKeyword("RADIAL_DIST");

            if (_fadeToSkybox)
            {
                _material.DisableKeyword("USE_SKYBOX");
                _material.SetColor("_FogColor", _FogColor);// RenderfogColor);//v0.1            
            }
            else
            {
                _material.DisableKeyword("USE_SKYBOX");
                _material.SetColor("_FogColor", _FogColor);// RenderfogColor);
            }

            //v0.1  //URP v0.1
            //if (noiseTexture == null)
            //{
            //noiseTexture = new Texture2D(1280, 720);
            //}
            if (_material != null && noiseTexture != null)
            {
                //if (noiseTexture == null)
                //{
                    //noiseTexture = new Texture2D(1280, 720); //URP v0.1
                //}
                _material.SetTexture("_NoiseTex", noiseTexture);
            }

            // Calculate vectors towards frustum corners.
            Camera camera = currentCamera; //Camera.main;
            var cam = camera;// GetComponent<Camera>();
            var camtr = cam.transform;

            //Debug.Log(currentCamera.name);

            ////////// SCATTER
            var camPos = camtr.position;
            float FdotC = camPos.y - _fogHeight;
            float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);


            ///// ffrustrum
            //float camNear = cam.nearClipPlane;
            //float camFar = cam.farClipPlane;
            //float camFov = cam.fieldOfView;
            //float camAspect = cam.aspect;

            //Matrix4x4 frustumCorners = Matrix4x4.identity;

            //float fovWHalf = camFov * 0.5f;

            //Vector3 toRight = camtr.right * camNear * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
            //Vector3 toTop = camtr.up * camNear * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            //Vector3 topLeft = (camtr.forward * camNear - toRight + toTop);
            //float camScale = topLeft.magnitude * camFar / camNear;

            //topLeft.Normalize();
            //topLeft *= camScale;

            //Vector3 topRight = (camtr.forward * camNear + toRight + toTop);
            //topRight.Normalize();
            //topRight *= camScale;

            //Vector3 bottomRight = (camtr.forward * camNear + toRight - toTop);
            //bottomRight.Normalize();
            //bottomRight *= camScale;

            //Vector3 bottomLeft = (camtr.forward * camNear - toRight - toTop);
            //bottomLeft.Normalize();
            //bottomLeft *= camScale;

            //frustumCorners.SetRow(0, topLeft);
            //frustumCorners.SetRow(1, topRight);
            //frustumCorners.SetRow(2, bottomRight);
            //frustumCorners.SetRow(3, bottomLeft);

            //_material.SetMatrix("_FrustumCornersWS", frustumCorners);

            _material.SetFloat("farCamDistFactor",farCamDistFactor);

            _material.SetVector("_CameraWS", camPos);

            //Debug.Log("_HeightParams="+ new Vector4(_fogHeight, FdotC, paramK, heightDensity * 0.5f));

            _material.SetVector("_HeightParams", new Vector4(_fogHeight, FdotC, paramK, heightDensity * 0.5f));
            _material.SetVector("_DistanceParams", new Vector4(-Mathf.Max(startDistance, 0.0f), 0, 0, 0));
            _material.SetFloat("_NoiseDensity", noiseDensity);
            _material.SetFloat("_NoiseScale", noiseScale);
            _material.SetFloat("_NoiseThickness", noiseThickness);
            _material.SetVector("_NoiseSpeed", noiseSpeed);
            _material.SetFloat("_OcclusionDrop", occlusionDrop);
            _material.SetFloat("_OcclusionExp", occlusionExp);
            _material.SetInt("noise3D", noise3D);
            //SM v1.7
            _material.SetFloat("luminance", luminance);
            _material.SetFloat("lumFac", lumFac);
            _material.SetFloat("Multiplier1", ScatterFac);
            _material.SetFloat("Multiplier2", TurbFac);
            _material.SetFloat("Multiplier3", HorizFac);
            _material.SetFloat("turbidity", turbidity);
            _material.SetFloat("reileigh", reileigh);
            _material.SetFloat("mieCoefficient", mieCoefficient);
            _material.SetFloat("mieDirectionalG", mieDirectionalG);
            _material.SetFloat("bias", bias);
            _material.SetFloat("contrast", contrast);
            _material.SetVector("v3LightDir", Sun);//new Vector3(1, 1, 1));// Sun);//.forward);
            _material.SetVector("v3LightDirFOG", SunFOG);
            _material.SetVector("_TintColor", new Vector4(TintColor.r, TintColor.g, TintColor.b, 1));//68, 155, 345
            _material.SetVector("_TintColorK", new Vector4(TintColorK.x, TintColorK.y, TintColorK.z, 1));
            _material.SetVector("_TintColorL", new Vector4(TintColorL.x, TintColorL.y, TintColorL.z, 1));

            float Foggy = 0;
            if (FogSky) //ClearSkyFac
            {
                Foggy = 1;
            }
            _material.SetFloat("FogSky", Foggy);
            _material.SetFloat("ClearSkyFac", ClearSkyFac);
            //////// END SCATTER

            //LOCAL LIGHT
            _material.SetVector("localLightPos", new Vector4(PointL.x, PointL.y, PointL.z, PointL.w));//68, 155, 345
            _material.SetVector("localLightColor", new Vector4(PointLParams.x, PointLParams.y, PointLParams.z, PointLParams.w));//68, 155, 345                                       
                                                                                                                                //END LOCAL LIGHT



            //////////// CLOUDS
            //v3.5.3
            //if (!useFluidTexture)
            //{
            _material.SetTexture("_InteractTexture", _InteractTexture);
            //}
            //else
            //{
            //    _material.SetTexture("_InteractTexture", fluidFlow.GetTexture("_MainTex")); //v4.0
            //}
            _material.SetVector("_InteractTexturePos", _InteractTexturePos);
            _material.SetVector("_InteractTextureAtr", _InteractTextureAtr);
            _material.SetVector("_InteractTextureOffset", _InteractTextureOffset); //v4.0
            //v3.5.1
            _material.SetFloat("_NearZCutoff", _NearZCutoff);
            _material.SetFloat("_HorizonYAdjust", _HorizonYAdjust);
            _material.SetFloat("_HorizonZAdjust", _HorizonZAdjust);//v2.1.24
            _material.SetFloat("_FadeThreshold", _FadeThreshold);
            //v4.1f
            _material.SetFloat("_mobileFactor", _mobileFactor); //v4.1f
            _material.SetFloat("_alphaFactor", _alphaFactor);
            //v3.5
            _material.SetFloat("_SampleCount0", _SampleCount0);
            _material.SetFloat("_SampleCount1", _SampleCount1);
            _material.SetInt("_SampleCountL", _SampleCountL);
            _material.SetFloat("_NoiseFreq1", _NoiseFreq1);
            _material.SetFloat("_NoiseFreq2", _NoiseFreq2);
            _material.SetFloat("_NoiseAmp1", _NoiseAmp1);
            _material.SetFloat("_NoiseAmp2", _NoiseAmp2);
            _material.SetFloat("_NoiseBias", _NoiseBias);
            //v4.8.6
            if (Application.isPlaying)
            {
                _material.SetVector("_Scroll1", _Scroll1);
                _material.SetVector("_Scroll2", _Scroll2);
            }
            else
            {
                _material.SetVector("_Scroll1", Vector4.zero);
                _material.SetVector("_Scroll2", Vector4.zero);
            }
            _material.SetFloat("_Altitude0", _Altitude0);
            _material.SetFloat("_Altitude1", _Altitude1);
            _material.SetFloat("_FarDist", _FarDist);
            _material.SetFloat("_HGCoeff", _HGCoeff);
            _material.SetFloat("_Exposure", _ExposureUnder); //v4.0
            //v4.8.4
            if (!adjustNightLigting)
            {
                _material.SetFloat("_Scatter", _Scatter);
                _material.SetVector("_GroundColor", _GroundColor);//
                _material.SetFloat("_BackShade", _BackShade);
                _material.SetFloat("turbidity", turbidity);
                _material.SetFloat("_Extinct", _Extinct);
            }
            _material.SetFloat("_SunSize", _SunSize);
            _material.SetVector("_SkyTint", _SkyTint);
            _material.SetFloat("_AtmosphereThickness", _AtmosphereThickness);
            //v3.5
            //_material.SetFloat("_BackShade",_BackShade); //v4.8.6 moved up for night time change
            _material.SetFloat("_UndersideCurveFactor", _UndersideCurveFactor);
            //v2.1.19
            if (localLight != null)
            {
                Vector3 localLightPos = localLight.transform.position;
                //float intensity = Mathf.Pow(10, 3 + (localLightFalloff - 3) * 3);
                currentLocalLightIntensity = Mathf.Pow(10, 3 + (localLightFalloff - 3) * 3);
                //_material.SetVector ("_LocalLightPos", new Vector4 (localLightPos.x, localLightPos.y, localLightPos.z, localLight.intensity * localLightIntensity * intensity));
                _material.SetVector("_LocalLightPos", new Vector4(localLightPos.x, localLightPos.y, localLightPos.z, localLight.intensity * localLightIntensity * currentLocalLightIntensity));
                _material.SetVector("_LocalLightColor", new Vector4(localLight.color.r, localLight.color.g, localLight.color.b, localLightFalloff));
            }
            else
            {
                if (currentLocalLightIntensity > 0)
                {
                    currentLocalLightIntensity = 0;
                    //_material.SetVector ("_LocalLightPos", new Vector4 (localLightPos.x, localLightPos.y, localLightPos.z, localLight.intensity * localLightIntensity * intensity));
                    _material.SetVector("_LocalLightColor", Vector4.zero);
                }
            } 
            //SM v1.7
            _material.SetFloat("luminance", luminance);
            _material.SetFloat("lumFac", lumFac);
            _material.SetFloat("Multiplier1", ScatterFac);
            _material.SetFloat("Multiplier2", TurbFac);
            _material.SetFloat("Multiplier3", HorizFac);
            //_material.SetFloat("turbidity",turbidity); //v4.8.6
            _material.SetFloat("reileigh", reileigh);
            _material.SetFloat("mieCoefficient", mieCoefficient);
            _material.SetFloat("mieDirectionalG", mieDirectionalG);
            _material.SetFloat("bias", bias);
            _material.SetFloat("contrast", contrast);
            //v4.8
            _material.SetFloat("varianceAltitude1", varianceAltitude1);
            //if (isForReflections)
            //{
            //    //v4.8
            //    if (adjustNightTimeSun && Application.isPlaying && Time.fixedTime > 5)
            //    {
            //        Vector3 getDir = _material.GetVector("v3LightDir");
            //        _material.SetVector("v3LightDir", Vector3.Lerp(getDir, new Vector3(-Sun.forward.x, Sun.forward.y, -Sun.forward.z), Time.deltaTime * 0.2f));
            //    }
            //    else
            //    {
            //        _material.SetVector("v3LightDir", new Vector3(-Sun.forward.x, Sun.forward.y, -Sun.forward.z));
            //    }
            //}
            //else
            //{
            //    //v4.8
            //    if (adjustNightTimeSun && Application.isPlaying && Time.fixedTime > 5)
            //    {
            //        Vector3 getDir = _material.GetVector("v3LightDir");
            //        _material.SetVector("v3LightDir", Vector3.Lerp(getDir, -Sun.forward, Time.deltaTime * 0.2f));
            //    }
            //    else
            //    {
            //        _material.SetVector("v3LightDir", -Sun.forward);
            //    }
            //}
            //_material.SetVector("_TintColor", new Vector4(TintColor.x, TintColor.y, TintColor.z, 1));//68, 155, 345
            //float Foggy = 0;
            //if (FogSky)
            //{
            //    Foggy = 1;
            //}
            //_material.SetFloat("FogSky", Foggy);
            //_material.SetFloat("ClearSkyFac", ClearSkyFac);
            var sceneMode = RenderSettings.fogMode;
            var sceneDensity = 0.01f; //RenderSettings.fogDensity;//v3.0
            var sceneStart = RenderSettings.fogStartDistance;
            var sceneEnd = RenderSettings.fogEndDistance;
            Vector4 sceneParams;
            bool linear = (sceneMode == FogMode.Linear);
            float diff = linear ? sceneEnd - sceneStart : 0.0f;
            float invDiffA = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;
            sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
            sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
            sceneParams.z = linear ? -invDiffA : 0.0f;
            sceneParams.w = linear ? sceneEnd * invDiffA : 0.0f;
            _material.SetVector("_SceneFogParams", sceneParams);
            _material.SetVector("_SceneFogMode", new Vector4((int)sceneMode, _useRadialDistance ? 1 : 0, 0, 0));
            //int pass = 0;
            //if (distanceFog && heightFog)
            //    pass = 0; // distance + height
            //else if (distanceFog)
            //    pass = 1; // distance only
            //else
            //    pass = 2; // height only
            //v4.8
            if (isForReflections)
            {
                _material.SetFloat("_invertX", 1);
            }
            else
            {
                _material.SetFloat("_invertX", 0);
            }
            if (isForReflections)
            {
                _material.SetFloat("_invertRay", -1);
            }
            else
            {
                _material.SetFloat("_invertRay", 1);
            }
            if (isForReflections)
            {                
                _material.SetVector("_WorldSpaceCameraPosC", camPos);
            }
            else
            {
                _material.SetVector("_WorldSpaceCameraPosC", camPos);
            }
            _material.SetTexture("_ColorRamp", colourPalette);
            if (GradientBounds != Vector2.zero)
            {
                _material.SetFloat("_Close", GradientBounds.x);
                _material.SetFloat("_Far", GradientBounds.y);
            }
            //v4.8.3
            //_material.SetTexture("_CloudTexP", _cloudBuffer);
            //_material.SetTexture("_CloudTex", _cloudBufferP);
            //_material.SetFloat("frameFraction", frameFraction);
            //v3.5
            _material.SetTexture("_NoiseTex1", _NoiseTex1);
            _material.SetTexture("_NoiseTex2", _NoiseTex2);

            //WORLD RECONSTRUCT        
            Matrix4x4 camToWorld = cam.cameraToWorldMatrix;
            _material.SetMatrix("_InverseView", camToWorld);

            int rtW = opaqueDesc.width;
            int rtH = opaqueDesc.height;

            if (!downScale) { 
                renderMe(cam, _material, opaqueDesc,context,cmd);
            }
            else
            {                
                //v2.1.15
                if (!fastest)
                {
                    if (tmpBuffer == null)
                    {
                        var format = cam.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
                        tmpBuffer = RenderTexture.GetTemporary(rtW + 125, rtH + 125, 0, format);//v2.1.19 - add extra pixels to cover for frame displacement

                        RenderTexture.active = tmpBuffer;
                        GL.ClearWithSkybox(false, cam);
						if (blendBackground) { //v2.1.20
                            //v4.8
                            if (isForReflections)
                            {
                                backgroundCam.transform.rotation = cam.transform.rotation;
                                backgroundCam.transform.position = cam.transform.position;
                            }
                            else
                            {
                                backgroundCam.transform.rotation = cam.transform.rotation;
                                backgroundCam.transform.position = cam.transform.position;
                            }							
							
                            //v4.1f                             
                            if (backgroundCam.targetTexture == null || backgroundCam.targetTexture.width != Screen.width || backgroundCam.targetTexture.height != Screen.height)
                            {
                                backgroundCam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
                            }                               
                            
                            //v4.8
                            if (isForReflections)
                            {
                                backgroundCam.transform.eulerAngles = new Vector3(backgroundCam.transform.eulerAngles.x, backgroundCam.transform.eulerAngles.y, 180);                               
                                backgroundCam.Render();
                                tmpBuffer.DiscardContents();//v4.1f
                                //Graphics.Blit(backgroundCam.targetTexture, tmpBuffer);
                                Blit(cmd,backgroundCam.targetTexture, tmpBuffer); //URP
                            }
                            else
                            {
                                backgroundCam.Render(); //TO FIX
                                //Graphics.Blit(backgroundCam.targetTexture, tmpBuffer, backgroundMat);
                                Blit(cmd, backgroundCam.targetTexture, tmpBuffer, backgroundMat); //URP
                            }
						}
                        //CustomGraphicsBlitOpt(source, destination, tmpBuffer, fogMaterial, pass, DistGradient, GradientBounds, colourPalette, texture3Dnoise1, texture3Dnoise2, cam, true, false, WebGL);
                        //CustomGraphicsBlitOpt(source, destination, tmpBuffer, fogMaterial, pass, DistGradient, GradientBounds, colourPalette, texture3Dnoise1, texture3Dnoise2, cam, false, false, WebGL);
                       // Debug.Log("tmpBuffer null");
                        CustomGraphicsBlitOpt(context, source, destination, tmpBuffer, _material, 6, DistGradient, GradientBounds, colourPalette, cam, true, false, WebGL,cmd, opaqueDesc);
                        CustomGraphicsBlitOpt(context, source, destination, tmpBuffer, _material, 6, DistGradient, GradientBounds, colourPalette, cam, false, false, WebGL, cmd, opaqueDesc);                        
                    }
                    else
                    {
                        //v2.1.19
                        if (toggleCounter != 0 && Application.isPlaying && splitPerFrames > 0)
                        {
                            //Debug.Log("tmpBuffer not null 1");
                            //v4.8.3
                            //if (splitPerFrames > 0 && toggleCounter != splitPerFrames) {
                            if (enableReproject)
                            {                                
                                int signer = 1;
                                if (WebGL)
                                {
                                    signer = -1;
                                }
                                //v4.0
                                Camera cam1 = cam;
                                if (currentCamera != null) //Camera.main != null)
                                {
                                    cam1 = currentCamera;// Camera.main;
                                }
                                Matrix4x4 scaler = cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(2, 1 * signer, -1)) * cam1.projectionMatrix.inverse * Matrix4x4.Scale(new Vector3(-1, 1, 1));//v2.1.19
                                
                                float frameFraction = (float)toggleCounter / (float)splitPerFrames;
                                //Debug.Log("frameFraction"+ frameFraction + "_toggleCounter="+ toggleCounter);

                                float Xdisp = cam.transform.eulerAngles.y - prevCameraRotP.y;
                                float Ydisp = -(cam.transform.eulerAngles.x - prevCameraRotP.x);
                                
                                if (Xdisp > 122f || Xdisp< -122f)
                                {
                                    Xdisp = 0;
                                }
                                if (Ydisp > 122f || Ydisp< -122f)
                                {                                   
                                    Ydisp = 0;
                                }

                                scaler[0, 3] = 1 * Xdisp;//0.0075f * Xdisp * 1;//0.005f * Xdisp;//0.009f * Xdisp;
                                scaler[1, 3] = 1 * Ydisp;//0.0062f * Ydisp * 1;// 0.0062f * Ydisp * 1;//0.012f * Ydisp;
                                scaler[2, 3] = 1;// 1f - 0.005f * (Xdisp+Ydisp); // 0.95f+ 0.005f*(splitPerFrames - toggleCounter); //image scaler
                                _material.SetMatrix("_WorldClip", scaler);

                                //v4.8.3
                                _material.SetTexture("_CloudTexP", _cloudBuffer);
                                _material.SetTexture("_CloudTex", _cloudBufferP);
                                _material.SetFloat("frameFraction", frameFraction);
                                //Debug.Log(frameFraction);

                                //connector.testTex = toTexture2D(_cloudBufferP);
                                //connector.testTex2 = toTexture2D(_cloudBuffer);

                                //Blit(cmd,_cloudBufferP, _cloudBufferP1, _material, 9);// 5);//v4.8.3       //URP 
                                _material.SetTexture("_SkyTex", _cloudBufferP);
                                //Blit(cmd, _cloudBuffer, _cloudBufferP1, _material, 9);

                                Graphics.Blit(_cloudBuffer, _cloudBufferP1, _material, 9);

                                //var format = allowHDR ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
                                //RenderTexture tmpBuffer1A = RenderTexture.GetTemporary(rtW, rtH, 0, format);
                                //Blit(cmd, source, tmpBuffer1A, _material, 9);

                                //connector.testTex2 = toTexture2D(_cloudBufferP1);
                            }
                            
                            toggleCounter--;
                            //CustomGraphicsBlitOpt(source, destination, tmpBuffer, fogMaterial, pass, DistGradient, GradientBounds, colourPalette, texture3Dnoise1, texture3Dnoise2, cam, false, true, WebGL);
                            CustomGraphicsBlitOpt(context, source, destination, tmpBuffer, _material, 6, DistGradient, GradientBounds, colourPalette, cam, false, true, WebGL,cmd,opaqueDesc);

                        }
                        else
                        {
                            //Debug.Log("tmpBuffer not null 2");

                            toggleCounter = splitPerFrames;
                            RenderTexture.active = tmpBuffer;
                            GL.ClearWithSkybox(false, cam);
                            if (blendBackground)
                            { //v2.1.20
                                //v4.8
                                if (isForReflections)
                                {
                                    backgroundCam.transform.rotation = cam.transform.rotation;
                                    backgroundCam.transform.position = cam.transform.position;
                                }
                                else
                                {
                                    backgroundCam.transform.rotation = cam.transform.rotation;
                                    backgroundCam.transform.position = cam.transform.position;
                                }

                                //v2.1.23                               
                                //v4.1f                                
                                if (backgroundCam.targetTexture == null || backgroundCam.targetTexture.width != Screen.width || backgroundCam.targetTexture.height != Screen.height)
                                {
                                    backgroundCam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
                                }                                

                                //v4.8
                                if (isForReflections)
                                {
                                    backgroundCam.transform.eulerAngles = new Vector3(backgroundCam.transform.eulerAngles.x, backgroundCam.transform.eulerAngles.y, 180);
                                    backgroundCam.Render();
                                    tmpBuffer.DiscardContents();//v4.1f
                                    //Graphics.Blit(backgroundCam.targetTexture, tmpBuffer);
                                    Blit(cmd, backgroundCam.targetTexture, tmpBuffer);
                                }
                                else
                                {
                                    backgroundCam.Render();
                                    tmpBuffer.DiscardContents();//v4.1f
                                    //Graphics.Blit(backgroundCam.targetTexture, tmpBuffer, backgroundMat);
                                    Blit(cmd, backgroundCam.targetTexture, tmpBuffer, backgroundMat);
                                }
                            }

                            //CustomGraphicsBlitOpt(source, destination, tmpBuffer, fogMaterial, pass, DistGradient, GradientBounds, colourPalette, texture3Dnoise1, texture3Dnoise2, cam, true, false, WebGL);
                            CustomGraphicsBlitOpt(context, source, destination, tmpBuffer, _material, 6, DistGradient, GradientBounds, colourPalette, cam, true, false, WebGL, cmd, opaqueDesc);

                            //Debug.Log("f");
                            //destination.DiscardContents();//v4.1f
                          ////tmpBuffer.DiscardContents();//v4.1f                           
                            //CustomGraphicsBlitOpt(source, destination, tmpBuffer, fogMaterial, pass, DistGradient, GradientBounds, colourPalette, texture3Dnoise1, texture3Dnoise2, cam, false, false, WebGL);
                            CustomGraphicsBlitOpt(context, source, destination, tmpBuffer, _material, 6, DistGradient, GradientBounds, colourPalette, cam, false, false, WebGL, cmd, opaqueDesc);

                        }
                    }
                    
                }
                else
                {
                    //renderMe(cam, _material, opaqueDesc, context, cmd);
                    //CustomGraphicsBlitOpt(source, destination,null, fogMaterial, pass, DistGradient, GradientBounds, colourPalette, texture3Dnoise1, texture3Dnoise2, cam, true, false, WebGL);
                    //CustomGraphicsBlitOpt(source, destination, null, fogMaterial, pass, DistGradient, GradientBounds, colourPalette, texture3Dnoise1, texture3Dnoise2, cam, false, false, WebGL);
                    CustomGraphicsBlitOpt(context, source, destination, null, _material, 6, DistGradient, GradientBounds, colourPalette, cam, true, false, WebGL, cmd, opaqueDesc);
                    CustomGraphicsBlitOpt(context, source, destination, null, _material, 6, DistGradient, GradientBounds, colourPalette, cam, false, false, WebGL, cmd, opaqueDesc);
                }				
			}
            ////END RENDERING
            
        }

        RenderTexture tmpBuffer;//v2.1.15
        //v2.1.20
        //public bool WebGL = false;
        public bool blendBackground = false;
        public Camera backgroundCam;
        public Material backgroundMat;

        void renderMe(Camera cam, Material _material, RenderTextureDescriptor opaqueDesc, ScriptableRenderContext context, CommandBuffer cmd)
        {
            int signer = 1;
            if (WebGL)
            {
                signer = -1;
            }
            Camera cam1 = cam;
            if (currentCamera != null) //Camera.main != null)
            {
                cam1 = currentCamera;// Camera.main;
            }
            Matrix4x4 scaler = cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(1, 1 * signer, -1)) * cam1.projectionMatrix.inverse * Matrix4x4.Scale(new Vector3(-1, 1, 1));//v2.1.19
            _material.SetMatrix("_WorldClip", scaler);
            // _material.SetMatrix("_WorldClip", cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(1, 1 * signer, -1)) * cam1.projectionMatrix.inverse * Matrix4x4.Scale(new Vector3(-1, 1, 1)));
            //_material.SetMatrix ("_WorldClip",  cam.cameraToWorldMatrix* cam.projectionMatrix.inverse  );             

            if (!fastest)
            {
                _material.SetInt("_fastest", 0);
                //Debug.Log(_material.GetInt("_fastest"));
            }
            else
            {
                _material.SetInt("_fastest", 1);
                //Debug.Log(_material.GetInt("_fastest"));
            }
            /////////// END CLOUDS


            //RENDER FINAL EFFECT
            int rtW = opaqueDesc.width;
            int rtH = opaqueDesc.height;
            //var format = camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default; //v3.4.9
            var format = allowHDR ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR; //v3.4.9 //v LWRP

            // Debug.Log(renderingData.cameraData.camera.allowHDR);

            //RenderTexture tmpBuffer1 = RenderTexture.GetTemporary(context.width, context.height, 0, format);
            RenderTexture tmpBuffer1 = RenderTexture.GetTemporary(rtW, rtH, 0, format);
            RenderTexture.active = tmpBuffer1;

            GL.ClearWithSkybox(false, cam);
            ////context.command.BlitFullscreenTriangle(context.source, tmpBuffer1);

            //Blit(cmd, source, m_TemporaryColorTexture.Identifier()); //KEEP BACKGROUND
            Blit(cmd, source, tmpBuffer1); //KEEP BACKGROUND
            // cmd.SetGlobalTexture("_ColorBuffer", lrDepthBuffer.Identifier());
            // Blit(cmd, m_TemporaryColorTexture.Identifier(), source, _material, (screenBlendMode == BlitVolumeCloudsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);

            _material.SetTexture("_MainTex", tmpBuffer1);
            
            /////context.command.BlitFullscreenTriangle(context.source, context.destination, _material, 0);
            //Blit(cmd, m_TemporaryColorTexture.Identifier(), source, _material, (screenBlendMode == BlitVolumeFogSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);
            Blit(cmd, tmpBuffer1, source, _material, 6);

            RenderTexture.ReleaseTemporary(tmpBuffer1);
            //END RENDER FINAL EFFECT

            ////RELEASE TEMPORARY TEXTURES AND COMMAND BUFFER
            //cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
            //cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }//END RENDERME BASE

        void renderMeOpt(Camera cam, Material _material, RenderTextureDescriptor opaqueDesc, ScriptableRenderContext context, CommandBuffer cmd)
        {
            int signer = 1;
            if (WebGL)
            {
                signer = -1;
            }
            Camera cam1 = cam;
            if (currentCamera != null) //Camera.main != null)
            {
                cam1 = currentCamera;// Camera.main;
            }
            Matrix4x4 scaler = cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(1, 1 * signer, -1)) * cam1.projectionMatrix.inverse * Matrix4x4.Scale(new Vector3(-1, 1, 1));//v2.1.19
            _material.SetMatrix("_WorldClip", scaler);
            // _material.SetMatrix("_WorldClip", cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(1, 1 * signer, -1)) * cam1.projectionMatrix.inverse * Matrix4x4.Scale(new Vector3(-1, 1, 1)));
            //_material.SetMatrix ("_WorldClip",  cam.cameraToWorldMatrix* cam.projectionMatrix.inverse  );             

            if (!fastest)
            {
                _material.SetInt("_fastest", 0);               
            }
            else
            {
                _material.SetInt("_fastest", 1);              
            }
            /////////// END CLOUDS

            //RENDER FINAL EFFECT
            int rtW = opaqueDesc.width;
            int rtH = opaqueDesc.height;           
            var format = allowHDR ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;             
            RenderTexture tmpBuffer1 = RenderTexture.GetTemporary(rtW, rtH, 0, format);
            RenderTexture.active = tmpBuffer1;

            GL.ClearWithSkybox(false, cam);           
            Blit(cmd, source, tmpBuffer1); //KEEP BACKGROUND           
            _material.SetTexture("_MainTex", tmpBuffer1);           
            
            Blit(cmd, tmpBuffer1, source, _material, 6);
            RenderTexture.ReleaseTemporary(tmpBuffer1);
            //END RENDER FINAL EFFECT

            ////RELEASE TEMPORARY TEXTURES AND COMMAND BUFFER
            //cmd.ReleaseTemporaryRT(lrDepthBuffer.id);            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }//END RENDERME BASE

        Texture2D toTexture2D(RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }

        //private RenderTargetIdentifier source { get; set; }
        //private RenderTargetHandle destination { get; set; }
        //void CustomGraphicsBlitOpt(RenderTexture source, RenderTexture dest, RenderTexture skysource, Material fxMaterial, int passNr, Gradient DistGradient,
        void CustomGraphicsBlitOpt(ScriptableRenderContext context, RenderTargetIdentifier source, UnityEngine.Rendering.Universal.RenderTargetHandle dest, RenderTexture skysource, Material fxMaterial, int passNr, Gradient DistGradient,
            Vector2 GradientBounds, Texture2D colourPalette, Camera cam, bool toggle, bool splitPerFrame, bool WebGL, CommandBuffer cmd ,RenderTextureDescriptor opaqueDesc)
        {
            //URP
            int rtW = opaqueDesc.width;
            int rtH = opaqueDesc.height;
            var format = allowHDR ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR; //v3.4.9 //v LWRP

            if (!fastest)
            {
                fxMaterial.SetInt("_fastest", 0);
            }
            else
            {
                fxMaterial.SetInt("_fastest", 1);
            }

            if (toggle)
            {
                //v4.8.3                
                if (enableReproject || countCameraSteady > 1)
                {
                    //connector.testTex = toTexture2D(new1);
                    Graphics.Blit(new1, _cloudBufferP);
                    //Blit(cmd, new1, _cloudBufferP);
                    //connector.testTex = toTexture2D(_cloudBufferP);
                }

                //v2.1.15
                if (!fastest)
                {
                    fxMaterial.SetTexture("_SkyTex", skysource);
                }

                fxMaterial.SetTexture("_CloudTex", _cloudBuffer);

                //v4.5
                if (new1 == null)
                {
                    new1 = CreateBuffer();
                }
                //RenderTexture.active = new1;

                //fxMaterial.SetTexture ("_MainTex", source);
                fxMaterial.SetTexture("_MainTex", null);

                //v3.5
                //fxMaterial.SetTexture("_NoiseTex1", tex3D1);
                //fxMaterial.SetTexture("_NoiseTex2", tex3D2);

                //VFOG (v3.4.9c - put -1 in second 1 for WebGL)
                //v2.1.20
                int signer = 1;
                if (WebGL)
                {
                    signer = -1;
                }
                //v4.0
                Camera cam1 = cam;
                if (currentCamera != null ) //Camera.main != null)
                {
                    cam1 = currentCamera;// Camera.main;
                }
                //Matrix4x4 scaler = cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(1, 1 * signer, -1)) * cam1.projectionMatrix.inverse * Matrix4x4.Scale(new Vector3(-1, 1, 1));//v2.1.19
                Matrix4x4 scaler = cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(1, 1 * signer, 1)) * cam1.projectionMatrix.inverse;//v2.1.19 //URP v0.2                                                                                                                                                                 //scaler[0,3] = scaler[0,3] + 0.1f;

                fxMaterial.SetMatrix("_WorldClip", scaler);
                
                fxMaterial.SetTexture("_ColorRamp", colourPalette);

                if (GradientBounds != Vector2.zero)
                {
                    fxMaterial.SetFloat("_Close", GradientBounds.x);
                    fxMaterial.SetFloat("_Far", GradientBounds.y);
                }

                Blit(cmd,source,new1,fxMaterial,6);

                //GL.PushMatrix();
                //GL.LoadOrtho();

                //fxMaterial.SetPass(passNr);

                //GL.Begin(GL.QUADS);

                //GL.MultiTexCoord2(0, 0.0f, 0.0f);
                //GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

                //GL.MultiTexCoord2(0, 1.0f, 0.0f);
                //GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

                //GL.MultiTexCoord2(0, 1.0f, 1.0f);
                //GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

                //GL.MultiTexCoord2(0, 0.0f, 1.0f);
                //GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

                //GL.End();
                //GL.PopMatrix();

                //Graphics.Blit(new1, _cloudBuffer);//v4.8.3
                Blit(cmd,new1, _cloudBuffer); //URP
                //release
                prevCameraRotP = cam.transform.eulerAngles;

                context.ExecuteCommandBuffer(cmd);
                //CommandBufferPool.Release(cmd);
            }
            else
            {
                //URP
                RenderTexture tmpBuffer1 = RenderTexture.GetTemporary(rtW, rtH, 0, format);

                //RenderTexture.active = tmpBuffer1; 
                //GL.ClearWithSkybox(false, cam);

                Blit(cmd, source, tmpBuffer1);   
                fxMaterial.SetTexture("_MainTex", tmpBuffer1);

                //fxMaterial.SetTexture("_MainTex", source);
                if (!fastest)
                {
                    fxMaterial.SetTexture("_SkyTex", skysource);//v2.1.15
                }

                if (splitPerFrames > 0 && enableReproject && Application.isPlaying)
                {
                    if (splitPerFrame)
                    {
                        fxMaterial.SetTexture("_CloudTex", _cloudBufferP1);
                    }
                    else
                    {
                        //v4.8.3
                        if (autoReproject)
                        {
                            if (autoReproject && enableReproject && countCameraSteady > 2)
                            {
                                fxMaterial.SetTexture("_CloudTex", _cloudBufferP1);
                            }
                            else
                            {
                                fxMaterial.SetTexture("_CloudTex", _cloudBuffer);
                            }
                        }
                        else
                        {
                            fxMaterial.SetTexture("_CloudTex", _cloudBufferP1);
                        }
                    }
                }
                else
                {
                    fxMaterial.SetTexture("_CloudTex", _cloudBuffer); //v4.8.3
                }

                //URP                
                RenderTexture tmpBuffer2 = RenderTexture.GetTemporary(rtW, rtH, 0, format);
                //RenderTexture.active = tmpBuffer1;                
                //Blit(cmd, source, tmpBuffer1);

              //  RenderTexture.active = tmpBuffer2;
                //RenderTexture.active = dest.Identifier();// dest;

                //v2.1.20
                int signer = 1;
                if (WebGL)
                {
                    signer = -1;
                }
                //v4.0
                Camera cam1 = cam;
                if (currentCamera !=null) //Camera.main != null)
                {
                    cam1 = currentCamera;// Camera.main;
                }
                // Matrix4x4 scaler = cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(2, 1 * signer, -1)) * cam1.projectionMatrix.inverse * Matrix4x4.Scale(new Vector3(-1, 1, 1));//v2.1.19
                // Matrix4x4 scaler = cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(-1, -1 * signer, 1)) * cam1.projectionMatrix.inverse * Matrix4x4.Scale(new Vector3(-1, 1, 1));//v2.1.19
                //Matrix4x4 scaler = cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(1 * signer, 1 * signer, 1 * signer)) * cam1.projectionMatrix.inverse;//v2.1.19 //URP v0.2
                Matrix4x4 scaler = cam.cameraToWorldMatrix * Matrix4x4.Scale(new Vector3(1, 1 * signer, 1)) * cam1.projectionMatrix.inverse;//v2.1.19 //URP v0.2

                if (splitPerFrame)
                {
                    float Xdisp = cam.transform.eulerAngles.y - prevCameraRot.y;
                    float Ydisp = -(cam.transform.eulerAngles.x - prevCameraRot.x);

                    //v4.8.3
                    if (autoReproject)
                    {
                        //Debug.Log(Xdisp + " :" + Ydisp);
                        if (Mathf.Abs(Xdisp) == 0.000000f && Mathf.Abs(Ydisp) == 0.000000f)
                        {
                            if (countCameraSteady > 8)
                            {
                                enableReproject = true; connector.enableReproject = true;
                            }
                            else { countCameraSteady++; }
                        }
                        else
                        {
                            //Debug.Log(toggleCounter + " :::" + countCameraSteady);
                            if (toggleCounter == 1 && countCameraSteady > 0)
                            {                                
                                enableReproject = false; connector.enableReproject = false;
                                countCameraSteady = 0;

                                //Debug.Log(Xdisp + " :" + Ydisp);
                            }
                        }
                    }

                    if (Xdisp > 122f || Xdisp < -122f)
                    {                        
                        Xdisp = 0;
                    }
                    if (Ydisp > 122f || Ydisp < -122f)
                    {                        
                        Ydisp = 0;
                    }

                    scaler[0, 3] = 0.009f * Xdisp;//0.005f * Xdisp;
                    scaler[1, 3] = 0.012f * Ydisp;
                    scaler[2, 3] = 1;// 1f - 0.005f * (Xdisp+Ydisp); // 0.95f+ 0.005f*(splitPerFrames - toggleCounter); //image scaler
                }
                else
                {
                    prevCameraRot = cam.transform.eulerAngles;
                }
                fxMaterial.SetMatrix("_WorldClip", scaler);

                //GL.PushMatrix();
                //GL.LoadOrtho();

                if (splitPerFrame && cameraMotionCompensate)
                {
                  //  fxMaterial.SetPass(8); //4
                    Blit(cmd, source, tmpBuffer2, fxMaterial, 8);
                }
                else
                {
                    //fxMaterial.SetPass(7);//3
                    Blit(cmd, source, tmpBuffer2, fxMaterial, 7);
                }

                Blit(cmd, tmpBuffer2, source);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);

                RenderTexture.ReleaseTemporary(tmpBuffer1);
                RenderTexture.ReleaseTemporary(tmpBuffer2);
                return;

                //GL.Begin(GL.QUADS);

                //GL.MultiTexCoord2(0, 0.0f, 0.0f);
                //GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

                //GL.MultiTexCoord2(0, 1.0f, 0.0f);
                //GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

                //GL.MultiTexCoord2(0, 1.0f, 1.0f);
                //GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

                //GL.MultiTexCoord2(0, 0.0f, 1.0f);
                //GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

                //GL.End();
                //GL.PopMatrix();

                ////RESTORE STATE TO DESTINATION
                //Blit(cmd, tmpBuffer2, destination.Identifier());
                //Blit(cmd, tmpBuffer2, source);

                //Blit(cmd, destination.Identifier(), source, fxMaterial, 6);
                //Blit(cmd, destination.Identifier(), source);
            }

            //TEST
            //RenderTexture tmpBuffer1A = RenderTexture.GetTemporary(rtW/4, rtH/4, 0, format);
            //RenderTexture.active = tmpBuffer1A;
            //GL.ClearWithSkybox(false, cam);
            //Blit(cmd, source, tmpBuffer1A); //KEEP BACKGROUND           
            //fxMaterial.SetTexture("_MainTex", tmpBuffer1A);
            //Blit(cmd, tmpBuffer1A, source, fxMaterial, 6);
            //RenderTexture.ReleaseTemporary(tmpBuffer1A);


            

           // RenderTexture.ReleaseTemporary(tmpBuffer1);
            //RenderTexture.ReleaseTemporary(tmpBuffer2);
        }
            /////////////////////// END VOLUME FOG SRP ///////////////////////////////// 


        }
}
