using Unity.Entities;
using UnityEngine;

public struct PlayerSpawner : IComponentData
{
	/// <summary>The player prefab in entity form.</summary>
	public Entity player;
}

[DisallowMultipleComponent]
public class PlayerSpawnerAuthoring : MonoBehaviour
{
	/// <summary>The player prefab.</summary>
	[Tooltip("The player prefab.")]
	[SerializeField] private GameObject playerPrefab = null;

	public GameObject GetPlayerPrefab()
	{
		return this.playerPrefab;
	}
}

public class PlayerSpawnerBaker : Baker<PlayerSpawnerAuthoring>
{
	public override void Bake(PlayerSpawnerAuthoring authoring)
	{
		Entity mainHubSpawner = GetEntity(TransformUsageFlags.Dynamic);
		AddComponent(mainHubSpawner, new PlayerSpawner{player = GetEntity(authoring.GetPlayerPrefab(), TransformUsageFlags.Dynamic)});
	}
}