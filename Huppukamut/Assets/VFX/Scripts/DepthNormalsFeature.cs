// MIT License
// Copyright (c) 2021 NedMakesGames
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class DepthNormalsFeature : ScriptableRendererFeature
{
    private Material material;
    private DepthNormalsRenderPass renderPass;

    public override void Create()
    {
        material = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
        renderPass = new DepthNormalsRenderPass(material);
        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderPass == null) return;
        renderer.EnqueuePass(renderPass);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CoreUtils.Destroy(material);
        }
    }

    class DepthNormalsRenderPass : ScriptableRenderPass
    {
        private Material material;
        private List<ShaderTagId> shaderTags;
        private FilteringSettings filteringSettings;

        public DepthNormalsRenderPass(Material material)
        {
            this.material = material;
            shaderTags = new List<ShaderTagId>
            {
                new ShaderTagId("DepthOnly"),
                //new ShaderTagId("SRPDefaultUnlit"),
                //new ShaderTagId("UniversalForward"),
                //new ShaderTagId("LightweightForward"),
            };
            filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // Access URP data from ContextContainer
            if (!frameData.Contains<UniversalRenderingData>() ||
                !frameData.Contains<UniversalCameraData>() ||
                !frameData.Contains<UniversalLightData>()) return;

            var universalRenderingData = frameData.Get<UniversalRenderingData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            var lightData = frameData.Get<UniversalLightData>();

            using var builder = renderGraph.AddRasterRenderPass<PassData>("DepthNormals", out var passData);

            // Create texture descriptor based on camera
            var cameraDesc = cameraData.cameraTargetDescriptor;
            var texDesc = new TextureDesc(cameraDesc)
            {
                name = "_DepthNormalsTexture",
                depthBufferBits = 0 // Color-only texture
            };

            var destination = renderGraph.CreateTexture(texDesc);

            // Create drawing settings using split URP data (single ShaderTagId for this overload)
            var sortFlags = cameraData.defaultOpaqueSortFlags;
            var drawSettings = RenderingUtils.CreateDrawingSettings(shaderTags[0], universalRenderingData, cameraData, lightData, sortFlags);
            drawSettings.overrideMaterial = material;

            // Create renderer list params
            var rendererListParams = new RendererListParams(universalRenderingData.cullResults, drawSettings, filteringSettings);

            // Create and use renderer list
            passData.rendererList = renderGraph.CreateRendererList(rendererListParams);
            builder.UseRendererList(passData.rendererList);

            // Set render target (color attachment)
            builder.SetRenderAttachment(destination, 0);

            // Expose as global texture for shader sampling
            builder.SetGlobalTextureAfterPass(destination, Shader.PropertyToID("_DepthNormalsTexture"));

            // Set render function
            builder.SetRenderFunc<PassData>(Execute);
        }

        private class PassData
        {
            public RendererListHandle rendererList;
        }

        private static void Execute(PassData data, RasterGraphContext context)
        {
            // Clear the render target to black (color only, no depth)
            context.cmd.ClearRenderTarget(false, true, Color.black);

            // Draw the objects in the list
            context.cmd.DrawRendererList(data.rendererList);
        }
    }
}