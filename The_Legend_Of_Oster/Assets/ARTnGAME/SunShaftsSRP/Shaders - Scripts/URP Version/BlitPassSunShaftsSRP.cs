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
    internal class BlitPassSunShaftsSRP : UnityEngine.Rendering.Universal.ScriptableRenderPass
    {

        //v0.4  - Unity 2020.1
#if UNITY_2020_2_OR_NEWER
        public BlitSunShaftsSRP.BlitSettings settings;
        UnityEngine.Rendering.Universal.RenderTargetHandle _handle;
        public override void OnCameraSetup(CommandBuffer cmd, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            _handle.Init(settings.textureId);
            destination = (settings.destination == BlitSunShaftsSRP.Target.Color)
                ? UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget
                : _handle;

            var renderer = renderingData.cameraData.renderer;
            source = renderer.cameraColorTarget;
        }
#endif


        public bool enableShafts = true;
        //SUN SHAFTS         
        public BlitSunShaftsSRP.BlitSettings.SunShaftsResolution resolution = BlitSunShaftsSRP.BlitSettings.SunShaftsResolution.Normal;
        public BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode screenBlendMode = BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen;
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

        public Material blitMaterial = null;
        public int blitShaderPassIndex = 0;
        public FilterMode filterMode { get; set; }

        private RenderTargetIdentifier source { get; set; }
        private UnityEngine.Rendering.Universal.RenderTargetHandle destination { get; set; }

        UnityEngine.Rendering.Universal.RenderTargetHandle m_TemporaryColorTexture;
        string m_ProfilerTag;


        //SUN SHAFTS
        RenderTexture lrColorB;
       // RenderTexture lrDepthBuffer;
       // RenderTargetHandle lrColorB;
        UnityEngine.Rendering.Universal.RenderTargetHandle lrDepthBuffer;

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public BlitPassSunShaftsSRP(UnityEngine.Rendering.Universal.RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag,BlitSunShaftsSRP.BlitSettings settings)
        {
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


        connectSuntoSunShaftsURP connector;

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {            
            //grab settings if script on scene camera
            if (connector == null)
            {
                connector = renderingData.cameraData.camera.GetComponent<connectSuntoSunShaftsURP>();
                if(connector == null && Camera.main != null)
                {
                    connector = Camera.main.GetComponent<connectSuntoSunShaftsURP>();                    
                }                
            }
            //Debug.Log(Camera.main.GetComponent<connectSuntoSunShaftsURP>().sun.transform.position);
            if (connector != null)
            {
                this.enableShafts = connector.enableShafts;
                this.sunTransform = connector.sun.transform.position;
                this.screenBlendMode = connector.screenBlendMode;
                //public Vector3 sunTransform = new Vector3(0f, 0f, 0f); 
                this.radialBlurIterations = connector.radialBlurIterations;
                this.sunColor = connector.sunColor;
                this.sunThreshold = connector.sunThreshold;
                this.sunShaftBlurRadius = connector.sunShaftBlurRadius;
                this.sunShaftIntensity = connector.sunShaftIntensity;
                this.maxRadius = connector.maxRadius;
                this.useDepthTexture = connector.useDepthTexture;
            }

            //if still null, disable effect
            bool connectorFound = true;
            if (connector == null)
            {
                connectorFound = false;
            }

            if (enableShafts && connectorFound)
            {
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDesc.depthBufferBits = 0;

                // Can't read and write to same color target, create a temp render target to blit. 
                if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
                {
                    cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);
                    //Blit(cmd, source, m_TemporaryColorTexture.Identifier(), blitMaterial, 0);// blitShaderPassIndex);
                    //Blit(cmd, m_TemporaryColorTexture.Identifier(), source);

                    ////blitMaterial.SetFloat("_Delta",100);
                    //Blit(cmd, source, m_TemporaryColorTexture.Identifier(), blitMaterial, 0);// blitShaderPassIndex);
                    //Blit(cmd, m_TemporaryColorTexture.Identifier(), source);

                    RenderShafts(context, renderingData, cmd, opaqueDesc);
                }
                else
                {
                    //Blit(cmd, source, destination.Identifier(), blitMaterial, blitShaderPassIndex);
                }

                // RenderShafts(context, renderingData);
                //Camera camera = Camera.main;
                //cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                //cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, blitMaterial);
                //cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);

                //context.ExecuteCommandBuffer(cmd);
                // CommandBufferPool.Release(cmd);
            }
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);

               // cmd.ReleaseTemporaryRT(lrColorB.id);
                cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
                //RenderTexture.ReleaseTemporary(lrColorBACK);
            }
        }

        //RenderTexture lrColorBACK;
        //RenderTargetHandle lrColorBACK;

        //SUN SHAFTS
        public void RenderShafts(ScriptableRenderContext context, UnityEngine.Rendering.Universal.RenderingData renderingData, CommandBuffer cmd, RenderTextureDescriptor opaqueDesc)
        {

            //CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            //RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            //var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/GrayscaleShafts"));

            ////           var sheetSHAFTS = context.propertySheets.Get(Shader.Find("Hidden/Custom/GrayscaleShafts"));
            Material sheetSHAFTS = blitMaterial;

            //sheet.properties.SetFloat("_Blend", settings.blend);
            sheetSHAFTS.SetFloat("_Blend", blend);

            //scontext.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

            //if (CheckResources() == false)
            //{
            //    Graphics.Blit(source, destination);
            //    return;
            //}
            Camera camera = Camera.main;
            // we actually need to check this every frame
            if (useDepthTexture)
            {
                // GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
                camera.depthTextureMode |= DepthTextureMode.Depth;
            }
            //int divider = 4;
            //if (settings.resolution == SunShaftsHDRP.SunShaftsResolution.Normal)
            //    divider = 2;
            // else if (settings.resolution == SunShaftsHDRP.SunShaftsResolution.High)
            //    divider = 1;

            Vector3 v = Vector3.one * 0.5f;
           // Debug.Log(sunTransform);
            if (sunTransform != Vector3.zero) {
                //v = camera.WorldToViewportPoint(sunTransform);
                //v = sunTransform;
                //v = camera.WorldToViewportPoint(-sunTransform);
                v = Camera.main.WorldToViewportPoint(sunTransform);// - Camera.main.transform.position;
            }
            else {
                v = new Vector3(0.5f, 0.5f, 0.0f);
            }
            //Debug.Log("v="+v);


            //TextureDimension dim = renderingData.cameraData.cameraTargetDescriptor.dimension;


            //v0.1
            int rtW = opaqueDesc.width;///context.width; //source.width / divider;
            int rtH = opaqueDesc.height;// context.width; //source.height / divider;

            // Debug.Log(rtW + " ... " + rtH);

            

           // lrColorB = RenderTexture.GetTemporary(rtW, rtH, 0);
    //        lrDepthBuffer = RenderTexture.GetTemporary(rtW, rtH, 0);
            cmd.GetTemporaryRT(lrDepthBuffer.id, opaqueDesc, filterMode);

            //TEST1
            // Blit(cmd, source, lrDepthBuffer.Identifier(), blitMaterial,1);// blitShaderPassIndex);
            // Blit(cmd, lrDepthBuffer.Identifier(), source);
            // cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
            // return;


            // mask out everything except the skybox
            // we have 2 methods, one of which requires depth buffer support, the other one is just comparing images

            //    sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(1.0f, 1.0f, 0.0f, 0.0f) * sunShaftBlurRadius);
            //    sunShaftsMaterial.SetVector("_SunPosition", new Vector4(v.x, v.y, v.z, maxRadius));
            //    sunShaftsMaterial.SetVector("_SunThreshold", sunThreshold);
            sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(1.0f, 1.0f, 0.0f, 0.0f) * sunShaftBlurRadius);
           // sheetSHAFTS.SetVector("_SunPosition", new Vector4(v.x*0.5f+0.5f, v.y , v.z, maxRadius)); //new Vector4(v.x+0.25f, v.y, v.z, maxRadius));
            //Debug.Log(v.x);
            //Debug.Log(v.y);
            sheetSHAFTS.SetVector("_SunThreshold", sunThreshold);

            if (!useDepthTexture)
            {
                //var format= GetComponent<Camera>().hdr ? RenderTextureFormat.DefaultHDR: RenderTextureFormat.Default;
                var format = camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default; //v3.4.9
                RenderTexture tmpBuffer = RenderTexture.GetTemporary(rtW, rtH, 0, format);
                RenderTexture.active = tmpBuffer;
                GL.ClearWithSkybox(false, camera);

                //sunShaftsMaterial.SetTexture("_Skybox", tmpBuffer);
                sheetSHAFTS.SetTexture("_Skybox", tmpBuffer);
                //        Graphics.Blit(source, lrDepthBuffer, sunShaftsMaterial, 3);

   //             context.command.BlitFullscreenTriangle(source, lrDepthBuffer, sheetSHAFTS, 3);
                Blit(cmd, source, lrDepthBuffer.Identifier(), sheetSHAFTS, 3);

                RenderTexture.ReleaseTemporary(tmpBuffer);
            }
            else
            {
                //          Graphics.Blit(source, lrDepthBuffer, sunShaftsMaterial, 2);
  //              context.command.BlitFullscreenTriangle(source, lrDepthBuffer, sheetSHAFTS, 2);
                Blit(cmd, source, lrDepthBuffer.Identifier(), sheetSHAFTS, 2);
            }
            //  context.command.BlitFullscreenTriangle(lrDepthBuffer, context.destination, sheet, 5);
            // return;
            // paint a small black small border to get rid of clamping problems
            //      DrawBorder(lrDepthBuffer, simpleClearMaterial);

            // radial blur:

            //Blit(cmd, source, lrDepthBuffer.Identifier(), blitMaterial,1);// blitShaderPassIndex);
            //cmd.SetGlobalTexture("_ColorBuffer", lrDepthBuffer.Identifier());
            //Blit(cmd, source, lrDepthBuffer.Identifier(), blitMaterial, 5);   
            // Blit(cmd, source, lrColorB, blitMaterial, 5);
            // Blit(cmd, lrColorB, source);
            //cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
            // return;

            //        lrColorBACK = RenderTexture.GetTemporary(rtW, rtH, 0);
           // cmd.GetTemporaryRT(lrColorBACK.id, opaqueDesc, FilterMode.Bilinear);
            Blit(cmd, source, m_TemporaryColorTexture.Identifier()); //KEEP BACKGROUND
            //Blit(cmd, source, lrColorBACK.Identifier());

            //settings.radialBlurIterations =  Mathf.Clamp((int)settings.radialBlurIterations, 1, 4);
            radialBlurIterations = Mathf.Clamp(radialBlurIterations, 1, 4);

            float ofs = sunShaftBlurRadius * (1.0f / 768.0f);

            //sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            //sunShaftsMaterial.SetVector("_SunPosition", new Vector4(v.x, v.y, v.z, maxRadius));
            sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));

            float adjustX = 0.5f;
            if (v.x < 0.5f) {
                //adjustX = -0.5f;
                float diff = 0.5f - v.x;
                adjustX = adjustX - 0.5f * diff;
            }
            float adjustY = 0.5f;
            if (v.y > 1.25f)
            {
                //adjustX = -0.5f;
                float diff2 = v.y - 1.25f;
                adjustY = adjustY - 0.3f * diff2;
            }
            if (v.y > 1.8f)
            {
                //adjustX = -0.5f;
                v.y = 1.8f;
                float diff3 = v.y - 1.25f;
                adjustY = 0.5f - 0.3f * diff3;
            }

            sheetSHAFTS.SetVector("_SunPosition", new Vector4(v.x * 0.5f + adjustX, v.y * 0.5f + adjustY, v.z, maxRadius));
            //Debug.Log(v.y);

            //TEST2
            //Blit(cmd, lrDepthBuffer.Identifier(), source);
            //cmd.GetTemporaryRT(lrColorB.id, opaqueDesc, filterMode);
            //RenderTexture lrColorBA = RenderTexture.GetTemporary(rtW, rtH, 0);
            // Blit(cmd, lrDepthBuffer.Identifier(), lrColorBA, sheetSHAFTS, 1);
            // Blit(cmd, lrColorBA, source);
            // Blit(cmd, lrDepthBuffer.Identifier(), source);
            // return;
            //RenderTexture.ReleaseTemporary(lrColorB);
            for (int it2 = 0; it2 < radialBlurIterations; it2++)
            {
                // each iteration takes 2 * 6 samples
                // we update _BlurRadius each time to cheaply get a very smooth look

               lrColorB = RenderTexture.GetTemporary(rtW, rtH, 0);               
               
 //               cmd.GetTemporaryRT(lrColorB.id, opaqueDesc, filterMode);
                // Graphics.Blit(lrDepthBuffer, lrColorB, sunShaftsMaterial, 1);

                //             context.command.BlitFullscreenTriangle(lrDepthBuffer, lrColorB, sheetSHAFTS, 1);
                Blit(cmd, lrDepthBuffer.Identifier(), lrColorB, sheetSHAFTS, 1);

 //              RenderTexture.ReleaseTemporary(lrDepthBuffer.Identifier());
                cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
                ofs = sunShaftBlurRadius * (((it2 * 2.0f + 1.0f) * 6.0f)) / 768.0f;
                //sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
                sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));

 //               lrDepthBuffer = RenderTexture.GetTemporary(rtW, rtH, 0);
                cmd.GetTemporaryRT(lrDepthBuffer.id, opaqueDesc, filterMode);

                // Graphics.Blit(lrColorB, lrDepthBuffer, sunShaftsMaterial, 1);
                //              context.command.BlitFullscreenTriangle(lrColorB, lrDepthBuffer, sheetSHAFTS, 1);
                Blit(cmd, lrColorB, lrDepthBuffer.Identifier(), sheetSHAFTS, 1);

               RenderTexture.ReleaseTemporary(lrColorB);
  //              cmd.ReleaseTemporaryRT(lrColorB.id);
                ofs = sunShaftBlurRadius * (((it2 * 2.0f + 2.0f) * 6.0f)) / 768.0f;
                // sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
                sheetSHAFTS.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            }
            
            // put together:

            if (v.z >= 0.0f)
            {
                //sunShaftsMaterial.SetVector("_SunColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, sunColor.a) * sunShaftIntensity);
                sheetSHAFTS.SetVector("_SunColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, sunColor.a) * sunShaftIntensity);
            }
            else
            {
                // sunShaftsMaterial.SetVector("_SunColor", Vector4.zero); // no backprojection !
                sheetSHAFTS.SetVector("_SunColor", Vector4.zero); // no backprojection !
            }
            //sunShaftsMaterial.SetTexture("_ColorBuffer", lrDepthBuffer);
            //         sheetSHAFTS.SetTexture("_ColorBuffer", lrDepthBuffer.);
            cmd.SetGlobalTexture("_ColorBuffer", lrDepthBuffer.Identifier());
            //    Graphics.Blit(context.source, context.destination, sunShaftsMaterial, (screenBlendMode == ShaftsScreenBlendMode.Screen) ? 0 : 4);


            //          context.command.BlitFullscreenTriangle(context.source, context.destination, sheetSHAFTS, (screenBlendMode == BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);
            //Blit(cmd, source, destination.Identifier(), sheetSHAFTS, (screenBlendMode == BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);
           // Blit(cmd, source, destination.Identifier(), sheetSHAFTS, (screenBlendMode == BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);

            // Blit(cmd, source, lrDepthBuffer.Identifier(), blitMaterial, 5);
            Blit(cmd, m_TemporaryColorTexture.Identifier(), source, sheetSHAFTS, (screenBlendMode == BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);
            //Blit(cmd, lrColorBACK.Identifier(), source, sheetSHAFTS, (screenBlendMode == BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen) ? 0 : 4);

            cmd.ReleaseTemporaryRT(lrDepthBuffer.id);
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
            //cmd.ReleaseTemporaryRT(lrColorBACK.id);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            //cmd.ReleaseTemporaryRT(lrColorB.id);
            RenderTexture.ReleaseTemporary(lrColorB);
            //RenderTexture.ReleaseTemporary(lrColorBACK);
            //          RenderTexture.ReleaseTemporary(lrDepthBuffer);

        }


    }
}
