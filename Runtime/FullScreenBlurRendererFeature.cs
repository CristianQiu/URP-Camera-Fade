using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// The full screen blur renderer feature.
/// </summary>
[Tooltip("Adds support to do a full screen blur.")]
[DisallowMultipleRendererFeature("FullScreenBlur")]
public sealed class FullScreenBlurRendererFeature : ScriptableRendererFeature
{
	#region Private Attributes

	[HideInInspector]
	[SerializeField] private Shader fullScreenBlurShader;

	private Material fullScreenBlurMaterial;
	private FullScreenBlurRenderPass fullScreenBlurRenderPass;

	#endregion

	#region Scriptable Renderer Feature Methods

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public override void Create()
	{
		ValidateResourcesForFullScreenBlurRenderPass(true);

		fullScreenBlurRenderPass = new FullScreenBlurRenderPass(fullScreenBlurMaterial);
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="renderer"></param>
	/// <param name="renderingData"></param>
	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		bool isPostProcessEnabled = renderingData.postProcessingEnabled && renderingData.cameraData.postProcessEnabled;
		bool shouldAddCameraFadeRenderPass = isPostProcessEnabled && ShouldAddFullScreenBlurRenderPass(renderingData.cameraData.cameraType);

		if (shouldAddCameraFadeRenderPass)
			renderer.EnqueuePass(fullScreenBlurRenderPass);
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="disposing"></param>
	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);

		fullScreenBlurRenderPass?.Dispose();

		CoreUtils.Destroy(fullScreenBlurMaterial);
	}

	#endregion

	#region Methods

	/// <summary>
	/// Validates the resources used by the full screen blur render pass.
	/// </summary>
	/// <param name="forceRefresh"></param>
	/// <returns></returns>
	private bool ValidateResourcesForFullScreenBlurRenderPass(bool forceRefresh)
	{
		if (forceRefresh)
		{
#if UNITY_EDITOR
			fullScreenBlurShader = Shader.Find("Hidden/FullScreenBlur");
#endif
			CoreUtils.Destroy(fullScreenBlurMaterial);
			fullScreenBlurMaterial = CoreUtils.CreateEngineMaterial(fullScreenBlurShader);
		}

		return fullScreenBlurShader != null && fullScreenBlurMaterial != null;
	}

	/// <summary>
	/// Gets whether the full screen blur render pass should be enqueued to the renderer.
	/// </summary>
	/// <param name="cameraType"></param>
	/// <returns></returns>
	private bool ShouldAddFullScreenBlurRenderPass(CameraType cameraType)
	{
		FullScreenBlurVolumeComponent fullScreenBlurVolumeComponent = VolumeManager.instance.stack.GetComponent<FullScreenBlurVolumeComponent>();

		bool isVolumeOk = fullScreenBlurVolumeComponent != null && fullScreenBlurVolumeComponent.IsActive();
		bool isCameraOk = cameraType != CameraType.Preview && cameraType != CameraType.Reflection;
		bool areResourcesOk = ValidateResourcesForFullScreenBlurRenderPass(false);

		return isActive && isVolumeOk && isCameraOk && areResourcesOk;
	}

	#endregion
}