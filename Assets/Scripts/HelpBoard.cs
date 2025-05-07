using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelpBoard : MonoBehaviour
{
  [SerializeField] private  List<HelpDetailsInfo> allHelpItems = new List<HelpDetailsInfo>();
  // TODO: create interface to create help items
  [SerializeField] private GameObject helpItemPrefab;
  [SerializeField] private Transform helpListContent;
  [SerializeField] private GameObject helpDetails;
  [SerializeField] private GameObject helpList;

  void Start()
    {
        refreshHelpDetails();
    }

    public void refreshHelpDetails(){
        // delete previous items
        foreach (Transform child in helpListContent)
        {
            Destroy(child.gameObject);
        }

        // add allHelpItems
        foreach(HelpDetailsInfo curItem in allHelpItems){
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

    public List<HelpDetailsInfo> GetAllHelpItems()
    {
        return allHelpItems;
    }
}