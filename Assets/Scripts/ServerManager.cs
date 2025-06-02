using System.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using static Unity.Scenes.SceneSystem;

public class ServerManager : MonoBehaviour
{
	/// <summary>The ip address of the server.</summary>
	[Tooltip("The ip address of the server.")]
	[SerializeField] private string address = "127.0.0.1";

	/// <summary>The port to listen to.</summary>
	[Tooltip("The port to listen to.")]
	[SerializeField] private ushort port = 7979;

	/// <summary>The client world. WIll be null if only the server was requested.</summary>
	private World world = null;

	public void Start()
	{
		// If the server isn't requested, self destruct.
		if(ClientServerBootstrap.RequestedPlayType == ClientServerBootstrap.PlayType.Client)
		{
			Destroy(this.gameObject);
			return;
		}

		// Load the worlds asynchronously to allow asynchronous sub scene loading for the server.
		StartCoroutine(this.StartServer());
	}

	/// <summary>
	/// Start the the server world. Sets the server to start listening automatically.
	/// </summary>
	/// <returns>Nothing.</returns>
	private IEnumerator StartServer()
	{
		// Create the server world if it's requested.
		this.world = ClientServerBootstrap.CreateServerWorld("ServerWorld");

		// Destroy the auto-generated local world to replace it with the server world.
		this.DestroyLocalWorld();

		// Set the world for generating objects.
		World.DefaultGameObjectInjectionWorld = this.world;

		// Wait until the server is created before loading the main hub sub scene.
		while(!this.world.IsCreated)
		{
			yield return null;
		}

		// Load the main hub sub scene.
		SubSceneManager scenes = FindFirstObjectByType<SubSceneManager>();
		LoadParameters parameters = new LoadParameters{Flags = SceneLoadFlags.BlockOnStreamIn};
		Entity scene = LoadSceneAsync(this.world.Unmanaged, scenes.GetMainHubGUID(Scenes.MAIN_HUB), parameters);
		
		// Wait until the sub scene has finished loading.
		while(!IsSceneLoaded(this.world.Unmanaged, scene))
		{
			this.world.Update();
		}

		// Start listening on the server.
		EntityQuery streamDriver = this.world.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
		streamDriver.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(NetworkEndpoint.Parse(this.address, this.port));
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
	/// Get the server world.
	/// </summary>
	/// <returns>The server world. Should never return null unless you're fast enough to call this before it's created.</returns>
	public World GetWorld()
	{
		return this.world;
	}

	/// <summary>
	/// Get the server's entity manager.
	/// </summary>
	/// <returns>The server's entity manager. Should never return null unless you're fast enough to call this before it's created.</returns>
	public EntityManager GetEntityManager()
	{
		return this.world.EntityManager;
	}
}