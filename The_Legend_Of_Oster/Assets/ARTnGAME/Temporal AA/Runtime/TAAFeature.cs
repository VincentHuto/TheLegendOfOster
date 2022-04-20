﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Naiwen.TAA
{

    sealed class TAAFeature : ScriptableRendererFeature
    {
        #region Fields
        static ScriptableRendererFeature s_Instance;
        //readonly CameraSettingPass m_cameraSettingPass;//v0.4
        // readonly TAAPass m_TAAPass;//v0.4
        CameraSettingPass m_cameraSettingPass;//v0.4
        TAAPass m_TAAPass;//v0.4
        Dictionary<Camera, TAAData> m_TAADatas;
        Matrix4x4 previewView;
        Matrix4x4 previewProj;
        #endregion

        internal TAAFeature()
        {
            // Set data
            s_Instance = this;
            //m_cameraSettingPass = new CameraSettingPass(); //v0.4
            // m_TAAPass = new TAAPass();//v0.4
            // m_TAADatas = new Dictionary<Camera, TAAData>();//v0.4
        }
        void OnEnable()
        {
            m_cameraSettingPass = new CameraSettingPass(); //v0.4
            m_TAAPass = new TAAPass();//v0.4
            m_TAADatas = new Dictionary<Camera, TAAData>();//v0.4
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var camera = renderingData.cameraData.camera;
            TAAData TaaData;

            //v0.4
            if (m_cameraSettingPass == null)
            {
                m_cameraSettingPass = new CameraSettingPass(); //v0.4
            }
            if (m_TAAPass == null)
            {
                m_TAAPass = new TAAPass();//v0.4
            }
            if (m_TAADatas == null)
            {
                m_TAADatas = new Dictionary<Camera, TAAData>();//v0.4
            }

            if (m_TAADatas != null)//v0.4
            {

                if (!m_TAADatas.TryGetValue(camera, out TaaData))
                {
                    TaaData = new TAAData();
                    m_TAADatas.Add(camera, TaaData);
                }
                var stack = VolumeManager.instance.stack;
                var TaaComonent = stack.GetComponent<TemporalAntiAliasing>();
                if (TaaComonent.IsActive() && !renderingData.cameraData.isSceneViewCamera)
                {
                    UpdateTAAData(renderingData, TaaData, TaaComonent);
                    m_cameraSettingPass.Setup(TaaData);
                    renderer.EnqueuePass(m_cameraSettingPass);
                    m_TAAPass.Setup(TaaData, TaaComonent);
                    renderer.EnqueuePass(m_TAAPass);
                }
                else if (!TaaComonent.IsActive())
                {
                    m_TAAPass.Clear();
                }
            }
            
        }

        public override void Create()
        {
            name = "TAA";
        }

        void UpdateTAAData(RenderingData renderingData, TAAData TaaData, TemporalAntiAliasing Taa)
        {
            Camera camera = renderingData.cameraData.camera;
            Vector2 additionalSample = Utils.GenerateRandomOffset()* Taa.spread.value;
            TaaData.sampleOffset = additionalSample;
            TaaData.porjPreview = previewProj;
            TaaData.viewPreview = previewView;
            TaaData.projOverride = camera.orthographic
                       ? Utils.GetJitteredOrthographicProjectionMatrix(camera, TaaData.sampleOffset)
                       : Utils.GetJitteredPerspectiveProjectionMatrix(camera, TaaData.sampleOffset);
            TaaData.sampleOffset = new Vector2(TaaData.sampleOffset.x / camera.scaledPixelWidth, TaaData.sampleOffset.y / camera.scaledPixelHeight);
            previewView = camera.worldToCameraMatrix;
            previewProj = camera.projectionMatrix;
        }
    }
}
