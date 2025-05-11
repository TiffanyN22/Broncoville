using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyHelpBoardItem : MonoBehaviour
{

    [SerializeField] private TMP_Text itemTopic; 
    [SerializeField] private Button deleteButton;
    [SerializeField] private string helpRequester;
    private List<HelpDetailsInfo> allHelpItems;
    private ManageHelpItems manageHelpItems;

    void Start(){
      deleteButton.onClick.AddListener(clickedDelete);
    }

    public void SetInfo(string topic, string requester){
      itemTopic.text = topic;
      helpRequester = requester;
    }

    public void SetAllHelpItems(List<HelpDetailsInfo> itemList){
      allHelpItems = itemList;
    }

    public void SetManageHelpItems(ManageHelpItems manager){
      manageHelpItems = manager;
    }


    public void clickedDelete(){
      allHelpItems.Remove(allHelpItems.FirstOrDefault(curItem => curItem.topic == itemTopic.text && curItem.requester == helpRequester));
      manageHelpItems.showMyHelpItems();
    }
}