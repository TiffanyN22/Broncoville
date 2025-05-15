using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
	/// <summary>The main canvas for the menus.</summary>
	[Tooltip("The main canvas for the menus.")]
	[SerializeField] private Canvas canvas = null;

	/// <summary>The main menu prefab.</summary>
	[Tooltip("The main menu prefab.")]
	[SerializeField] private MainMenuUIController mainMenuPrefab = null;

	/// <summary>The client's world.</summary>
	private World world = null;

	public void Start()
	{
		// If the client isn't requested, self destruct.
		if(ClientServerBootstrap.RequestedPlayType == ClientServerBootstrap.PlayType.Server)
		{
			Destroy(this.canvas.gameObject);
			Destroy(this.gameObject);
			return;
		}

		// Create the main menu.
		Instantiate(this.mainMenuPrefab, this.canvas.transform);

		// Create the client world.
		this.world = ClientServerBootstrap.CreateClientWorld("ClientWorld");

		// Destroy the auto-generated local world to replace it with the client world.
		this.DestroyLocalWorld();

		// Set the world for generating objects only if the client exists.
		if(FindFirstObjectByType<ServerManager>() == null)
		{
			World.DefaultGameObjectInjectionWorld = this.world;
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

	/// <summary>
	/// Get the client world.
	/// </summary>
	/// <returns>The client world. Should never return null unless you're fast enough to call this before it's created.</returns>
	public World GetWorld()
	{
		return this.world;
	}

	/// <summary>
	/// Get the client's entity manager.
	/// </summary>
	/// <returns>The client's entity manager. Should never return null unless you're fast enough to call this before it's created.</returns>
	public EntityManager GetEntityManager()
	{
		return this.world.EntityManager;
	}

	/// <summary>
	/// Connect the client to the server.
	/// </summary>
	/// <param name="address">The address of the server.</param>
	/// <param name="port">The port the server is listening on.</param>
	public void Connect(string address, ushort port)
	{
		// Attempt to connect the client to the server if it isn't already connected.
		if(!this.IsConnected())
		{
			EntityQuery streamDriver = this.world.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
			streamDriver.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(this.world.EntityManager, NetworkEndpoint.Parse(address, port));
		}
	}

	/// <summary>
	/// Check if the client is connected to the server.
	/// </summary>
	/// <returns>True if the client is connected to the server, false otherwise.</returns>
	public bool IsConnected()
	{
		// Get all network stream connections on the client.
		EntityQuery connections = this.world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<NetworkStreamConnection>());
		
		if(connections.IsEmpty)
		{
			return false;
		}

		// Although there should only be one connection, check all to see if at least one is connected.
		foreach(NetworkStreamConnection connection in connections.ToComponentDataArray<NetworkStreamConnection>(Allocator.Temp))
		{
			if(connection.CurrentState == ConnectionState.State.Connected)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Disconnect the client from the server.
	/// </summary>
	public void Disconnect()
	{
		// Get the active connection and send a disconnect request.
		EntityQuery connections = this.world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<NetworkStreamConnection>());
		
		foreach(Entity entity in connections.ToEntityArray(Allocator.Temp))
		{
			this.world.EntityManager.AddComponent<NetworkStreamRequestDisconnect>(entity);
		}
	}
}