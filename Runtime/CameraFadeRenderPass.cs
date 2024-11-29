using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif

/// <summary>
/// The camera fade render pass.
/// </summary>
public sealed class CameraFadeRenderPass : ScriptableRenderPass
{
	#region Definitions

#if UNITY_6000_0_OR_NEWER

	/// <summary>
	/// Holds the data needed by the execution of the camera fade render pass subpasses.
	/// </summary>
	private class PassData
	{
		public TextureHandle target;
		public TextureHandle source;

		public Material material;
		public int materialPassIndex;

		public TextureHandle blitTextureHandle;
	}

#endif

	#endregion

	#region Private Attributes

	private static readonly int ColorId = Shader.PropertyToID("_Color");
	private static readonly int ProgressId = Shader.PropertyToID("_Progress");

	private Material cameraFadeMaterial;
	private RTHandle blitRtHandle;

	#endregion

	#region Initialization Methods

	public CameraFadeRenderPass(Material cameraFadeMaterial) : base()
	{
		profilingSampler = new ProfilingSampler("Camera Fade");
		renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

#if UNITY_6000_0_OR_NEWER
		requiresIntermediateTexture = false;
#endif

		this.cameraFadeMaterial = cameraFadeMaterial;
	}

	#endregion

	#region Scriptable Render Pass Methods

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="cmd"></param>
	/// <param name="renderingData"></param>
	public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
	{
		base.OnCameraSetup(cmd, ref renderingData);

		RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
		cameraTargetDescriptor.depthBufferBits = (int)DepthBits.None;

		RenderingUtils.ReAllocateIfNeeded(ref blitRtHandle, cameraTargetDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_CameraFadeTarget");
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="context"></param>
	/// <param name="renderingData"></param>
	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		CommandBuffer cmd = CommandBufferPool.Get();

		using (new ProfilingScope(cmd, profilingSampler))
		{
			CameraFadeVolumeComponent cameraFadeVolume = VolumeManager.instance.stack.GetComponent<CameraFadeVolumeComponent>();

			cameraFadeMaterial.SetColor(ColorId, cameraFadeVolume.color.value);
			cameraFadeMaterial.SetFloat(ProgressId, cameraFadeVolume.progress.value);

			RTHandle cameraColorRt = renderingData.cameraData.renderer.cameraColorTargetHandle;
			Blitter.BlitCameraTexture(cmd, cameraColorRt, blitRtHandle, cameraFadeMaterial, 0);
			Blitter.BlitCameraTexture(cmd, blitRtHandle, cameraColorRt);
		}

		context.ExecuteCommandBuffer(cmd);

		cmd.Clear();

		CommandBufferPool.Release(cmd);
	}

#if UNITY_6000_0_OR_NEWER

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="renderGraph"></param>
	/// <param name="frameData"></param>
	public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
	{
		UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
		UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

		CreateRenderGraphTextures(renderGraph, cameraData, out TextureHandle blitTextureHandle);

		using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Camera Fade Pass", out PassData passData, profilingSampler))
		{
			passData.source = resourceData.cameraColor;
			passData.target = blitTextureHandle;
			passData.material = cameraFadeMaterial;
			passData.materialPassIndex = 0;

			builder.SetRenderAttachment(blitTextureHandle, 0);
			builder.UseTexture(resourceData.cameraColor);
			builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
		}

		resourceData.cameraColor = blitTextureHandle;
	}

#endif

	#endregion

	#region Methods

#if UNITY_6000_0_OR_NEWER

	/// <summary>
	/// Creates and returns all the necessary render graph textures.
	/// </summary>
	/// <param name="renderGraph"></param>
	/// <param name="cameraData"></param>
	/// <param name="blitTextureHandle"></param>
	private void CreateRenderGraphTextures(RenderGraph renderGraph, UniversalCameraData cameraData, out TextureHandle blitTextureHandle)
	{
		RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
		cameraTargetDescriptor.depthBufferBits = (int)DepthBits.None;

		blitTextureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_CameraFade", false);
	}

	/// <summary>
	/// Executes the pass with the information from the pass data.
	/// </summary>
	/// <param name="passData"></param>
	/// <param name="context"></param>
	private static void ExecutePass(PassData passData, RasterGraphContext context)
	{
		CameraFadeVolumeComponent cameraFadeVolume = VolumeManager.instance.stack.GetComponent<CameraFadeVolumeComponent>();

		Material cameraFadeMaterial = passData.material;

		cameraFadeMaterial.SetColor(ColorId, cameraFadeVolume.color.value);
		cameraFadeMaterial.SetFloat(ProgressId, cameraFadeVolume.progress.value);

		Blitter.BlitTexture(context.cmd, passData.source, Vector2.one, passData.material, passData.materialPassIndex);
	}

#endif

	/// <summary>
	/// Disposes the resources used by this pass.
	/// </summary>
	public void Dispose()
	{
		blitRtHandle?.Release();
	}

	#endregion
}