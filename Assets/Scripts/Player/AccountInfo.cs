using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct AccountData : IComponentData
{
	/// <summary>The account's username.</summary>
	[GhostField] public FixedString32Bytes name;

	/// <summary>The player's body color.</summary>
	[GhostField] public Color bodyColor;

	/// <summary>The player's hair color.</summary>
	[GhostField] public Color hairColor;

	/// <summary>The player's hair style.</summary>
	[GhostField] public HairStyle hairStyle;
}

public class AccountInfo : MonoBehaviour
{

}

class AccountBaker : Baker<AccountInfo>
{
	public override void Bake(AccountInfo authoring)
	{
		// Add the player data component to the entity during game object conversion.
		Entity player = GetEntity(TransformUsageFlags.Dynamic);
		AddComponent<AccountData>(player);
	}
}