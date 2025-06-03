using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct PlayerMovementSystem : ISystem
{
	public void OnUpdate(ref SystemState state)
	{
		// Get all player inputs and apply them.
		foreach ((RefRO<PlayerInputData> input, RefRW<LocalTransform> position, RefRW<PhysicsVelocity> physics) in SystemAPI.Query<RefRO<PlayerInputData>, RefRW<LocalTransform>, RefRW<PhysicsVelocity>>().WithAll<Simulate>())
		{
			// Normalize the input to prevent illegal inputs.
			float2 safeInput = math.normalizesafe(input.ValueRO.movement);

			// Move the player.
			physics.ValueRW.Linear = new float3(2f * safeInput.x, 2f * safeInput.y, 0f);
			position.ValueRW.Position = new float3(position.ValueRW.Position.x, position.ValueRW.Position.y, position.ValueRW.Position.y);
		}
	}
}