using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
public class HelpBoardEntryList : MonoBehaviour
{
  private Dictionary<Guid, HelpDetailsInfo> allHelpItems = new Dictionary<Guid, HelpDetailsInfo>();
  [SerializeField] private List<HelpDetailsInfo> allHelpItemsList = new List<HelpDetailsInfo>();

  void Start()
  {
    for (int i = 0; i < allHelpItemsList.Count; ++i)
    {
      if (allHelpItemsList[i].guid == Guid.Empty)
      {
        allHelpItemsList[i].guid = Guid.NewGuid();
        // HelpDetailsInfo myStruct = myList[0]; // Retrieve the struct
        // myStruct.SomeProperty = "New Value"; // Modify the local variable

        // myList[0] = myStruct
      }
      allHelpItems.Add(allHelpItemsList[i].guid, allHelpItemsList[i]);
    }
  }

  public List<HelpDetailsInfo> getAllHelpItems()
  {
    return allHelpItems.Values.ToList();
  }

  public HelpDetailsInfo getHelpDetailsInfoByGuid(Guid guid)
  {
    return allHelpItems[guid];
  }

  public void addItem(HelpDetailsInfo newInfo)
  {
    if (newInfo.guid == Guid.Empty)
    {
      newInfo.guid = Guid.NewGuid();
    }
    allHelpItemsList.Add(newInfo);

    if (!allHelpItems.ContainsKey(newInfo.guid))
    {
      allHelpItems.Add(newInfo.guid, newInfo);
    }
    else
    {
      allHelpItems[newInfo.guid] = newInfo;
    }
  }

  public void updateDescription(Guid guid, string description)
  {
    allHelpItems[guid].description = description;
    HelpDetailsInfo itemInList = allHelpItemsList.Find(item => item.guid == guid);
    itemInList.description = description;
  }

  public void deleteItem(Guid guid)
  {
    allHelpItems.Remove(guid);
    HelpDetailsInfo itemInList = allHelpItemsList.Find(item => item.guid == guid);
    allHelpItemsList.Remove(itemInList);
  }
}