using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct LoginRequestRpc : IRpcCommand
{
	/// <summary>The account's username.</summary>
	public FixedString32Bytes username;

	/// <summary>The account's password.</summary>
	public FixedString32Bytes password;
}

public struct LoginResponseRpc : IRpcCommand
{
	/// <summary>Whether or not the log in was successful.</summary>
	public bool accepted;

	/// <summary>If log in was unsuccessful, the reason why.</summary>
	public FixedString128Bytes reason;
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerLoginSystem : ISystem
{
	/// <summary>A list of all currently connected client ids.</summary>
	private ComponentLookup<NetworkId> clients;

	public void OnCreate(ref SystemState state)
	{
		// Only run this system if log in requests are available.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<LoginRequestRpc, ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));

		// Get a redonly component lookup for network ids.
		this.clients = state.GetComponentLookup<NetworkId>(true);
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

		// Update the list of connected clients.
		this.clients.Update(ref state);

		// Get all unprocessed log in requests and iterate through them all.
		foreach((RefRO<LoginRequestRpc> login, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<LoginRequestRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			string username = login.ValueRO.username.ToString();
			FixedString128Bytes errorMessage = "";
			bool success = true;

			// Check if an account with the same username as the request is already signed in.
			foreach(RefRO<AccountData> activeAccount in SystemAPI.Query<RefRO<AccountData>>())
			{
				if(activeAccount.ValueRO.name.Equals(login.ValueRO.username))
				{
					success = false;
					errorMessage = "That account is currently in use.";
					break;
				}
			}

			Account account = null;

			// If the account is not in use, attempt to sign in.
			if(success)
			{
				account = Account.LoadFromFile(username);
				string password = login.ValueRO.password.ToString();

				// If the account doesn't exist or the password is wrong, send the client a vague message.
				if(account == null || !password.Equals(account.GetPassword()))
				{
					success = false;
					errorMessage = "The username or password is incorrect.";
				}
			}

			// Send a response message to the client saying whether or not account creation was successful.
			Entity response = commandBuffer.CreateEntity();

			commandBuffer.AddComponent(response, new LoginResponseRpc{accepted = success, reason = errorMessage});
			commandBuffer.AddComponent(response, new SendRpcCommandRequest{TargetConnection = request.ValueRO.SourceConnection});

			if(success)
			{
				// Set the client's connection to in-game.
				commandBuffer.AddComponent<NetworkStreamInGame>(request.ValueRO.SourceConnection);
				commandBuffer.AddComponent(request.ValueRO.SourceConnection, new AccountData{name = login.ValueRO.username});

				// Send a message to the client telling it to also go in-game.
				Entity goInGame = commandBuffer.CreateEntity();

				commandBuffer.AddComponent<GoInGameRpc>(goInGame);
				commandBuffer.AddComponent(goInGame, new SendRpcCommandRequest{TargetConnection = request.ValueRO.SourceConnection});

				// Send a message to the client telling to load the main hub sub scene.
				Entity loadMainHub = commandBuffer.CreateEntity();

				commandBuffer.AddComponent(loadMainHub, new LoadSceneRpc{subsceneHash = Object.FindFirstObjectByType<SubSceneManager>().GetMainHubGUID()});
				commandBuffer.AddComponent(loadMainHub, new SendRpcCommandRequest{TargetConnection = request.ValueRO.SourceConnection});

				// Spawn the player in the world and set it's owner to the newly signed in client.
				MainHubSpawnerData spawner = SystemAPI.GetSingleton<MainHubSpawnerData>();
				Entity newPlayer = commandBuffer.Instantiate(spawner.player);
				NetworkId playerId = this.clients[request.ValueRO.SourceConnection];

				commandBuffer.SetComponent(newPlayer, new AccountData{name = account.GetUsername(), bodyColor = account.GetBodyColor(), hairColor = account.GetHairColor(), hairStyle = account.GetHairStyle()});
				commandBuffer.SetComponent(newPlayer, new GhostOwner{NetworkId = playerId.Value});
			}

			// Destroy the message now that it has been processed.
			commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientLoginSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<LoginResponseRpc, ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

		// Update the login UI upon recieving a response from the server.
		foreach((RefRO<LoginResponseRpc> login, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<LoginResponseRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			LoginUIController loginUI = Object.FindFirstObjectByType<LoginUIController>();

			if(loginUI == null)
			{
				Debug.Log("Client Login System: Failed to find the login UI.");
			}
			else
			{
				loginUI.Login(login.ValueRO.accepted, login.ValueRO.reason.ToString());
			}
			
			commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}