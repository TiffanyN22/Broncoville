using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
public struct GetHelpRpc : IRpcCommand
{

}

public struct HelpBoardEntryRpc : IRpcCommand
{
  public FixedString32Bytes topic;
  public FixedString32Bytes requester;
  public int id;
  public int numHelpBoardEntries;
  public FixedString128Bytes guid;
  // public Guid guid;
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerHelpBoardSystem : ISystem
{

  public void OnCreate(ref SystemState state)
  {
    // Only run this system if create account requests are available.
    EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<GetHelpRpc>().WithAll<ReceiveRpcCommandRequest>();
    state.RequireForUpdate(state.GetEntityQuery(builder));
  }
  
  public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

    // Get all unprocessed create account requests and iterate through them all.
    foreach ((RefRO<GetHelpRpc> getHelp, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<GetHelpRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
    {
      List<HelpDetailsInfo> allHelpItems = GameObject.FindFirstObjectByType<HelpBoardEntryList>().getAllHelpItems();
      for (int i = 0; i < allHelpItems.Count; ++i)
      {
        Entity response = commandBuffer.CreateEntity();
        HelpBoardEntryRpc newRpc = new HelpBoardEntryRpc { topic = allHelpItems[i].topic, requester = allHelpItems[i].requester, id = i, numHelpBoardEntries = allHelpItems.Count, guid = allHelpItems[i].guid.ToString()};
        Debug.Log(newRpc.guid);
        commandBuffer.AddComponent(response,newRpc);
        commandBuffer.AddComponent(response, new SendRpcCommandRequest { TargetConnection = request.ValueRO.SourceConnection });
      }

      commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}