using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Volume component for the camera fade.
/// </summary>
[DisplayInfo(name = "Camera Fade")]
[VolumeComponentMenu("Custom/Camera Fade")]
[VolumeRequiresRendererFeatures(typeof(CameraFadeRendererFeature))]
[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
public sealed class CameraFadeVolumeComponent : VolumeComponent, IPostProcessComponent
{
	#region Public Attributes

	public ColorParameter color = new ColorParameter(Color.black, false, false, true);
	public ClampedFloatParameter progress = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

	#endregion

	#region IPostProcessComponent Methods

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns></returns>
	public bool IsActive()
	{
		return progress.value > 0.0f;
	}

	#endregion
}