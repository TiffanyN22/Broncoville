using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct CustomizePonyRpc : IRpcCommand
{
	public HairStyle hairStyle;
	public Color bodyColor;
}


[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct CustomizePonySystem : ISystem
{
	private ComponentLookup<NetworkId> clients;
	public void OnCreate(ref SystemState state)
	{
		// Only run this system if create account requests are available.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<CustomizePonyRpc>().WithAll<ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));

		this.clients = state.GetComponentLookup<NetworkId>(true);
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

		this.clients.Update(ref state);

		foreach ((RefRO<CustomizePonyRpc> customizePony, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<CustomizePonyRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			HairStyle hairStyle = customizePony.ValueRO.hairStyle;
			Color bodyColor = customizePony.ValueRO.bodyColor;

			NetworkId playerId = this.clients[request.ValueRO.SourceConnection];

			foreach ((RefRO<NetworkId> id, Entity connection) in SystemAPI.Query<RefRO<NetworkId>>().WithEntityAccess())
			{
				if (id.ValueRO.Value != playerId.Value)
				{
					continue;
				}

				AccountData account = state.EntityManager.GetComponentData<AccountData>(connection);
				AccountData updated = new AccountData { name = account.name, bodyColor = bodyColor, hairColor = account.hairColor, hairStyle = hairStyle };
				commandBuffer.SetComponent(entity, updated);

				Account a = Account.LoadFromFile(account.name.ToString());
				new Account(a.GetUsername(), a.GetPassword(), bodyColor, a.GetHairColor(), hairStyle).SaveToFile();

				foreach ((RefRO<GhostOwner> ghost, Entity owner) in SystemAPI.Query<RefRO<GhostOwner>>().WithEntityAccess())
				{
					if (ghost.ValueRO.NetworkId == id.ValueRO.Value)
					{
						commandBuffer.SetComponent(owner, updated);
					}
				}
			}
			
			commandBuffer.DestroyEntity(entity);
		}
		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}
