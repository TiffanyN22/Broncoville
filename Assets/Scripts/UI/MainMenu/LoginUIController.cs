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
	private bool checkScene = false;

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
		if(!checkScene)
		{
			return;
		}

		// If the login was successful, wait until the main hub has loaded before closing.
		ClientManager client = FindFirstObjectByType<ClientManager>();
		SubSceneManager subSceneManager = FindFirstObjectByType<SubSceneManager>();
		EntityQuery scenes = client.GetEntityManager().CreateEntityQuery(ComponentType.ReadOnly<SceneReference>());

		foreach(Entity entity in scenes.ToEntityArray(Allocator.Temp))
		{
			SceneReference subscene = client.GetEntityManager().GetComponentData<SceneReference>(entity);

			if(subscene.SceneGUID == subSceneManager.GetGUID(Location.MAIN_HUB) && SceneSystem.IsSceneLoaded(client.GetWorld().Unmanaged, entity))
			{
				Destroy(this.loadingScreen.gameObject);
				this.checkScene = false;
				this.Close();
			}
		}
	}

	/// <summary>
	/// Go back to the previous menu and disconnect from the server.
	/// </summary>
	public new void Back()
	{
		FindFirstObjectByType<ClientManager>().Disconnect();
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
		EntityManager entities = FindFirstObjectByType<ClientManager>().GetEntityManager();
		Entity loginRequest = entities.CreateEntity(typeof(LoginRequestRpc), typeof(SendRpcCommandRequest));
		entities.SetComponentData(loginRequest, new LoginRequestRpc{username = this.usernameField.text, password = this.passwordField.text});
	}

	/// <summary>
	/// Login the user. This function should only be called by the login 
	/// system. Calling it elsewhere may lead to a client-server desync.
	/// </summary>
	/// <param name="success">Whether or not the login was successful.</param>
	/// <param name="reason">The reason why the login failed.</param>
	public void Login(bool success, string reason)
	{
		if(this.loadingScreen == null)
		{
			return;
		}

		if(success)
		{
			this.loadingScreen.SetLoadingMessage("Loading World...");
			this.loadingScreen.SetBackgroundColor(Color.white);
			this.checkScene = true;
		}
		else
		{
			Destroy(this.loadingScreen.gameObject);
			this.errorMessage.SetText(reason);
		}
	}

	/// <summary>
	/// Open the create account menu.
	/// </summary>
	public void CreateAccount()
	{
		Instantiate(this.createAccountPrefab, this.transform.parent).Open(this);
	}
}