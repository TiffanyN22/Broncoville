using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public struct LoadSceneRpc : IRpcCommand
{
	/// <summary>The hash of the sub scene to load.</summary>
	public Unity.Entities.Hash128 subsceneHash;
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientLoadSceneSystem : ISystem
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

		// Sub-scenes can't be loaded or unloaded in foreach loops because it causes structual changes to the entity hierarchy.
		Unity.Entities.Hash128 subSceneHash = new Unity.Entities.Hash128{Value = uint4.zero};

		foreach((RefRO<LoadSceneRpc> sceneHash, Entity entity) in SystemAPI.Query<RefRO<LoadSceneRpc>>().WithEntityAccess())
		{
			subSceneHash = sceneHash.ValueRO.subsceneHash;
			commandBuffer.DestroyEntity(entity);
		}

		if(subSceneHash.IsValid)
		{
			Object.FindFirstObjectByType<ClientManager>().LoadSubScene(subSceneHash);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}

