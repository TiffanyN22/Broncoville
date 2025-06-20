using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;

public class ManageHelpItems : MonoBehaviour
{
    [SerializeField] GameObject helpBoardGroup;
    [SerializeField] GameObject addHelpItemGroup;
    [SerializeField] GameObject myHelpItemsGroup;
    [SerializeField] GameObject helpListGroup;
    [SerializeField] HelpBoard helpBoard;
    [SerializeField] private GameObject myHelpItemsPrefab;
    [SerializeField] private Transform myHelpListContent;
    private string username = "Unknown"; 

    // AddHelpItem Variable
    [SerializeField] private TMP_InputField addItemTitleInput;
    [SerializeField] private TMP_InputField addItemDescriptionInput;

    List<HelpDetailsInfo> myHelpItems;

    // Start is called before the first frame update
    void Start()
    {
        EntityManager entities = FindFirstObjectByType<ClientManager>().GetEntityManager();
        EntityQuery query = entities.CreateEntityQuery(typeof(NetworkId));
        NativeArray<NetworkId> accounts = query.ToComponentDataArray<NetworkId>(Allocator.Temp);

        if (accounts.Length == 0)
        {
            Debug.Log("Faile to find NetworkId");
            return;
        }

        int id = accounts[0].Value;

        query = entities.CreateEntityQuery(typeof(GhostOwner));
        NativeArray<Entity> players = query.ToEntityArray(Allocator.Temp);

        for (int i = 0; i < players.Length; ++i)
        {
            GhostOwner ghost = entities.GetComponentData<GhostOwner>(players[i]);

            if (ghost.NetworkId == id)
            {
                AccountData account = entities.GetComponentData<AccountData>(players[i]);
                username = account.name.ToString();
                break;
            }
        }
        // // EntityManager entities = FindFirstObjectByType<ClientManager>().GetEntityManager();
        //     // NativeArray<AccountData> accounts = entities.CreateEntityQuery(typeof(NetworkId), typeof(AccountInfo));
        //     foreach (query)
        //         username = entities.GetComponentData<AccountData>(query.GetSingletonEntity()).name.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CloseHelpBoard()
    {
        if (helpBoardGroup != null) helpBoardGroup.SetActive(false);
    }

    public void ClickedAddItemPanel()
    {
        if (addHelpItemGroup != null && !addHelpItemGroup.activeSelf) addHelpItemGroup.SetActive(true);
        if (myHelpItemsGroup != null && myHelpItemsGroup.activeSelf) myHelpItemsGroup.SetActive(false);
    }

    public void ClickedCloseAddItem()
    {
        if (addHelpItemGroup != null && addHelpItemGroup.activeSelf) addHelpItemGroup.SetActive(false);
        // if (helpListGroup != null && !helpListGroup.activeSelf) helpListGroup.SetActive(true);

        // helpBoard.refreshItemListFromServer();
        if (myHelpItemsGroup != null && !myHelpItemsGroup.activeSelf) myHelpItemsGroup.SetActive(true);
        showMyHelpItems();
        addItemTitleInput.text = "";
        addItemDescriptionInput.text = "";
    }

    public void ClickedAddItem()
    {
        if ((addItemTitleInput.text != "") && (addItemDescriptionInput.text != ""))
        {
            HelpDetailsInfo newHelpItem = new HelpDetailsInfo(addItemTitleInput.text, username, addItemDescriptionInput.text);

            // Create item rpc
            EntityManager clientManager = FindFirstObjectByType<ClientManager>().GetEntityManager();
            Entity createHelpItemRequest = clientManager.CreateEntity(typeof(CreateHelpItemRequestRpc), typeof(SendRpcCommandRequest));
            string newGuid = newHelpItem.guid.ToString();
            clientManager.SetComponentData(createHelpItemRequest, new CreateHelpItemRequestRpc { topic = addItemTitleInput.text, requester = username, numHelpBoardEntries = 1, guid = newGuid });

            // Create description rpc
            int descriptionLength = addItemDescriptionInput.text.Length;
            for (int j = 0; j < descriptionLength; j += 125)
            {
                Entity createHelpDescriptionRequest = clientManager.CreateEntity(typeof(CreateHelpDescriptionRequestRpc), typeof(SendRpcCommandRequest));
                clientManager.SetComponentData(createHelpDescriptionRequest, new CreateHelpDescriptionRequestRpc { descriptionNumPackets = descriptionLength / 125 + 1, index = j, guid = newGuid, description = addItemDescriptionInput.text.Substring(j, Math.Min(descriptionLength - j, 125)) });
            }
            myHelpItems.Add(newHelpItem);
            ClickedCloseAddItem();
        }
    }

    public void ClickedViewMyItems()
    {
        if (helpListGroup != null && helpListGroup.activeSelf) helpListGroup.SetActive(false);
        if (myHelpItemsGroup != null && !myHelpItemsGroup.activeSelf) myHelpItemsGroup.SetActive(true);
        myHelpItems = helpBoard.GetAllHelpItems().Where(h => h.requester == username).ToList();
        showMyHelpItems();
    }

    public void ClickedClosedMyItems()
    {
        if (helpListGroup != null && !helpListGroup.activeSelf) helpListGroup.SetActive(true);
        if (myHelpItemsGroup != null && myHelpItemsGroup.activeSelf) myHelpItemsGroup.SetActive(false);
        helpBoard.refreshHelpDetails();
    }

    public void showMyHelpItems()
    {
        // delete previous items
        foreach (Transform child in myHelpListContent)
        {
            Destroy(child.gameObject);
        }

        // add new items
        foreach (HelpDetailsInfo curItem in myHelpItems)
        {
            AddMyItemToScrollview(curItem);
        }
    }

    public void AddMyItemToScrollview(HelpDetailsInfo newItem)
    {
        GameObject helpItemObject = Instantiate(myHelpItemsPrefab, myHelpListContent);
        MyHelpBoardItem helpItem = helpItemObject.GetComponent<MyHelpBoardItem>();
        helpItem.SetInfo(newItem.topic, username, newItem.guid);
        helpItem.SetAllHelpItems(helpBoard.GetAllHelpItems().ToList());
        helpItem.SetManageHelpItems(this);
        helpItem.transform.localScale = Vector3.one;
    }

    public void DeleteMyHelpItems(Guid guid)
    {
        HelpDetailsInfo itemToDelete = myHelpItems.Find(curItem => curItem.guid == guid);
        myHelpItems.Remove(itemToDelete);
    }
}
