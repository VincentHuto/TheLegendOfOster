#define URP

using UnityEngine;
using System.Collections.Generic;
//using Unity.Mathematics;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

//using UnityEngine.Experimental.Rendering.LWRP;
#if URP
using UnityEngine.Rendering.Universal;
#endif

[ExecuteAlways]//[ImageEffectAllowedInSceneView] //[ExecuteAlways]
public class DepthRendererSM_LWRP : MonoBehaviour//, IBeforeCameraRender
{
    #if URP
    [System.Serializable]
    public enum ResolutionMulltiplier
    {
        Full,
        Half,
        Third,
        Quarter
    }
    public int depthResolution = 2048;
    [System.Serializable]
    public class PlanarReflectionSettings
    {
        public ResolutionMulltiplier m_ResolutionMultiplier = ResolutionMulltiplier.Third;
        public float m_ClipPlaneOffset = 0.07f;
        public LayerMask m_ReflectLayers = -1;
    }

    public List<Material> Materials = new List<Material>();
    public string depthTextureName = "_TopDownDepthTexture";

    [SerializeField]
    public PlanarReflectionSettings m_settings = new PlanarReflectionSettings();

    public GameObject target;
    [FormerlySerializedAs("camOffset")] public float m_planeOffset;

   // private static Camera m_ReflectionCamera;
    public Camera m_ReflectionCamera;

    private Vector2 m_TextureSize = new Vector2(256, 128);
    private RenderTexture m_ReflectionTexture = null;
    //private int planarReflectionTextureID = Shader.PropertyToID("_RefractionTextureSM");
    private int planarReflectionTextureID;
    public string refractionTextureName = "_ShoreContourTex";//"_topDownDepthSM";
    public float depthCameraHeight = 1000;

    private Vector2 m_OldReflectionTextureSize;

    // Cleanup all the objects we possibly have created
    private void OnDisable()
    {
        prevRenderer = -1;
        Cleanup();
    }

    private void OnDestroy()
    {
        prevRenderer = -1;
        Cleanup();
    }
    public bool clearAtStart = false;
    void Start()
    {
        if (clearAtStart) //use for full refraction case
        {
            Cleanup();
            RenderPipelineManager.beginCameraRendering += DoDepthSM;
            //RenderPipelineManager.endCameraRendering += DoReflections;

            planarReflectionTextureID = Shader.PropertyToID(refractionTextureName);
        }

       
        prevRenderer = -1;
}

    //NEW
    //sample code 
    //https://github.com/0lento/OcclusionProbes/blob/package/OcclusionProbes/OcclusionProbes.cs#L41
    private void OnEnable()
    {
        //v0.1
        if (m_ReflectionCamera && prevRenderer != rendererID)
        {
            UniversalAdditionalCameraData thisCameraData = m_ReflectionCamera.gameObject.GetComponent<UniversalAdditionalCameraData>();
            thisCameraData.SetRenderer(rendererID);
            prevRenderer = rendererID;
        }

        RenderPipelineManager.beginCameraRendering += DoDepthSM;
        //RenderPipelineManager.endCameraRendering += DoReflections;

        planarReflectionTextureID = Shader.PropertyToID(refractionTextureName);
    }
    //END NEW

    void Cleanup()
    {
        if (m_ReflectionCamera)
        {
            m_ReflectionCamera.targetTexture = null;
            SafeDestroy(m_ReflectionCamera.gameObject);
        }
        if (m_ReflectionTexture)
        {
            RenderTexture.ReleaseTemporary(m_ReflectionTexture);
        }
        if (m_ReflectionTextureDEPTH)
        {
            RenderTexture.ReleaseTemporary(m_ReflectionTextureDEPTH);
        }
        //new
        RenderPipelineManager.beginCameraRendering -= DoDepthSM;
        //RenderPipelineManager.endCameraRendering += DoReflections;

        prevRenderer = -1;
    }

