using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
public class HelpBoardEntryList : MonoBehaviour
{
  [SerializeField] private Dictionary<Guid, HelpDetailsInfo> allHelpItems = new Dictionary<Guid, HelpDetailsInfo>();

  public List<HelpDetailsInfo> getAllHelpItems()
  {
    return allHelpItems.Values.ToList();
  }

  public HelpDetailsInfo getHelpDetailsInfoByGuid(Guid guid)
  {
    return allHelpItems[guid];
  }

    // TODO: set/edit functions
}