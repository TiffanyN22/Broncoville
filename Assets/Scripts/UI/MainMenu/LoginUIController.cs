using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIController : UIBase
{
	/// <summary>The username input field.</summary>
	[Tooltip("The username input field.")]
	[SerializeField] private TMP_InputField usernameField = null;

	/// <summary>The password input field.</summary>
	[Tooltip("The password input field.")]
	[SerializeField] private TMP_InputField passwordField = null;

	/// <summary>The toggle for saving the username and password.</summary>
	[Tooltip("The toggle for saving the username and password.")]
	[SerializeField] private Toggle saveToggle = null;

	/// <summary>The login button for logging in to the server.</summary>
	[Tooltip("The login button for logging in to the server.")]
	[SerializeField] private Button loginButton = null;

	/// <summary>The back button for returning to the join menu.</summary>
	[Tooltip("The back button for returning to the join menu.")]
	[SerializeField] private Button backButton = null;

	/// <summary>The create account button to switch to the create account UI.</summary>
	[Tooltip("The create account button to switch to the create account UI.")]
	[SerializeField] private Button createAccountButton = null;

	/// <summary>The text box used for displaying error messages.</summary>
	[Tooltip("The text box used for displaying error messages.")]
	[SerializeField] private TextMeshProUGUI errorMessage = null;

	/// <summary>The create account menu prefab.</summary>
	[Tooltip("The create account menu prefab.")]
	[SerializeField] private CreateAccountUIController createAccountPrefab = null;

	/// <summary>The loading screen prefab.</summary>
	[Tooltip("The loading screen prefab.")]
	[SerializeField] private LoadingScreenUIController loadingScreenPrefab = null;

	/// <summary>The loading screen object currently being overlayed.</summary>
	private LoadingScreenUIController loadingScreen = null;

	/// <summary>Whether or not the client logged in successfully. Used to switch loading screen logic when loading the hub.</summary>
	private bool loggedIn = false;

	public void Start()
	{
		// Add the listeners to the buttons.
		this.backButton.onClick.AddListener(this.Back);
		this.loginButton.onClick.AddListener(this.Login);
		this.createAccountButton.onClick.AddListener(this.CreateAccount);

		// Autofill the saved username and password.
		if(PlayerPrefs.HasKey("DefaultUsername") && PlayerPrefs.HasKey("DefaultPassword"))
		{
			this.usernameField.text = PlayerPrefs.GetString("DefaultUsername");
			this.passwordField.text = PlayerPrefs.GetString("DefaultPassword");
		}
	}

	public void Update()
	{
		// Update is only used for networking, so return if not waiting for a response.
		if(this.loadingScreen == null)
		{
			return;
		}

		ConnectionManager manager = FindFirstObjectByType<ConnectionManager>();
		EntityManager entities = manager.GetClientWorld().EntityManager;
		
		if(!this.loggedIn)
		{
			// Check for entities containing the log in response message.
			EntityQuery connections = entities.CreateEntityQuery(ComponentType.ReadOnly<LoginResponseRpc>());
			
			// If a log in response was found, open the loading screen UI or show an error.
			foreach(Entity entity in connections.ToEntityArray(Allocator.Temp))
			{
				LoginResponseRpc response = entities.GetComponentData<LoginResponseRpc>(entity);
				Destroy(this.loadingScreen.gameObject);

				if(!(this.loggedIn = response.accepted))
				{
					Debug.Log("Failed to log in.");
					this.errorMessage.SetText(response.reason.ToString());
				}
				else
				{
					this.loadingScreen = Instantiate(this.loadingScreenPrefab, this.transform.parent);
					this.loadingScreen.SetLoadingMessage("Loading World...");
				}

				entities.DestroyEntity(entity);
			}

			return;
		}
		
		// Check for entities containing sub scenes.
		EntityQuery scenes = entities.CreateEntityQuery(ComponentType.ReadOnly<SceneReference>());

		// If a sub scene was found, check if it's the main hub and whether or not it's loaded.
		foreach(Entity entity in scenes.ToEntityArray(Allocator.Temp))
		{
			SceneReference subscene = entities.GetComponentData<SceneReference>(entity);

			if(subscene.SceneGUID != manager.GetMainHubSubScene().SceneGUID)
			{
				continue;
			}

			if(SceneSystem.IsSceneLoaded(manager.GetClientWorld().Unmanaged, entity))
			{
				Destroy(this.loadingScreen.gameObject);
				this.Close();
			}
		}
	}

	/// <summary>
	/// Go back to the previous menu and disconnect from the server.
	/// </summary>
	public new void Back()
	{
		FindFirstObjectByType<ConnectionManager>().DisconnectClient();
		base.Back();
	}

	/// <summary>
	/// Verify the username and password fields and send a log in request to the server.
	/// </summary>
	public void Login()
	{
		// Verify the username.
		this.errorMessage.SetText(Account.VerifyUsername(this.usernameField.text));

		if(this.errorMessage.text.Length > 0)
		{
			return;
		}

		// Verify the password.
		this.errorMessage.SetText(Account.VerifyPassword(this.passwordField.text));
		
		if(this.errorMessage.text.Length > 0)
		{
			return;
		}

		// Save the username and password.
		if(this.saveToggle.isOn)
		{
			PlayerPrefs.SetString("DefaultUsername", this.usernameField.text);
			PlayerPrefs.SetString("DefaultPassword", this.passwordField.text);
		}

		// Overlay the loading screen to block user input.
		loadingScreen = Instantiate(this.loadingScreenPrefab, this.transform.parent);
		loadingScreen.SetBackgroundColor(new Color(0.6875f, 0.6875f, 0.6875f, 0.3125f));
		loadingScreen.SetLoadingMessage("Logging in...");

		// Send a log in request to the server.
		EntityManager clientManager = FindFirstObjectByType<ConnectionManager>().GetClientEntityManager();
		Entity loginRequest = clientManager.CreateEntity(typeof(LoginRequestRpc), typeof(SendRpcCommandRequest));
		clientManager.SetComponentData(loginRequest, new LoginRequestRpc{username = this.usernameField.text, password = this.passwordField.text});
	}

	/// <summary>
	/// Open the create account menu.
	/// </summary>
	public void CreateAccount()
	{
		Instantiate(this.createAccountPrefab, this.transform.parent).Open(this);
	}
}