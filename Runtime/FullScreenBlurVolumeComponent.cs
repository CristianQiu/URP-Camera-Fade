using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Volume component for the full screen blur.
/// </summary>
#if UNITY_2023_1_OR_NEWER
[VolumeComponentMenu("Custom/FullScreenBlur")]
#if UNITY_6000_0_OR_NEWER
[VolumeRequiresRendererFeatures(typeof(FullScreenBlurRendererFeature))]
#endif
[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
#else
[VolumeComponentMenuForRenderPipeline("Custom/FullScreenBlur", typeof(UniversalRenderPipeline))]
#endif
public sealed class FullScreenBlurVolumeComponent : VolumeComponent, IPostProcessComponent
{
	#region Public Attributes

	public ClampedFloatParameter progress = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);
	public ClampedIntParameter blurRadius = new ClampedIntParameter(8, 2, 32);

	#endregion

	#region Initialization Methods

	public FullScreenBlurVolumeComponent() : base()
	{
		displayName = "FullScreenBlur";
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