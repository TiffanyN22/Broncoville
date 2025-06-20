using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Scenes;
using UnityEngine;

public enum Location
{
	MAIN_HUB,
	STUDY_ROOM_LOBBY,
	STUDY_ROOM
}

public class SubSceneManager : MonoBehaviour
{
	/// <summary>The main hub sub-scene.</summary>
	[Tooltip("The main hub sub-scene.")]
	[SerializeField] private SubScene mainHubSubScene = null;
	[SerializeField] private SubScene studyRoomScene = null;

	/// <summary>The main hub sub-scene.</summary>
	[Tooltip("The main hub sub-scene.")]
	[SerializeField] private SubScene studyRoomLobbySubScene = null;

	private Dictionary<Unity.Entities.Hash128, Vector2> offset = new Dictionary<Unity.Entities.Hash128, Vector2>();

	public void Start()
	{
		this.offset.Add(this.mainHubSubScene.SceneGUID, this.mainHubSubScene.transform.position);
		this.offset.Add(this.studyRoomLobbySubScene.SceneGUID, this.studyRoomLobbySubScene.transform.position);
		this.offset.Add(this.studyRoomScene.SceneGUID, this.studyRoomScene.transform.position);
	}

	/// <summary>
	/// Get the main hub's sub scene. 
	/// </summary>
	/// <returns></returns>
	public SubScene GetSubScene(Location location)
	{
		switch(location)
		{
		case Location.MAIN_HUB:
			return this.mainHubSubScene;
		case Location.STUDY_ROOM_LOBBY:
			return this.studyRoomLobbySubScene;
		case Location.STUDY_ROOM:
			return this.studyRoomScene;
		default:
			return null;
		}
	}

	/// <summary>
	/// Get the main hub's sub scene. 
	/// </summary>
	/// <returns></returns>
	public Vector2 GetOffset(Unity.Entities.Hash128 guid)
	{
		return this.offset.ContainsKey(guid) ? this.offset[guid] : Vector2.zero;
	}

	/// <summary>
	/// Get the main hub's GUID. Useful for sending RPCs.
	/// </summary>
	/// <returns>The main hub's 'GUID.</returns>
	public Unity.Entities.Hash128 GetGUID(Location location)
	{
		switch(location)
		{
		case Location.MAIN_HUB:
			return this.mainHubSubScene.SceneGUID;
		case Location.STUDY_ROOM_LOBBY:
			return this.studyRoomLobbySubScene.SceneGUID;
		case Location.STUDY_ROOM:
			return this.studyRoomScene.SceneGUID;
		default:
			return new Unity.Entities.Hash128{Value = uint4.zero};
		}
	}
}