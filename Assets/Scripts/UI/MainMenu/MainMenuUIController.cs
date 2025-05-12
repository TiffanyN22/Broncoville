using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : UIBase
{
	/// <summary>The Join button for connecting to the server.</summary>
	[Tooltip("The Join button for connecting to the server.")]
	[SerializeField] private Button joinButton = null;

	/// <summary>The quit button for exiting the application.</summary>
	[Tooltip("The quit button for exiting the application.")]
	[SerializeField] private Button quitButton = null;

	/// <summary>The join server menu prefab.</summary>
	[Tooltip("The join server menu prefab.")]
	[SerializeField] private JoinServerUIController joinServerPrefab = null;

	public void Start()
	{
		// Add the listeners to the buttons.
		this.joinButton.onClick.AddListener(this.Join);
		this.quitButton.onClick.AddListener(this.Quit);
	}

	/// <summary>
	/// Open the join server menu.
	/// </summary>
	private void Join()
	{
		Instantiate(this.joinServerPrefab, this.transform.parent).Open(this);
	}

	/// <summary>
	/// Quit the application.
	/// </summary>
	private void Quit()
	{
		Application.Quit();
	}
}