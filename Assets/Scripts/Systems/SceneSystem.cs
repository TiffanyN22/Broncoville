using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Scenes;

public struct LoadSceneRpc : IRpcCommand
{
	/// <summary>The hash of the sub scene to load.</summary>
	public Hash128 subsceneHash;
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientSceneSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		// Only run this system if load secene requests are available.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<LoadSceneRpc>().WithAll<ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

		// Get all unprocessed load scene requests and iterate through them all.
		foreach((RefRO<LoadSceneRpc> sceneHash, Entity entity) in SystemAPI.Query<RefRO<LoadSceneRpc>>().WithEntityAccess())
		{
			// Load the sub scene specified in the request.
			Entity scene = SceneSystem.LoadSceneAsync(state.World.Unmanaged, sceneHash.ValueRO.subsceneHash, new SceneSystem.LoadParameters{AutoLoad = false, Flags = SceneLoadFlags.BlockOnImport | SceneLoadFlags.BlockOnStreamIn});

			// Destroy the message now that it has been processed.
			commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}

