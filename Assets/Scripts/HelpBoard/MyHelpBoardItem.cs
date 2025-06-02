using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
public class MyHelpBoardItem : MonoBehaviour
{

  [SerializeField] private TMP_Text itemTopic; 
  [SerializeField] private Button deleteButton;
  [SerializeField] private string helpRequester;
  [SerializeField] private Guid itemGuid;
  private List<HelpDetailsInfo> allHelpItems;
  private ManageHelpItems manageHelpItems;

  void Start(){
    deleteButton.onClick.AddListener(clickedDelete);
  }

  public void SetInfo(string topic, string requester, Guid guid)
  {
    itemTopic.text = topic;
    helpRequester = requester;
    itemGuid = guid;
  }

  public void SetAllHelpItems(List<HelpDetailsInfo> itemList){
    allHelpItems = itemList;
  }

  public void SetManageHelpItems(ManageHelpItems manager){
    manageHelpItems = manager;
  }


  public void clickedDelete()
  {
    manageHelpItems.DeleteMyHelpItems(itemGuid);
    manageHelpItems.showMyHelpItems();

    HelpDetailsInfo deletedItem = allHelpItems.FirstOrDefault(curItem => curItem.guid == itemGuid);
    if (deletedItem == null)
    {
      Debug.Log("item to delete doesn't exist");
      return;
    }
    EntityManager clientManager = FindFirstObjectByType<ClientManager>().GetEntityManager();
    Entity deleteHelpItemRequest = clientManager.CreateEntity(typeof(DeleteHelpItemRequestRpc), typeof(SendRpcCommandRequest));
    clientManager.SetComponentData(deleteHelpItemRequest, new DeleteHelpItemRequestRpc { guid = deletedItem.guid.ToString() });

    allHelpItems.Remove(deletedItem);
  }
}