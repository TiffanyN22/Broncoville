using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using System;
using System.Collections.Generic;

public struct CreateHelpItemRequestRpc : IRpcCommand
{
	public FixedString32Bytes topic;
  public FixedString32Bytes requester;
  public int numHelpBoardEntries;
}

public struct CreateHelpItemResponseRpc : IRpcCommand
{
	/// <summary>Whether or not the new help item successful.</summary>
	public bool accepted;

	/// <summary>If new item creation was unsuccessful, the reason why.</summary>
	public FixedString128Bytes reason;
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerCreateHelpItemSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		// Only run this system if create help item requests are available.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<CreateHelpItemRequestRpc>().WithAll<ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
		HelpBoardEntryList helpBoardEntryList = GameObject.FindFirstObjectByType<HelpBoardEntryList>();
		// List<HelpDetailsInfo> allHelpItems = GameObject.FindFirstObjectByType<HelpBoardEntryList>().getAllHelpItems();

		// Get all unprocessed create help item requests and iterate through them all.
		foreach ((RefRO<CreateHelpItemRequestRpc> createHelpItem, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<CreateHelpItemRequestRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			// Send a response message to the client saying whether or not account creation was successful.
			Entity response = commandBuffer.CreateEntity();

			string topic = createHelpItem.ValueRO.topic.ToString();
			string requester = createHelpItem.ValueRO.requester.ToString();
			HelpDetailsInfo helpDetails = new HelpDetailsInfo(topic, requester, "Loading...");

			// helpDetails.SaveToFile();
			helpBoardEntryList.addItem(helpDetails);

			FixedString128Bytes reason = "";
			// TODO: input validation if needed

			commandBuffer.AddComponent(response, new CreateHelpItemResponseRpc { accepted = reason.Length == 0, reason = reason });
			commandBuffer.AddComponent(response, new SendRpcCommandRequest { TargetConnection = request.ValueRO.SourceConnection });

			// Destroy the message now that it has been processed.
			commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}