using Unity.Scenes;
using UnityEngine;

public enum Scenes {MAIN_HUB, STUDY_ROOM_LOBBY, STUDY_ROOM}
public class SubSceneManager : MonoBehaviour
{
	/// <summary>The main hub sub-scene.</summary>
	[Tooltip("The main hub sub-scene.")]
	[SerializeField] private SubScene mainHubSubScene = null;
	[SerializeField] private SubScene studyRoomLobbyScene = null;
	[SerializeField] private SubScene studyRoomScene = null;

	/// <summary>
	/// Get the main hub's sub scene. 
	/// </summary>
	/// <returns></returns>
	public SubScene GetSubScene(Scenes scene)
	{
		switch (scene)
		{
			case (Scenes.MAIN_HUB):
				return this.mainHubSubScene;
			case (Scenes.STUDY_ROOM_LOBBY):
				return this.studyRoomLobbyScene;
			case (Scenes.STUDY_ROOM):
				return this.studyRoomScene;
			default:
				return this.mainHubSubScene;
		}
	}

	/// <summary>
	/// Get the main hub's GUID. Useful for sending RPCs.
	/// </summary>
	/// <returns>The main hub's 'GUID.</returns>
	public Unity.Entities.Hash128 GetMainHubGUID(Scenes scene)
	{
		switch (scene)
		{
			case (Scenes.MAIN_HUB):
				return this.mainHubSubScene.SceneGUID;
			case (Scenes.STUDY_ROOM_LOBBY):
				return this.studyRoomLobbyScene.SceneGUID;
			case (Scenes.STUDY_ROOM):
				return this.studyRoomScene.SceneGUID;
			default:
				return this.mainHubSubScene.SceneGUID;
		}
	}
}