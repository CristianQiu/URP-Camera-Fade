using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// The camera fade renderer feature.
/// </summary>
[Tooltip("Adds support to fade the camera color to or from a color.")]
[DisallowMultipleRendererFeature("Camera Fade")]
public sealed class CameraFadeRendererFeature : ScriptableRendererFeature
{
	#region Private Attributes

	[HideInInspector]
	[SerializeField] private Shader cameraFadeShader;

	private Material cameraFadeMaterial;

	private CameraFadeRenderPass cameraFadeRenderPass;

	#endregion

	#region Scriptable Renderer Feature Methods

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public override void Create()
	{
		ValidateResourcesForCameraFadeRenderPass(true);

		cameraFadeRenderPass = new CameraFadeRenderPass(cameraFadeMaterial);
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="renderer"></param>
	/// <param name="renderingData"></param>
	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		bool isPostProcessEnabled = renderingData.postProcessingEnabled && renderingData.cameraData.postProcessEnabled;
		bool shouldAddCameraFadeRenderPass = isPostProcessEnabled && ShouldAddCameraFadeRenderPass(renderingData.cameraData.cameraType);

		if (shouldAddCameraFadeRenderPass)
			renderer.EnqueuePass(cameraFadeRenderPass);
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="disposing"></param>
	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);

		CoreUtils.Destroy(cameraFadeMaterial);
	}

	#endregion

	#region Methods

	/// <summary>
	/// Validates the resources used by the camera fade render pass.
	/// </summary>
	/// <param name="forceRefresh"></param>
	/// <returns></returns>
	private bool ValidateResourcesForCameraFadeRenderPass(bool forceRefresh)
	{
		if (forceRefresh)
		{
#if UNITY_EDITOR
			cameraFadeShader = Shader.Find("Hidden/CameraFade");
#endif
			CoreUtils.Destroy(cameraFadeMaterial);
			cameraFadeMaterial = CoreUtils.CreateEngineMaterial(cameraFadeShader);
		}

		return cameraFadeShader != null && cameraFadeMaterial != null;
	}

	/// <summary>
	/// Gets whether the camera fade render pass should be enqueued to the renderer.
	/// </summary>
	/// <param name="cameraType"></param>
	/// <returns></returns>
	private bool ShouldAddCameraFadeRenderPass(CameraType cameraType)
	{
		CameraFadeVolumeComponent volume = VolumeManager.instance.stack.GetComponent<CameraFadeVolumeComponent>();

		bool isVolumeOk = volume != null && volume.IsActive();
		bool isRenderPassOk = cameraFadeRenderPass != null;
		bool areResourcesOk = ValidateResourcesForCameraFadeRenderPass(false);
		bool isCameraOk = cameraType != CameraType.Preview && cameraType != CameraType.Reflection;

		return isActive && isVolumeOk && isRenderPassOk && areResourcesOk && isCameraOk;
	}

	#endregion
}