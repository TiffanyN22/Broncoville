using TMPro;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountUIController : UIBase
{
	/// <summary>The username input field.</summary>
	[Tooltip("The username input field.")]
	[SerializeField] private TMP_InputField usernameField = null;

	/// <summary>The password input field.</summary>
	[Tooltip("The password input field.")]
	[SerializeField] private TMP_InputField passwordField = null;

	/// <summary>The password input field for verification.</summary>
	[Tooltip("The password input field for verification.")]
	[SerializeField] private TMP_InputField verifyField = null;

	/// <summary>The login button for logging in to the server.</summary>
	[Tooltip("The login button for logging in to the server.")]
	[SerializeField] private Button createButton = null;

	/// <summary>The back button for returning to the join menu.</summary>
	[Tooltip("The back button for returning to the join menu.")]
	[SerializeField] private Button cancelButton = null;

	/// <summary>The text box used for displaying error messages.</summary>
	[Tooltip("The text box used for displaying error messages.")]
	[SerializeField] private TextMeshProUGUI errorMessage = null;

	/// <summary>The loading screen prefab.</summary>
	[Tooltip("The loading screen prefab.")]
	[SerializeField] private LoadingScreenUIController loadingScreenPrefab = null;

	/// <summary>The loading screen object currently being overlayed.</summary>
	private LoadingScreenUIController loadingScreen = null;

	public void Start()
	{
		// Add the listeners to the buttons.
		this.createButton.onClick.AddListener(this.CreateAccount);
		this.cancelButton.onClick.AddListener(this.Back);
	}

	/// <summary>
	/// Verify the username and password fields and send a create account request to the server.
	/// </summary>
	public void CreateAccount()
	{
		// Verify username format.
		this.errorMessage.SetText(Account.VerifyUsername(this.usernameField.text));
		
		if(this.errorMessage.text.Length > 0)
		{
			return;
		}

		// Verify password format.
		this.errorMessage.SetText(Account.VerifyPassword(this.passwordField.text));
		
		if(this.errorMessage.text.Length > 0)
		{
			return;
		}

		// Verify matching passwords.
		if(!this.passwordField.text.Equals(this.verifyField.text))
		{
			this.errorMessage.SetText("Password fields do not match.");
			return;
		}

		// Overlay the loading screen to block user input.
		this.loadingScreen = Instantiate(this.loadingScreenPrefab, this.transform.parent);
		this.loadingScreen.SetBackgroundColor(new Color(0.6875f, 0.6875f, 0.6875f, 0.3125f));
		this.loadingScreen.SetLoadingMessage("Creating Account...");
		
		// Send a create account request to the server.
		EntityManager clientManager = FindFirstObjectByType<ClientManager>().GetEntityManager();
		Entity createAccountRequest = clientManager.CreateEntity(typeof(CreateAccountRequestRpc), typeof(SendRpcCommandRequest));
		clientManager.SetComponentData(createAccountRequest, new CreateAccountRequestRpc{username = this.usernameField.text, password = this.passwordField.text});
	}

	/// <summary>
	/// Handle the create account response from the server. This should only be called by 
	/// the create account system. Calling it elsewhere may lead to a client-server desync.
	/// </summary>
	/// <param name="success">Whether or not the account creation wsa successful.</param>
	/// <param name="reason">The reason why the account creation failed.</param>
	public void CreateAccount(bool success, string reason)
	{
		Destroy(this.loadingScreen.gameObject);

		if(success)
		{
			this.Back();
		}
		else
		{
			this.errorMessage.SetText(reason);
		}
	}
}