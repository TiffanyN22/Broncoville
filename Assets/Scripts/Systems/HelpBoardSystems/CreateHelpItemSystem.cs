using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct CreateHelpItemRequestRpc : IRpcCommand
{
	public FixedString32Bytes topic;
	public FixedString32Bytes requester;
	public int numHelpBoardEntries;
	public FixedString128Bytes guid;
}

public struct CreateHelpDescriptionRequestRpc : IRpcCommand
{
	public int descriptionNumPackets;
	public int index;
	public FixedString128Bytes guid;
	public FixedString128Bytes description;
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
	private static Dictionary<string, string[]> allDescriptions;
	public void OnCreate(ref SystemState state)
	{
		// if (allDescriptions == null)
		allDescriptions = new Dictionary<string, string[]>();

		// Only run this system if create help item requests are available.
		EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAny<CreateHelpItemRequestRpc>().WithAny<CreateHelpDescriptionRequestRpc>().WithAll<ReceiveRpcCommandRequest>();
		state.RequireForUpdate(state.GetEntityQuery(builder));
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
		HelpBoardEntryList helpBoardEntryList = UnityEngine.Object.FindFirstObjectByType<HelpBoardEntryList>();
		// List<HelpDetailsInfo> allHelpItems = GameObject.FindFirstObjectByType<HelpBoardEntryList>().getAllHelpItems();

		// Get all unprocessed create help item requests and iterate through them all.
		foreach ((RefRO<CreateHelpItemRequestRpc> createHelpItem, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<CreateHelpItemRequestRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			// Send a response message to the client saying whether or not account creation was successful.
			Entity response = commandBuffer.CreateEntity();

			string topic = createHelpItem.ValueRO.topic.ToString();
			string requester = createHelpItem.ValueRO.requester.ToString();
			string guid = createHelpItem.ValueRO.guid.ToString();

			EntityManager entities = UnityEngine.Object.FindFirstObjectByType<ClientManager>().GetEntityManager();

			HelpDetailsInfo helpDetails = new HelpDetailsInfo(topic, requester, "Loading...", Guid.Parse(guid));

			// helpDetails.SaveToFile();
			helpBoardEntryList.addItem(helpDetails);
			Debug.Log(topic);

			FixedString128Bytes reason = "";
			// TODO: input validation if needed

			commandBuffer.AddComponent(response, new CreateHelpItemResponseRpc { accepted = reason.Length == 0, reason = reason });
			commandBuffer.AddComponent(response, new SendRpcCommandRequest { TargetConnection = request.ValueRO.SourceConnection });

			// Destroy the message now that it has been processed.
			commandBuffer.DestroyEntity(entity);
		}

		foreach ((RefRO<CreateHelpDescriptionRequestRpc> createHelpDescriptionItem, RefRO<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRO<CreateHelpDescriptionRequestRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
		{
			Entity response = commandBuffer.CreateEntity();

			int numPackets = createHelpDescriptionItem.ValueRO.descriptionNumPackets;
			int index = createHelpDescriptionItem.ValueRO.index;
			string guid = createHelpDescriptionItem.ValueRO.guid.ToString();
			string description = createHelpDescriptionItem.ValueRO.description.ToString();


			if (!allDescriptions.ContainsKey(guid))
			{
				allDescriptions[guid] = new string[numPackets];
			}
			allDescriptions[guid][index] = description;

			// check if can add becuase whole description is full
			if (allDescriptions.TryGetValue(guid, out string[] values) && values != null && values.All(curVal => curVal != null))
			{
				StringBuilder s = new StringBuilder(allDescriptions[guid].Length * 125 + 1);
				foreach (string curString in allDescriptions[guid])
				{
					s.Append(curString);
				}

				helpBoardEntryList.updateDescription(Guid.Parse(guid), s.ToString());
			}
			commandBuffer.DestroyEntity(entity);
		}
		commandBuffer.Playback(state.EntityManager);
		commandBuffer.Dispose();
	}
}