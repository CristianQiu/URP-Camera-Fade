using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

/// <summary>
/// The camera fade render pass.
/// </summary>
public sealed class CameraFadeRenderPass : ScriptableRenderPass
{
	#region Definitions

	/// <summary>
	/// Holds the data needed by the execution of the render pass.
	/// </summary>
	private class PassData
	{
		public Material material;
		public int materialPassIndex;
	}

	#endregion

	#region Private Attributes

	private static readonly int ColorId = Shader.PropertyToID("_Color");
	private static readonly int ProgressId = Shader.PropertyToID("_Progress");

	private Material material;

	#endregion

	#region Initialization Methods

	public CameraFadeRenderPass(Material material) : base()
	{
		profilingSampler = new ProfilingSampler("Camera Fade");
		renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
		requiresIntermediateTexture = false;

		this.material = material;
	}

	#endregion

	#region Scriptable Render Pass Methods

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="renderGraph"></param>
	/// <param name="frameData"></param>
	public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
	{
		UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
		TextureHandle blitTextureHandle = CreateRenderGraphTextureHandle(renderGraph, resourceData);

		using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Camera Fade", out PassData passData, profilingSampler))
		{
			passData.material = material;
			passData.materialPassIndex = 0;

			builder.SetRenderAttachment(blitTextureHandle, 0, AccessFlags.WriteAll);
			builder.UseTexture(resourceData.cameraColor);
			builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
		}

		resourceData.cameraColor = blitTextureHandle;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Creates and returns the necessary render graph texture handle to blit to.
	/// </summary>
	/// <param name="renderGraph"></param>
	/// <param name="resourceData"></param>
	/// <returns></returns>
	private TextureHandle CreateRenderGraphTextureHandle(RenderGraph renderGraph, UniversalResourceData resourceData)
	{
		TextureDesc cameraColorDescriptor = renderGraph.GetTextureDesc(resourceData.cameraColor);
		cameraColorDescriptor.clearBuffer = false;

		return renderGraph.CreateTexture(cameraColorDescriptor);
	}

	/// <summary>
	/// Updates the material parameters according to the volume settings.
	/// </summary>
	/// <param name="material"></param>
	private static void UpdateMaterialParameters(Material material)
	{
		CameraFadeVolumeComponent volume = VolumeManager.instance.stack.GetComponent<CameraFadeVolumeComponent>();

		material.SetColor(ColorId, volume.color.value);
		material.SetFloat(ProgressId, volume.progress.value);
	}

	/// <summary>
	/// Executes the pass with the information from the pass data.
	/// </summary>
	/// <param name="passData"></param>
	/// <param name="context"></param>
	private static void ExecutePass(PassData passData, RasterGraphContext context)
	{
		UpdateMaterialParameters(passData.material);

		Blitter.BlitTexture(context.cmd, Vector2.one, passData.material, passData.materialPassIndex);
	}

	#endregion
}