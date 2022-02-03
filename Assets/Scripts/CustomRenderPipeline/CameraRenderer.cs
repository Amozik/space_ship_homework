using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomRenderPipeline
{
    public partial class CameraRenderer
    {
        private const string BUFFER_NAME = "Camera Render";
        
        private readonly CommandBuffer _commandBuffer = new CommandBuffer {name = BUFFER_NAME};

        private static readonly List<ShaderTagId> drawingShaderTagIds = new List<ShaderTagId>
        {
            new ShaderTagId("SRPDefaultUnlit"),
        };
        
        private ScriptableRenderContext _context;
        private Camera _camera;
        private CullingResults _cullingResult;

        public void Render(ScriptableRenderContext context, Camera camera)
        {
            _camera = camera;
            _context = context;

            if (!Cull(out var parameters))
            {
                return;
            }
            
            Settings(parameters);
            DrawVisible();
            DrawUnsupportedShaders();
            DrawGizmos();
            Submit();

        }
        
        private bool Cull(out ScriptableCullingParameters parameters)
        {
            return _camera.TryGetCullingParameters(out parameters);
        }

        private void Settings(ScriptableCullingParameters parameters)
        {
            _cullingResult = _context.Cull(ref parameters);
            _context.SetupCameraProperties(_camera);
            _commandBuffer.ClearRenderTarget(true, true, Color.clear);
            _commandBuffer.BeginSample(BUFFER_NAME);
            ExecuteCommandBuffer();
        }

        private void Submit()
        {
            _commandBuffer.EndSample(BUFFER_NAME);
            ExecuteCommandBuffer();
            _context.Submit();
        }
        private void ExecuteCommandBuffer()
        {
            _context.ExecuteCommandBuffer(_commandBuffer);
            _commandBuffer.Clear();
        }

        private DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTags, SortingCriteria sortingCriteria,
            out SortingSettings sortingSettings)
        {
            sortingSettings = new SortingSettings(_camera)
            {
                criteria = sortingCriteria,
            };
            var drawingSettings = new DrawingSettings(shaderTags[0], sortingSettings);
            
            for (var i = 1; i < shaderTags.Count; i++)
            {
                drawingSettings.SetShaderPassName(i, shaderTags[i]);
            }
            
            return drawingSettings;
        }

        private void DrawVisible()
        {
            var drawingSettings = CreateDrawingSettings(drawingShaderTagIds, SortingCriteria.CommonOpaque, out var sortingSettings);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            
            _context.DrawRenderers(_cullingResult, ref drawingSettings, ref filteringSettings);
            
            _context.DrawSkybox(_camera);
            
            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            
            _context.DrawRenderers(_cullingResult, ref drawingSettings, ref filteringSettings);
        }
    }
}