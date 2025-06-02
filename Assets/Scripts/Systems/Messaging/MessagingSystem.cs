using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct MessagingSendMessageRpc : IRpcCommand
{
  /// <summary>The message being sent.</summary>
  public FixedString128Bytes message;
}

public struct ClientReceiveMessageRpc : IRpcCommand
{
  /// <summary>The message being sent.</summary>
  public FixedString128Bytes message;
}


[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerMessagingSystem : ISystem
{
	/// <summary>A list of all currently connected client ids.</summary>
	private ComponentLookup<NetworkId> clients;

	public void OnCreate(ref SystemState state)
	{
		// Only run this system if log in requests are available.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<MessagingSendMessageRpc, ReceiveRpcCommandRequest>();
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
		foreach((RefRO<MessagingSendMessageRpc> sentMessage, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<MessagingSendMessageRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			string sentMessageString = sentMessage.ValueRO.message.ToString();
			Debug.Log(sentMessageString);

			// Entity sendingEntity = commandBuffer.CreateEntity();
			// commandBuffer.AddComponent(sendingEntity, new ClientReceiveMessageRpc{message = sentMessageString});

			// Send to all connections
			foreach ((RefRO<NetworkStreamConnection> connection, Entity entity2) in SystemAPI.Query<RefRO<NetworkStreamConnection>>().WithEntityAccess())
			{
					Entity sendingEntity = commandBuffer.CreateEntity();

					commandBuffer.AddComponent(sendingEntity, new ClientReceiveMessageRpc { message = sentMessageString });

					commandBuffer.AddComponent(sendingEntity, new SendRpcCommandRequest { TargetConnection = entity2 });
			}




			// Destroy the message now that it has been processed.
			commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientMessagingSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<ClientReceiveMessageRpc, ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

		// Update the messaging UI upon recieving a response from the server.
		foreach((RefRO<ClientReceiveMessageRpc> message, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<ClientReceiveMessageRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			Messaging messagingUI = Object.FindFirstObjectByType<Messaging>();

			if(messagingUI == null)
			{
				Debug.Log("Client Login System: Failed to find the login UI.");
			}
			else
			{
				// Debug.Log("Received " + message.ValueRO.message.ToString());
				messagingUI.AddMessage(message.ValueRO.message.ToString());
			}
			
			commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}