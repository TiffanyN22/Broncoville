using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct PlayerMovementSystem : ISystem
{
	public void OnUpdate(ref SystemState state)
	{
		// Get all player inputs and apply them.
		foreach ((RefRO<PlayerInputData> input, RefRW<LocalTransform> transform) in SystemAPI.Query<RefRO<PlayerInputData>, RefRW<LocalTransform>>().WithAll<Simulate>())
		{
			// Normalize the input to prevent illegal inputs.
			float2 safeInput = math.normalizesafe(input.ValueRO.movement);

			// Move the player.
			transform.ValueRW.Position += new float3(safeInput.x * SystemAPI.Time.DeltaTime * 2f, safeInput.y * SystemAPI.Time.DeltaTime * 2f, 0f);
			transform.ValueRW.Position = new float3(transform.ValueRW.Position.x, transform.ValueRW.Position.y, transform.ValueRW.Position.y);
		}
	}
}