using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUIController : UIBase
{
	/// <summary>The background panel.</summary>
	[Tooltip("The background panel.")]
	[SerializeField] private Image backgroundPanel = null;

	/// <summary>The loading message to display.</summary>
	[Tooltip("The loading message to display.")]
	[SerializeField] private TextMeshProUGUI messageLabel = null;

	/// <summary>The loading icon to be animated.</summary>
	[Tooltip("The loading icon to be animated.")]
	[SerializeField] private RawImage loadingIcon = null;

	/// <summary>The maximum scale offset that the icon will experience.</summary>
	[Tooltip("The maximum scale offset that the icon will experience.")]
	[SerializeField] private float scaleOffset = 0.25f;

	/// <summary>The animation speed multiplier.</summary>
	[Tooltip("The animation speed multiplier.")]
	[SerializeField] private float animationSpeed = 10f;

	/// <summary>The progress of the animation. Loops after 2π.</summary>
	private float progress = 0f;

	public void Update()
	{
		// Increase the animation progress, clamping from 0 ≤ progress ≤ 2π.
		progress += animationSpeed * Time.deltaTime;

		if(progress > 2f * MathF.PI)
		{
			progress -= 2f * MathF.PI;
		}

		// Set the icons scale.
		this.loadingIcon.transform.localScale = new Vector3(1f + scaleOffset * MathF.Sin(progress), 1f - scaleOffset * MathF.Sin(progress), 1f);
	}

	/// <summary>
	/// Set the background color of the loading screen.
	/// </summary>
	/// <param name="color">The new background color.</param>
	public void SetBackgroundColor(Color color)
	{
		this.backgroundPanel.color = color;
	}

	/// <summary>
	/// Set the message to display on the loading screen.
	/// </summary>
	/// <param name="message">The message to display.</param>
	public void SetLoadingMessage(string message)
	{
		this.messageLabel.SetText(message);
	}
}