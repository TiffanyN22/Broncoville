using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

public struct GoInGameRpc : IRpcCommand
{
	// Feed me code.
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientInGameSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		// Only run this system if the client is connected but not in game.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

		// Get all unprocessed go in game requests and iterate through them all.
		foreach((RefRO<GoInGameRpc> _, Entity entity) in SystemAPI.Query<RefRO<GoInGameRpc>>().WithEntityAccess())
		{
			// Get the current connection's network id and add the in-game component.
			foreach((RefRO<NetworkId> _, Entity id) in SystemAPI.Query<RefRO<NetworkId>>().WithEntityAccess())
			{
				commandBuffer.AddComponent<NetworkStreamInGame>(id);
			}
			
			// Destroy the message now that it has been processed.
			commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}