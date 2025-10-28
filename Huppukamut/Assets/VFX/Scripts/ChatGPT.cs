using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;  // may be required for newer APIs

public class DepthNormalsFeature : ScriptableRendererFeature
{
    class RenderPass : ScriptableRenderPass
    {
        private Material _material;
        private RTHandle _depthNormalsHandle;
        private List<ShaderTagId> _shaderTags;
        private FilteringSettings _filteringSettings;

        public RenderPass(Material material)
        {
            _material = material;

            _shaderTags = new List<ShaderTagId>()
            {
                new ShaderTagId("DepthOnly"),
                // can add others if needed
            };

            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            // Set injection point:
            this.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Prepare the RTHandle for depth+normals texture
            var cameraDesc = renderingData.cameraData.cameraTargetDescriptor;
            // we zero out depth buffer bits for color target
            cameraDesc.depthBufferBits = 0;

            // Allocate or reallocate the RTHandle
            RenderingUtils.ReAllocateIfNeeded(
                ref _depthNormalsHandle,
                cameraDesc,
                FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_DepthNormalsTexture"
            );

            ConfigureTarget(_depthNormalsHandle);
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("DepthNormalsPass");

            using (new ProfilingScope(cmd, new ProfilingSampler("DepthNormalsPass")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var drawSettings = CreateDrawingSettings(_shaderTags, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                drawSettings.overrideMaterial = _material;

                var cullResults = renderingData.cullResults;
                context.DrawRenderers(cullResults, ref drawSettings, ref _filteringSettings);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // We release the handle only when disposing this pass
            // But if you want you can release per-camera (optional)
            _depthNormalsHandle?.Release();
            _depthNormalsHandle = null;
        }
    }

    [SerializeField]
    private Material depthNormalsMaterial = null;

    private RenderPass _renderPass;

    public override void Create()
    {
        if (depthNormalsMaterial == null)
        {
            depthNormalsMaterial = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
        }

        _renderPass = new RenderPass(depthNormalsMaterial);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        // If you needed to store camera target handle or other setup, do it here.
        // In this case we don't need special setup, so nothing here.
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_renderPass);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            if (depthNormalsMaterial != null)
            {
                CoreUtils.Destroy(depthNormalsMaterial);
                depthNormalsMaterial = null;
            }
        }
    }
}