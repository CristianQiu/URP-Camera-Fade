using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

/// <summary>
/// The camera fade render pass.
/// </summary>
public sealed class CameraFadeRenderPass : ScriptableRenderPass
{
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
		UpdateMaterialParameters();

		UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
		TextureHandle blitTextureHandle = CreateRenderGraphTextureHandle(renderGraph, resourceData);

		RenderGraphUtils.BlitMaterialParameters blitParameters = new RenderGraphUtils.BlitMaterialParameters(resourceData.cameraColor, blitTextureHandle, material, 0);
		RenderGraphUtils.AddBlitPass(renderGraph, blitParameters, "Camera Fade");

		resourceData.cameraColor = blitTextureHandle;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Updates the material parameters according to the volume settings.
	/// </summary>
	private void UpdateMaterialParameters()
	{
		CameraFadeVolumeComponent volume = VolumeManager.instance.stack.GetComponent<CameraFadeVolumeComponent>();

		material.SetColor(ColorId, volume.color.value);
		material.SetFloat(ProgressId, volume.progress.value);
	}

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

	#endregion
}