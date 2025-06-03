using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerWarpSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<WarpLocation, PhysicsCollider, LocalTransform>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

		foreach((RefRO<WarpLocation> location, RefRO<PhysicsCollider> trigger, RefRO<LocalTransform> triggerPosition) in SystemAPI.Query<RefRO<WarpLocation>, RefRO<PhysicsCollider>, RefRO<LocalTransform>>())
		{
			Aabb triggerBox = trigger.ValueRO.Value.Value.CalculateAabb();
			float3 triggerPos = triggerPosition.ValueRO.Position;
			triggerBox.Max += triggerPos;
			triggerBox.Min += triggerPos;

			foreach((RefRO<AccountData> account, RefRO<PhysicsCollider> hitbox, RefRW<LocalTransform> playerPosition, RefRO<GhostOwner> owner, Entity player) in SystemAPI.Query<RefRO<AccountData>, RefRO<PhysicsCollider>, RefRW<LocalTransform>, RefRO<GhostOwner>>().WithEntityAccess())
			{
				Aabb playerBox = hitbox.ValueRO.Value.Value.CalculateAabb();
				float3 playerPos = playerPosition.ValueRO.Position;
				playerBox.Max += playerPos;
				playerBox.Min += playerPos;

				if(!triggerBox.Overlaps(playerBox))
				{
					continue;
				}

				EntityManager serverEntities = Object.FindFirstObjectByType<ServerManager>().GetEntityManager();
				Unity.Entities.Hash128 guid = Object.FindFirstObjectByType<SubSceneManager>().GetGUID(location.ValueRO.subScene);
				PlayerSpawner playerSpawner = new PlayerSpawner{player = Entity.Null};

				foreach((RefRO<PlayerSpawner> spawner, Entity spawnerEntity) in SystemAPI.Query<RefRO<PlayerSpawner>>().WithEntityAccess())
				{
					if(serverEntities.GetSharedComponent<SceneSection>(spawnerEntity).SceneGUID == guid)
					{
						playerSpawner = spawner.ValueRO;
						break;
					}
				}

				if(playerSpawner.player == Entity.Null)
				{
					Debug.Log("Failed to find the player spawner.");
					continue;
				}

				Entity newPlayer = commandBuffer.Instantiate(playerSpawner.player);

				commandBuffer.SetComponent(newPlayer, account.ValueRO);
				commandBuffer.SetComponent(newPlayer, owner.ValueRO);
				commandBuffer.SetComponent(newPlayer, new LocalTransform{Position = location.ValueRO.position, Scale = 1f, Rotation = quaternion.identity});

				commandBuffer.DestroyEntity(player);
				
				foreach((RefRO<NetworkId> id, Entity connection) in SystemAPI.Query<RefRO<NetworkId>>().WithEntityAccess())
				{
					if(id.ValueRO.Value != owner.ValueRO.NetworkId)
					{
						continue;
					}

					Entity loadScene = commandBuffer.CreateEntity();

					commandBuffer.AddComponent(loadScene, new LoadSceneRpc{subsceneHash = Object.FindAnyObjectByType<SubSceneManager>().GetGUID(location.ValueRO.subScene)});
					commandBuffer.AddComponent(loadScene, new SendRpcCommandRequest{TargetConnection = connection});
					break;
				}
			}
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}

