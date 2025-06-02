using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public struct PlayerAnimation : IComponentData
{
	public float nextFrame;
	public int frame;
}

[MaterialProperty("FrameNumber")]
public struct MaterialOverrideFrameNumber : IComponentData
{
	public float frameNumber;
}

[MaterialProperty("BodyColor")]
public struct MaterialOverrideBodyColor : IComponentData
{
	public Color bodyColor;
}

[MaterialProperty("HairColor")]
public struct MaterialOverrideHairColor : IComponentData
{
	public Color hairColor;
}

[MaterialProperty("FlipDirection")]
public struct MaterialOverrideFlipDirection : IComponentData
{
	public float flipDirection;
}

[MaterialProperty("HairStyle")]
public struct MaterialOverrideHairStyle : IComponentData
{
	public float hairStyle;
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientPlayerAnimationSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		// Only run this system if the client is connected but not in game.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<AccountData, PlayerAnimation, PlayerInputData, MaterialOverrideFrameNumber>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
		EntityManager entities = GameObject.FindFirstObjectByType<ClientManager>().GetEntityManager();

		foreach((RefRO<AccountData> account, RefRW<PlayerAnimation> animation, RefRO<PlayerInputData> input, RefRW<MaterialOverrideFrameNumber> frame, Entity entity) in SystemAPI.Query<RefRO<AccountData>, RefRW<PlayerAnimation>, RefRO<PlayerInputData>, RefRW<MaterialOverrideFrameNumber>>().WithEntityAccess())
		{
			entities.SetComponentData(entity, new MaterialOverrideBodyColor{bodyColor = account.ValueRO.bodyColor});
			entities.SetComponentData(entity, new MaterialOverrideHairColor{hairColor = account.ValueRO.hairColor});
			entities.SetComponentData(entity, new MaterialOverrideHairStyle{hairStyle = (int) account.ValueRO.hairStyle});

			if(input.ValueRO.movement.x != 0f)
			{
				commandBuffer.SetComponent(entity, new MaterialOverrideFlipDirection{flipDirection = input.ValueRO.movement.x < 0f ? 1f : 0f});
			}

			if(input.ValueRO.movement.x == 0 && input.ValueRO.movement.y == 0)
			{
				frame.ValueRW.frameNumber = 0;
			}
			else if((animation.ValueRW.nextFrame -= Time.deltaTime) <= 0)
			{
				++frame.ValueRW.frameNumber;
				frame.ValueRW.frameNumber %= 6;
				animation.ValueRW.nextFrame = 0.125f;
			}
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientSetupPlayerAnimationSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		// Only run this system if the client is connected but not in game.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<AccountData>().WithNone<PlayerAnimation>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

		foreach((RefRO<AccountData> _, Entity entity) in SystemAPI.Query<RefRO<AccountData>>().WithNone<PlayerAnimation>().WithEntityAccess())
		{
			commandBuffer.AddComponent<PlayerAnimation>(entity);
			commandBuffer.AddComponent<MaterialOverrideFrameNumber>(entity);
			commandBuffer.AddComponent<MaterialOverrideBodyColor>(entity);
			commandBuffer.AddComponent<MaterialOverrideHairColor>(entity);
			commandBuffer.AddComponent<MaterialOverrideFlipDirection>(entity);
			commandBuffer.AddComponent<MaterialOverrideHairStyle>(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}