    void SafeDestroy(Object obj)
    {
        if (Application.isEditor)
        {
            DestroyImmediate(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    private void UpdateCamera(Camera src, Camera dest)
    {
        if (dest == null)
            return;
        //dest.CopyFrom(src);
    }

    //void Update()
    //{
    //    //apply to materials
    //    for (int i = 0; i < Materials.Count; i++)
    //    {
    //        Materials[i].SetTexture(depthTextureName, m_ReflectionTextureDEPTH);
    //        Materials[i].SetVector("_DepthCameraPos", new Vector4(Camera.main.transform.position.x, depthCameraHeight, Camera.main.transform.position.z, 1));
    //    }
    //}

    private void UpdateReflectionCamera(Camera realCamera)
    {

        planarReflectionTextureID = Shader.PropertyToID(refractionTextureName); //////////// reset name if changed

        if (m_ReflectionCamera == null)
            m_ReflectionCamera = CreateMirrorObjects(realCamera);


        //v0.1
        UniversalAdditionalCameraData thisCameraData = m_ReflectionCamera.gameObject.GetComponent<UniversalAdditionalCameraData>();
        if (m_ReflectionCamera && rendererID != prevRenderer)// && thisCameraData.scriptableRenderer. != rendererID)
        {
            //UniversalAdditionalCameraData thisCameraData = m_ReflectionCamera.gameObject.GetComponent<UniversalAdditionalCameraData>();
            thisCameraData.SetRenderer(rendererID);
            prevRenderer = rendererID;
        }

        // find out the reflection plane: position and normal in world space
        Vector3 pos = Vector3.zero;
        Vector3 normal = Vector3.up;
        if (target != null)
        {
            pos = target.transform.position + Vector3.up * m_planeOffset;
            normal = target.transform.up;
        }

        UpdateCamera(realCamera, m_ReflectionCamera);

        // Render reflection
        // Reflect camera around reflection plane
        float d = -Vector3.Dot(normal, pos) - m_settings.m_ClipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        Matrix4x4 reflection = Matrix4x4.identity;
        reflection *= Matrix4x4.Scale(new Vector3(1, -1, 1));

        CalculateReflectionMatrix(ref reflection, reflectionPlane);
        Vector3 oldpos = realCamera.transform.position - new Vector3(0, pos.y * 2, 0);
        Vector3 newpos = ReflectPosition(oldpos);
    //    m_ReflectionCamera.transform.forward = Vector3.Scale(realCamera.transform.forward, new Vector3(1, -1, 1));
    //    m_ReflectionCamera.worldToCameraMatrix = realCamera.worldToCameraMatrix * reflection;

        // Setup oblique projection matrix so that near plane is our reflection
        // plane. This way we clip everything below/above it for free.
        Vector4 clipPlane = CameraSpacePlane(m_ReflectionCamera, pos - Vector3.up * 0.1f, normal, 1.0f);
        Matrix4x4 projection = realCamera.CalculateObliqueMatrix(clipPlane);
  //      m_ReflectionCamera.projectionMatrix = projection;
        m_ReflectionCamera.cullingMask = m_settings.m_ReflectLayers; // never render water layer
                                                                     //      m_ReflectionCamera.transform.position = newpos;

        
    }

    // Calculates reflection matrix around the given plane
    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }

    private static Vector3 ReflectPosition(Vector3 pos)
    {
        Vector3 newPos = new Vector3(pos.x, -pos.y, pos.z);
        return newPos;
    }

    private float GetScaleValue()
    {
        switch (m_settings.m_ResolutionMultiplier)
        {
            case ResolutionMulltiplier.Full:
                return 1f;
            case ResolutionMulltiplier.Half:
                return 0.5f;
            case ResolutionMulltiplier.Third:
                return 0.33f;
            case ResolutionMulltiplier.Quarter:
                return 0.25f;
        }
        return 0.5f; // default to half res
    }

    // Compare two int2
    //private static bool Int2Compare(int2 a, int2 b)
    //{
    //    if (a.x == b.x && a.y == b.y)
    //        return true;
    //    else
    //        return false;
    //}

    // Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * m_settings.m_ClipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    private Camera CreateMirrorObjects(Camera currentCamera)
    {
        GameObject go =
            new GameObject("Depth Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(),
                typeof(Camera));
        UnityEngine.Rendering.Universal.UniversalAdditionalCameraData lwrpCamData =
            go.AddComponent(typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)) as UnityEngine.Rendering.Universal.UniversalAdditionalCameraData;
        lwrpCamData.renderShadows = false; // turn off shadows for the reflection camera
        lwrpCamData.requiresColorOption = UnityEngine.Rendering.Universal.CameraOverrideOption.Off;
        lwrpCamData.requiresDepthOption = UnityEngine.Rendering.Universal.CameraOverrideOption.On;
        var reflectionCamera = go.GetComponent<Camera>();
        ////////reflectionCamera.transform.SetPositionAndRotation(transform.position, transform.rotation);
        //reflectionCamera.targetTexture = m_ReflectionTexture;

        reflectionCamera.transform.position = currentCamera.transform.position + new Vector3(0, depthCameraHeight, 0);
        reflectionCamera.transform.forward = -Vector3.up;

        reflectionCamera.allowMSAA = currentCamera.allowMSAA;
        reflectionCamera.depth = -10;
        reflectionCamera.enabled = false;
        reflectionCamera.allowHDR = currentCamera.allowHDR;
        go.hideFlags = HideFlags.DontSave;

        return reflectionCamera;
    }

    private Vector2 ReflectionResolution(Camera cam, float scale)
    {
        var x = (int)(cam.pixelWidth * scale * GetScaleValue());
        var y = (int)(cam.pixelHeight * scale * GetScaleValue());
        return new Vector2(x, y);
    }

    // public void ExecuteBeforeCameraRender(
    //     LightweightRenderPipeline pipelineInstance,
    //     ScriptableRenderContext context,
    //     Camera camera)
    RenderTexture m_ReflectionTextureDEPTH;


    public bool updatePerDistance = true;
    public Vector3 previousPos = Vector3.zero;
    public float updateDistance = 20;
    //NEW

    //v0.1
    public int rendererID = 1;
    public int prevRenderer = -1;

    public void DoDepthSM(ScriptableRenderContext context, Camera camera)
    {

        camera = Camera.main;

        //v0.1
        if (m_ReflectionCamera && prevRenderer != rendererID)
        {
            UniversalAdditionalCameraData thisCameraData = m_ReflectionCamera.gameObject.GetComponent<UniversalAdditionalCameraData>();
            thisCameraData.SetRenderer(rendererID);
            prevRenderer = rendererID;
        }

        //Debug.Log("in " + previousPos);
        // Debug.Log("in1 " + camera.transform.position);
        //if(updatePerDistance && (camera.transform.position - prevPos).mag )
        if (Application.isPlaying && Time.fixedTime > 0.1f && updatePerDistance && (camera.transform.position - previousPos).magnitude < updateDistance)
        {
            return;
        }
        
        previousPos = camera.transform.position;

            //if (!enabled)
            // return;

        //GL.invertCulling = true; /////////////////////////// no cull in refractions
        // RenderSettings.fog = false;
        var max = QualitySettings.maximumLODLevel;
        var bias = QualitySettings.lodBias;
        QualitySettings.maximumLODLevel = 1;
        QualitySettings.lodBias = bias * 0.5f;

        UpdateReflectionCamera(camera);

        var res = ReflectionResolution(camera, 1);
        if (m_ReflectionTexture == null)
        {
            m_ReflectionTexture = RenderTexture.GetTemporary((int)res.x, (int)res.y, 16, RenderTextureFormat.DefaultHDR);
        }

        if (m_ReflectionTextureDEPTH == null)
        {
            // m_ReflectionTextureDEPTH = RenderTexture.GetTemporary(res.x, res.y, 24, RenderTextureFormat.Depth);
           m_ReflectionTextureDEPTH = RenderTexture.GetTemporary(depthResolution, depthResolution, 24, RenderTextureFormat.Depth); //URP
           // m_ReflectionTextureDEPTH = RenderTexture.GetTemporary(depthResolution, depthResolution, 24, RenderTextureFormat.ARGBFloat);
        }
        m_ReflectionCamera.targetTexture = m_ReflectionTextureDEPTH;// m_ReflectionTexture;
        m_ReflectionCamera.depthTextureMode = DepthTextureMode.Depth;

        //   m_ReflectionCamera.SetTargetBuffers(m_ReflectionTexture.colorBuffer, m_ReflectionTextureDEPTH.depthBuffer);      
        //   m_ReflectionCamera.targetTexture = null;

        //new
        //m_ReflectionCamera.CopyFrom(camera); 


        //INFINIGRASS
        //       m_ReflectionCamera.farClipPlane = camera.farClipPlane;
        ///m_ReflectionCamera.transform.position
        m_ReflectionCamera.orthographic = true;
        m_ReflectionCamera.transform.position = new Vector3(camera.transform.position.x, depthCameraHeight, camera.transform.position.z);
        m_ReflectionCamera.farClipPlane = depthCameraHeight;
        m_ReflectionCamera.orthographicSize = depthCameraHeight;

        // m_ReflectionCamera.projectionMatrix = camera.projectionMatrix;
        //m_ReflectionCamera.worldToCameraMatrix = camera.worldToCameraMatrix;
        // m_ReflectionCamera.transform.position = camera.transform.position + new Vector3(0,depthCameraHeight,0);
        m_ReflectionCamera.transform.forward = -Vector3.up; // m_ReflectionCamera.transform.rotation = camera.transform.rotation;

        //m_ReflectionCamera.clearFlags = CameraClearFlags.SolidColor;
        //LightweightRenderPipeline.RenderSingleCamera(pipelineInstance, context, m_ReflectionCamera);
        Vector3 prevPos = camera.transform.position;
        Quaternion prevPRot = camera.transform.rotation;

        //camera.transform.position = m_ReflectionCamera.transform.position;
        //camera.transform.rotation = m_ReflectionCamera.transform.rotation;

        //LightweightRenderPipeline.RenderSingleCamera(context, camera);
        //camera.transform.position = prevPos;
        //camera.transform.rotation = prevPRot;

        UnityEngine.Rendering.Universal.UniversalRenderPipeline.RenderSingleCamera(context, m_ReflectionCamera);

        GL.invertCulling = false;
        //RenderSettings.fog = true;
        QualitySettings.maximumLODLevel = max;
        QualitySettings.lodBias = bias;
        Shader.SetGlobalTexture(planarReflectionTextureID, m_ReflectionTextureDEPTH);

        //apply to materials
        for (int i = 0; i < Materials.Count; i++)
        {
            Materials[i].SetTexture(depthTextureName, m_ReflectionTextureDEPTH);
            Materials[i].SetVector("_DepthCameraPos", new Vector4(Camera.main.transform.position.x, depthCameraHeight, Camera.main.transform.position.z,1));
            //Materials[i].SetFloat("_TerrainScale", depthCameraHeight);
        }
        //Debug.Log(Camera.main.transform.position);

        //if (1 == 0)
        //{
        //    if (testTex == null)
        //    {
        //        testTex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        //    }
        //    //Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        //    RenderTexture.active = m_ReflectionTexture;
        //    testTex.ReadPixels(new Rect(0, 0, m_ReflectionTexture.width, m_ReflectionTexture.height), 0, 0);
        //    testTex.Apply();
        //}

        //m_ReflectionCamera.tex
        //Shader.SetGlobalTexture(planarReflectionTextureID, m_ReflectionTexture.depthBuffer);
        //m_ReflectionTexture.de
    }

    void OnGUI1()
    {
        testTex = toTexture2D(m_ReflectionTexture);
        if (testTex != null && Event.current.type.Equals(EventType.Repaint))
        {
            Graphics.DrawTexture(new Rect(10, 10, testTex.width, testTex.height), testTex);
            //Graphics.DrawTexture(new Rect(testTex.width + 10, 10, testTex2.width, testTex2.height), testTex2);
        }
    }
    public Texture2D testTex;

    Texture2D toTexture2D(RenderTexture rTex)
    {
        if (testTex == null)
        {
            testTex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        }
        //Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        testTex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        testTex.Apply();
        return testTex;
    }

#endif
}