using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public struct PlayerInputData : IInputComponentData
{
	/// <summary>The player's movement direction.</summary>
	public float2 movement;
}

[DisallowMultipleComponent]
public class PlayerInput : MonoBehaviour
{
	// ECHO Echo echo echo ...
}

class PlayerInputBaker : Baker<PlayerInput>
{
	public override void Bake(PlayerInput authoring)
	{
		// Add the player input data component to the entity during game object conversion.
		Entity playerInput = GetEntity(TransformUsageFlags.Dynamic);
		AddComponent<PlayerInputData>(playerInput);
	}
}