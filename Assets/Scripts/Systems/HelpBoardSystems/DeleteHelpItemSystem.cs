using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct DeleteHelpItemRequestRpc : IRpcCommand
{
	public FixedString128Bytes guid;
}


[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerDeleteHelpItemSystem : ISystem
{
	private static Dictionary<string, string[]> allDescriptions;
	public void OnCreate(ref SystemState state)
	{
		// if (allDescriptions == null)
		allDescriptions = new Dictionary<string, string[]>();

		// Only run this system if create help item requests are available.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<DeleteHelpItemRequestRpc>().WithAll<ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
		HelpBoardEntryList helpBoardEntryList = UnityEngine.Object.FindFirstObjectByType<HelpBoardEntryList>();

		// Get all unprocessed create help item requests and iterate through them all.
		foreach ((RefRO<DeleteHelpItemRequestRpc> deleteHelpItem, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<DeleteHelpItemRequestRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			// Send a response message to the client saying whether or not account creation was successful.
			Entity response = commandBuffer.CreateEntity();

			string guid = deleteHelpItem.ValueRO.guid.ToString();
			helpBoardEntryList.deleteItem(Guid.Parse(guid));

			// Destroy the message now that it has been processed.
			commandBuffer.DestroyEntity(entity);
		}

		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}