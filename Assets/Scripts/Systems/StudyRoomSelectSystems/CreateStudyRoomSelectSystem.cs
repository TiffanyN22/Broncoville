using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct CreateStudyRoomSelectRpc : IRpcCommand
{
	public bool isPublic;
  public FixedString128Bytes roomName;
  public int room4NumID;
  public FixedString128Bytes roomGuid;
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerCreateStudyRoomSelectionSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		// Only run this system if create help item requests are available.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAny<CreateStudyRoomSelectRpc>().WithAny<CreateHelpDescriptionRequestRpc>().WithAll<ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
		StudyRoomEntryList studyRoomEntryList = UnityEngine.Object.FindFirstObjectByType<StudyRoomEntryList>();

		// Get all unprocessed create help item requests and iterate through them all.
		foreach ((RefRO<CreateStudyRoomSelectRpc> createStudyRoomSelect, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<CreateStudyRoomSelectRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			// Send a response message to the client saying whether or not account creation was successful.
			Entity response = commandBuffer.CreateEntity();

			bool isPublic = createStudyRoomSelect.ValueRO.isPublic;
			string roomName = createStudyRoomSelect.ValueRO.roomName.ToString();
			int room4NumID = createStudyRoomSelect.ValueRO.room4NumID;
			string roomGuid = createStudyRoomSelect.ValueRO.roomGuid.ToString();

			Debug.Log("In create study room");
			Debug.Log(roomGuid);

			EntityManager entities = UnityEngine.Object.FindFirstObjectByType<ClientManager>().GetEntityManager();

			OpenRoom newRoom = new OpenRoom(isPublic, roomName, room4NumID, Guid.Parse(roomGuid));

			studyRoomEntryList.addRoom(newRoom);

			// Destroy the message now that it has been processed.
			commandBuffer.DestroyEntity(entity);
		}
		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}