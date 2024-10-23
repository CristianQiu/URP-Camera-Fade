using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Volume component for the camera fade.
/// </summary>
#if UNITY_2023_1_OR_NEWER
[VolumeComponentMenu("Custom/Camera Fade")]
#if UNITY_6000_0_OR_NEWER
[VolumeRequiresRendererFeatures(typeof(CameraFadeRendererFeature))]
#endif
[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
#else
[VolumeComponentMenuForRenderPipeline("Custom/Camera Fade", typeof(UniversalRenderPipeline))]
#endif
public sealed class CameraFadeVolumeComponent : VolumeComponent, IPostProcessComponent
{
	#region Public Attributes

	public ColorParameter color = new ColorParameter(Color.black, false, false, false, false);
	public ClampedFloatParameter progress = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

	#endregion

	#region Initialization Methods

	public CameraFadeVolumeComponent() : base()
	{
		displayName = "Camera Fade";
	}

	#endregion

	#region IPostProcessComponent Methods

#if !UNITY_2023_1_OR_NEWER

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns></returns>
	public bool IsTileCompatible()
	{
		return true;
	}

#endif

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