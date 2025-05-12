using TMPro;
using Unity.Collections;
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

	public void Update()
	{
		// Update is only used for networking, so return if not waiting for a response.
		if(this.loadingScreen == null)
		{
			return;
		}
		
		// Check for entities containing the create account response message.
		EntityManager entities = FindFirstObjectByType<ConnectionManager>().GetClientEntityManager();
		EntityQuery connections = entities.CreateEntityQuery(ComponentType.ReadOnly<CreateAccountResponseRpc>());
	
		// If a create account response was found, go back to the log in screen or show an error.
		foreach(Entity entity in connections.ToEntityArray(Allocator.Temp))
		{
			CreateAccountResponseRpc response = entities.GetComponentData<CreateAccountResponseRpc>(entity);
			
			if(response.accepted)
			{
				this.Back();
			}
			else
			{
				this.errorMessage.SetText(response.reason.ToString());
			}

			Destroy(this.loadingScreen.gameObject);
			entities.DestroyEntity(entity);
		}
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
		EntityManager clientManager = FindFirstObjectByType<ConnectionManager>().GetClientEntityManager();
		Entity createAccountRequest = clientManager.CreateEntity(typeof(CreateAccountRequestRpc), typeof(SendRpcCommandRequest));
		clientManager.SetComponentData(createAccountRequest, new CreateAccountRequestRpc{username = this.usernameField.text, password = this.passwordField.text});
	}
}