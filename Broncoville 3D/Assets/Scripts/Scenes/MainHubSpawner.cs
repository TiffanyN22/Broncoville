using Unity.Entities;
using UnityEngine;

public struct MainHubSpawnerData : IComponentData
{
	/// <summary>The player prefab in entity form.</summary>
	public Entity player;
}

[DisallowMultipleComponent]
public class MainHubSpawner : MonoBehaviour
{
	/// <summary>The player prefab.</summary>
	[Tooltip("The player prefab.")]
	[SerializeField] private GameObject playerPrefab = null;

	// Optimal documentation right here folks.
	/// <summary>
	/// Get the player prefab.
	/// </summary>
	/// <returns>The player prefab.</returns>
	public GameObject GetPlayerPrefab()
	{
		return this.playerPrefab;
	}
}

public class MainHubSpawnerBaker : Baker<MainHubSpawner>
{
	public override void Bake(MainHubSpawner authoring)
	{
		// Add the min hub spawner data component to the entity during game object conversion.
		Entity mainHubSpawner = GetEntity(TransformUsageFlags.Dynamic);
		AddComponent(mainHubSpawner, new MainHubSpawnerData{player = GetEntity(authoring.GetPlayerPrefab(), TransformUsageFlags.Dynamic)});
	}
}