using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Scenes;
using UnityEngine;
using static Unity.Scenes.SceneSystem;

public class ConnectionManager : MonoBehaviour
{
	/// <summary>The ip address to connect to.</summary>
	[Tooltip("The ip address to connect to.")]
	[SerializeField] private string clientAddress = "127.0.0.1";

	/// <summary>The ip address to listen to.</summary>
	[Tooltip("The ip address to listen to.")]
	[SerializeField] private string serverAddress = "127.0.0.1";

	/// <summary>The port to listen/connect to.</summary>
	[Tooltip("The port to listen/connect to.")]
	[SerializeField] private ushort port = 7979;

	/// <summary>The main canvas for the menus.</summary>
	[Tooltip("The main canvas for the menus.")]
	[SerializeField] private Canvas canvas = null;

	/// <summary>The main menu prefab.</summary>
	[Tooltip("The main menu prefab.")]
	[SerializeField] private MainMenuUIController mainMenuPrefab = null;

	/// <summary>The main hub sub scene.</summary>
	[Tooltip("The main hub sub scene.")]
	[SerializeField] private SubScene mainHub = null;

	/// <summary>The client world. WIll be null if only the server was requested.</summary>
	private World clientWorld = null;

	/// <summary>The server world. WIll be null if only the server was requested.</summary>
	private World serverWorld = null;

	public void Start()
	{
		// Create the main menu if the client is requested.
		if(this.IsClient())
		{
			Instantiate(this.mainMenuPrefab, this.canvas.transform);
		}

		// Load the worlds asynchronously to allow asynchronous sub scene loading for the server.
		StartCoroutine(this.StartClientServer());
	}

	/// <summary>
	/// Start the client and the server worlds. Sets the server to start listening automatically.
	/// </summary>
	/// <returns>Nothing.</returns>
	private IEnumerator StartClientServer()
	{
		// Create the client and server worlds if they're requested.
		this.serverWorld = this.IsServer() ? ClientServerBootstrap.CreateServerWorld("ServerWorld") : null;
		this.clientWorld = this.IsClient() ? ClientServerBootstrap.CreateClientWorld("ClientWorld") : null;

		// Destroy the auto-generated local world to replace it with the client/server worlds.
		this.DestroyLocalWorld();

		// Set the world for generating objects.
		World.DefaultGameObjectInjectionWorld = this.serverWorld != null ? this.serverWorld : this.clientWorld;

		if(this.serverWorld != null)
		{
			// Wait until the server is created before loading the main hub sub scene.
			while(!this.serverWorld.IsCreated)
			{
				yield return null;
			}

			// Load the main hub sub scene.
			LoadParameters parameters = new LoadParameters{Flags = SceneLoadFlags.BlockOnStreamIn};
			Entity scene = LoadSceneAsync(this.serverWorld.Unmanaged, new Unity.Entities.Hash128(this.mainHub.SceneGUID.Value), parameters);
			
			// Wait until the sub scene has finished loading.
			while(!IsSceneLoaded(this.serverWorld.Unmanaged, scene))
			{
				this.serverWorld.Update();
			}

			// Start listening on the server.
			EntityQuery streamDriver = this.serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
			streamDriver.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(NetworkEndpoint.Parse(this.serverAddress, this.port));
		}
	}	

	/// <summary>
	/// Connect the client to the server.
	/// </summary>
	public void ConnectClient()
	{
		if(this.clientWorld == null)
		{
			Debug.LogWarning("Conenct(): The client doesn't exist.");
			return;
		}

		// Attempt to connect the client to the server if it isn't already connected.
		if(!this.IsClientConnected())
		{
			EntityQuery streamDriver = this.clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
			streamDriver.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(this.clientWorld.EntityManager, NetworkEndpoint.Parse(this.clientAddress, this.port));
		}
	}

	/// <summary>
	/// Disconnect the client from the server.
	/// </summary>
	public void DisconnectClient()
	{
		if(this.clientWorld == null)
		{
			Debug.LogWarning("Disconnect(): The client doesn't exist.");
			return;
		}

		// Get the active connection and send a disconnect request.
		EntityQuery connections = this.clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<NetworkStreamConnection>());
		
