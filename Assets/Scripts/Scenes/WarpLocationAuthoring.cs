using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public struct WarpLocation : IInputComponentData
{
	/// <summary>The sub scene to warp to.</summary>
	public Location subScene;

	/// <summary>The position to appear at in the sub scene.</summary>
	public float3 position;
}

[DisallowMultipleComponent]
public class WarpLocationAuthoring : MonoBehaviour
{
	/// <summary>The subscene that the warp will teleport the player to.</summary>
	[Tooltip("The subscene that the warp will teleport the player to.")]
	[SerializeField] private Location warpLocation = Location.MAIN_HUB;

	/// <summary>The position that the player will be placed at.</summary>
	[Tooltip("The position that the player will be placed at.")]
	[SerializeField] private Vector3 warpPosition = Vector2.zero;

	public Location GetLocation()
	{
		return this.warpLocation;
	}

	public Vector3 GetPosition()
	{
		return this.warpPosition;
	}
}

class WarpLocationBaker : Baker<WarpLocationAuthoring>
{
	public override void Bake(WarpLocationAuthoring authoring)
	{
		Entity entity = GetEntity(TransformUsageFlags.Dynamic);
		AddComponent(entity, new WarpLocation{subScene = authoring.GetLocation(), position = authoring.GetPosition()});
	}
}