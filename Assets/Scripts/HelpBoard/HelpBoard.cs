using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
public class HelpBoard : MonoBehaviour
{
    // TODO: create interface to create help items
    private HelpDetailsInfo[] allHelpItems = null;
    // private Dictionary<int, List<string>> allHelpDescriptions = new Dictionary<int, List<string>>();
    [SerializeField] private GameObject helpItemPrefab;
    [SerializeField] private Transform helpListContent;
    [SerializeField] private GameObject helpDetails;
    [SerializeField] private GameObject helpList;

    void Start()
    {
        // Send a create account request to the server.
		EntityManager clientManager = FindFirstObjectByType<ClientManager>().GetEntityManager();
		Entity getHelp = clientManager.CreateEntity(typeof(GetHelpRpc), typeof(SendRpcCommandRequest));
    }

    public void Update()
    {
        // Check for entities containing the create account response message.
        EntityManager entities = FindFirstObjectByType<ClientManager>().GetEntityManager();
        EntityQuery entries = entities.CreateEntityQuery(ComponentType.ReadOnly<HelpBoardEntryRpc>());

        // If a create account response was found, go back to the log in screen or show an error.
        foreach (Entity entity in entries.ToEntityArray(Allocator.Temp))
        {
            HelpBoardEntryRpc response = entities.GetComponentData<HelpBoardEntryRpc>(entity);

            if (allHelpItems == null)
            {
                allHelpItems = new HelpDetailsInfo[response.numHelpBoardEntries];
                for (int i = 0; i < response.numHelpBoardEntries; i++)
                {
                    allHelpItems[i] = new HelpDetailsInfo("Loading...", "Loading...", "Loading...");
                }
            }
            allHelpItems[response.id].topic = response.topic.ToString();
            allHelpItems[response.id].requester = response.requester.ToString();
            allHelpItems[response.id].guid = Guid.Parse(response.guid.ToString()); // TODO: fix
            Debug.Log(response.guid);
            // Debug.Log(response);

            refreshHelpDetails();

            entities.DestroyEntity(entity);
        }
    }

    public void refreshHelpDetails()
    {
        // delete previous items
        foreach (Transform child in helpListContent)
        {
            Destroy(child.gameObject);
        }

        // add allHelpItems
        foreach (HelpDetailsInfo curItem in allHelpItems)
        {
            AddItemToScrollview(curItem);
        }
    }

    public void AddItemToScrollview(HelpDetailsInfo newItem)
    {
        GameObject helpItemObject = Instantiate(helpItemPrefab, helpListContent);
        HelpBoardItem helpItem = helpItemObject.GetComponent<HelpBoardItem>();
        helpItem.helpDetailsInfo = newItem;
        helpItem.SetTopic(newItem.topic);
        helpItem.SetHelpDetails(helpDetails);
        helpItem.SetHelpList(helpList);
        helpItem.transform.localScale = Vector3.one;
    }

    public HelpDetailsInfo[] GetAllHelpItems()
    {
        return allHelpItems;
    }
}