		foreach(Entity entity in connections.ToEntityArray(Allocator.Temp))
		{
			this.clientWorld.EntityManager.AddComponent<NetworkStreamRequestDisconnect>(entity);
		}
	}

	/// <summary>
	/// Destroy the auto-generated local world.
	/// </summary>
	private void DestroyLocalWorld()
	{
		foreach(World world in World.All)
		{
			if(world.Flags == WorldFlags.Game)
			{
				world.Dispose();
				break;
			}
		}
	}

	// Master documentation writer over here.
	/// <summary>
	/// Get the main hub sub scene.
	/// </summary>
	/// <returns>The main hub scene.</returns>
	public SubScene GetMainHubSubScene()
	{
		return this.mainHub;
	}

	/// <summary>
	/// Get the client world.
	/// </summary>
	/// <returns>The client world, or null if the client wasn't requested.</returns>
	public World GetClientWorld()
	{
		return this.clientWorld;
	}

	/// <summary>
	/// Get the server world.
	/// </summary>
	/// <returns>The server world, or null if the server wasn't requested.</returns>
	public World GetServerWorld()
	{
		return this.serverWorld;
	}

	/// <summary>
	/// Check whether or not the client was requested.
	/// </summary>
	/// <returns>True if the playmode is client or host, false otherwise.</returns>
	public bool IsClient()
	{
		return ClientServerBootstrap.RequestedPlayType != ClientServerBootstrap.PlayType.Server;
	}

	/// <summary>
	/// Check whether or not the client was requested.
	/// </summary>
	/// <returns>True if the playmode is server or host, false otherwise.</returns>
	public bool IsServer()
	{
		return ClientServerBootstrap.RequestedPlayType != ClientServerBootstrap.PlayType.Client;
	}

	/// <summary>
	/// Set the ip address that the client should connect to.
	/// </summary>
	/// <param name="ipAddress">The new ip address.</param>
	public void SetClientAddress(string ipAddress)
	{
		this.clientAddress = ipAddress;
	}

	/// <summary>
	/// Set the ip address that the server should listen to.
	/// </summary>
	/// <param name="ipAddress">The new ip address.</param>
	public void SetServerAddress(string ipAddress)
	{
		this.serverAddress = ipAddress;
	}

	/// <summary>
	/// Set the port number that the application should listen to.
	/// </summary>
	/// <param name="ipAddress">The new port number.</param>
	public void SetPort(ushort portNumber)
	{
		this.port = portNumber;
	}

	/// <summary>
	/// Check if the client is connected to the server.
	/// </summary>
	/// <returns>True if the client is connected to the server, false otherwise.</returns>
	public bool IsClientConnected()
	{
		if(this.clientWorld == null)
		{
			Debug.LogWarning("IsClientConnected(): The client doesn't exist.");
			return false;
		}

		// Get all network stream connections on the client.
		EntityQuery connections = this.clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<NetworkStreamConnection>());
		
		if(connections.IsEmpty)
		{
			return false;
		}

		// Although there should only be one connection, check all to see if at least one is connected.
		bool result = false;

		foreach(NetworkStreamConnection connection in connections.ToComponentDataArray<NetworkStreamConnection>(Allocator.Temp))
		{
			result |= connection.CurrentState == ConnectionState.State.Connected;
		}

		return result;
	}

	/// <summary>
	/// Get the client's entity manager.
	/// </summary>
	/// <returns>The client's entity manager or null if the client doesn't exist.</returns>
	public EntityManager GetClientEntityManager()
	{
		if(this.clientWorld == null)
		{
			Debug.LogWarning("GetClientEntityManager(): The client doesn't exist.");
			return new EntityManager();
		}

		return this.clientWorld.EntityManager;
	}

	/// <summary>
	/// Get the server's entity manager.
	/// </summary>
	/// <returns>The server's entity manager or null if the server doesn't exist.</returns>
	public EntityManager GetServerEntityManager()
	{
		if(this.serverWorld == null)
		{
			Debug.LogWarning("GetServerEntityManager(): The server doesn't exist.");
			return new EntityManager();
		}

		return this.serverWorld.EntityManager;
	}
}