using UnityEngine;
using UnityEngine.Rendering;


// Inheriting from `ScriptableRendererFeature` will add it to the
// `Renderer Features` list of the custom LWRP renderer data asset.
public class RenderRefractPassSM : UnityEngine.Rendering.Universal.ScriptableRendererFeature
{
    private class MyCustomPass : UnityEngine.Rendering.Universal.ScriptableRenderPass
    {
        // Just a tag used to pick up a buffer from the pool.
        private const string commandBufferName = nameof(MyCustomPass);
        // Corresponds to `Tags { "LightMode" = "MyCustomPass" }` in the shaders.
        // You have to add this tag for the corresponding shaders to associate them with this pass.
        private static readonly ShaderTagId shaderTag = new ShaderTagId(nameof(MyCustomPass));
        // An arbitrary name to store temporary render texture.
        private static readonly int tempRTPropertyId = Shader.PropertyToID("_TempRT");
        // Name of the grab texture used in the shaders.
        private static readonly int grabTexturePropertyId = Shader.PropertyToID("_MyGrabTexture");

        public MyCustomPass()
        {
            renderPassEvent = UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingTransparents;
        }

        public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            // Grab screen texture and assign it to a global texture property.
            var cmd = CommandBufferPool.Get(commandBufferName);
            cmd.GetTemporaryRT(tempRTPropertyId, renderingData.cameraData.cameraTargetDescriptor);
            cmd.Blit(BuiltinRenderTextureType.CameraTarget, tempRTPropertyId);
            cmd.SetGlobalTexture(grabTexturePropertyId, tempRTPropertyId);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);

            // Draw the objects that are using materials associated with this pass.
            var drawingSettings = CreateDrawingSettings(shaderTag, ref renderingData, SortingCriteria.CommonTransparent);
            var filteringSettings = new FilteringSettings(RenderQueueRange.transparent);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);

            cmd.ReleaseTemporaryRT(tempRTPropertyId);
        }
    }

    private MyCustomPass grabScreenPass;

    public override void Create()
    {
        grabScreenPass = new MyCustomPass();
    }

    public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
    {
        renderer.EnqueuePass(grabScreenPass);
    }
}