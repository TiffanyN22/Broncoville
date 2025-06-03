using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
public struct GetHelpDescriptionRpc : IRpcCommand
{
  // public Guid id;
  public FixedString128Bytes id;
}

public struct HelpBoardEntryDescriptionRpc : IRpcCommand
{
  public int descriptionNumPackets;
  public int index;
  public FixedString128Bytes description;
}



[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerHelpBoardDescriptionSystem : ISystem
{

  public void OnCreate(ref SystemState state)
  {
    // Only run this system if create account requests are available.
    EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<GetHelpDescriptionRpc>().WithAll<ReceiveRpcCommandRequest>();
    state.RequireForUpdate(state.GetEntityQuery(builder));
  }
  
  public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

    // Get all unprocessed create account requests and iterate through them all.
    foreach ((RefRO<GetHelpDescriptionRpc> getHelpDescription, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<GetHelpDescriptionRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
    {
      Debug.Log("Help Board description system");
      Debug.Log(getHelpDescription.ValueRO.id);
      HelpDetailsInfo helpItem = GameObject.FindFirstObjectByType<HelpBoardEntryList>().getHelpDetailsInfoByGuid(Guid.Parse(getHelpDescription.ValueRO.id.ToString()));
      int descriptionLength = helpItem.description.Length;
      for (int j = 0; j < descriptionLength; j += 125)
      {
        Entity descriptionResponse = commandBuffer.CreateEntity();
        commandBuffer.AddComponent(descriptionResponse, new HelpBoardEntryDescriptionRpc {descriptionNumPackets = descriptionLength / 125 + 1, index = j / 125, description = helpItem.description.Substring(j, Math.Min(descriptionLength - j, 125))});
        commandBuffer.AddComponent(descriptionResponse, new SendRpcCommandRequest { TargetConnection = request.ValueRO.SourceConnection });
      }
      commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}