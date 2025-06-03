using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinServerUIController : UIBase
{
	/// <summary>The IP address input field for specifying the server's address.</summary>
	[Tooltip("The IP address input field for specifying the server's address.")]
	[SerializeField] private TMP_InputField addressField = null;

	/// <summary>The port number input field for specifying the server's port.</summary>
	[Tooltip("The port number input field for specifying the server's port.")]
	[SerializeField] private TMP_InputField portField = null;

	/// <summary>The toggle for setting the server as the default.</summary>
	[Tooltip("The toggle for setting the server as the default.")]
	[SerializeField] private Toggle setDefaultToggle = null;

	/// <summary>The join button for connecting to the server.</summary>
	[Tooltip("The join button for connecting to the server.")]
	[SerializeField] private Button joinButton = null;

	/// <summary>The back button for returning to the previous menu.</summary>
	[Tooltip("The back button for returning to the previous menu.")]
	[SerializeField] private Button backButton = null;

	/// <summary>The text box used for displaying error messages.</summary>
	[Tooltip("The text box used for displaying error messages.")]
	[SerializeField] private TextMeshProUGUI errorMessage = null;

	/// <summary>The loading screen prefab.</summary>
	[Tooltip("The loading screen prefab.")]
	[SerializeField] private LoadingScreenUIController loadingScreenPrefab = null;

	/// <summary>The log in menu prefab.</summary>
	[Tooltip("The log in menu prefab.")]
	[SerializeField] private LoginUIController loginPrefab = null;

	/// <summary>The loading screen object currently being overlayed.</summary>
	private LoadingScreenUIController loadingScreen = null;

	public void Start()
	{
		// Add the listeners to the buttons.
		this.backButton.onClick.AddListener(this.Back);
		this.joinButton.onClick.AddListener(this.JoinServer);

		// Autofill the saved address and port.
		if(PlayerPrefs.HasKey("DefaultAddress") && PlayerPrefs.HasKey("DefaultPort"))
		{
			this.addressField.text = PlayerPrefs.GetString("DefaultAddress");
			this.portField.text = PlayerPrefs.GetString("DefaultPort");
		}
	}

	public void Update()
	{
		// Wait until a successful connection is made before opening the login menu.
		if(FindFirstObjectByType<ClientManager>().IsConnected())
		{
			Instantiate(this.loginPrefab, this.transform.parent).Open(this);
			Destroy(this.loadingScreen.gameObject);
		}
	}

	/// <summary>
	/// Verify the address and port fields and send a connection request to the server.
	/// </summary>
	public void JoinServer()
	{
		// Verify the address format.
		this.errorMessage.SetText(this.VerifyAddress(this.addressField.text));

		if(this.errorMessage.text.Length > 0)
		{
			return;
		}

		// Verify the port format.
		this.errorMessage.SetText(this.VerifyPort(this.portField.text));

		if(this.errorMessage.text.Length > 0)
		{
			return;
		}

		// Save the address and port.
		if(this.setDefaultToggle.isOn)
		{
			PlayerPrefs.SetString("DefaultAddress", this.addressField.text);
			PlayerPrefs.SetString("DefaultPort", this.portField.text);
		}

		// Overlay the loading screen to block user input.
		this.loadingScreen = Instantiate(this.loadingScreenPrefab, this.transform.parent);
		this.loadingScreen.SetBackgroundColor(new Color(0.6875f, 0.6875f, 0.6875f, 0.3125f));
		this.loadingScreen.SetLoadingMessage("Joining...");

		// Send a connect request to the server.
		FindFirstObjectByType<ClientManager>().Connect(this.addressField.text, ushort.Parse(this.portField.text));
	}

	/// <summary>
	/// Verify the ip address by checking for formatting errors or invalid values.
	/// </summary>
	/// <param name="address">The address to verify.</param>
	/// <returns>A string containing the error message on failure, or an empty string upon success.</returns>
	public string VerifyAddress(string address)
	{
		// Check null.
		if(address == null)
		{
			return "Null is not a valid input.";
		}

		// Check format.
		string[] splitAddress = address.Split('.');

		if(splitAddress.Length != 4)
		{
			return "The IP address is not in the right format.\nCorrect Format: #.#.#.#";
		}

		// Check if the values are valid unsigned bytes.
		for(int i = 0; i < 4; ++i)
		{
			try
			{
				byte.Parse(splitAddress[i]);
			}
			catch(OverflowException)
			{
				return splitAddress[i] + " is too large.\nCorrect Value: 0 ≤ # ≤ 255";
			}
			catch(FormatException)
			{
				return splitAddress[i] + " is not a number.";
			}
			catch(ArgumentNullException)
			{
				return "Null is not a valid input.";
			}
		}

		return "";
	}
	
	/// <summary>
	/// Verify the port by checking for formatting errors or invalid values.
	/// </summary>
	/// <param name="portString">The port to verify.</param>
	/// <returns>A string containing the error message on failure, or an empty string upon success.</returns>
	public string VerifyPort(string portString)
	{
		// Check if the port is a valid unsigned short.
		try
		{
			return ushort.Parse(portString) == 0 ? "Port 0 is reserved by the system." : "";
		}
		catch(FormatException)
		{
			return portString + " is not a number.";
		}
		catch(OverflowException)
		{
			return portString + " is too large.\nCorrect Value: 1 ≤ # ≤ " + ushort.MaxValue;
		}
		catch(ArgumentNullException)
		{
			return "Null is not a valid input.";
		}
	}
}