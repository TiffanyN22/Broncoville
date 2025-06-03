using Unity.Entities;
using UnityEngine;

public struct PlayerData : IComponentData
{
	// You hear wind whistling through these braces.
}

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
	// I must consume.
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