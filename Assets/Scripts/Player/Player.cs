using Unity.Entities;
using UnityEngine;

public struct PlayerData : IComponentData
{
	// I'm so empty.
}

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
	// I am the void.
}

class PlayerBaker : Baker<Player>
{
	public override void Bake(Player authoring)
	{
		// Add the player data component to the entity during game object conversion.
		Entity player = GetEntity(TransformUsageFlags.Dynamic);
		AddComponent<PlayerData>(player);
	}
}