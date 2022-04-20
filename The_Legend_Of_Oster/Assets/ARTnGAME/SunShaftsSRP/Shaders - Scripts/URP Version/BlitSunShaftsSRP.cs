using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.Universal
{
    public class BlitSunShaftsSRP : UnityEngine.Rendering.Universal.ScriptableRendererFeature
    {
        [System.Serializable]
        public class BlitSettings
        {
            public UnityEngine.Rendering.Universal.RenderPassEvent Event = UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingOpaques;
            
            public Material blitMaterial = null;
            public int blitMaterialPassIndex = -1;
            public Target destination = Target.Color;
            public string textureId = "_BlitPassTexture";

            /////SUN SHAFTS
            [Range(0f, 1f), Tooltip("SunShafts effect intensity.")]
            public float blend = 0.5f;

            public enum SunShaftsResolution
            {
                Low = 0,
                Normal = 1,
                High = 2,
            }

            public enum ShaftsScreenBlendMode
            {
                Screen = 0,
                Add = 1,
            }

            public SunShaftsResolution resolution = SunShaftsResolution.Normal;
            public ShaftsScreenBlendMode screenBlendMode = ShaftsScreenBlendMode.Screen;
            public Vector3 sunTransform =  new Vector3(0f, 0f, 0f); // Transform sunTransform;
            public int radialBlurIterations = 2 ;
            public Color sunColor = Color.white ;
            public Color sunThreshold =new Color(0.87f, 0.74f, 0.65f) ;
            public float sunShaftBlurRadius = 2.5f ;
            public float sunShaftIntensity = 1.15f ;
            public float maxRadius = 0.75f ;
            public bool useDepthTexture = true ;
        }
        
        public enum Target
        {
            Color,
            Texture
        }

        public BlitSettings settings = new BlitSettings();
        UnityEngine.Rendering.Universal.RenderTargetHandle m_RenderTextureHandle;

        BlitPassSunShaftsSRP blitPass;

        
        public override void Create()
        {
            var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
                        

            blitPass = new BlitPassSunShaftsSRP(settings.Event, settings.blitMaterial, settings.blitMaterialPassIndex, name, settings);
            m_RenderTextureHandle.Init(settings.textureId);
        }

        public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {

            //v0.4
#if UNITY_2020_2_OR_NEWER           
                if (settings.blitMaterial == null)
                {
                    Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                    return;
                }

                blitPass.renderPassEvent = settings.Event;
                blitPass.settings = settings;

                renderer.EnqueuePass(blitPass);           
#else
            var src = renderer.cameraColorTarget;
            var dest = (settings.destination == Target.Color) ? UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget : m_RenderTextureHandle;

            if (settings.blitMaterial == null)
            {
                Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }

            blitPass.Setup(src, dest);
            renderer.EnqueuePass(blitPass);
#endif
        }
    }
}

