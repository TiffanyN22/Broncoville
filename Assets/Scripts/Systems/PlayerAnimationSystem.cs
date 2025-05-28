using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor.Animations;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct PlayerAnimationSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		// Only run this system if the client is connected but not in game.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerData, AccountData, PlayerInputData, MaterialMeshInfo>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
		EntityManager entities = GameObject.FindFirstObjectByType<ClientManager>().GetEntityManager();

		foreach((RefRW<PlayerData> player, RefRO<AccountData> account, RefRO<PlayerInputData> input, RefRO<MaterialMeshInfo> meshInfo, Entity entity) in SystemAPI.Query<RefRW<PlayerData>, RefRO<AccountData>, RefRO<PlayerInputData>, RefRO<MaterialMeshInfo>>().WithEntityAccess())
		{
			RenderMeshArray meshes = entities.GetSharedComponentManaged<RenderMeshArray>(entity);
			Material bodyMaterial = meshes.GetMaterial(meshInfo.ValueRO);

			DynamicBuffer<Child> hair = entities.GetBuffer<Child>(entity);
			meshes = entities.GetSharedComponentManaged<RenderMeshArray>(hair[0].Value);
			Material hairMaterial = meshes.GetMaterial(entities.GetComponentData<MaterialMeshInfo>(hair[0].Value));
			
			bodyMaterial.color = account.ValueRO.bodyColor;
			hairMaterial.color = account.ValueRO.hairColor;

			if (input.ValueRO.movement.x < 0)
			{
				bodyMaterial.mainTextureScale = new Vector2(-1f, 0.125f);
				hairMaterial.mainTextureScale = new Vector2(-1f, 0.125f);
			}
			else if (input.ValueRO.movement.x > 0)
			{
				bodyMaterial.mainTextureScale = new Vector2(1f, 0.125f);
				hairMaterial.mainTextureScale = new Vector2(1f, 0.125f);
			}
			else if (bodyMaterial.mainTextureScale.y != 0.125f)
			{
				bodyMaterial.mainTextureScale = new Vector2(1f, 0.125f);
				hairMaterial.mainTextureScale = new Vector2(1f, 0.125f);
			}

			if(input.ValueRO.movement.x == 0 && input.ValueRO.movement.y == 0)
			{
				bodyMaterial.mainTextureOffset = new Vector2(bodyMaterial.mainTextureScale.x == -1f ? 1f : 0f, 0.875f);
				hairMaterial.mainTextureOffset = new Vector2(hairMaterial.mainTextureScale.x == -1f ? 1f : 0f, 0.875f);
				player.ValueRW.nextFrameTime = 0;
				player.ValueRW.frame = 0;
			}
			else if((player.ValueRW.nextFrameTime -= Time.deltaTime) <= 0)
			{
				bodyMaterial.mainTextureOffset = new Vector2(bodyMaterial.mainTextureScale.x == -1f ? 1f : 0f, 0.75f - 0.125f * player.ValueRO.frame);
				hairMaterial.mainTextureOffset = new Vector2(hairMaterial.mainTextureScale.x == -1f ? 1f : 0f, 0.75f - 0.125f * player.ValueRO.frame);
				++player.ValueRW.frame;
				player.ValueRW.frame %= 6;
				player.ValueRW.nextFrameTime = 0.125f;
			}
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}