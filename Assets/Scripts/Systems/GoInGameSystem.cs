using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

public struct GoInGameRpc : IRpcCommand
{
	// Feed me code.
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientGoInGameSystem : ISystem
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

		// Find the client's network id and add the in-game component.
		foreach((RefRO<GoInGameRpc> _, Entity entity) in SystemAPI.Query<RefRO<GoInGameRpc>>().WithEntityAccess())
		{
			foreach((RefRO<NetworkId> _, Entity id) in SystemAPI.Query<RefRO<NetworkId>>().WithEntityAccess())
			{
				commandBuffer.AddComponent<NetworkStreamInGame>(id);
			}
			
			commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}