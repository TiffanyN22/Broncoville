using Unity.Scenes;
using UnityEngine;

public class SubSceneManager : MonoBehaviour
{
	/// <summary>The main hub sub-scene.</summary>
	[Tooltip("The main hub sub-scene.")]
	[SerializeField] private SubScene mainHubSubScene = null;

	/// <summary>
	/// Get the main hub's sub scene. 
	/// </summary>
	/// <returns></returns>
	public SubScene GetMainHubSubScene()
	{
		return this.mainHubSubScene;
	}

	/// <summary>
	/// Get the main hub's GUID. Useful for sending RPCs.
	/// </summary>
	/// <returns>The main hub's 'GUID.</returns>
	public Unity.Entities.Hash128 GetMainHubGUID()
	{
		return this.mainHubSubScene.SceneGUID;
	}
}