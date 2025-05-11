using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial struct PlayerControlSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		// Only run this if the player is in-game.
		state.RequireForUpdate<NetworkStreamInGame>();
	}

	public void OnUpdate(ref SystemState state)
	{
		// Get the player's input data and update it.
		foreach(var playerInput in SystemAPI.Query<RefRW<PlayerInputData>>().WithAll<GhostOwnerIsLocal>())
		{
			// Use WASD to control the player.
			Vector2 moveVec = default;

			if(Input.GetKey(KeyCode.W))
			{
				moveVec += Vector2.up;
			}

			if(Input.GetKey(KeyCode.S))
			{
				moveVec += Vector2.down;
			}

			if(Input.GetKey(KeyCode.A))
			{
				moveVec += Vector2.left;
			}

			if(Input.GetKey(KeyCode.D))
			{
				moveVec += Vector2.right;
			}

			playerInput.ValueRW.movement = moveVec;
		}
	}
}