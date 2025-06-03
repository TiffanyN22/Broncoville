using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
public struct GetStudyRoomSelectRpc : IRpcCommand
{

}

public struct StudyRoomSelectEntryRpc : IRpcCommand
{
  public bool isPublic;
  public FixedString128Bytes roomName; // TODO: input validatoin
  public int room4NumID;
  public FixedString128Bytes roomGuid;
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerStudyRoomSelectSystem : ISystem
{

  public void OnCreate(ref SystemState state)
  {
    // Only run this system if create account requests are available.
    EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<GetStudyRoomSelectRpc>().WithAll<ReceiveRpcCommandRequest>();
    state.RequireForUpdate(state.GetEntityQuery(builder));
  }
  
  public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

    // Get all unprocessed create account requests and iterate through them all.
    foreach ((RefRO<GetStudyRoomSelectRpc> getStudyRoomSelect, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<GetStudyRoomSelectRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
    {
      List<OpenRoom> allRooms = GameObject.FindFirstObjectByType<StudyRoomEntryList>().getAllRooms();
      for (int i = 0; i < allRooms.Count; ++i)
      {
        Entity response = commandBuffer.CreateEntity();
        StudyRoomSelectEntryRpc newRpc = new StudyRoomSelectEntryRpc { isPublic = allRooms[i].isPublic, roomName = allRooms[i].roomName, room4NumID = allRooms[i].room4NumID, roomGuid = allRooms[i].roomGuid.ToString()};
        // Debug.Log(newRpc.guid);
        commandBuffer.AddComponent(response,newRpc);
        commandBuffer.AddComponent(response, new SendRpcCommandRequest { TargetConnection = request.ValueRO.SourceConnection });
      }

      commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}