using System.Collections.Generic;
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
}

public struct HelpBoardEntryDescriptionRpc : IRpcCommand
{
  public int id;
  // public int descriptionNumPackets;
  public int index;
  public FixedString128Bytes description;
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
        commandBuffer.AddComponent(response, new HelpBoardEntryRpc { topic = allHelpItems[i].topic, requester = allHelpItems[i].requester, id = i, numHelpBoardEntries = allHelpItems.Count});
        commandBuffer.AddComponent(response, new SendRpcCommandRequest { TargetConnection = request.ValueRO.SourceConnection });
        int descriptionLength = allHelpItems[i].description.Length;
        // for (int j = 0; j < descriptionLength; j += 125)
        // {
        //   Entity descriptionResponse = commandBuffer.CreateEntity();
        //   commandBuffer.AddComponent(descriptionResponse, new HelpBoardEntryDescriptionRpc { id = i, index = j / 125, description = allHelpItems[i].description.Substring(j, min(descriptionLength - j, 125)) });
        //   commandBuffer.AddComponent(descriptionResponse, new SendRpcCommandRequest { TargetConnection = request.valueRO.SourceConnection });
        // }
      }

      commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}